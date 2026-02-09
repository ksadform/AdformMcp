using System.Text.Json.Nodes;


namespace AdformMcp.Confluence;

public static class ConfluenceService
{

    public static string? ExtractDocumentBody(string pageData, string primaryBody)
    {

        var (doc, error) = Util.Unwrap(() => JsonNode.Parse(pageData));
        if (Util.HasError(error) || doc == null) return null;

        var documentBodyValue = doc["body"]?[primaryBody]?["value"];
        if (documentBodyValue == null) return null;

        var valueString = documentBodyValue.GetValue<string>();
        return string.IsNullOrWhiteSpace(valueString) ? null : valueString;
    }
}
