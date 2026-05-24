module SmolVm.Machine

open Fable.Core
open Fable.Core.JS
open SmolVm.Types
open SmolVm.Execution
open SmolVm.Container
open SmolVm.Internal.Interop

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
    abstract logs      : ?options: obj -> obj
    abstract createContainer  : options: obj   -> Promise<obj>
    abstract listContainers   : unit           -> Promise<obj[]>
    abstract getContainer     : id: string     -> Promise<obj>
    abstract listImages       : unit           -> Promise<ImageInfo[]>
    abstract pullImage        : image: string * ?ociPlatform: string -> Promise<ImageInfo>

type Machine(js: JsMachine) =
    static member Create(config: MachineConfig) : Promise<Machine> =
        let cfg = machineConfigToJs config
        let p : Promise<JsMachine> = emitJsExpr cfg "Machine.create($0)"
        p |> Promise.map Machine
    member internal _.Js = js
    member _.Name : string = js.name
    member _.IsStarted : bool = js.isStarted
    member _.State : MachineState option =
        js.state
        |> Option.map (function
            | "created" -> MachineCreated
            | "running" -> MachineRunning
            | "stopped" -> MachineStopped
            | other      -> MachineUnknown other)
    member _.Mounts : MountInfo[] = js.mounts
    member _.Info : MachineInfo option = js.info
    member _.Start() : Promise<unit> = js.start()
    member _.Stop() : Promise<unit> = js.stop()
    member _.Delete() : Promise<unit> = js.delete()
    member _.Status() : Promise<MachineInfo> = js.status()
    member _.Exec(command: string[], ?options: ExecOptions) : Promise<ExecResult> =
        promise {
            let! raw =
                match options with
                | None   -> js.exec(command)
                | Some o -> js.exec(command, execOptsToJs o)
            return ExecResult(unbox<ExecResponse> raw)
        }
    member _.Run(image: string, command: string[], ?options: ExecOptions) : Promise<ExecResult> =
        promise {
            let! raw =
                match options with
                | None   -> js.run(image, command)
                | Some o -> js.run(image, command, execOptsToJs o)
            return ExecResult(unbox<ExecResponse> raw)
        }
    member _.Logs(?options: LogsOptions) : JS.AsyncIterableIterator<string> =
        match options with
        | None   -> unbox (js.logs())
        | Some o -> unbox (js.logs(logsOptsToJs o))
    member _.CreateContainer(options: ContainerOptions) : Promise<Container> =
        promise {
            let! raw = js.createContainer(containerOptsToJs options)
            return Container(unbox<JsContainer> raw)
        }
    member _.ListContainers() : Promise<Container[]> =
        promise {
            let! raws = js.listContainers()
            return raws |> Array.map (fun r -> Container(unbox<JsContainer> r))
        }
    member _.GetContainer(id: string) : Promise<Container> =
        promise {
            let! raw = js.getContainer(id)
            return Container(unbox<JsContainer> raw)
        }
    member _.ListImages() : Promise<ImageInfo[]> = js.listImages()
    member _.PullImage(image: string, ?ociPlatform: string) : Promise<ImageInfo> =
        match ociPlatform with
        | Some p -> js.pullImage(image, p)
        | None   -> js.pullImage(image)

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
