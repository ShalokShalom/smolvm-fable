module SmolVm.Machine

open Fable.Core
open Fable.Core.JS
open SmolVm.Types
open SmolVm.Execution
open SmolVm.Container

// ============================================================================
// Raw JS import  —  maps directly onto the Machine class in machine.ts
// ============================================================================

/// Low-level Fable binding to the JS Machine class from machine.ts.
/// Prefer the ergonomic `Machine` wrapper below.
[<Import("Machine", "smolvm")>]
type JsMachine =
    abstract name      : string
    abstract start     : unit -> Promise<unit>
    abstract stop      : unit -> Promise<unit>
    abstract delete    : unit -> Promise<unit>
    abstract status    : unit -> Promise<MachineInfo>
    abstract isStarted : bool
    abstract state     : string option
    abstract mounts    : MountInfo[]
    abstract info      : MachineInfo option
    abstract exec      : command: string array * ?options: obj -> Promise<obj>
    abstract run       : image: string * command: string array * ?options: obj -> Promise<obj>
    abstract logs      : ?options: obj -> obj   // AsyncIterable<string> in JS
    abstract createContainer  : options: obj   -> Promise<obj>
    abstract listContainers   : unit           -> Promise<obj[]>
    abstract getContainer     : id: string     -> Promise<obj>
    abstract listImages       : unit           -> Promise<ImageInfo[]>
    abstract pullImage        : image: string * ?ociPlatform: string -> Promise<ImageInfo>

// ============================================================================
// Helpers to build plain JS objects
//
// IMPORTANT: F# option values must be unwrapped before crossing the JS
// boundary.  Inside anonymous records Fable emits them as discriminated-union
// objects ({ tag: "Some", fields: [x] }) which the JS SDK does not understand.
// Option.toObj  → null for None, the value for Some (reference types)
// Option.toNullable → Nullable for None (for value types such as int)
// ============================================================================

/// Convert a string→string map to the JS env-var array shape.
let private toEnvVars (env: Map<string,string> option) : obj =
    match env with
    | None   -> null
    | Some m ->
        m
        |> Map.toArray
        |> Array.map (fun (k,v) -> {| name = k; value = v |})
        |> box

/// Serialise ExecOptions to a plain JS object.
let private execOptsToJs (o: ExecOptions) : obj =
    upcast {| env         = toEnvVars o.env
              workdir     = Option.toObj o.workdir
              timeoutSecs = Option.toNullable o.timeout |}

/// Serialise LogsOptions to a plain JS object.
let private logsOptsToJs (o: LogsOptions) : obj =
    upcast {| follow = Option.toNullable o.follow
              since  = Option.toObj o.since
              tail   = Option.toNullable o.tail |}

/// Serialise MachineConfig to a plain JS object.
let private machineConfigToJs (cfg: MachineConfig) : obj =
    upcast {| name      = cfg.name
              serverUrl = Option.toObj cfg.serverUrl
              mounts    = Option.toObj cfg.mounts
              ports     = Option.toObj cfg.ports
              resources = Option.toObj cfg.resources |}

// ============================================================================
// Ergonomic F# wrapper around Machine
// Mirrors the Machine class from machine.ts
// ============================================================================

