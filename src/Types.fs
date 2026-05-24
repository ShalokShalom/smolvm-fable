module SmolVm.Types

open Fable.Core

// ============================================================================
// Primitive alias types re-exported from OpenAPI models
// (kept as plain F# records / discriminated unions for ergonomics)
// ============================================================================

/// A single environment variable name/value pair.
[<Pojo>]
type EnvVar =
    { name  : string
      value : string }

/// A host→guest port mapping.
[<Pojo>]
type PortSpec =
    { host  : int
      guest : int }

/// A host mount specification.
/// `tag` is the virtiofs tag used to reference the mount inside the VM / container.
[<Pojo>]
type MountSpec =
    { /// Path on the host
      hostPath  : string
      /// virtiofs tag used inside the VM
      tag       : string
      /// Whether the mount is read-only
      readOnly  : bool option }

/// VM resource limits.
[<Pojo>]
type ResourceSpec =
    { /// Number of vCPUs
      vcpus  : int option
      /// Memory in MiB
      memory : int option }

// ============================================================================
// SDK-specific configuration types
// (mirror of MachineConfig / ExecOptions / ContainerOptions in types.ts)
// ============================================================================

/// Configuration used when creating a machine.
[<Pojo>]
type MachineConfig =
    { /// Unique name for the machine
      name      : string
      /// smolvm daemon URL (default: "http://127.0.0.1:8080")
      serverUrl : string option
      /// Host mounts to attach
      mounts    : MountSpec[] option
      /// Port mappings (host→guest)
      ports     : PortSpec[] option
      /// VM resource configuration
      resources : ResourceSpec option }

/// Machine lifecycle state.
/// Cases are prefixed to avoid name collisions with ContainerState.
type MachineState =
    | MachineCreated
    | MachineRunning
    | MachineStopped

/// Container lifecycle state.
/// Cases are prefixed to avoid name collisions with MachineState.
type ContainerState =
    | ContainerCreated
    | ContainerRunning
    | ContainerStopped

/// Options for command execution.
[<Pojo>]
type ExecOptions =
    { /// Environment variables as a string→string map
      env     : Map<string,string> option
      /// Working directory
      workdir : string option
      /// Timeout in seconds
      timeout : int option }

/// Options for log streaming.
/// Mirrors LogsOptions / LogsQuery in types.ts and client.ts.
[<Pojo>]
type LogsOptions =
    { /// Only return logs after this timestamp (ISO-8601)
      since  : string option
      /// Whether to follow the log stream
      follow : bool option
      /// Number of tail lines to return
      tail   : int option }

/// A single volume mount for a container (source = virtiofs tag).
/// Mirrors ContainerMountSpec in the generated OpenAPI models.
[<Pojo>]
type ContainerMount =
    { /// virtiofs tag from the parent machine’s MountSpec
      source   : string
      /// Target path inside the container
      target   : string
      readOnly : bool option }

/// Options for container creation.
/// Mirrors ContainerOptions in types.ts (workdir + mounts only).
[<Pojo>]
type ContainerOptions =
    { /// OCI image reference (e.g. "python:3.12-alpine")
      image   : string
      /// Command to run inside the container
      command : string[] option
      /// Environment variables
      env     : Map<string,string> option
      /// Working directory
      workdir : string option
      /// Volume mounts
      mounts  : ContainerMount[] option }

/// Code-execution options (extends ExecOptions with an image override).
[<Pojo>]
type CodeOptions =
    { /// Inherited from ExecOptions
      env     : Map<string,string> option
      workdir : string option
      timeout : int option
      /// Override the default OCI image for code execution
      image   : string option }

// ============================================================================
// API response types (mirrors of generated OpenAPI models)
// ============================================================================

/// Info returned by the API for a machine mount.
[<Pojo>]
type MountInfo =
    { hostPath : string
      tag      : string
      readOnly : bool }

/// Full machine information as returned by the API.
[<Pojo>]
type MachineInfo =
    { name      : string
      state     : string       // raw string from API; see MachineState DU
      mounts    : MountInfo[] }

/// Paginated list of machines.
[<Pojo>]
type ListMachinesResponse =
    { machines : MachineInfo[] }

/// Raw execution response from the API.
[<Pojo>]
type ExecResponse =
    { exitCode : int
      stdout   : string
      stderr   : string }

/// OCI image info.
[<Pojo>]
type ImageInfo =
    { id       : string
      name     : string
      size     : int64 }

/// Paginated list of images.
[<Pojo>]
type ListImagesResponse =
    { images : ImageInfo[] }

/// Response from a pull-image request.
[<Pojo>]
type PullImageResponse =
    { image : ImageInfo }

/// Generic delete response.
[<Pojo>]
type DeleteResponse =
    { deleted : bool }

/// Container info as returned by the API.
[<Pojo>]
type ContainerInfo =
    { id        : string
      image     : string
      command   : string[]
      state     : string       // raw string from API
      createdAt : float }

/// Paginated list of containers.
[<Pojo>]
type ListContainersResponse =
    { containers : ContainerInfo[] }

/// API error response body.
[<Pojo>]
type ApiErrorResponse =
    { error : string
      code  : string option }

/// Health-check response from the daemon.
[<Pojo>]
type HealthResponse =
    { status : string }
