# AdformMcp - MCP Server

A Model Context Protocol (MCP) server that enables AI assistants to fetch and parse Confluence content directly.

## Prerequisites

### Get Your Confluence API Token

1. Visit [Atlassian API Token Manager](https://id.atlassian.com/manage-profile/security/api-tokens)
2. Click **"Create API token"**
3. Give it a descriptive label (e.g., "MCP Server")
4. **Copy and save the generated token** - you'll need it for configuration

## Installation & Permissions

### Download

1. Go to the [Releases](https://github.com/ksadform/AdformMcp/releases) page
2. Download the appropriate binary for your Mac:
   - **Apple Silicon (M1/M2/M3)**: `AdformMcp-osx-arm64.tar.gz`
   - **Intel Mac**: `AdformMcp-osx-x64.tar.gz`
3. Extract the archive:
   ```bash
   tar -xzf AdformMcp-osx-arm64.tar.gz
   ```
4. Move to a permanent location:
   ```bash
   mkdir -p ~/bin
   mv AdformMcp ~/bin/
   chmod +x ~/bin/AdformMcp
   ```
 5. MacOS Permission
    ```bash
    xattr -dr com.apple.quarantine ~/bin/AdformMcp
    ```

## Usage

### Configuration Format

Add this configuration to your MCP client settings:

```json
{
  "adform-mcp-server": {
    "command": "/absolute/path/to/AdformMcp",
    "args": [],
    "env": {
      "CONFLUENCE_EMAIL": "your.email@example.com",
      "CONFLUENCE_TOKEN": "your-api-token-here"
    }
  }
}
```

### Setup Steps

1. Locate your MCP client's configuration file (refer to your editor's MCP documentation)

2. Add the configuration above with your actual values:
   - Replace `/absolute/path/to/AdformMcp` with the full path to the binary (e.g., `/Users/yourusername/bin/AdformMcp`)
   - Replace `your.email@example.com` with your Adform email
   - Replace `your-api-token-here` with the API token from Prerequisites

3. Restart your editor/MCP client

4. Verify by asking your AI assistant: *"Ping the Adform Confluence server"*
   - Expected response: "Pong"

### Finding the Absolute Path

To get the full path to your binary:

```bash
# If you placed it in ~/bin/
realpath ~/bin/AdformMcp

# Or use pwd if in the same directory
pwd  # Then append /AdformMcp
```

## Available Tools

### `GetAmplitudeInstruction`
Get the Amplitude tracking implementation guide and instructions.

**Parameters:** None

**Example:** *"Get the Amplitude tracking implementation guide"*

**Returns:** Complete guide with instructions on how to implement Amplitude tracking

### `GetAmplitudeData`
Fetch and parse Amplitude event data from a Confluence page by ID.

**Parameters:**
- `id` (string): Confluence page ID

**Example:** *"Get Amplitude data from Confluence page 123456789"*

**Returns:** Parsed Amplitude data tables from the specified Confluence page

### `ImplementAmplitudeEvents`
Implement Amplitude events by combining the instruction guide with Amplitude data from a Confluence page.

**Parameters:**
- `id` (string): Confluence page ID

**Example:** *"Implement Amplitude events from Confluence page 123456789"*

**Returns:** Combined implementation guide and Amplitude event data ready for implementation. This tool automatically fetches both the instruction guide and the specific event data from the Confluence page.

## Troubleshooting

| Issue | Solution |
|-------|----------|
| "Permission denied" | Run `chmod +x /path/to/AdformMcp` |
| "CONFLUENCE_EMAIL not set" | Verify `env` section in config has both email and token |
| Server not detected | Use absolute path (e.g., `/Users/name/bin/AdformMcp`), not `~/bin/AdformMcp` |
| Wrong architecture | Check with `uname -m`: use `arm64` or `x86_64` version accordingly |
| Token expired | Regenerate a new API token from Atlassian |
| Network issues | Ensure you can access `https://adform.atlassian.net` |

## Environment Variables

The server requires these environment variables (configured in the `env` section):

- `CONFLUENCE_EMAIL`: Your Adform Confluence email address
- `CONFLUENCE_TOKEN`: Your Atlassian API token

## License

Internal Adform tool.
