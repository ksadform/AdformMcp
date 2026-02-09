using AdformMcp;
using AdformMcp.Confluence;
using ModelContextProtocol.Server;
using System.ComponentModel;

[McpServerToolType]
public static class ConfluenceTools
{
    private const string AmplitudeInstructionPath = "Instructions/Amplitude.md";

    [McpServerTool, Description("Fetches a Confluence page by its ID and converts the content to Markdown format. Returns the page content as a Markdown string.")]
    public static async Task<string> GetConfluencePageInMarkdown(string id)
    {
        var (pageData, pageDataError) = await Util.Unwrap(() => ConfluenceHttpClient.GetPageById(id, PrimaryBodyRepresentation.View));
        if (Util.HasError(pageDataError) || pageData == null)
            return "Unable to fetch page data from Confluence";
        var body = ConfluenceService.ExtractDocumentBody(pageData, PrimaryBodyRepresentation.View);

        if (body == null) return "NO DATA";
        var converter = new ReverseMarkdown.Converter();
        var markdown = converter.Convert(body);
        return markdown;
    }

    [McpServerTool, Description("Get Amplitude tracking implementation guide and instructions")]
    public static async Task<string> GetAmplitudeInstruction()
    {
        try
        {
            // Try multiple paths to find the file
            var possiblePaths = new[]
            {
                Path.Combine(AppContext.BaseDirectory, AmplitudeInstructionPath),
                Path.Combine(Directory.GetCurrentDirectory(), AmplitudeInstructionPath),
                Path.Combine(AppContext.BaseDirectory, "..", "..", "..", AmplitudeInstructionPath)
            };

            string? filePath = null;
            foreach (var path in possiblePaths)
            {
                var normalizedPath = Path.GetFullPath(path);
                if (File.Exists(normalizedPath))
                {
                    filePath = normalizedPath;
                    break;
                }
            }

            if (filePath == null)
            {
                return $"Error: Amplitude instruction file not found. Searched in:\n" +
                       string.Join("\n", possiblePaths.Select(Path.GetFullPath));
            }

            var content = await File.ReadAllTextAsync(filePath);
            return content;
        }
        catch (Exception ex)
        {
            return $"Error reading Amplitude instruction file: {ex.Message}";
        }
    }


    [McpServerTool, Description("Implement Amplitude events by first getting the instruction guide and then fetching the Amplitude data from Confluence page")]
    public static async Task<string> ImplementAmplitudeEvents(string id)
    {
        try
        {
            // Step 1: Get the Amplitude instruction guide
            var instruction = await GetAmplitudeInstruction();

            if (instruction.StartsWith("Error:"))
            {
                return $"Failed to get Amplitude instruction: {instruction}";
            }

            // Step 2: Get the Amplitude data from Confluence
            var amplitudeData = await GetConfluencePageInMarkdown(id);

            // Step 2: Get the Amplitude data from Confluence pages in parallel
            var pageIds = new[]
            {
                id,
                ConfluencePageId.AmplitudeEventsTaxonomy,
                ConfluencePageId.AmplitudePropertiesTaxonomy,
                ConfluencePageId.AmplitudeImplementationPattern,

            };

            var pageDataTasks = pageIds.Select(GetConfluencePageInMarkdown).ToArray();
            var pageDataResults = await Task.WhenAll(pageDataTasks);
            
            var amplitudePageData = pageDataResults[0];
            var eventsTaxonomyData = pageDataResults[1];
            var propertiesTaxonomyData = pageDataResults[2];
            var implementationPatternData = pageDataResults[3];


            if (amplitudeData == null || amplitudeData.StartsWith("Unable to fetch"))
            {
                return $"Failed to get Amplitude data: {amplitudeData ?? "No data returned"}";
            }

            return Prompt.ImplementAmplitude(id, instruction, amplitudeData);            // Step 3: Combine both for implementation
        }
        catch (Exception ex)
        {
            return $"Error implementing Amplitude events: {ex.Message}";
        }
    }
}
