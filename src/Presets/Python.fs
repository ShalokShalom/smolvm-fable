module SmolVm.Presets.Python

open Fable.Core
open Fable.Core.JS
open SmolVm.Types
open SmolVm.Execution
open SmolVm.Machine

// ============================================================================
// PythonMachine
// Mirrors PythonMachine from presets/python.ts
//
// Design note: upstream PythonMachine extends Machine via JS class inheritance.
// In F# / Fable the cleanest equivalent is composition: PythonMachine holds a
// Machine field and delegates all base calls through it. This avoids the broken
// pattern of `inherit Machine(failwith ...)` that evaluated `failwith` eagerly.
// ============================================================================

/// Default OCI image for Python execution.
/// Mirrors PythonMachine.DEFAULT_IMAGE in python.ts.
[<Literal>]
let DefaultImage = "python:3.12-alpine"

/// A machine specialised for running Python code.
/// Delegates lifecycle and execution to an inner Machine.
/// Mirrors the PythonMachine class from presets/python.ts.
type PythonMachine private (inner: Machine) =

    // -----------------------------------------------------------------------
    // Static factory
    // -----------------------------------------------------------------------

    /// Create and start a Python machine.
    /// Mirrors PythonMachine.create(config) in python.ts.
    static member Create(config: MachineConfig) : Promise<PythonMachine> =
        promise {
            let! m = Machine.Create(config)
            return PythonMachine(m)
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
    // Python-specific helpers
    // -----------------------------------------------------------------------

    /// Run Python source code directly.
    /// Equivalent to `python -c <code>` in a container.
    /// Mirrors PythonMachine.runCode(code, options?) in python.ts.
    member _.RunCode(code: string, ?options: CodeOptions) : Promise<ExecResult> =
        let image = options |> Option.bind (fun o -> o.image) |> Option.defaultValue DefaultImage
        let exec: ExecOptions =
            { env     = options |> Option.bind (fun o -> o.env)
              workdir = options |> Option.bind (fun o -> o.workdir)
              timeout = options |> Option.bind (fun o -> o.timeout) }
        inner.Run(image, [| "python"; "-c"; code |], exec)

    /// Run a Python file by path (path must be accessible inside the machine).
    /// Mirrors PythonMachine.runFile(path, options?) in python.ts.
    member _.RunFile(path: string, ?options: CodeOptions) : Promise<ExecResult> =
        let image = options |> Option.bind (fun o -> o.image) |> Option.defaultValue DefaultImage
        let exec: ExecOptions =
            { env     = options |> Option.bind (fun o -> o.env)
              workdir = options |> Option.bind (fun o -> o.workdir)
              timeout = options |> Option.bind (fun o -> o.timeout) }
        inner.Run(image, [| "python"; path |], exec)

    /// Install packages via pip.
    /// Mirrors PythonMachine.pip(packages, options?) in python.ts.
    member _.Pip(packages: string[], ?options: ExecOptions) : Promise<ExecResult> =
        inner.Run(DefaultImage, Array.append [| "pip"; "install" |] packages, ?options = options)

    /// Run two blocks of Python code sequentially (setup + main) in a single invocation.
    /// Mirrors PythonMachine.runWithSetup(setupCode, mainCode, options?) in python.ts.
    member this.RunWithSetup(setupCode: string, mainCode: string, ?options: CodeOptions) : Promise<ExecResult> =
        this.RunCode(setupCode + "\n" + mainCode, ?options = options)

    /// Return the Python interpreter version string.
    /// Mirrors PythonMachine.version(options?) in python.ts.
    member this.Version(?options: CodeOptions) : Promise<string> =
        promise {
            let! r = this.RunCode("import sys; print(sys.version)", ?options = options)
            return r.Stdout.Trim()
        }

    /// List installed packages in pip freeze format.
    /// Returns each package as a "name==version" string.
    /// Mirrors PythonMachine.listPackages(options?) in python.ts.
    member _.ListPackages(?options: ExecOptions) : Promise<string[]> =
        promise {
            let! r = inner.Run(DefaultImage, [| "pip"; "list"; "--format=freeze" |], ?options = options)
            return r.Stdout.Trim().Split('\n') |> Array.filter (fun s -> s.Length > 0)
        }
