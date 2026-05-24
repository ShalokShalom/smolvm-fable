# smolvm-fable — AI Agent Guide

## Project Overview

This is an **F# / Fable** binding library for the [smolvm Node.js SDK](https://github.com/smol-machines/smolvm-sdk/tree/main/smolvm-node). It provides typed F# wrappers around the smolvm API for creating and managing micro-VMs.

### Key files

| File | Purpose |
|---|---|
| `src/Types.fs` | Core F# types (MachineConfig, ExecOptions, etc.) |
| `src/Execution.fs` | ExecResult class + ExecutionError active pattern |
| `src/Machine.fs` | Machine wrapper + withMachine/quickExec/quickRun helpers |
| `src/Container.fs` | Container wrapper |
| `src/Presets/Node.fs` | NodeMachine preset (RunCode, Npm, Npx, etc.) |
| `src/Errors.fs` | Error hierarchy with active patterns |
| `src/Internal/Interop.fs` | JS interop serializers |
| `tests/SmolVm.Fable.Tests/` | Snapshot-based test suite (54 tests) |

### Build & Test

```bash
# Restore packages
dotnet restore

# Build main library only
dotnet build SmolVm.Fable.fsproj

# Run tests (all 54 pass)
dotnet run --project tests/SmolVm.Fable.Tests/SmolVm.Fable.Tests.fsproj

# Transpile to JavaScript via Fable 5
fable tests/SmolVm.Fable.Tests/SmolVm.Fable.Tests.fsproj --noCache

# Run transpiled tests on Node
node tests/SmolVm.Fable.Tests/.fable/Main.js
```

## FSI MCP Server (for AI-assisted development)

The project includes an **FSI MCP Server** that lets AI agents interact with an F# Interactive REPL session in real time.

### Starting the server

```bash
# One-time setup (clone + build)
just fsi-mcp-setup

# Start the server
just fsi-mcp
```

The server runs on `http://localhost:5020/sse`.

### Available MCP tools

| Tool | Description |
|---|---|
| `send_fsharp_code` | Send F# code to the REPL (`agentName: string`, `code: string`). End code lines with `;;` just like FSI. |
| `load_f_sharp_script` | Load and execute an `.fsx` script file |
| `get_recent_fsi_events` | Get recent REPL events with timestamps. Includes INPUT (what was sent), OUTPUT (what FSI printed), and EXCEPTION events. |
| `get_fsi_status` | Get session info, event count, available tools |

### Using with opencode

The `opencode.json` already has the MCP configured as a remote server. Once the server is running, opencode detects the tools automatically.

### Workflow example for AI agents

1. **Explore types**: Use `send_fsharp_code` to evaluate F# expressions and inspect types
2. **Test bindings**: Send small code snippets to verify F# code before editing project files
3. **Debug errors**: When compilation fails, use FSI to reproduce and understand the issue
4. **Verify fixes**: After making changes, run `dotnet run --project ...` to confirm tests pass

### F# Interactive tips

- End all statements with `;;` (double semicolon) as required by FSI
- Use `#r "path/to/dll"` to reference assemblies within the session
- Use `#load "file.fs"` to load source files
- Type definitions persist in the session until it's restarted
