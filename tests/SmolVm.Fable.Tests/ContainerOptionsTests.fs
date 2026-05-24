module SmolVm.Fable.Tests.ContainerOptionsTests

open SmolVm.Fable.Tests.Helpers
open SmolVm.Types      // ContainerOptions, ContainerMount, MachineConfig, MountSpec
open Scriptorium.Quill
open type Scriptorium.Quill.Test

// ---------------------------------------------------------------------------
// ContainerOptions snapshot tests
//
// ContainerOptions corresponds to the `options` object passed to
// Machine.createContainer / Machine.run in the JS SDK.
//
// Key differences from the earlier (incorrect) draft:
//   - The type is ContainerOptions, not ContainerConfig.
//   - `image` is required (string, not option).
//   - `env` is Map<string,string> option, not string list.
//   - Volume mounts use ContainerMount (tag = virtiofs tag, target = path).
// ---------------------------------------------------------------------------

/// Minimal ContainerOptions: only the required `image` field.
let private minimalOpts : ContainerOptions =
    { image   = "alpine:3.19"
      command = None
      env     = None
      workdir = None
      mounts  = None }

let tests =
    testSequenced (
        "ContainerOptions",
        [

            test (
                "minimal options snapshot",
                fun (t: TestContext) ->
                    snap t minimalOpts
            )

            test (
                "options with command",
                fun (t: TestContext) ->
                    let opts = { minimalOpts with command = Some [| "echo"; "hello" |] }
                    snap t opts
            )

            test (
                "options with env vars",
                fun (t: TestContext) ->
                    let env = Map.ofList [ "PATH", "/usr/local/bin:/usr/bin"; "HOME", "/root" ]
                    let opts = { minimalOpts with image = "ubuntu:22.04"; env = Some env }
                    snap t opts
            )

            test (
                "options with workdir",
                fun (t: TestContext) ->
                    let opts = { minimalOpts with workdir = Some "/app" }
                    snap t opts
            )

            test (
                "options with volume mount",
                fun (t: TestContext) ->
                    // ContainerMount.tag is the virtiofs tag from MountSpec.tag,
                    // not a host path.  The host path is resolved by the daemon.
                    let mount : ContainerMount =
                        { tag      = "data"       // virtiofs tag
                          target   = "/mnt/data"
                          readOnly = Some true }
                    let opts = { minimalOpts with mounts = Some [| mount |] }
                    snap t opts
            )

            test (
                "machine config with mounts feeding a container",
                fun (t: TestContext) ->
                    // Show the connection: MountSpec.tag on the machine side
                    // must match ContainerMount.tag on the container side.
                    let machineMount : MountSpec =
                        { hostPath = "/host/data"; tag = "data"; readOnly = Some false }
                    let machineCfg : MachineConfig =
                        { name = "runner"; serverUrl = None
                          mounts = Some [| machineMount |]; ports = None; resources = None }
                    let containerMount : ContainerMount =
                        { tag = "data"; target = "/mnt/data"; readOnly = Some true }
                    let containerOpts : ContainerOptions =
                        { image   = "busybox"
                          command = Some [| "ls"; "/mnt/data" |]
                          env     = None
                          workdir = None
                          mounts  = Some [| containerMount |] }
                    // Snapshot both sides of the relationship.
                    snap t {| machine = machineCfg; container = containerOpts |}
            )

        ]
    )
