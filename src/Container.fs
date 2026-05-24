module SmolVm.Container

open Fable.Core
open Fable.Core.JS
open SmolVm.Types
open SmolVm.Execution
open SmolVm.Internal.Interop

[<Import("Container", "smolvm")>]
type JsContainer =
    abstract id        : string
    abstract start     : unit     -> Promise<unit>
    abstract stop      : ?timeout : int -> Promise<unit>
    abstract delete    : ?force   : bool -> Promise<unit>
    abstract exec      : command  : string array * ?options : obj -> Promise<obj>
    abstract refresh   : unit     -> Promise<obj>
    abstract state     : string
    abstract image     : string
    abstract command   : string array
    abstract createdAt : float
    abstract info      : ContainerInfo

type Container(js: JsContainer) =
    member _.Id : string = js.id
    member _.Start() : Promise<unit> = js.start()
    member _.Stop(?timeout: int) : Promise<unit> =
        match timeout with
        | Some t -> js.stop(t)
        | None   -> js.stop()
    member _.Delete(?force: bool) : Promise<unit> =
        match force with
        | Some f -> js.delete(f)
        | None   -> js.delete()
    member _.Exec(command: string[], ?options: ExecOptions) : Promise<ExecResult> =
        promise {
            let! raw =
                match options with
                | None   -> js.exec(command)
                | Some o -> js.exec(command, execOptsToJs o)
            return ExecResult(unbox<ExecResponse> raw)
        }
    member _.Refresh() : Promise<ContainerInfo> =
        promise {
            let! raw = js.refresh()
            return unbox<ContainerInfo> raw
        }
    member _.State : ContainerState =
        match js.state with
        | "created" -> ContainerCreated
        | "running" -> ContainerRunning
        | "stopped" -> ContainerStopped
        | other      -> ContainerUnknown other
    member _.Image : string = js.image
    member _.Command : string[] = js.command
    member _.CreatedAt : float = js.createdAt
    member _.Info : ContainerInfo = js.info
