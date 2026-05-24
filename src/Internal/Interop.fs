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

/// Serialise ExecOptions to the JS object shape sent to the smolvm API.
/// Field names match ExecRequest / RunRequest from the OpenAPI model:
///   env -> EnvVar[], workdir -> string?, timeoutSecs -> number?
let execOptsToJs (o: ExecOptions) : obj =
    jsOptions<obj> (fun x ->
        x?env         <- toEnvVars o.env
        x?workdir     <- Option.toObj o.workdir
        x?timeoutSecs <- Option.toNullable o.timeout)

/// Serialise LogsOptions to the query-param shape accepted by streamLogs.
/// Upstream LogsOptions has: follow?: boolean, since?: string  (no tail).
let logsOptsToJs (o: LogsOptions) : obj =
    jsOptions<obj> (fun x ->
        x?follow <- Option.toNullable o.follow
        x?since  <- Option.toObj o.since)

/// Serialise ContainerOptions to the CreateContainerRequest shape.
/// Mounts: upstream ContainerMountSpec uses { source: tag, target, readonly }.
let containerOptsToJs (options: ContainerOptions) : obj =
    let mounts =
        options.mounts
        |> Option.map (fun ms ->
            ms |> Array.map (fun m ->
                {| source   = m.tag
                   target   = m.target
                   ``readonly`` = Option.toNullable m.readOnly |} |> box))
        |> Option.toObj
    jsOptions<obj> (fun x ->
        x?image   <- options.image
        x?command <- Option.toObj options.command
        x?env     <- toEnvVars options.env
        x?workdir <- Option.toObj options.workdir
        x?mounts  <- mounts)
