#!/bin/bash

# AdformMcp Run Script for macOS
# This script runs the appropriate executable for your Mac architecture

set -e

# Colors for output
GREEN='\033[0;32m'
BLUE='\033[0;34m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${BLUE}🚀 AdformMcp Server${NC}"
echo ""

# Detect architecture
ARCH=$(uname -m)
EXECUTABLE=""

if [ "$ARCH" = "arm64" ]; then
    echo -e "${BLUE}Detected: Apple Silicon (ARM64)${NC}"
    EXECUTABLE="./publish/osx-arm64/AdformMcp"
elif [ "$ARCH" = "x86_64" ]; then
    echo -e "${BLUE}Detected: Intel Mac (x64)${NC}"
    EXECUTABLE="./publish/osx-x64/AdformMcp"
else
    echo -e "${RED}❌ Unknown architecture: $ARCH${NC}"
    exit 1
fi

# Check if executable exists
if [ ! -f "$EXECUTABLE" ]; then
    echo -e "${RED}❌ Executable not found: $EXECUTABLE${NC}"
    echo ""
    echo -e "${YELLOW}Please build the project first:${NC}"
    echo "  ./build.sh"
    echo ""
    echo -e "${YELLOW}Or run with dotnet:${NC}"
    echo "  dotnet run"
    exit 1
fi

# Make sure it's executable
chmod +x "$EXECUTABLE"

# Run the application
echo -e "${GREEN}✅ Starting server...${NC}"
echo ""
exec "$EXECUTABLE"
