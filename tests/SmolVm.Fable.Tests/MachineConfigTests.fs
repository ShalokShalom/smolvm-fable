module SmolVm.Fable.Tests.MachineConfigTests

open SmolVm.Fable.Tests.Helpers
open SmolVm.Types      // MachineConfig, ResourceSpec, MountSpec, PortSpec
open Scriptorium.Quill
open type Scriptorium.Quill.Test

// ---------------------------------------------------------------------------
// MachineConfig snapshot tests
//
// MachineConfig is a [<Pojo>] record.  All fields use camelCase to mirror the
// JS object shape.  There is no static Default — we construct literal values.
// ---------------------------------------------------------------------------

/// Minimal valid MachineConfig: only the required `name` field is supplied;
/// all option fields are None, matching the zero-config JS example.
let private minimalConfig : MachineConfig =
    { name      = "my-machine"
      serverUrl = None
      mounts    = None
      ports     = None
      resources = None }

let tests =
    testSequenced (
        "MachineConfig",
        [

            test (
                "minimal config snapshot",
                fun (t: TestContext) ->
                    snap t minimalConfig
            )

            test (
                "config with custom serverUrl",
                fun (t: TestContext) ->
                    let cfg = { minimalConfig with serverUrl = Some "http://192.168.1.100:8080" }
                    snap t cfg
            )

            test (
                "config with resource limits",
                fun (t: TestContext) ->
                    let cfg =
                        { minimalConfig with
                            name      = "constrained"
                            resources = Some { vcpus = Some 2; memory = Some 512 } }
                    snap t cfg
            )

            test (
                "config with memory-only resources",
                fun (t: TestContext) ->
                    let cfg =
                        { minimalConfig with
                            name      = "memory-only"
                            resources = Some { vcpus = None; memory = Some 256 } }
                    snap t cfg
            )

            test (
                "config with mounts",
                fun (t: TestContext) ->
                    let mount : MountSpec =
                        { hostPath = "/data"
                          tag      = "data"
                          readOnly = Some true }
                    let cfg = { minimalConfig with mounts = Some [| mount |] }
                    snap t cfg
            )

            test (
                "config with port mappings",
                fun (t: TestContext) ->
                    let port : PortSpec = { host = 8080; guest = 80 }
                    let cfg = { minimalConfig with ports = Some [| port |] }
                    snap t cfg
            )

            test (
                "round-trip sprintf representation snapshot",
                fun (t: TestContext) ->
                    // Uses sprintf "%A" so .NET and Fable-JS representations can
                    // be compared independently of JSON serialisation.
                    let cfg =
                        { minimalConfig with
                            name      = "round-trip"
                            resources = Some { vcpus = Some 1; memory = Some 128 } }
                    snapWith t (sprintf "%A") cfg
            )

        ]
    )
