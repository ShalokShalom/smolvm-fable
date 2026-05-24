module SmolVm.Types

open Fable.Core

type PortSpec =
    { host  : int
      guest : int }

type MountSpec =
    { hostPath  : string
      tag       : string
      readOnly  : bool option }

type ResourceSpec =
    { vcpus  : int option
      memory : int option }

/// Passed directly to Machine.create() in JS — Fable serialises records to
/// plain JS objects by default, so no manual conversion is needed.
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

type ExecOptions =
    { env     : Map<string,string> option
      workdir : string option
      timeout : int option }

/// Mirrors LogsQuery used by client.ts streamLogs.
/// `tail` is supported by the HTTP client even though SDK types.ts omits it.
type LogsOptions =
    { since  : string option
      follow : bool option
      tail   : int option }

/// Mirrors ContainerMountSpec: tag is the virtiofs tag,
/// target is the container-side mount path.
type ContainerMount =
    { tag      : string
      target   : string
      readOnly : bool option }

/// SDK-level container creation options.
type ContainerOptions =
    { image   : string
      command : string[] option
      env     : Map<string,string> option
      workdir : string option
      mounts  : ContainerMount[] option }

type CodeOptions =
    { env     : Map<string,string> option
      workdir : string option
      timeout : int option
      image   : string option }

type MountInfo =
    { hostPath : string
      tag      : string
      readOnly : bool }

type MachineInfo =
    { name      : string
      state     : string
      mounts    : MountInfo[] }

type ListMachinesResponse =
    { machines : MachineInfo[] }

type ExecResponse =
    { exitCode : int
      stdout   : string
      stderr   : string }

type ImageInfo =
    { id       : string
      name     : string
      size     : int64 }

type ListImagesResponse =
    { images : ImageInfo[] }

type PullImageResponse =
    { image : ImageInfo }

type DeleteResponse =
    { deleted : bool }

type ContainerInfo =
    { id        : string
      image     : string
      command   : string[]
      state     : string
      createdAt : float }

type ListContainersResponse =
    { containers : ContainerInfo[] }

type ApiErrorResponse =
    { error : string
      code  : string option }

type HealthResponse =
    { status : string }
