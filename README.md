# smolvm-fable

Idiomatic **F# / [Fable](https://fable.io)** bindings for the
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

2. Start opencode in your project. It will connect to the running server and expose the `SendFSharpCode`, `LoadFSharpScript`, `GetRecentFsiEvents`, and `GetFsiStatus` tools.

You can use F# Interactive normally in your IDE while the AI agent shares the same REPL session.
