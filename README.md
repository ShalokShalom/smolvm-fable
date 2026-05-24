# smolvm-fable

Idiomatic **F# / [Fable](https://fable.io)** bindings for the
[smolvm Node.js SDK](https://github.com/smol-machines/smolvm-sdk/tree/main/smolvm-node).

This library is a **Fable-compatible wrapper** around the upstream `smolvm`
npm package. It exposes every public API from `smolvm-node` as typed F# with
idiomatic naming conventions and no unsafe JS-object leakage.

> Upstream source inspected at commit `d9872f1` of
> [smol-machines/smolvm-sdk](https://github.com/smol-machines/smolvm-sdk).

---

## Status

| Area | State |
|---|---|
| Core binding (`Types`, `Execution`, `Container`, `Machine`) | ✅ Stable |
| Language presets (`PythonMachine`, `NodeMachine`) | ✅ Stable |
| Error hierarchy (`SmolvmError` … `ExecutionError`) | ✅ Stable |
| Tests (Scriptorium.Quill snapshot suite) | ✅ Compiles & passes |
| NuGet package | 🚧 Not yet published |

---

## Installation

### 1. Add the npm dependency

```bash
npm install smolvm
# or
pnpm add smolvm
```

### 2. Reference this library in your Fable project

Add a `<ProjectReference>` or `<PackageReference>` pointing to
`SmolVm.Fable.fsproj` (or the NuGet package once published), then add
`Fable.Core` as a NuGet dependency:

```xml
<PackageReference Include="Fable.Core" Version="4.*" />
```

---

## Quick Start

```fsharp
open Fable.Core.JS
open SmolVm.Machine
open SmolVm.Types

let run () = promise {
    // Create a machine (maps to Machine.create({ name: '...' }) in JS)
    let! machine =
        Machine.Create
            { name      = "hello"
              serverUrl = None
              mounts    = None
              ports     = None
              resources = None }

    // Run a bare command in the VM
    let! result = machine.Exec([| "echo"; "Hello from F#!" |])
    printfn "%s" result.Stdout

    // Run a command in an OCI image
    let! r2 = machine.Run("alpine:latest", [| "uname"; "-a" |])
    printfn "%s" r2.Stdout

    do! machine.Stop()
    do! machine.Delete()
}
```

### Resource-safe wrapper

```fsharp
open SmolVm.Machine
open SmolVm.Types

let cfg =
    { name      = "worker"
      serverUrl = None
      mounts    = None
      ports     = None
      resources = None }

withMachine cfg (fun m -> promise {
    let! r = m.Exec([| "date" |])
    printfn "%s" r.Stdout
})
|> Promise.start
```

### Python preset

```fsharp
open SmolVm.Presets.Python
open SmolVm.Types

let runPython () = promise {
    let! py = PythonMachine.Create { name = "py-runner"; serverUrl = None; mounts = None; ports = None; resources = None }
    let! r  = py.RunCode("print('hello from Python!')")
    printfn "%s" r.Stdout
    do! py.Stop()
    do! py.Delete()
}
```

### Node.js preset

```fsharp
open SmolVm.Presets.Node
open SmolVm.Types

let runNode () = promise {
    let! nd = NodeMachine.Create { name = "node-runner"; serverUrl = None; mounts = None; ports = None; resources = None }
    let! r  = nd.RunCode("console.log('hello from Node!')")
    printfn "%s" r.Stdout
    do! nd.Stop()
    do! nd.Delete()
}
```

---

## API Reference

### `MachineConfig`

| Field | Type | Required | Description |
|---|---|---|---|
| `name` | `string` | ✓ | Unique name for the machine |
| `serverUrl` | `string option` | — | smolvm daemon URL (default `http://127.0.0.1:8080`) |
| `mounts` | `MountSpec[] option` | — | Host mounts (virtiofs) |
| `ports` | `PortSpec[] option` | — | Host→guest port mappings |
| `resources` | `ResourceSpec option` | — | CPU / memory limits |

### `ResourceSpec`

| Field | Type | Description |
|---|---|---|
| `vcpus` | `int option` | Number of virtual CPUs |
| `memory` | `int option` | Memory in MiB |

### `Machine`

#### Static factory

| Method | JS equivalent | Description |
|---|---|---|
| `Machine.Create(config)` | `Machine.create(config)` | Create and start a machine |

#### Lifecycle

| Method | JS equivalent | Description |
|---|---|---|
| `machine.Start()` | `machine.start()` | Start (idempotent) |
| `machine.Stop()` | `machine.stop()` | Stop the machine |
| `machine.Delete()` | `machine.delete()` | Delete the machine |
| `machine.Status()` | `machine.status()` | Fetch current `MachineInfo` |

#### Properties

| Property | Type | JS equivalent | Description |
|---|---|---|---|
| `machine.Name` | `string` | `machine.name` | Machine name |
| `machine.IsStarted` | `bool` | `machine.isStarted` | Whether the machine has been started |
| `machine.State` | `MachineState option` | `machine.state` | Typed lifecycle state |
| `machine.Mounts` | `MountInfo[]` | `machine.mounts` | Attached mounts |
| `machine.Info` | `MachineInfo option` | `machine.info` | Last known machine info |

#### Execution

| Method | Signature | JS equivalent | Description |
|---|---|---|---|
| `machine.Exec` | `string[] → ?ExecOptions → Promise<ExecResult>` | `machine.exec(cmd, opts?)` | Run a bare VM command |
| `machine.Run` | `string → string[] → ?ExecOptions → Promise<ExecResult>` | `machine.run(image, cmd, opts?)` | Run inside an OCI image |

#### Logs

| Method | Signature | JS equivalent | Description |
|---|---|---|---|
| `machine.Logs` | `?LogsOptions → AsyncIterableIterator<string>` | `machine.logs(opts?)` | Stream machine logs via SSE |

#### Containers

| Method | Signature | JS equivalent | Description |
|---|---|---|---|
| `machine.CreateContainer` | `ContainerOptions → Promise<Container>` | `machine.createContainer(opts)` | Create a container |
| `machine.ListContainers` | `unit → Promise<Container[]>` | `machine.listContainers()` | List containers |
| `machine.GetContainer` | `string → Promise<Container>` | `machine.getContainer(id)` | Get container by ID |

#### Images

| Method | Signature | JS equivalent | Description |
|---|---|---|---|
| `machine.ListImages` | `unit → Promise<ImageInfo[]>` | `machine.listImages()` | List OCI images |
| `machine.PullImage` | `string → ?string → Promise<ImageInfo>` | `machine.pullImage(img, plat?)` | Pull an OCI image |

---

### `ExecOptions`

| Field | Type | Description |
|---|---|---|
| `env` | `Map<string,string> option` | Environment variables |
| `workdir` | `string option` | Working directory |
| `timeout` | `int option` | Timeout in seconds |

### `ExecResult`

| Member | Type | JS equivalent | Description |
|---|---|---|---|
| `ExitCode` | `int` | `result.exitCode` | Process exit code |
| `Stdout` | `string` | `result.stdout` | Standard output |
| `Stderr` | `string` | `result.stderr` | Standard error |
| `Success` | `bool` | `result.success` | `true` when exit code = 0 |
| `Output` | `string` | `result.output` | stdout + stderr combined |
| `AssertSuccess()` | `unit → ExecResult` | `result.assertSuccess()` | Raises `ExecutionError` if failed |

---

### `Container`

| Member | Signature | JS equivalent | Description |
|---|---|---|---|
| `Id` | `string` | `container.id` | Container ID |
| `State` | `ContainerState` | `container.state` | Typed lifecycle state |
| `Image` | `string` | `container.image` | OCI image reference |
| `Command` | `string[]` | `container.command` | Container command |
| `CreatedAt` | `float` | `container.createdAt` | Unix timestamp |
| `Info` | `ContainerInfo` | `container.info` | Raw API info |
| `Start()` | `unit → Promise<unit>` | `container.start()` | Start container |
| `Stop(?timeout)` | `?int → Promise<unit>` | `container.stop(t?)` | Stop container |
| `Delete(?force)` | `?bool → Promise<unit>` | `container.delete(f?)` | Delete container |
| `Exec(cmd, ?opts)` | `string[] → ?ExecOptions → Promise<ExecResult>` | `container.exec(cmd, opts?)` | Execute command |
| `Refresh()` | `unit → Promise<ContainerInfo>` | `container.refresh()` | Refresh info |

---

### `PythonMachine`

Extends `Machine` with Python conveniences.

| Method | Signature | JS equivalent |
|---|---|---|
| `PythonMachine.Create(cfg)` | `MachineConfig → Promise<PythonMachine>` | `PythonMachine.create(cfg)` |
| `RunCode(code, ?opts)` | `string → ?CodeOptions → Promise<ExecResult>` | `runCode(code, opts?)` |
| `RunFile(path, ?opts)` | `string → ?CodeOptions → Promise<ExecResult>` | `runFile(path, opts?)` |
| `Pip(pkgs, ?opts)` | `string[] → ?ExecOptions → Promise<ExecResult>` | `pip(pkgs, opts?)` |
| `RunWithSetup(setup, main, ?opts)` | `string → string → ?CodeOptions → Promise<ExecResult>` | `runWithSetup(...)` |
| `Version(?opts)` | `?CodeOptions → Promise<string>` | `version(opts?)` |
| `ListPackages(?opts)` | `?ExecOptions → Promise<string[]>` | `listPackages(opts?)` |

---

### `NodeMachine`

Extends `Machine` with Node.js conveniences.

| Method | Signature | JS equivalent |
|---|---|---|
| `NodeMachine.Create(cfg)` | `MachineConfig → Promise<NodeMachine>` | `NodeMachine.create(cfg)` |
| `RunCode(code, ?opts)` | `string → ?CodeOptions → Promise<ExecResult>` | `runCode(code, opts?)` |
| `RunFile(path, ?opts)` | `string → ?CodeOptions → Promise<ExecResult>` | `runFile(path, opts?)` |
| `Npm(args, ?opts)` | `string[] → ?ExecOptions → Promise<ExecResult>` | `npm(args, opts?)` |
| `NpmInstall(pkgs, ?opts)` | `string[] → ?ExecOptions → Promise<ExecResult>` | `npmInstall(pkgs, opts?)` |
| `Npx(args, ?opts)` | `string[] → ?ExecOptions → Promise<ExecResult>` | `npx(args, opts?)` |
| `Version(?opts)` | `?CodeOptions → Promise<string>` | `version(opts?)` |
| `RunESM(code, ?opts)` | `string → ?CodeOptions → Promise<ExecResult>` | `runESM(code, opts?)` |
| `Evaluate(expr, ?opts)` | `string → ?CodeOptions → Promise<ExecResult>` | `evaluate(expr, opts?)` |

---

### Error types

| F# exception | JS class | HTTP status | Description |
|---|---|---|---|
| `SmolvmError(msg,code,status)` | `SmolvmError` | — | Base error |
| `NotFoundError msg` | `NotFoundError` | 404 | Resource not found |
| `ConflictError msg` | `ConflictError` | 409 | Duplicate / conflict |
| `BadRequestError msg` | `BadRequestError` | 400 | Invalid request |
| `TimeoutError msg` | `TimeoutError` | 408 | Operation timed out |
| `InternalError msg` | `InternalError` | 500 | Daemon error |
| `ConnectionError msg` | `ConnectionError` | 0 | Network failure |
| `ExecutionError(code,out,err)` | `ExecutionError` | — | Non-zero exit code |

---

### Helper functions

| Function | Signature | JS equivalent | Description |
|---|---|---|---|
| `withMachine cfg fn` | `MachineConfig → (Machine → Promise<'T>) → Promise<'T>` | `withMachine(cfg, fn)` | RAII-style machine scope |
| `quickExec cmd ?cfg` | `string[] → ?MachineConfig → Promise<ExecResult>` | `quickExec(cmd, opts?)` | One-shot bare command |
| `quickRun img cmd ?cfg` | `string → string[] → ?MachineConfig → Promise<ExecResult>` | `quickRun(img, cmd, opts?)` | One-shot OCI command |

---

## Implementation notes

### F# `option` and the JS boundary

F# `option` values are discriminated unions.  If passed directly into JS
object literals they arrive as `{ tag: "Some", fields: [x] }` — which the
smolvm daemon will not understand.  Every helper that builds a plain JS object
for the SDK call uses `Option.toObj` (reference types → `null`) or
`Option.toNullable` (value types such as `int` → `Nullable`) to strip the DU
wrapper before crossing the boundary.

### `Machine.Create` vs. `new Machine()`

The upstream JS SDK exposes only a static `Machine.create(config)` factory —
there is no public constructor.  The Fable binding mirrors this with a static
`Machine.Create(config)` member that uses `emitJsExpr` to call the JS static
method and then wraps the resulting `JsMachine` in the F# `Machine` type.

### Async model

All async operations return `Fable.Core.JS.Promise<'T>`.  Use the `promise {
… }` computation expression (from `Fable.Core.JS`) to sequence them
idiomatically.

---

## Running the tests

```bash
cd tests/SmolVm.Fable.Tests
dotnet run
```

Tests use [Scriptorium.Quill](https://github.com/ShalokShalom/Scriptorium) for
snapshot and assertion testing.  Snapshots live in
`tests/SmolVm.Fable.Tests/__snapshots__/` and are committed to source control.

---

## Project layout

```
SmolVm.Fable.fsproj          # Library project
src/
  Types.fs                   # All record / DU types (no JS interop)
  Errors.fs                  # F# exception hierarchy mirroring JS errors
  Execution.fs               # ExecResult wrapper
  Container.fs               # Container class
  Machine.fs                 # Machine class + withMachine / quickExec / quickRun
  Presets/
    Python.fs                # PythonMachine
    Node.fs                  # NodeMachine
tests/
  SmolVm.Fable.Tests/
    *.fs                     # Snapshot & assertion tests
```

---

## Source attribution

This binding was derived entirely from the TypeScript sources of
[smol-machines/smolvm-sdk](https://github.com/smol-machines/smolvm-sdk)
at commit `d9872f1ca2b7872ea60a4167addc8f536263b0a0`:

- `smolvm-node/src/types.ts` — type definitions
- `smolvm-node/src/machine.ts` — Machine class + helpers
- `smolvm-node/src/container.ts` — Container class
- `smolvm-node/src/execution.ts` — ExecResult + ExecutionError
- `smolvm-node/src/errors.ts` — error hierarchy
- `smolvm-node/src/logs.ts` — SSE streaming utilities
- `smolvm-node/src/presets/python.ts` — PythonMachine preset
- `smolvm-node/src/presets/node.ts` — NodeMachine preset

## License

MIT
