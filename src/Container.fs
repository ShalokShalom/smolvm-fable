module SmolVm.Container

open Fable.Core
open Fable.Core.JS
open Fable.Core.JsInterop
open SmolVm.Types
open SmolVm.Execution
open SmolVm.Internal.Interop

// ============================================================================
// Container
//
// The upstream JS Container class is constructed as:
//   new Container(parent: ContainerParent, info: ContainerInfo)
// It is never exported as a standalone class for direct external construction.
// All Container instances are created internally by Machine methods
// (createContainer, listContainers, getContainer) which return live objects.
//
// We bind JsContainer as an interface over those live objects so we can call
// instance methods on them. We never construct JsContainer ourselves.
// ============================================================================

/// Interface over a live JS Container instance returned by Machine methods.
/// Do not construct directly — always receive from Machine.
[<Import("Container", "smolvm")>]
type JsContainer =
    abstract id        : string
    abstract info      : ContainerInfo
    abstract state     : string
    abstract image     : string
    abstract command   : string array
    abstract createdAt : float
    abstract start     : unit -> Promise<unit>
    abstract stop      : ?timeoutSecs : int -> Promise<unit>
    abstract delete    : ?force : bool -> Promise<unit>
    abstract exec      : command: string array * ?options: obj -> Promise<obj>
    abstract refresh   : unit -> Promise<ContainerInfo>

type Container(js: JsContainer) =
    member _.Id        : string        = js.id
    member _.Info      : ContainerInfo = js.info
    member _.Image     : string        = js.image
    member _.Command   : string[]      = js.command
    member _.CreatedAt : float         = js.createdAt

    member _.State : ContainerState =
        match js.state with
        | "created" -> ContainerCreated
        | "running" -> ContainerRunning
        | "stopped" -> ContainerStopped
        | other      -> ContainerUnknown other

    member _.Start() : Promise<unit> = js.start()

    /// Stop the container. `timeout` is forwarded as `timeoutSecs`.
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

    member _.Refresh() : Promise<ContainerInfo> = js.refresh()
