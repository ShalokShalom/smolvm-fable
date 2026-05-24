# smolvm-fable

Idiomatic **F# / [Fable](https://fable.io)** bindings for the
[smolvm Node.js SDK](https://github.com/smol-machines/smolvm-sdk/tree/main/smolvm-node).

This library is a **Fable-compatible wrapper** around the upstream `smolvm`
npm package. It exposes every public API from `smolvm-node` as typed F# with
idiomatic naming conventions.

> Upstream source inspected at commit `d9872f1` of
> [smol-machines/smolvm-sdk](https://github.com/smol-machines/smolvm-sdk).

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
    let! machine = Machine.Create { name = "hello"; serverUrl = None; mounts = None; ports = None; resources = None }

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

let cfg = { name = "worker"; serverUrl = None; mounts = None; ports = None; resources = None }

withMachine cfg (fun m -> promise {
    let! r = m.Exec([| "date" |])
    printfn "%s" r.Stdout
})
|> Promise.start
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
