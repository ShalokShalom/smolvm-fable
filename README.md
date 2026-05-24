# smolvm-fable

Idiomatic **F# / [Fable 5](https://fable.io)** bindings for the
[smolvm Node.js SDK](https://github.com/smol-machines/smolvm-sdk/tree/main/smolvm-node).

This library is a **Fable-compatible wrapper** around the upstream `smolvm`
npm package. It exposes the core `smolvm-node` API as typed F# with
idiomatic naming conventions and no unsafe JS-object leakage, plus a
Node.js-oriented convenience preset for common JavaScript workloads.

## Status

| Area | State |
|---|---|
| Core binding (`Types`, `Execution`, `Container`, `Machine`) | ✅ Stable |
| Node preset (`NodeMachine`) | ✅ Stable |
| Error hierarchy (`SmolvmError` … `ExecutionError`) | ✅ Stable |
| NuGet package | 🚧 Not yet published |

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) or later
- [Node.js 20+](https://nodejs.org/)
- [Fable 5](https://fable.io) CLI (`dotnet tool install --global fable`)
- [just](https://github.com/casey/just) command runner (optional)

## Build & Test

```bash
# Restore NuGet packages
dotnet restore

# Build the binding library
dotnet build SmolVm.Fable.fsproj

# Run all 54 snapshot tests (on .NET)
dotnet run --project tests/SmolVm.Fable.Tests/SmolVm.Fable.Tests.fsproj

# Transpile to JavaScript via Fable 5 and run on Node
fable tests/SmolVm.Fable.Tests/SmolVm.Fable.Tests.fsproj --noCache
node tests/SmolVm.Fable.Tests/.fable/Main.js
```

Or use the `justfile`:

```bash
just build         # restore + build
just test          # run .NET tests
just build-js      # Fable transpile
just test-js       # run Node tests
just test-all      # both
```

### Demo

A working demo is in `demo/Demo.fs` — it creates a Machine via the smolvm SDK
bindings and runs Node.js code:

```bash
cd demo
npm install smolvm
fable --noCache
node Demo.fs.js
```

Requires a running smolvm daemon (default: `http://127.0.0.1:8080`).

## FSI MCP Server

This project includes an optional [FSI MCP Server](https://github.com/jovaneyck/fsi-mcp-server) integration for AI-assisted F# development. It exposes F# Interactive as an MCP tool so that AI agents can evaluate code, load scripts, and inspect REPL output in the same session you're working in.

### One-time setup

```bash
just fsi-mcp-setup
```

### Usage

1. Start the FSI MCP server in a separate terminal:
   ```bash
   just fsi-mcp
   ```
   The server runs on `http://localhost:5020/sse`.

2. Start opencode in your project. It will connect to the running server and expose the `send_fsharp_code`, `load_f_sharp_script`, `get_recent_fsi_events`, and `get_fsi_status` tools.

See `AGENTS.md` for the complete AI agent guide and MCP tool reference.
