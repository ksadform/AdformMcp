namespace AdformMcp.Confluence;

public enum PrimaryBodyRepresentation
{
    storage,
    atlas_doc_format,
    view,
    export_view,
    anonymous_export_view,
    styled_view,
    editor
}

public static class ConfluenceUrl
{
    public const string Base = "https://adform.atlassian.net";

    public static string GetPageById(string id, PrimaryBodyRepresentation? bodyFormat = null)
    {
        var url = $"{Base}/wiki/api/v2/pages/{id}";

        if (bodyFormat.HasValue)
        {
            url += $"?body-format={bodyFormat}";
        }

        return url;
    }
}


public static class ConfluenceSecret
{
    public static string Email => Environment.GetEnvironmentVariable("CONFLUENCE_EMAIL")
        ?? throw new InvalidOperationException("CONFLUENCE_EMAIL environment variable is not set");

    public static string Token => Environment.GetEnvironmentVariable("CONFLUENCE_TOKEN")
        ?? throw new InvalidOperationException("CONFLUENCE_TOKEN environment variable is not set");
}


