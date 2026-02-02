# GitHub Actions Workflows

This directory contains GitHub Actions workflows for building, testing, and releasing the AdformMcp server on macOS.

## Workflows

### 1. CI - macOS Build (`ci.yml`)

**Triggers:**
- On every push to any branch
- On pull requests to `main` or `master`

**What it does:**
- Builds the project on macOS-latest
- Runs a smoke test of the application
- Creates self-contained executables for:
  - macOS x64 (Intel)
  - macOS ARM64 (Apple Silicon)
- Uploads build artifacts (available for 7 days)

**Usage:**
This workflow runs automatically on every push. You can download the built artifacts from the Actions tab in GitHub.

### 2. Build and Release (`build-and-release.yml`)

**Triggers:**
- On push to `main` or `master` branches
- On pull requests to `main` or `master`
- On release creation
- Manual trigger via workflow_dispatch
- On tags starting with `v` (e.g., `v1.0.0`)

**What it does:**
- Builds and tests the application on macOS
- Creates release archives for both Intel and Apple Silicon Macs
- Automatically attaches binaries to GitHub releases
- Generates release notes

**Usage:**

#### Option 1: Manual Release
1. Go to Actions tab in GitHub
2. Select "Build and Release" workflow
3. Click "Run workflow"
4. Choose the branch and run

#### Option 2: Create a Release via GitHub UI
1. Go to Releases in GitHub
2. Click "Draft a new release"
3. Create a tag (e.g., `v1.0.0`)
4. Fill in release details
5. Click "Publish release"
6. The workflow will automatically build and attach the macOS binaries

#### Option 3: Create a Tag via Command Line
```bash
# Tag the current commit
git tag v1.0.0

# Push the tag to GitHub
git push origin v1.0.0
```

The workflow will automatically:
- Build the application
- Create release archives
- Create a GitHub release with the binaries attached

## Build Outputs

The workflows produce the following artifacts:

### CI Workflow
- `AdformMcp-osx-x64/` - Intel Mac executable
- `AdformMcp-osx-arm64/` - Apple Silicon executable

### Release Workflow
- `AdformMcp-osx-x64.tar.gz` - Intel Mac archive
- `AdformMcp-osx-arm64.tar.gz` - Apple Silicon archive

## Installing the Released Binary

After downloading a release:

```bash
# Extract the archive
tar -xzf AdformMcp-osx-x64.tar.gz  # or AdformMcp-osx-arm64.tar.gz

# Make executable (if needed)
chmod +x AdformMcp

# Run the application
./AdformMcp
```

## Requirements

- .NET 10.0 SDK (automatically installed by GitHub Actions)
- macOS runner (GitHub provides these)

## Troubleshooting

### Workflow fails with .NET not found
- Ensure the `dotnet-version` in the workflow matches your project's target framework
- Current version: `10.0.x`

### Build fails
- Check the Actions tab for detailed error logs
- Ensure all NuGet packages are restored correctly
- Verify the project builds locally with `dotnet build`

### Release not created
- Ensure you have created a tag starting with `v`
- Check that you have proper permissions in the repository
- Verify the `GITHUB_TOKEN` has sufficient permissions

## Customization

### Change target frameworks
Edit the `dotnet-version` in the workflow files:
```yaml
- name: Setup .NET
  uses: actions/setup-dotnet@v4
  with:
    dotnet-version: "10.0.x"  # Change this
```

### Add more platforms
Add to the `matrix.runtime` section:
```yaml
strategy:
  matrix:
    runtime:
      - osx-x64
      - osx-arm64
      - linux-x64    # Add Linux
      - win-x64      # Add Windows
```

### Change artifact retention
Modify the `retention-days` in the upload-artifact step:
```yaml
- name: Upload artifact
  uses: actions/upload-artifact@v4
  with:
    name: AdformMcp
    path: ./publish/
    retention-days: 30  # Keep for 30 days instead of 7
```
