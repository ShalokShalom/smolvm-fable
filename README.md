# smolvm-fable

Idiomatic **F# / [Fable 5](https://fable.io)** bindings for
[smolvm](https://github.com/smol-machines/smolvm-sdk/).

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

## Quick Start

### Basic Usage

Create a machine, execute a command, and clean up:

```fsharp
open SmolVm.Types
open SmolVm.Machine
open SmolVm.Execution

let config =
    { name      = "my-machine"
      serverUrl = None      // defaults to http://127.0.0.1:8080
      mounts    = None
      ports     = None
      resources = None }

async {
    let! m = Machine.Create config |> Async.AwaitPromise
    try
        let! echo = m.Exec([| "echo"; "Hello, World!" |]) |> Async.AwaitPromise
        printfn "%s" echo.Stdout 

        // Run a command in a container image
        let! py   = m.Run("python:3.12", [| "python"; "-c"; "print(2+2)" |]) |> Async.AwaitPromise
        printfn "%s" py.Stdout
    finally
        do! m.Stop()   |> Async.AwaitPromise
        do! m.Delete() |> Async.AwaitPromise
} |> Async.StartAsPromise |> ignore
```

### Helper Functions

For short-lived tasks, use `withMachine` for automatic cleanup:

```fsharp
open SmolVm.Machine

let result =
    withMachine config (fun m ->
        async {
            let! r = m.Exec([| "uname"; "-a" |]) |> Async.AwaitPromise
            return r
        } |> Async.StartAsPromise
    ) |> Async.AwaitPromise |> Async.RunSynchronously

printfn "%s" result.Stdout
// Machine is automatically stopped and deleted
```

`quickExec` and `quickRun` create a throw-away machine for single commands:

```fsharp
open SmolVm.Machine

let! r = quickExec [| "whoami" |] None |> Async.AwaitPromise
printfn "%s" r.Stdout
```

### Node.js Sandbox

Run JavaScript code with the Node preset:

```fsharp
open SmolVm.Presets.Node

async {
    let! nm = NodeMachine.Create config |> Async.AwaitPromise
    try
        // Run JavaScript source
        let! js = nm.RunCode "console.log(process.version); console.log(process.platform)"
        printfn "%s" js.Stdout

        // Run ES modules
        let! esm = nm.RunESM "const msg = 'Hello from ESM!'; console.log(msg)"
        printfn "%s" esm.Stdout

        // Use npm
        let! npm = nm.Npm([| "init"; "-y" |])
        printfn "%s" npm.Stdout
    finally
        do! nm.Stop()   |> Async.AwaitPromise
        do! nm.Delete() |> Async.AwaitPromise
} |> Async.StartAsPromise |> ignore
```

### Error Handling

The binding provides active patterns for matching SDK errors:

```fsharp
open SmolVm.Errors

try
    let! r = m.Exec([| "some-command" |]) |> Async.AwaitPromise
    r.AssertSuccess() |> ignore
with
| NotFoundErr  e  -> printfn "Resource not found: %s" e.message
| TimeoutErr   e  -> printfn "Command timed out: %s" e.message
| ConnectionErr e -> printfn "Daemon unreachable: %s" e.message
| SmolvmErr    e  -> printfn "SDK error [%s]: %s" e.code e.message
```

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) or later
- [Node.js 20+](https://nodejs.org/)
- [Fable 5](https://fable.io) CLI (`dotnet tool install --global fable`)
- [just](https://github.com/casey/just) command runner (optional)

## Build & Test

```bash
# Restore NuGet packages
dotnet restore

# Build the binding library (0 errors, 0 warnings)
dotnet build SmolVm.Fable.fsproj

# Run all 54 snapshot tests (on .NET)
dotnet run --project tests/SmolVm.Fable.Tests/SmolVm.Fable.Tests.fsproj

# Transpile the demo to JavaScript
cd demo && fable --noCache && node Demo.fs.js
```

Or use the `justfile`:

```bash
just build         # restore + build
just test          # run .NET tests (54 pass)
just test-js       # build JS + run Node tests (main library only — snapshot tests are .NET-only)
```

> **Note**: `just test-js` compiles and runs the library's unit tests on Node.
> Snapshot tests (`MachineConfigTests`, `ExecResultTests`, etc.) are **.NET-only**
> because `Scriptorium.Nib.Snapshot` does not ship Fable JS runtime files.

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

This project includes an optional [FSI MCP Server](https://github.com/jovaneyck/fsi-mcp-server) integration for AI-assisted F# development.   
It exposes F# Interactive as an MCP tool so that AI agents can evaluate code, load scripts, and inspect REPL output in the same session you're working in.

### One-time setup

```bash
just fsi-mcp-setup
```

It is meant to work with the Opencode Desktop or CLI app.

### Usage

1. Start the FSI MCP server in a separate terminal:
   ```bash
   just fsi-mcp
   ```
   The server runs on `http://localhost:5020/sse`.

2. Start opencode in your project. It will connect to the running server and expose the `send_fsharp_code`, `load_f_sharp_script`, `get_recent_fsi_events`, and `get_fsi_status` tools.

See `AGENTS.md` for the complete AI agent guide and MCP tool reference.
