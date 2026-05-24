module SmolVm.Fable.Tests.ContainerConfigTests

open SmolVm.Fable.Tests.Helpers
open SmolVm.Fable
open Scriptorium.Quill
open type Scriptorium.Quill.Test

// ──────────────────────────────────────────────────────────────────────────────
// ContainerConfig snapshot tests
//
// ContainerConfig is used inside MachineConfig.Container to describe OCI
// container options: image, environment variables, entrypoint overrides, and
// volume mounts.  These snapshots guard the field names and default values
// from silent changes.
// ──────────────────────────────────────────────────────────────────────────────

let tests =
    testSequenced (
        "ContainerConfig",
        [

            test (
                "default container config snapshot",
                fun (t: TestContext) ->
                    let cfg = ContainerConfig.Default
                    snap t cfg
            )

            test (
                "image-only config",
                fun (t: TestContext) ->
                    let cfg = { ContainerConfig.Default with Image = Some "alpine:3.19" }
                    snap t cfg
            )

            test (
                "config with env vars",
                fun (t: TestContext) ->
                    let cfg =
                        { ContainerConfig.Default with
                            Image = Some "ubuntu:22.04"
                            Env = [ "PATH=/usr/local/bin:/usr/bin"; "HOME=/root" ] }
                    snap t cfg
            )

            test (
                "config with entrypoint override",
                fun (t: TestContext) ->
                    let cfg =
                        { ContainerConfig.Default with
                            Image = Some "python:3.12-slim"
                            Entrypoint = Some "/usr/bin/python3"
                            Args = [ "-c"; "print('hello')" ] }
                    snap t cfg
            )

            test (
                "config with volume mounts",
                fun (t: TestContext) ->
                    let mount = { HostPath = "/data"; GuestPath = "/mnt/data"; ReadOnly = true }
                    let cfg =
                        { ContainerConfig.Default with
                            Image = Some "busybox"
                            Mounts = [ mount ] }
                    snap t cfg
            )

            test (
                "machine config with embedded container",
                fun (t: TestContext) ->
                    let container =
                        { ContainerConfig.Default with
                            Image = Some "node:20-alpine"
                            Env = [ "NODE_ENV=production" ] }
                    let machine =
                        { MachineConfig.Default with
                            Name = "node-runner"
                            Container = Some container
                            Memory = Some 256 }
                    snap t machine
            )

        ]
    )
