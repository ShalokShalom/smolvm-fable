module SmolVm.Internal.Interop

open Fable.Core.JsInterop
open SmolVm.Types

/// Convert an optional env map to the EnvVar[] shape the smolvm API expects.
let toEnvVars (env: Map<string,string> option) : obj =
    match env with
    | None   -> null
    | Some m ->
        m
        |> Map.toArray
        |> Array.map (fun (k,v) -> {| name = k; value = v |})
        |> box

/// Serialise ExecOptions to the JS object shape accepted by the API.
/// Field names match ExecRequest / RunRequest:
///   env -> EnvVar[], workdir -> string?, timeoutSecs -> number?
let execOptsToJs (o: ExecOptions) : obj =
    jsOptions<obj> (fun x ->
        x?env         <- toEnvVars o.env
        x?workdir     <- Option.toObj o.workdir
        x?timeoutSecs <- Option.toNullable o.timeout)

/// Serialise LogsOptions to the LogsQuery shape used by client.ts streamLogs.
/// `tail` is supported by the HTTP client even though SDK types.ts omits it.
let logsOptsToJs (o: LogsOptions) : obj =
    jsOptions<obj> (fun x ->
        x?follow <- Option.toNullable o.follow
        x?since  <- Option.toObj o.since
        x?tail   <- Option.toNullable o.tail)

/// Serialise ContainerOptions to the CreateContainerRequest shape.
/// ContainerMount.tag maps to source (the virtiofs tag used as host-side key).
let containerOptsToJs (options: ContainerOptions) : obj =
    let mounts =
        options.mounts
        |> Option.map (fun ms ->
            ms |> Array.map (fun m ->
                {| source       = m.tag
                   target       = m.target
                   ``readonly`` = Option.toNullable m.readOnly |} |> box))
        |> Option.toObj
    jsOptions<obj> (fun x ->
        x?image   <- options.image
        x?command <- Option.toObj options.command
        x?env     <- toEnvVars options.env
        x?workdir <- Option.toObj options.workdir
        x?mounts  <- mounts)
