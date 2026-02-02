# AdformMcp - Confluence MCP Server

A Model Context Protocol (MCP) server that provides tools for interacting with Confluence pages. This server enables AI assistants to fetch and parse Confluence content directly.

## Features

- Fetch Confluence pages by ID
- Parse Amplitude data tables from Confluence pages
- Ping tool for testing connectivity

## Installation

### Download the Binary

1. Go to the [Releases](https://github.com/YOUR_USERNAME/AdformMcp/releases) page
2. Download the latest release for your Mac:
   - **Apple Silicon (M1/M2/M3)**: `AdformMcp-osx-arm64.tar.gz`
   - **Intel Mac**: `AdformMcp-osx-x64.tar.gz`
3. Extract the archive:
   ```bash
   tar -xzf AdformMcp-osx-arm64.tar.gz
   ```
4. Make it executable:
   ```bash
   chmod +x AdformMcp
   ```
5. Move it to a permanent location (e.g., `~/bin/` or `/usr/local/bin/`)

## Environment Variables

You need to set up Confluence credentials as environment variables. These will be configured in your MCP client.

### Getting Your Confluence API Token

1. Go to [Atlassian API Tokens](https://id.atlassian.com/manage-profile/security/api-tokens)
2. Click "Create API token"
3. Give it a label (e.g., "MCP Server")
4. Copy the generated token

## Connecting to Claude Desktop

To connect this server to Claude Desktop, add the following configuration to your Claude Desktop config file:

**Location:** `~/Library/Application Support/Claude/claude_desktop_config.json`

```json
{
  "mcpServers": {
    "adform-confluence": {
      "command": "/absolute/path/to/AdformMcp",
      "env": {
        "CONFLUENCE_EMAIL": "your.email@example.com",
        "CONFLUENCE_TOKEN": "your-confluence-api-token"
      }
    }
  }
}
```

### Configuration Steps

1. Replace `/absolute/path/to/AdformMcp` with the full path where you placed the binary
   - Example: `/Users/yourusername/bin/AdformMcp`
2. Replace `your.email@example.com` with your Adform email
3. Replace `your-confluence-api-token` with the API token you generated
4. Save the file
5. Restart Claude Desktop

**How to find the full path:**
```bash
# If you placed it in your current directory
pwd  # Shows current directory, then append /AdformMcp

# Or if you placed it in ~/bin/
echo ~/bin/AdformMcp
```

## Connecting to Other MCP Clients

For other MCP clients that support stdio transport:

**Command:**
```
/path/to/AdformMcp
```

**Environment Variables Required:**
- `CONFLUENCE_EMAIL`: Your Confluence email
- `CONFLUENCE_TOKEN`: Your Confluence API token

## Available Tools

Once connected, you can use these tools through your MCP client (e.g., Claude):

### `ping`
Test tool to verify the server is running.

**Example:** "Can you ping the Adform Confluence server?"

**Returns:** "Pong"

### `GetAmplitudeData`
Fetches a Confluence page by ID and parses Amplitude data tables.

**Parameters:**
- `id` (string): The Confluence page ID

**Example:** "Can you get the Amplitude data from Confluence page 123456789?"

**Returns:** Parsed Amplitude data from the page, or an error message if the page cannot be fetched.

## Troubleshooting

### "CONFLUENCE_EMAIL environment variable is not set"
Make sure you've added both `CONFLUENCE_EMAIL` and `CONFLUENCE_TOKEN` to the `env` section of your MCP client configuration.

### Claude Desktop doesn't see the server
1. Ensure the path to the binary is **absolute** (starts with `/`), not relative
2. Check that the binary has execute permissions: `chmod +x /path/to/AdformMcp`
3. Verify the JSON syntax in your config file is correct (use a JSON validator if needed)
4. Restart Claude Desktop after modifying the config file
5. Check Claude Desktop logs at: `~/Library/Logs/Claude/`

### "Permission denied" error
Run: `chmod +x /path/to/AdformMcp`

### Connection issues
- Verify your Confluence API token is valid and not expired
- Ensure you have network access to `https://adform.atlassian.net`
- Check that your email and token are correctly set in the configuration
- Try regenerating your API token if issues persist

### Wrong architecture
If the binary won't run:
- On Apple Silicon: Make sure you downloaded the `osx-arm64` version
- On Intel Mac: Make sure you downloaded the `osx-x64` version
- Check your Mac's architecture: `uname -m` (returns `arm64` or `x86_64`)

## Verifying Installation

After setup, restart Claude Desktop and try asking:
```
Can you ping the Adform Confluence server?
```

If you get "Pong" back, everything is working correctly!

## Support

For issues and questions, contact your internal Adform support team or create an issue on the GitHub repository.

## License

Internal Adform tool.
