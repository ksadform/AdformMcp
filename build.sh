#!/bin/bash

# AdformMcp Build Script for macOS
# This script builds the application for both Intel and Apple Silicon Macs

set -e

echo "🚀 Building AdformMcp for macOS..."
echo ""

# Colors for output
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Clean previous builds
echo -e "${BLUE}🧹 Cleaning previous builds...${NC}"
dotnet clean
rm -rf ./publish

# Restore dependencies
echo -e "${BLUE}📦 Restoring dependencies...${NC}"
dotnet restore

# Build the project
echo -e "${BLUE}🔨 Building project...${NC}"
dotnet build --configuration Release --no-restore

echo ""
echo -e "${YELLOW}📱 Building self-contained executables...${NC}"
echo ""

# Build for Intel Macs (x64)
echo -e "${BLUE}Building for Intel Macs (x64)...${NC}"
dotnet publish \
  --configuration Release \
  --runtime osx-x64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  -p:IncludeNativeLibrariesForSelfExtract=true \
  --output ./publish/osx-x64

# Build for Apple Silicon Macs (ARM64)
echo -e "${BLUE}Building for Apple Silicon (ARM64)...${NC}"
dotnet publish \
  --configuration Release \
  --runtime osx-arm64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  -p:IncludeNativeLibrariesForSelfExtract=true \
  --output ./publish/osx-arm64

echo ""
echo -e "${GREEN}✅ Build completed successfully!${NC}"
echo ""
echo "📂 Build outputs:"
echo "  Intel Mac:     ./publish/osx-x64/AdformMcp"
echo "  Apple Silicon: ./publish/osx-arm64/AdformMcp"
echo ""

# Create archives
echo -e "${BLUE}📦 Creating archives...${NC}"
cd ./publish/osx-x64
tar -czf ../AdformMcp-osx-x64.tar.gz *
cd ../osx-arm64
tar -czf ../AdformMcp-osx-arm64.tar.gz *
cd ../..

echo -e "${GREEN}✅ Archives created:${NC}"
echo "  ./publish/AdformMcp-osx-x64.tar.gz"
echo "  ./publish/AdformMcp-osx-arm64.tar.gz"
echo ""

# Detect current architecture and suggest which binary to run
ARCH=$(uname -m)
if [ "$ARCH" = "arm64" ]; then
    echo -e "${YELLOW}💡 You're on Apple Silicon. Run with:${NC}"
    echo "  ./publish/osx-arm64/AdformMcp"
elif [ "$ARCH" = "x86_64" ]; then
    echo -e "${YELLOW}💡 You're on Intel Mac. Run with:${NC}"
    echo "  ./publish/osx-x64/AdformMcp"
fi

echo ""
echo -e "${GREEN}🎉 All done!${NC}"
