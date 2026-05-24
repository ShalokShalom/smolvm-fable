module SmolVm.Fable.Tests.ResourceSpecTests

open SmolVm.Fable.Tests.Helpers
open SmolVm.Types      // ResourceSpec, MachineConfig
open Scriptorium.Quill
open type Scriptorium.Quill.Test
open Scriptorium.Nib.Assertion

// ---------------------------------------------------------------------------
// ResourceSpec snapshot tests
//
// ResourceSpec is { vcpus: int option; memory: int option } and lives inside
// MachineConfig.resources.  There is no separate GPU flag in the current
// binding — the earlier draft incorrectly assumed a `Gpu` field.
// ---------------------------------------------------------------------------

let private baseCfg name resources : MachineConfig =
    { name = name; serverUrl = None; mounts = None; ports = None
      resources = Some resources }

let tests =
    testSequenced (
        "ResourceSpec",
        [

            test (
                "vcpus only snapshot",
                fun (t: TestContext) ->
                    snap t (baseCfg "vcpu-only" { vcpus = Some 2; memory = None })
            )

            test (
                "memory only snapshot",
                fun (t: TestContext) ->
                    snap t (baseCfg "mem-only" { vcpus = None; memory = Some 512 })
            )

            test (
                "minimum memory (64 MiB) snapshot",
                fun (t: TestContext) ->
                    snap t (baseCfg "tiny" { vcpus = Some 1; memory = Some 64 })
            )

            test (
                "maximum memory (32 GiB) snapshot",
                fun (t: TestContext) ->
                    snap t (baseCfg "large" { vcpus = Some 8; memory = Some 32768 })
            )

            test (
                "combined vcpus and memory snapshot",
                fun (t: TestContext) ->
                    snap t (baseCfg "full-spec" { vcpus = Some 4; memory = Some 4096 })
            )

            test (
                "both fields None snapshot",
                fun (t: TestContext) ->
                    // resources = Some { vcpus = None; memory = None } means
                    // the block is present but defers both values to the daemon default.
                    snap t (baseCfg "defer-all" { vcpus = None; memory = None })
            )

            test (
                "memory value is positive",
                fun _ ->
                    let spec = { vcpus = Some 2; memory = Some 128 }
                    assertThat spec.memory.Value (isGreaterThan 0)
            )

            test (
                "vcpu count is positive",
                fun _ ->
                    let spec = { vcpus = Some 4; memory = None }
                    assertThat spec.vcpus.Value (isGreaterThan 0)
            )

        ]
    )
