module SmolVm.Machine

open Fable.Core
open Fable.Core.JS
open Fable.Core.JsInterop
open SmolVm.Types
open SmolVm.Execution
open SmolVm.Container
open SmolVm.Internal.Interop

// ============================================================================
// JsMachine interface
//
// Mirrors the public surface of the Machine class exported from "smolvm".
// The static factory Machine.create(config) is bound via the import function
// and emitJsExpr to avoid fragile Emit strings that produce double-wrapped
// Promises.
// ============================================================================

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
    abstract logs      : ?options: obj -> JS.AsyncIterable<string>
    abstract createContainer : options: obj -> Promise<obj>
    abstract listContainers  : unit         -> Promise<obj[]>
    abstract getContainer    : id: string   -> Promise<obj>
    abstract listImages      : unit         -> Promise<ImageInfo[]>
    abstract pullImage       : image: string * ?ociPlatform: string -> Promise<ImageInfo>

/// Import Machine class to force Fable import statement.
let private _machineImport : JsMachine = import "Machine" "smolvm"

/// Static factory — calls Machine.create(config) from the smolvm SDK.
let private machineCreate (config: MachineConfig) : Promise<JsMachine> =
    emitJsExpr (config, _machineImport) "$1.create($0)"

type Machine(js: JsMachine) =

    /// Create and start a machine. Mirrors Machine.create(config) in JS.
    /// MachineConfig records are serialised as plain JS objects by Fable 5.
    static member Create(config: MachineConfig) : Promise<Machine> =
        async {
            let! m = Async.AwaitPromise(machineCreate config)
            return Machine(m)
        } |> Async.StartAsPromise

    member internal _.Js = js
    member _.Name      : string             = js.name
    member _.IsStarted : bool               = js.isStarted
    member _.Mounts    : MountInfo[]        = js.mounts
    member _.Info      : MachineInfo option = js.info

    member _.State : MachineState option =
        js.state
        |> Option.map (function
            | "created" -> MachineCreated
            | "running" -> MachineRunning
            | "stopped" -> MachineStopped
            | other      -> MachineUnknown other)

    member _.Start()  : Promise<unit>        = js.start()
    member _.Stop()   : Promise<unit>        = js.stop()
    member _.Delete() : Promise<unit>        = js.delete()
    member _.Status() : Promise<MachineInfo> = js.status()

    member _.Exec(command: string[], ?options: ExecOptions) : Promise<ExecResult> =
        async {
            let! raw =
                match options with
                | None   -> js.exec(command) |> Async.AwaitPromise
                | Some o -> js.exec(command, execOptsToJs o) |> Async.AwaitPromise
            return ExecResult(unbox<ExecResponse> raw)
        } |> Async.StartAsPromise

    member _.Run(image: string, command: string[], ?options: ExecOptions) : Promise<ExecResult> =
        async {
            let! raw =
                match options with
                | None   -> js.run(image, command) |> Async.AwaitPromise
                | Some o -> js.run(image, command, execOptsToJs o) |> Async.AwaitPromise
            return ExecResult(unbox<ExecResponse> raw)
        } |> Async.StartAsPromise

    member _.Logs(?options: LogsOptions) : JS.AsyncIterable<string> =
        match options with
        | None   -> js.logs()
        | Some o -> js.logs(logsOptsToJs o)

    member _.CreateContainer(options: ContainerOptions) : Promise<Container> =
        async {
            let! raw = js.createContainer(containerOptsToJs options) |> Async.AwaitPromise
            return Container(unbox<JsContainer> raw)
        } |> Async.StartAsPromise

    member _.ListContainers() : Promise<Container[]> =
        async {
            let! raws = js.listContainers() |> Async.AwaitPromise
            return raws |> Microsoft.FSharp.Collections.Array.map (fun r -> Container(unbox<JsContainer> r))
        } |> Async.StartAsPromise

    member _.GetContainer(id: string) : Promise<Container> =
        async {
            let! raw = js.getContainer(id) |> Async.AwaitPromise
            return Container(unbox<JsContainer> raw)
        } |> Async.StartAsPromise

    member _.ListImages() : Promise<ImageInfo[]> = js.listImages()

    member _.PullImage(image: string, ?ociPlatform: string) : Promise<ImageInfo> =
        match ociPlatform with
        | Some p -> js.pullImage(image, p)
        | None   -> js.pullImage(image)

// ============================================================================
// Module-level helpers
// ============================================================================

/// Run `fn` against a freshly created machine, then stop + delete it.
/// Cleanup runs even if `fn` raises; the original exception is re-raised.
let withMachine (config: MachineConfig) (fn: Machine -> Promise<'T>) : Promise<'T> =
    async {
        let! m = Machine.Create(config) |> Async.AwaitPromise
        try
            let! outcome = fn m |> Async.AwaitPromise
            try do! m.Stop()   |> Async.AwaitPromise with _ -> ()
            try do! m.Delete() |> Async.AwaitPromise with _ -> ()
            return outcome
        with e ->
            try do! m.Stop()   |> Async.AwaitPromise with _ -> ()
            try do! m.Delete() |> Async.AwaitPromise with _ -> ()
            return raise e
    } |> Async.StartAsPromise

let quickExec (command: string[]) (config: MachineConfig option) : Promise<ExecResult> =
    let cfg =
        config
        |> Option.defaultValue
            { name      = sprintf "quick-exec-%d" (int (Fable.Core.JS.Constructors.Date.now()))
              serverUrl = None
              mounts    = None
              ports     = None
              resources = None }
    withMachine cfg (fun m -> m.Exec(command))

let quickRun (image: string) (command: string[]) (config: MachineConfig option) : Promise<ExecResult> =
    let cfg =
        config
        |> Option.defaultValue
            { name      = sprintf "quick-run-%d" (int (Fable.Core.JS.Constructors.Date.now()))
              serverUrl = None
              mounts    = None
              ports     = None
              resources = None }
    withMachine cfg (fun m -> m.Run(image, command))
