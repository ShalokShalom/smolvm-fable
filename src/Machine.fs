module SmolVm.Machine

open Fable.Core
open Fable.Core.JS
open SmolVm.Types
open SmolVm.Execution
open SmolVm.Container

// ============================================================================
// Raw JS imports  —  map directly onto machine.ts exports
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
    abstract createContainer  : options: obj         -> Promise<obj>
    abstract listContainers   : unit               -> Promise<obj[]>
    abstract getContainer     : id: string          -> Promise<obj>
    abstract listImages       : unit               -> Promise<ImageInfo[]>
    abstract pullImage        : image: string * ?ociPlatform: string -> Promise<ImageInfo>

    /// Static factory — Machine.create(config) in JS.
    [<Emit("Machine.create($1)")>]
    abstract create : config: obj -> Promise<JsMachine>

/// Raw JS helper functions exported from machine.ts
[<Import("withMachine", "smolvm")>]
let jsWithMachine : obj -> (JsMachine -> Promise<obj>) -> Promise<obj> = jsNative

[<Import("quickExec", "smolvm")>]
let jsQuickExec : string array -> obj option -> Promise<obj> = jsNative

[<Import("quickRun", "smolvm")>]
let jsQuickRun : string -> string array -> obj option -> Promise<obj> = jsNative

// ============================================================================
// Helpers to build JS option objects
// ============================================================================

let private toEnvVars (env: Map<string,string> option) =
    env
    |> Option.map (fun m ->
        m |> Map.toArray |> Array.map (fun (k,v) -> {| name = k; value = v |}))

let private execOptsToJs (o: ExecOptions) : obj =
    upcast {| env         = toEnvVars o.env
              workdir     = o.workdir
              timeoutSecs = o.timeout |}

let private machineConfigToJs (cfg: MachineConfig) : obj =
    upcast {| name      = cfg.name
              serverUrl = cfg.serverUrl
              mounts    = cfg.mounts
              ports     = cfg.ports
              resources = cfg.resources |}

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
    [<Emit("Machine.create($1)")>]
    static member Create(config: MachineConfig) : Promise<Machine> =
        promise {
            let cfg = machineConfigToJs config
            let! jsM = js.create cfg  // resolved via Emit on the static JS factory
            return Machine(jsM)
        }

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
    /// `tail` limits the number of lines returned (verified in upstream client.ts).
    /// Mirrors Machine.logs(options?) in machine.ts.
    member _.Logs(?options: LogsOptions) : JS.AsyncIterableIterator<string> =
        let optsObj =
            options
            |> Option.map (fun o ->
                {| follow = o.follow
                   since  = o.since
                   tail   = o.tail |})
        match optsObj with
        | None   -> unbox (js.logs())
        | Some o -> unbox (js.logs(upcast o))

    // -----------------------------------------------------------------------
    // Containers
    // -----------------------------------------------------------------------

    /// Create a container inside the machine.
    /// Mirrors Machine.createContainer(options) in machine.ts.
    member _.CreateContainer(options: ContainerOptions) : Promise<Container> =
        promise {
            let env = toEnvVars options.env
            let optsObj =
                {| image   = options.image
                   command = options.command
                   env     = env
                   workdir = options.workdir
                   mounts  = options.mounts |}
            let! raw = js.createContainer(upcast optsObj)
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
let withMachine (config: MachineConfig) (fn: Machine -> JS.Promise<'T>) : JS.Promise<'T> =
    promise {
        let! m = Machine.Create(config)
        try
            return! fn m
        finally
            try m.Stop() |> ignore with _ -> ()
            try m.Delete() |> ignore with _ -> ()
    }

/// Create a temporary machine, run a bare command, and clean up.
/// Mirrors quickExec(command, options?) in machine.ts.
let quickExec (command: string[]) (?options: MachineConfig) : JS.Promise<ExecResult> =
    promise {
        let cfg =
            options
            |> Option.defaultValue
                { name = sprintf "quick-exec-%d" (int (JS.Date.now()))
                  serverUrl = None; mounts = None; ports = None; resources = None }
        return! withMachine cfg (fun m -> m.Exec(command))
    }

/// Create a temporary machine, run a command in a container image, and clean up.
/// Mirrors quickRun(image, command, options?) in machine.ts.
let quickRun (image: string) (command: string[]) (?options: MachineConfig) : JS.Promise<ExecResult> =
    promise {
        let cfg =
            options
            |> Option.defaultValue
                { name = sprintf "quick-run-%d" (int (JS.Date.now()))
                  serverUrl = None; mounts = None; ports = None; resources = None }
        return! withMachine cfg (fun m -> m.Run(image, command))
    }
