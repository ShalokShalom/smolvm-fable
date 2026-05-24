# smolvm-fable — task runner
# Requires: just, dotnet >=10, node >=20, fable (dotnet tool install --global fable)

# Default: list all recipes
default:
    @just --list

# ─── Build ────────────────────────────────────────────────────────────────────

# Restore all NuGet packages
restore:
    dotnet restore SmolVm.Fable.fsproj
    dotnet restore tests/SmolVm.Fable.Tests/SmolVm.Fable.Tests.fsproj

# Build the binding library
build: restore
    dotnet build SmolVm.Fable.fsproj

# Transpile the test project to JavaScript via Fable
build-js: restore
    cd tests/SmolVm.Fable.Tests && fable --noCache

# ─── Tests ───────────────────────────────────────────────────────────────────

# Run snapshot tests on .NET
test: restore
    dotnet run --project tests/SmolVm.Fable.Tests/SmolVm.Fable.Tests.fsproj

# Run snapshot tests via Node (Fable-compiled)
test-js: build-js
    cd tests/SmolVm.Fable.Tests && node Main.fs.js

# Run both targets
test-all: test test-js

# ─── Snapshots ───────────────────────────────────────────────────────────────

# Update (or create) all snapshots on .NET
update-snapshots: restore
    UPDATE_SNAPSHOTS=1 dotnet run --project tests/SmolVm.Fable.Tests/SmolVm.Fable.Tests.fsproj

# Update (or create) all snapshots via Node
update-snapshots-js: build-js
    cd tests/SmolVm.Fable.Tests && UPDATE_SNAPSHOTS=1 node Main.fs.js

# ─── FSI MCP Server ─────────────────────────────────────────────────────────

# Clone and build the FSI MCP server (one-time setup).
# If already cloned, just rebuilds.
fsi-mcp-setup:
    if [ -d tools/fsi-mcp-server ]; then \
        echo "Already cloned, rebuilding..."; \
    else \
        git clone https://github.com/jovaneyck/fsi-mcp-server.git tools/fsi-mcp-server; \
    fi
    cd tools/fsi-mcp-server && sed -i 's/net9.0/net10.0/' server/fsi-mcp-server.fsproj
    dotnet build tools/fsi-mcp-server

# Start the FSI MCP server on http://localhost:5020/sse
# Run this in a separate terminal before or during opencode sessions.
fsi-mcp:
    dotnet run --project tools/fsi-mcp-server/server

# ─── Housekeeping ────────────────────────────────────────────────────────────

# Remove all build artefacts
clean:
    dotnet clean SmolVm.Fable.fsproj
    dotnet clean tests/SmolVm.Fable.Tests/SmolVm.Fable.Tests.fsproj
    rm -rf tests/SmolVm.Fable.Tests/.fable
    rm -rf tests/SmolVm.Fable.Tests/node_modules
    rm -f tests/SmolVm.Fable.Tests/*.fs.js
    rm -rf tests/SmolVm.Fable.Tests/fable_modules
