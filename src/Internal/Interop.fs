module SmolVm.Internal.Interop

open Fable.Core
open SmolVm.Types

let toEnvVars (env: Map<string,string> option) : obj =
    match env with
    | None   -> null
    | Some m ->
        m
        |> Map.toArray
        |> Array.map (fun (k,v) -> {| name = k; value = v |})
        |> box

let execOptsToJs (o: ExecOptions) : obj =
    upcast {| env         = toEnvVars o.env
              workdir     = Option.toObj o.workdir
              timeoutSecs = Option.toNullable o.timeout |}

let logsOptsToJs (o: LogsOptions) : obj =
    upcast {| follow = Option.toNullable o.follow
              since  = Option.toObj o.since
              tail   = Option.toNullable o.tail |}

let machineConfigToJs (cfg: MachineConfig) : obj =
    upcast {| name      = cfg.name
              serverUrl = Option.toObj cfg.serverUrl
              mounts    = Option.toObj cfg.mounts
              ports     = Option.toObj cfg.ports
              resources = Option.toObj cfg.resources |}

let containerOptsToJs (options: ContainerOptions) : obj =
    upcast {| image   = options.image
              command = Option.toObj options.command
              env     = toEnvVars options.env
              workdir = Option.toObj options.workdir
              mounts  = Option.toObj options.mounts |}
