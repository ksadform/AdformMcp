using AdformMcp;
using AdformMcp.Confluence;
using ModelContextProtocol.Server;
using System.ComponentModel;

[McpServerToolType]
public static class ConfluenceTools
{
    private const string AmplitudeInstructionPath = "Instructions/Amplitude.md";

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

    [McpServerTool, Description("Fetch Amplitude event data from Confluence page by ID")]
    public static async Task<string?> GetAmplitudeData(string id)
    {
        var (pageData, pageDataError) = await Util.Unwrap(() => ConfluenceHttpClient.GetPageById(id, PrimaryBodyRepresentation.atlas_doc_format));
        if (Util.HasError(pageDataError) || pageData == null)
            return "Unable to fetch Amplitude page data from Confluence";

        var output = ConfluenceService.ParseAmplitudeTable(pageData);
        return output;
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
            var amplitudeData = await GetAmplitudeData(id);

            if (amplitudeData == null || amplitudeData.StartsWith("Unable to fetch"))
            {
                return $"Failed to get Amplitude data: {amplitudeData ?? "No data returned"}";
            }

            // Step 3: Combine both for implementation
            var result = $@"# Amplitude Events Implementation

## Instructions
{instruction}

---

## Amplitude Data from Confluence (Page ID: {id})
{amplitudeData}

---

You now have both the implementation guide and the specific Amplitude event data needed to implement the tracking.
Hold for Confirmation: Wait for the user to give the go-ahead before proceeding.
Identify Events: Note that the data may contain several different Amplitude events.
Clarify Scope: Ask the user if they want to set up all detected events or just a specific one.
Display Options: Present the list of event names in a clean, easy-to-read format.

";

            return result;
        }
        catch (Exception ex)
        {
            return $"Error implementing Amplitude events: {ex.Message}";
        }
    }
}
