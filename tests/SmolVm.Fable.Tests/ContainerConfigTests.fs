module SmolVm.Fable.Tests.ContainerConfigTests

open SmolVm.Fable.Tests.Helpers
open SmolVm.Types      // ContainerOptions, ContainerMount, MachineConfig, MountSpec
open Scriptorium.Quill
open type Scriptorium.Quill.Test

// ──────────────────────────────────────────────────────────────────────────────
// ContainerOptions snapshot tests  (formerly ContainerConfig)
//
// The binding uses ContainerOptions — the exact shape passed to
// Machine.createContainer / Machine.run in the JS SDK.  Key facts:
//   - `image`   is required (string, not option)
//   - `env`     is Map<string,string> option
//   - mounts    use ContainerMount { tag = virtiofs-tag; target = path }
//
// NOTE: there is no ContainerConfig.Default in this binding.  Tests build
// values from scratch using the actual F# record types.
// ──────────────────────────────────────────────────────────────────────────────

/// Minimal ContainerOptions — only the required `image` field set.
let private defaultOpts : ContainerOptions =
    { image   = "alpine:3.19"
      command = None
      env     = None
      workdir = None
      mounts  = None }

let tests =
    testSequenced (
        "ContainerConfig",
        [

            test (
                "default container config snapshot",
                fun (t: TestContext) ->
                    snap t defaultOpts
            )

            test (
                "image-only config",
                fun (t: TestContext) ->
                    let cfg = { defaultOpts with image = "alpine:3.19" }
                    snap t cfg
            )

            test (
                "config with env vars",
                fun (t: TestContext) ->
                    let env = Map.ofList [ "PATH", "/usr/local/bin:/usr/bin"; "HOME", "/root" ]
                    let cfg = { defaultOpts with image = "ubuntu:22.04"; env = Some env }
                    snap t cfg
            )

            test (
                "config with command and workdir",
                fun (t: TestContext) ->
                    let cfg =
                        { defaultOpts with
                            image   = "python:3.12-slim"
                            command = Some [| "python"; "-c"; "print('hello')" |]
                            workdir = Some "/app" }
                    snap t cfg
            )

            test (
                "config with volume mounts",
                fun (t: TestContext) ->
                    // ContainerMount.tag is the virtiofs tag defined in
                    // MachineConfig.mounts[].tag — not a host path.
                    let mount : ContainerMount =
                        { tag      = "data"       // matches MountSpec.tag
                          target   = "/mnt/data"
                          readOnly = Some true }
                    let cfg = { defaultOpts with image = "busybox"; mounts = Some [| mount |] }
                    snap t cfg
            )

            test (
                "machine config with embedded container options",
                fun (t: TestContext) ->
                    // Show the pairing: MountSpec.tag on the machine side must
                    // match ContainerMount.tag on the container side.
                    let machineMount : MountSpec =
                        { hostPath = "/host/data"; tag = "data"; readOnly = Some false }
                    let machineCfg : MachineConfig =
                        { name = "node-runner"; serverUrl = None
                          mounts = Some [| machineMount |]
                          ports = None
                          resources = Some { vcpus = None; memory = Some 256 } }
                    let containerMount : ContainerMount =
                        { tag = "data"; target = "/mnt/data"; readOnly = Some true }
                    let containerOpts : ContainerOptions =
                        { image   = "node:20-alpine"
                          command = None
                          env     = Some (Map.ofList [ "NODE_ENV", "production" ])
                          workdir = None
                          mounts  = Some [| containerMount |] }
                    snap t {| machine = machineCfg; container = containerOpts |}
            )

        ]
    )
