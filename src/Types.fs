module SmolVm.Types

open Fable.Core

[<Pojo>]
type PortSpec =
    { host  : int
      guest : int }

[<Pojo>]
type MountSpec =
    { hostPath  : string
      tag       : string
      readOnly  : bool option }

[<Pojo>]
type ResourceSpec =
    { vcpus  : int option
      memory : int option }

/// Passed directly to Machine.create() in JS — Fable serialises [<Pojo>]
/// records to plain JS objects, so no manual conversion is needed.
[<Pojo>]
type MachineConfig =
    { name      : string
      serverUrl : string option
      mounts    : MountSpec[] option
      ports     : PortSpec[] option
      resources : ResourceSpec option }

type MachineState =
    | MachineCreated
    | MachineRunning
    | MachineStopped
    | MachineUnknown of string

type ContainerState =
    | ContainerCreated
    | ContainerRunning
    | ContainerStopped
    | ContainerUnknown of string

[<Pojo>]
type ExecOptions =
    { env     : Map<string,string> option
      workdir : string option
      timeout : int option }

/// Mirrors LogsQuery used by client.ts streamLogs.
/// `tail` is supported by the HTTP client even though SDK types.ts omits it.
[<Pojo>]
type LogsOptions =
    { since  : string option
      follow : bool option
      tail   : int option }

/// Mirrors ContainerMountSpec: tag is the virtiofs tag,
/// target is the container-side mount path.
[<Pojo>]
type ContainerMount =
    { tag      : string
      target   : string
      readOnly : bool option }

/// SDK-level container creation options.
/// Note: upstream types.ts ContainerOptions omits image/command but
/// machine.ts createContainer uses them from this object before calling
/// client.createContainer. The F# type follows actual usage.
[<Pojo>]
type ContainerOptions =
    { image   : string
      command : string[] option
      env     : Map<string,string> option
      workdir : string option
      mounts  : ContainerMount[] option }

[<Pojo>]
type CodeOptions =
    { env     : Map<string,string> option
      workdir : string option
      timeout : int option
      image   : string option }

[<Pojo>]
type MountInfo =
    { hostPath : string
      tag      : string
      readOnly : bool }

[<Pojo>]
type MachineInfo =
    { name      : string
      state     : string
      mounts    : MountInfo[] }

[<Pojo>]
type ListMachinesResponse =
    { machines : MachineInfo[] }

[<Pojo>]
type ExecResponse =
    { exitCode : int
      stdout   : string
      stderr   : string }

[<Pojo>]
type ImageInfo =
    { id       : string
      name     : string
      size     : int64 }

[<Pojo>]
type ListImagesResponse =
    { images : ImageInfo[] }

[<Pojo>]
type PullImageResponse =
    { image : ImageInfo }

[<Pojo>]
type DeleteResponse =
    { deleted : bool }

[<Pojo>]
type ContainerInfo =
    { id        : string
      image     : string
      command   : string[]
      state     : string
      createdAt : float }

[<Pojo>]
type ListContainersResponse =
    { containers : ContainerInfo[] }

[<Pojo>]
type ApiErrorResponse =
    { error : string
      code  : string option }

[<Pojo>]
type HealthResponse =
    { status : string }
