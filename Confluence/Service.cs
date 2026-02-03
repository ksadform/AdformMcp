using System.Text.Json.Nodes;


namespace AdformMcp.Confluence;

public static class ConfluenceService
{
    public static string? ParseAmplitudeTable(string? pageData)
    {
        if (string.IsNullOrWhiteSpace(pageData)) return null;

        var documentBody = ExtractDocumentBody(pageData);
        if (documentBody == null) return "Unable to parse amplitude table";

        var contentDoc = ParseContentDocument(documentBody);
        if (contentDoc == null) return "Unable to parse document content";

        var tableNode = FindSingleTable(contentDoc);
        if (tableNode is string errorMessage) return errorMessage;

        var rows = ExtractTableRows(tableNode as JsonNode);
        if (rows == null) return "Table has no rows";



        var headers = ExtractHeaders(rows);
        if (headers == null) return "Table has no header row";

        var resultArray = ProcessDataRows(rows, headers);

        return resultArray.ToJsonString(new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true
        });
    }

    private static string? ExtractDocumentBody(string pageData)
    {
        var (doc, error) = Util.Unwrap(() => JsonNode.Parse(pageData));
        if (Util.HasError(error) || doc == null) return null;

        var documentBodyValue = doc["body"]?["atlas_doc_format"]?["value"];
        if (documentBodyValue == null) return null;

        var valueString = documentBodyValue.GetValue<string>();
        return string.IsNullOrWhiteSpace(valueString) ? null : valueString;
    }

    private static JsonNode? ParseContentDocument(string documentBody)
    {
        var (contentDoc, contentError) = Util.Unwrap(() => JsonNode.Parse(documentBody));
        if (Util.HasError(contentError) || contentDoc == null) return null;

        var content = contentDoc["content"];
        return content;
    }

    private static object? FindSingleTable(JsonNode? contentDoc)
    {
        if (contentDoc == null) return "No content found in document";

        var contentArray = contentDoc.AsArray();
        var tableCount = 0;
        JsonNode? tableNode = null;

        foreach (var item in contentArray)
        {
            if (item?["type"]?.GetValue<string>() == "table")
            {
                tableCount++;
                tableNode = item;
            }
        }

        if (tableCount == 0) return "No amplitude table found";
        if (tableCount > 1) return "Unable to parse amplitude table. Multiple tables found";

        return tableNode;
    }

    private static JsonArray? ExtractTableRows(JsonNode? tableNode)
    {
        var rows = tableNode?["content"]?.AsArray();
        if (rows == null || rows.Count == 0) return null;
        return rows;
    }

    private static List<string>? ExtractHeaders(JsonArray rows)
    {
        if (rows.Count == 0) return null;

        var headerRow = rows[0];
        var headerCells = headerRow?["content"]?.AsArray();
        if (headerCells == null || headerCells.Count == 0) return null;

        var headers = new List<string>();
        foreach (var cell in headerCells)
        {
            var cellType = cell?["type"]?.GetValue<string>();
            if (cellType == "tableHeader" || cellType == "tableCell")
            {
                var cellContent = cell?["content"]?.AsArray();
                var headerText = ExtractTextFromCell(cellContent);
                headers.Add(headerText);
            }
        }

        return headers;
    }

    private static JsonArray ProcessDataRows(JsonArray rows, List<string> headers)
    {
        var resultArray = new JsonArray();
        string? currentCategory = null;

        for (int i = 1; i < rows.Count; i++)
        {
            var row = rows[i];
            var cells = row?["content"]?.AsArray();


            if (cells == null) continue;

            var (rowObject, isCategoryRow, categoryValue) = BuildRowObject(cells, headers);

            // Skip rows where all fields are empty
            if (IsEmptyRow(rowObject))
            {
                continue;
            }

            // Skip duplicate header row (where values match header names)
            if (IsHeaderRow(rowObject, headers))
            {
                continue;
            }

            if (isCategoryRow)
            {
                currentCategory = categoryValue;
                continue;
            }

            if (!string.IsNullOrWhiteSpace(currentCategory))
            {
                rowObject["Category"] = currentCategory;
            }

            resultArray.Add(rowObject);
        }

        return resultArray;
    }

    private static bool IsEmptyRow(JsonObject rowObject)
    {
        foreach (var kvp in rowObject)
        {
            var value = kvp.Value?.GetValue<string>();
            if (!string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
        }
        return true;
    }

    private static bool IsHeaderRow(JsonObject rowObject, List<string> headers)
    {
        // Check if all non-empty values in the row match the header names
        var matchCount = 0;
        var nonEmptyCount = 0;

        foreach (var header in headers)
        {
            var value = rowObject[header]?.GetValue<string>();
            if (!string.IsNullOrWhiteSpace(value))
            {
                nonEmptyCount++;
                // Check if value matches header name (case-insensitive, ignoring whitespace)
                if (value.Replace(" ", "").Equals(header.Replace(" ", ""), StringComparison.OrdinalIgnoreCase))
                {
                    matchCount++;
                }
            }
        }

        // If most non-empty values match headers, it's likely a duplicate header row
        return nonEmptyCount > 0 && matchCount >= nonEmptyCount * 0.8;
    }

    private static (JsonObject rowObject, bool isCategoryRow, string? categoryValue) BuildRowObject(
        JsonArray cells,
        List<string> headers)
    {
        var rowObject = new JsonObject();
        var nonEmptyCount = 0;
        string? firstNonEmptyValue = null;

        for (int j = 0; j < headers.Count && j < cells.Count; j++)
        {
            var cellType = cells[j]?["type"]?.GetValue<string>();
            if (cellType == "tableHeader" || cellType == "tableCell")
            {
                var cellContent = cells[j]?["content"]?.AsArray();
                var cellText = ExtractTextFromCell(cellContent);
                rowObject[headers[j]] = cellText;

                if (!string.IsNullOrWhiteSpace(cellText))
                {
                    nonEmptyCount++;
                    if (firstNonEmptyValue == null)
                    {
                        firstNonEmptyValue = cellText;
                    }
                }
            }
        }

        var isCategoryRow = nonEmptyCount == 1 && !string.IsNullOrWhiteSpace(firstNonEmptyValue);
        return (rowObject, isCategoryRow, firstNonEmptyValue);
    }

    private static string ExtractTextFromCell(JsonArray? cellContent)
    {
        if (cellContent == null) return "";

        var textParts = new List<string>();
        foreach (var item in cellContent)
        {
            var itemType = item?["type"]?.GetValue<string>();
            if (itemType == "paragraph" || itemType == "heading")
            {
                var paragraphContent = item?["content"]?.AsArray();
                if (paragraphContent != null)
                {
                    foreach (var textNode in paragraphContent)
                    {
                        if (textNode?["type"]?.GetValue<string>() == "text")
                        {
                            var text = textNode?["text"]?.GetValue<string>();
                            if (!string.IsNullOrEmpty(text))
                            {
                                textParts.Add(text);
                            }
                        }
                    }
                }
            }
        }

        return string.Join(" ", textParts).Trim();
    }
}
