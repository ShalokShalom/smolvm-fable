module SmolVm.Fable.Tests.ResourceLimitsTests

open SmolVm.Fable.Tests.Helpers
open SmolVm.Types      // ResourceSpec, MachineConfig
open Scriptorium.Quill
open type Scriptorium.Quill.Test
open Scriptorium.Nib.Assertion

// ──────────────────────────────────────────────────────────────────────────────
// Resource-limit snapshot tests
//
// ResourceSpec = { vcpus: int option; memory: int option } embedded in
// MachineConfig.resources.  There is no top-level Vcpus / Memory / Gpu field
// on MachineConfig — those options live inside ResourceSpec.
//
// NOTE: GPU acceleration is a runtime feature of the smolvm daemon, not
// exposed as a typed field in the current binding.  Tests that referenced
// `Gpu` have been removed.
// ──────────────────────────────────────────────────────────────────────────────

/// Build a MachineConfig with a given name and ResourceSpec.
let private cfg name (res: ResourceSpec) : MachineConfig =
    { name      = name
      serverUrl = None
      mounts    = None
      ports     = None
      resources = Some res }

let tests =
    testSequenced (
        "ResourceLimits",
        [

            test (
                "minimum memory config snapshot",
                fun (t: TestContext) ->
                    snap t (cfg "tiny" { vcpus = Some 1; memory = Some 64 })
            )

            test (
                "maximum memory config snapshot",
                fun (t: TestContext) ->
                    snap t (cfg "large" { vcpus = Some 8; memory = Some 32768 })
            )

            test (
                "single vCPU config snapshot",
                fun (t: TestContext) ->
                    snap t (cfg "single-cpu" { vcpus = Some 1; memory = None })
            )

            test (
                "high-vCPU config snapshot",
                fun (t: TestContext) ->
                    snap t (cfg "high-cpu" { vcpus = Some 16; memory = None })
            )

            test (
                "combined resource config snapshot",
                fun (t: TestContext) ->
                    snap t (cfg "full-spec" { vcpus = Some 8; memory = Some 4096 })
            )

            test (
                "both fields None defers to daemon defaults",
                fun (t: TestContext) ->
                    snap t (cfg "defer-all" { vcpus = None; memory = None })
            )

            test (
                "memory value is positive",
                fun _ ->
                    let spec = { vcpus = Some 2; memory = Some 128 }
                    assertThat spec.memory.Value (isGreaterThan 0)
            )

            test (
                "vCPU count is positive",
                fun _ ->
                    let spec = { vcpus = Some 4; memory = None }
                    assertThat spec.vcpus.Value (isGreaterThan 0)
            )

        ]
    )
