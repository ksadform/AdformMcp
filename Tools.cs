using AdformMcp;
using AdformMcp.Confluence;
using ModelContextProtocol.Server;
using System.ComponentModel;

[McpServerToolType]
public static class ConfluenceTools
{
    [McpServerTool, Description("This is just for test ping tool")]
    public static async Task<string> Ping()
    {
        return "Pong";
    }

    [McpServerTool, Description("This tool is to fetch confluence page by id")]
    public static async Task<string?> GetAmplitudeData(string id)
    {

        var (pageData, pageDataError) = await Util.Unwrap(() => ConfluenceHttpClient.GetPageById(id, PrimaryBodyRepresentation.atlas_doc_format));
        if (Util.HasError(pageDataError) || pageData == null) return "Unable to fetch Amplitude page data";

        var output = ConfluenceService.ParseAmplitudeTable(pageData);

        return output;
    }

}
