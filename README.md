# AdformMcp Server

A Model Context Protocol (MCP) server built with .NET 10.0 for Adform integrations.

## Features

- MCP server with stdio transport
- Built on .NET 10.0
- Native support for macOS (Intel and Apple Silicon)
- Automated CI/CD with GitHub Actions

## Requirements

- .NET 10.0 SDK
- macOS (Intel or Apple Silicon)

## Building Locally

### Clone the repository

```bash
git clone <your-repo-url>
cd AdformMcp
```

### Restore dependencies

```bash
dotnet restore
```

### Build the project

```bash
dotnet build --configuration Release
```

### Run the application

```bash
dotnet run --configuration Release
```

## Publishing Self-Contained Executables

### For Intel Macs (x64)

```bash
dotnet publish \
  --configuration Release \
  --runtime osx-x64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  -p:IncludeNativeLibrariesForSelfExtract=true \
  --output ./publish/osx-x64
```

### For Apple Silicon Macs (ARM64)

```bash
dotnet publish \
  --configuration Release \
  --runtime osx-arm64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  -p:IncludeNativeLibrariesForSelfExtract=true \
  --output ./publish/osx-arm64
```

The compiled executable will be in the respective `publish/` directory and can be run without requiring .NET to be installed.

## GitHub Actions CI/CD

This project includes automated workflows for building and releasing on macOS.

### Continuous Integration

Every push to any branch automatically:
- Builds the project on macOS
- Runs smoke tests
- Creates artifacts for both Intel and Apple Silicon Macs

Artifacts are available in the Actions tab for 7 days.

### Creating Releases

#### Method 1: Via Git Tag

```bash
# Create and push a version tag
git tag v1.0.0
git push origin v1.0.0
```

This automatically:
- Builds the application
- Creates a GitHub Release
- Attaches compiled binaries for both architectures

#### Method 2: Via GitHub UI

1. Go to "Releases" in your GitHub repository
2. Click "Draft a new release"
3. Create a new tag (e.g., `v1.0.0`)
4. Add release notes
5. Click "Publish release"

The workflows will automatically build and attach the macOS binaries.

#### Method 3: Manual Workflow Dispatch

1. Go to the "Actions" tab
2. Select "Build and Release"
3. Click "Run workflow"
4. Choose your branch and run

### Release Artifacts

Each release includes:
- `AdformMcp-osx-x64.tar.gz` - For Intel Macs
- `AdformMcp-osx-arm64.tar.gz` - For Apple Silicon Macs

### Installing from Release

```bash
# Download the appropriate release for your Mac
# For Intel Macs: AdformMcp-osx-x64.tar.gz
# For Apple Silicon: AdformMcp-osx-arm64.tar.gz

# Extract
tar -xzf AdformMcp-osx-*.tar.gz

# Make executable (if needed)
chmod +x AdformMcp

# Run
./AdformMcp
```

## Project Structure

```
AdformMcp/
├── .github/
│   └── workflows/
│       ├── ci.yml                    # Continuous integration
│       ├── build-and-release.yml     # Release automation
│       └── README.md                 # Workflow documentation
├── Confluence/                       # Confluence-related files
├── Program.cs                        # Main application entry point
├── Tools.cs                          # MCP tools implementation
├── Util.cs                           # Utility functions
├── AdformMcp.csproj                  # Project file
└── README.md                         # This file
```

## Dependencies

- **Microsoft.Extensions.Hosting** (v10.0.2) - Hosting infrastructure
- **ModelContextProtocol** (v0.6.0-preview.1) - MCP server implementation

## Development

### Adding New Tools

Add new MCP tools in the `Tools.cs` file. Tools are automatically discovered and registered via the `WithToolsFromAssembly()` method.

### Configuration

The server uses stdio transport for communication. Configure the host in `Program.cs`.

## Troubleshooting

### .NET 10.0 Not Found

Ensure you have the .NET 10.0 SDK installed:

```bash
dotnet --version
```

Download from: https://dotnet.microsoft.com/download

### Build Fails

1. Clean the build artifacts:
   ```bash
   dotnet clean
   rm -rf bin/ obj/
   ```

2. Restore and rebuild:
   ```bash
   dotnet restore
   dotnet build
   ```

### Runtime Issues on macOS

If you get security warnings when running the published executable:

```bash
# Remove quarantine attribute
xattr -d com.apple.quarantine ./AdformMcp
```

Or right-click the executable, select "Open", and confirm in the security dialog.

## Contributing

1. Create a feature branch
2. Make your changes
3. Push to the branch
4. Create a Pull Request

The CI workflow will automatically build and test your changes.

## License

[Add your license information here]

## Support

[Add support/contact information here]
