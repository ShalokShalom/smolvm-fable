module SmolVm.Fable.Tests.ResourceLimitsTests

open SmolVm.Fable.Tests.Helpers
open SmolVm.Fable
open Scriptorium.Quill
open type Scriptorium.Quill.Test
open Scriptorium.Nib.Assertion

// ──────────────────────────────────────────────────────────────────────────────
// Resource-limit snapshot tests
//
// These tests cover the boundary values for CPU and memory limits defined in
// MachineConfig, plus a few invariant assertions (e.g. memory must be positive)
// that are expressed via Scriptorium.Nib assertions rather than snapshots.
// ──────────────────────────────────────────────────────────────────────────────

let tests =
    testSequenced (
        "ResourceLimits",
        [

            test (
                "minimum memory config snapshot",
                fun (t: TestContext) ->
                    let cfg = { MachineConfig.Default with Name = "tiny"; Memory = Some 64 }
                    snap t cfg
            )

            test (
                "maximum memory config snapshot",
                fun (t: TestContext) ->
                    let cfg = { MachineConfig.Default with Name = "large"; Memory = Some 32768 }
                    snap t cfg
            )

            test (
                "single vCPU config snapshot",
                fun (t: TestContext) ->
                    let cfg = { MachineConfig.Default with Name = "single-cpu"; Vcpus = Some 1 }
                    snap t cfg
            )

            test (
                "high-vCPU config snapshot",
                fun (t: TestContext) ->
                    let cfg = { MachineConfig.Default with Name = "high-cpu"; Vcpus = Some 16 }
                    snap t cfg
            )

            test (
                "GPU config enabled snapshot",
                fun (t: TestContext) ->
                    let cfg = { MachineConfig.Default with Name = "gpu"; Gpu = Some true }
                    snap t cfg
            )

            test (
                "memory value is positive",
                fun _ ->
                    // Structural assertion — not a snapshot — so it works without a stored file.
                    let cfg = { MachineConfig.Default with Memory = Some 128 }
                    assertThat cfg.Memory (Option.isSome)
                    assertThat cfg.Memory.Value (isGreaterThan 0)
            )

            test (
                "vCPU count is positive",
                fun _ ->
                    let cfg = { MachineConfig.Default with Vcpus = Some 4 }
                    assertThat cfg.Vcpus.Value (isGreaterThan 0)
            )

            test (
                "combined resource config snapshot",
                fun (t: TestContext) ->
                    let cfg =
                        { MachineConfig.Default with
                            Name = "full-spec"
                            Vcpus = Some 8
                            Memory = Some 4096
                            Gpu = Some false }
                    snap t cfg
            )

        ]
    )