/// Manages the lifecycle of a smolvm microVM machine.
/// Wraps the JS Machine class with idiomatic F# types.
type Machine(js: JsMachine) =

    // -----------------------------------------------------------------------
    // Static factory
    // -----------------------------------------------------------------------

    /// Create a new machine and start it.
    /// Mirrors Machine.create(config) in machine.ts.
    ///
    /// Implementation note: emitJsExpr inlines `Machine.create(cfg)` at the
    /// call site so Fable resolves the JS class reference correctly.  We then
    /// map over the returned Promise<JsMachine> to wrap it in the F# Machine.
    static member Create(config: MachineConfig) : Promise<Machine> =
        let cfg = machineConfigToJs config
        let p : Promise<JsMachine> = emitJsExpr cfg "Machine.create($0)"
        p |> Promise.map Machine

    // -----------------------------------------------------------------------
    // Expose inner JsMachine for sub-types that need to delegate
    // -----------------------------------------------------------------------
    member internal _.Js = js

    // -----------------------------------------------------------------------
    // Identity / state
    // -----------------------------------------------------------------------

    /// Unique name of the machine.
    member _.Name : string = js.name

    /// Whether the machine has been started at least once in this session.
    /// Mirrors Machine.isStarted in machine.ts.
    member _.IsStarted : bool = js.isStarted

    /// Current lifecycle state as a typed DU value.
    /// Mirrors the Machine.state getter (raw string) in machine.ts.
    member _.State : MachineState option =
        js.state
        |> Option.map (function
            | "running" -> MachineRunning
            | "stopped" -> MachineStopped
            | _         -> MachineCreated)

    /// Mount info for all host mounts attached to this machine.
    /// Mirrors Machine.mounts in machine.ts.
    member _.Mounts : MountInfo[] = js.mounts

    /// Raw MachineInfo from the last API response.
    /// Mirrors Machine.info in machine.ts.
    member _.Info : MachineInfo option = js.info

    // -----------------------------------------------------------------------
    // Lifecycle
    // -----------------------------------------------------------------------

    /// Create and start the machine.
    /// If already running, this is a no-op.
    /// Mirrors Machine.start() in machine.ts.
    member _.Start() : Promise<unit> = js.start()

    /// Stop the machine.
    /// Mirrors Machine.stop() in machine.ts.
    member _.Stop() : Promise<unit> = js.stop()

    /// Delete the machine and all associated state.
    /// Mirrors Machine.delete() in machine.ts.
    member _.Delete() : Promise<unit> = js.delete()

    // -----------------------------------------------------------------------
    // Status
    // -----------------------------------------------------------------------

    /// Fetch current machine status from the daemon.
    /// Mirrors Machine.status() in machine.ts.
    member _.Status() : Promise<MachineInfo> = js.status()

    // -----------------------------------------------------------------------
    // Execution
    // -----------------------------------------------------------------------

    /// Execute a command directly in the machine VM (no container image).
    /// `options` may carry env, workdir, and timeout.
    /// Mirrors Machine.exec(command, options?) in machine.ts.
    member _.Exec(command: string[], ?options: ExecOptions) : Promise<ExecResult> =
        promise {
            let! raw =
                match options with
                | None   -> js.exec(command)
                | Some o -> js.exec(command, execOptsToJs o)
            return ExecResult(unbox<ExecResponse> raw)
        }

    /// Run a command inside an OCI container image within the machine.
    /// `image` is an OCI image reference (e.g. "alpine:latest").
    /// Mirrors Machine.run(image, command, options?) in machine.ts.
    member _.Run(image: string, command: string[], ?options: ExecOptions) : Promise<ExecResult> =
        promise {
            let! raw =
                match options with
                | None   -> js.run(image, command)
                | Some o -> js.run(image, command, execOptsToJs o)
            return ExecResult(unbox<ExecResponse> raw)
        }

    // -----------------------------------------------------------------------
    // Logs
    // -----------------------------------------------------------------------

    /// Stream logs from the machine as an AsyncIterable<string>.
    /// Set `follow = true` to tail the stream.
    /// `tail` limits the number of lines returned.
    /// Mirrors Machine.logs(options?) in machine.ts.
    member _.Logs(?options: LogsOptions) : JS.AsyncIterableIterator<string> =
        match options with
        | None   -> unbox (js.logs())
        | Some o -> unbox (js.logs(logsOptsToJs o))

    // -----------------------------------------------------------------------
    // Containers
    // -----------------------------------------------------------------------

    /// Create a container inside the machine.
    /// Mirrors Machine.createContainer(options) in machine.ts.
    member _.CreateContainer(options: ContainerOptions) : Promise<Container> =
        promise {
            let optsObj =
                upcast {| image   = options.image
                          command = Option.toObj options.command
                          env     = toEnvVars options.env
                          workdir = Option.toObj options.workdir
                          mounts  = Option.toObj options.mounts |}
            let! raw = js.createContainer(optsObj)
            return Container(unbox<JsContainer> raw)
        }

    /// List all containers in the machine.
    /// Mirrors Machine.listContainers() in machine.ts.
    member _.ListContainers() : Promise<Container[]> =
        promise {
            let! raws = js.listContainers()
            return raws |> Array.map (fun r -> Container(unbox<JsContainer> r))
        }

    /// Get a single container by its ID.
    /// Raises System.Exception if not found.
    /// Mirrors Machine.getContainer(id) in machine.ts.
    member _.GetContainer(id: string) : Promise<Container> =
        promise {
            let! raw = js.getContainer(id)
            return Container(unbox<JsContainer> raw)
        }

    // -----------------------------------------------------------------------
    // Images
    // -----------------------------------------------------------------------

    /// List all OCI images available in the machine.
    /// Mirrors Machine.listImages() in machine.ts.
    member _.ListImages() : Promise<ImageInfo[]> = js.listImages()

    /// Pull an OCI image into the machine.
    /// `ociPlatform` is optional (e.g. "linux/amd64").
    /// Mirrors Machine.pullImage(image, ociPlatform?) in machine.ts.
    member _.PullImage(image: string, ?ociPlatform: string) : Promise<ImageInfo> =
        match ociPlatform with
        | Some p -> js.pullImage(image, p)
        | None   -> js.pullImage(image)


// ============================================================================
// Module-level helper functions
// Mirrors withMachine, quickExec, quickRun from machine.ts
// ============================================================================

/// Create a machine, run an async computation with it, then stop and delete it.
/// This is the recommended pattern for short-lived tasks.
/// Mirrors withMachine(config, fn) in machine.ts.
///
/// Cleanup note: stop and delete are awaited sequentially so the daemon always
/// receives them in order.  Errors in `fn` are re-raised after cleanup.
let withMachine (config: MachineConfig) (fn: Machine -> Promise<'T>) : Promise<'T> =
    promise {
        let! m = Machine.Create(config)
        let! outcome =
            fn m
            |> Promise.catch (fun e -> promise { return raise e })
        do! m.Stop()
        do! m.Delete()
        return outcome
    }

/// Create a temporary machine, run a bare command, and clean up.
/// `config` is optional; when omitted a minimal ephemeral config is used.
/// Mirrors quickExec(command, options?) in machine.ts.
let quickExec (command: string[]) (config: MachineConfig option) : Promise<ExecResult> =
    let cfg =
        config
        |> Option.defaultValue
            { name      = sprintf "quick-exec-%d" (int (JS.Date.now()))
              serverUrl = None
              mounts    = None
              ports     = None
              resources = None }
    withMachine cfg (fun m -> m.Exec(command))

/// Create a temporary machine, run a command in a container image, and clean up.
/// `config` is optional; when omitted a minimal ephemeral config is used.
/// Mirrors quickRun(image, command, options?) in machine.ts.
let quickRun (image: string) (command: string[]) (config: MachineConfig option) : Promise<ExecResult> =
    let cfg =
        config
        |> Option.defaultValue
            { name      = sprintf "quick-run-%d" (int (JS.Date.now()))
              serverUrl = None
              mounts    = None
              ports     = None
              resources = None }
    withMachine cfg (fun m -> m.Run(image, command))
