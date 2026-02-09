namespace AdformMcp.Confluence;

public static class PrimaryBodyRepresentation
{
    public const string Storage = "storage";
    public const string AtlasDocFormat = "atlas_doc_format";
    public const string View = "view";
    public const string ExportView = "export_view";
    public const string AnonymousExportView = "anonymous_export_view";
    public const string StyledView = "styled_view";
    public const string Editor = "editor";
}

public static class ConfluenceUrl
{
    public const string Base = "https://adform.atlassian.net";

    public static string GetPageById(string id, string? bodyFormat = null)
    {
        var url = $"{Base}/wiki/api/v2/pages/{id}";

        if (!string.IsNullOrEmpty(bodyFormat))
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


public static class ConfluencePageId
{
    public const string AmplitudeEventsTaxonomy = "886341676";
    public const string AmplitudePropertiesTaxonomy = "3929374737";
    public const string AmplitudeImplementationPattern = "5358321694";
}
