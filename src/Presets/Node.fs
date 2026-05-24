module SmolVm.Presets.Node

open Fable.Core
open Fable.Core.JS
open SmolVm.Types
open SmolVm.Execution
open SmolVm.Machine

// ============================================================================
// NodeMachine
// Mirrors NodeMachine from presets/node.ts
//
// Design note: upstream NodeMachine extends Machine via JS class inheritance.
// In F# / Fable the cleanest equivalent is composition: NodeMachine holds a
// Machine field and delegates all base calls through it. This avoids the broken
// pattern of `inherit Machine(failwith ...)` that evaluated `failwith` eagerly.
// ============================================================================

/// Default OCI image for Node.js execution.
/// Mirrors NodeMachine.DEFAULT_IMAGE in node.ts.
[<Literal>]
let DefaultImage = "node:22-alpine"

/// A machine specialised for running JavaScript / Node.js code.
/// Delegates lifecycle and execution to an inner Machine.
/// Mirrors the NodeMachine class from presets/node.ts.
type NodeMachine private (inner: Machine) =

    // -----------------------------------------------------------------------
    // Static factory
    // -----------------------------------------------------------------------

    /// Create and start a Node.js machine.
    /// Mirrors NodeMachine.create(config) in node.ts.
    static member Create(config: MachineConfig) : Promise<NodeMachine> =
        promise {
            let! m = Machine.Create(config)
            return NodeMachine(m)
        }

    // -----------------------------------------------------------------------
    // Delegated Machine members
    // -----------------------------------------------------------------------

    member _.Name        = inner.Name
    member _.IsStarted   = inner.IsStarted
    member _.State       = inner.State
    member _.Mounts      = inner.Mounts
    member _.Info        = inner.Info
    member _.Start()     = inner.Start()
    member _.Stop()      = inner.Stop()
    member _.Delete()    = inner.Delete()
    member _.Status()    = inner.Status()
    member _.Exec(command, ?options) = inner.Exec(command, ?options = options)
    member _.Run(image, command, ?options) = inner.Run(image, command, ?options = options)
    member _.Logs(?options) = inner.Logs(?options = options)
    member _.CreateContainer(options) = inner.CreateContainer(options)
    member _.ListContainers() = inner.ListContainers()
    member _.GetContainer(id) = inner.GetContainer(id)
    member _.ListImages() = inner.ListImages()
    member _.PullImage(image, ?platform) = inner.PullImage(image, ?ociPlatform = platform)

    // -----------------------------------------------------------------------
    // Node-specific helpers
    // -----------------------------------------------------------------------

    /// Run JavaScript source code directly.
    /// Equivalent to `node -e <code>` in a container.
    /// Mirrors NodeMachine.runCode(code, options?) in node.ts.
    member _.RunCode(code: string, ?options: CodeOptions) : Promise<ExecResult> =
        let image = options |> Option.bind (fun o -> o.image) |> Option.defaultValue DefaultImage
        let exec: ExecOptions =
            { env     = options |> Option.bind (fun o -> o.env)
              workdir = options |> Option.bind (fun o -> o.workdir)
              timeout = options |> Option.bind (fun o -> o.timeout) }
        inner.Run(image, [| "node"; "-e"; code |], exec)

    /// Run a JavaScript file by path.
    /// Mirrors NodeMachine.runFile(path, options?) in node.ts.
    member _.RunFile(path: string, ?options: CodeOptions) : Promise<ExecResult> =
        let image = options |> Option.bind (fun o -> o.image) |> Option.defaultValue DefaultImage
        let exec: ExecOptions =
            { env     = options |> Option.bind (fun o -> o.env)
              workdir = options |> Option.bind (fun o -> o.workdir)
              timeout = options |> Option.bind (fun o -> o.timeout) }
        inner.Run(image, [| "node"; path |], exec)

    /// Run npm commands.
    /// Mirrors NodeMachine.npm(args, options?) in node.ts.
    member _.Npm(args: string[], ?options: ExecOptions) : Promise<ExecResult> =
        inner.Run(DefaultImage, Array.append [| "npm" |] args, ?options = options)

    /// Install npm packages.
    /// Mirrors NodeMachine.npmInstall(packages, options?) in node.ts.
    member this.NpmInstall(packages: string[], ?options: ExecOptions) : Promise<ExecResult> =
        this.Npm(Array.append [| "install" |] packages, ?options = options)

    /// Run npx commands.
    /// Mirrors NodeMachine.npx(args, options?) in node.ts.
    member _.Npx(args: string[], ?options: ExecOptions) : Promise<ExecResult> =
        inner.Run(DefaultImage, Array.append [| "npx" |] args, ?options = options)

    /// Return the Node.js version string.
    /// Mirrors NodeMachine.version(options?) in node.ts.
    member this.Version(?options: CodeOptions) : Promise<string> =
        promise {
            let! r = this.RunCode("console.log(process.version)", ?options = options)
            return r.Stdout.Trim()
        }

    /// Run ES module code (passes --input-type=module to node).
    /// Mirrors NodeMachine.runESM(code, options?) in node.ts.
    member _.RunESM(code: string, ?options: CodeOptions) : Promise<ExecResult> =
        let image = options |> Option.bind (fun o -> o.image) |> Option.defaultValue DefaultImage
        let exec: ExecOptions =
            { env     = options |> Option.bind (fun o -> o.env)
              workdir = options |> Option.bind (fun o -> o.workdir)
              timeout = options |> Option.bind (fun o -> o.timeout) }
        inner.Run(image, [| "node"; "--input-type=module"; "-e"; code |], exec)

    /// Evaluate a JavaScript expression and return JSON-serialised output.
    /// Mirrors NodeMachine.evaluate(expression, options?) in node.ts.
    member this.Evaluate(expression: string, ?options: CodeOptions) : Promise<ExecResult> =
        this.RunCode(sprintf "console.log(JSON.stringify(%s))" expression, ?options = options)
