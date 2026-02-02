using System.Text.Json.Nodes;


namespace AdformMcp.Confluence;

public static class ConfluenceService
{

    public static string? ParseAmplitudeTable(string? pageData)
    {
        if (string.IsNullOrWhiteSpace(pageData)) return null;

        var (doc, error) = Util.Unwrap(() => JsonNode.Parse(pageData));
        if (Util.HasError(error) || doc == null) return null;

        // Extract the value field from body.atlas_doc_format.value
        var documentBodyValue = doc["body"]?["atlas_doc_format"]?["value"];
        if (documentBodyValue == null) return "Unable to parse amplitude table";

        // The value is a JSON string that needs to be parsed
        var valueString = documentBodyValue.GetValue<string>();
        if (string.IsNullOrWhiteSpace(valueString)) return "Empty document body";

        // Parse the JSON string to get the actual content
        var (contentDoc, contentError) = Util.Unwrap(() => JsonNode.Parse(valueString));
        if (Util.HasError(contentError) || contentDoc == null) return "Unable to parse document content";

        // Extract the content array
        var content = contentDoc["content"];
        if (content == null) return "No content found in document";

        // Check if content has a "table" type and find it
        var contentArray = content.AsArray();
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

        // Parse the table structure
        var rows = tableNode?["content"]?.AsArray();
        if (rows == null || rows.Count == 0) return "Table has no rows";

        // Extract headers from the first row
        var headerRow = rows[0];
        var headerCells = headerRow?["content"]?.AsArray();
        if (headerCells == null || headerCells.Count == 0) return "Table has no header row";

        var headers = new List<string>();
        foreach (var cell in headerCells)
        {
            var cellContent = cell?["content"]?.AsArray();
            var headerText = ExtractTextFromCell(cellContent);
            headers.Add(headerText);
        }

        // Extract data rows and handle category grouping
        var resultArray = new JsonArray();
        string? currentCategory = null;

        for (int i = 1; i < rows.Count; i++)
        {
            var row = rows[i];
            var cells = row?["content"]?.AsArray();
            if (cells == null) continue;

            var rowObject = new JsonObject();
            var nonEmptyCount = 0;
            string? firstNonEmptyValue = null;

            for (int j = 0; j < headers.Count && j < cells.Count; j++)
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

            // Check if this row is a category row (only one non-empty property)
            if (nonEmptyCount == 1 && !string.IsNullOrWhiteSpace(firstNonEmptyValue))
            {
                currentCategory = firstNonEmptyValue;
                // Don't add category rows to the result
                continue;
            }

            // Add category to regular rows
            if (!string.IsNullOrWhiteSpace(currentCategory))
            {
                rowObject["Category"] = currentCategory;
            }

            resultArray.Add(rowObject);
        }

        return resultArray.ToJsonString(new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true
        });
    }

    private static string ExtractTextFromCell(JsonArray? cellContent)
    {
        if (cellContent == null) return "";

        var textParts = new List<string>();
        foreach (var item in cellContent)
        {
            if (item?["type"]?.GetValue<string>() == "paragraph")
            {
                var paragraphContent = item["content"]?.AsArray();
                if (paragraphContent != null)
                {
                    foreach (var textNode in paragraphContent)
                    {
                        if (textNode?["type"]?.GetValue<string>() == "text")
                        {
                            var text = textNode["text"]?.GetValue<string>();
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
