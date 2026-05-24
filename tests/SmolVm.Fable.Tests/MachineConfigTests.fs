module SmolVm.Fable.Tests.MachineConfigTests

open SmolVm.Fable.Tests.Helpers
open SmolVm.Fable                // binding types
open Scriptorium.Quill
open type Scriptorium.Quill.Test

// ──────────────────────────────────────────────────────────────────────────────
// MachineConfig snapshot tests
//
// These tests verify that the default values of MachineConfig, and a selection
// of non-default variants, remain stable across refactors and Fable compilation
// targets.  Every test calls t.snapshot so the snapshot key is derived from the
// full test path (e.g. "MachineConfig > default config snapshot").
// ──────────────────────────────────────────────────────────────────────────────

let tests =
    testSequenced (
        "MachineConfig",
        [

            test (
                "default config snapshot",
                fun (t: TestContext) ->
                    // MachineConfig.Default is the zero-configuration starting point.
                    // Snapshotting it catches any accidental change to a default field.
                    let cfg = MachineConfig.Default
                    snap t cfg
            )

            test (
                "config with custom name",
                fun (t: TestContext) ->
                    let cfg = { MachineConfig.Default with Name = "my-machine" }
                    snap t cfg
            )

            test (
                "config with memory limit",
                fun (t: TestContext) ->
                    let cfg =
                        { MachineConfig.Default with
                            Name = "heavy"
                            Memory = Some 512 }
                    snap t cfg
            )

            test (
                "config with CPU and memory limits",
                fun (t: TestContext) ->
                    let cfg =
                        { MachineConfig.Default with
                            Name = "constrained"
                            Vcpus = Some 2
                            Memory = Some 256 }
                    snap t cfg
            )

            test (
                "config with kernel path override",
                fun (t: TestContext) ->
                    let cfg =
                        { MachineConfig.Default with
                            Name = "custom-kernel"
                            KernelPath = Some "/boot/vmlinux" }
                    snap t cfg
            )

            test (
                "round-trip: serialise then snapshot",
                fun (t: TestContext) ->
                    // Verify the serialised JSON representation so we can detect if
                    // the Fable JS output and the .NET output diverge in field naming.
                    let cfg =
                        { MachineConfig.Default with
                            Name = "round-trip"
                            Memory = Some 128 }
                    snapWith t (sprintf "%A") cfg
            )

        ]
    )
