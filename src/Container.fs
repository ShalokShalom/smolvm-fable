module SmolVm.Container

open Fable.Core
open Fable.Core.JS
open SmolVm.Types
open SmolVm.Execution

// ============================================================================
// Raw JS imports  —  map directly onto the JS Container class from container.ts
// ============================================================================

/// Low-level Fable binding to the JS Container class.
/// Prefer using the ergonomic Container wrapper below.
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

// ============================================================================
// Ergonomic F# wrapper around Container
// Mirrors the Container class from container.ts
// ============================================================================

/// Manages a container running inside a smolvm machine.
/// Wraps the JS Container class with idiomatic F# types.
type Container(js: JsContainer) =

    /// Unique container identifier.
    /// Mirrors Container.id in container.ts.
    member _.Id : string = js.id

    /// Start the container.
    /// Mirrors Container.start() in container.ts.
    member _.Start() : Promise<unit> = js.start()

    /// Stop the container.
    /// `timeout` is an optional grace period in seconds.
    /// Mirrors Container.stop(timeout?) in container.ts.
    member _.Stop(?timeout: int) : Promise<unit> =
        match timeout with
        | Some t -> js.stop(t)
        | None   -> js.stop()

    /// Delete the container.
    /// Set `force` to true to delete even if the container is running.
    /// Mirrors Container.delete(force?) in container.ts.
    member _.Delete(?force: bool) : Promise<unit> =
        match force with
        | Some f -> js.delete(f)
        | None   -> js.delete()

    /// Execute a command inside the container.
    /// `options` maps to ExecOptions (env, workdir, timeout).
    /// Mirrors Container.exec(command, options?) in container.ts.
    member _.Exec(command: string[], ?options: ExecOptions) : Promise<ExecResult> =
        promise {
            let optsObj =
                match options with
                | None -> None
                | Some o ->
                    let env =
                        o.env
                        |> Option.map (fun m ->
                            m
                            |> Map.toArray
                            |> Array.map (fun (k,v) -> {| name = k; value = v |}))
                    Some {| env = env; workdir = o.workdir; timeoutSecs = o.timeout |}
            let! raw =
                match optsObj with
                | None   -> js.exec(command)
                | Some o -> js.exec(command, o)
            let r = unbox<ExecResponse> raw
            return ExecResult(r)
        }

    /// Refresh container info from the daemon.
    /// Mirrors Container.refresh() in container.ts.
    member _.Refresh() : Promise<ContainerInfo> =
        promise {
            let! raw = js.refresh()
            return unbox<ContainerInfo> raw
        }

    /// Current lifecycle state.
    member _.State : ContainerState =
        match js.state with
        | "running" -> Running
        | "stopped" -> Stopped
        | _         -> Created

    /// OCI image reference the container was started from.
    member _.Image : string = js.image

    /// Command the container was started with.
    member _.Command : string[] = js.command

    /// UNIX timestamp (seconds) when the container was created.
    member _.CreatedAt : float = js.createdAt

    /// Raw ContainerInfo as returned by the API.
    member _.Info : ContainerInfo = js.info
