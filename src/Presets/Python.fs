module SmolVm.Presets.Python

open Fable.Core
open Fable.Core.JS
open SmolVm.Types
open SmolVm.Execution
open SmolVm.Machine

// ============================================================================
// PythonMachine
// Mirrors PythonMachine from presets/python.ts
// ============================================================================

/// Default OCI image for Python execution.
/// Mirrors PythonMachine.DEFAULT_IMAGE in python.ts.
[<Literal>]
let DefaultImage = "python:3.12-alpine"

/// A machine specialised for running Python code.
/// Thin layer on top of Machine with convenience methods.
/// Mirrors the PythonMachine class from presets/python.ts.
type PythonMachine(config: MachineConfig) =
    inherit Machine(failwith "JS Machine not yet created")   // placeholder; use Create

    // The real JS object is acquired through Create below and held here.
    let mutable inner: Machine option = None

    let m () =
        match inner with
        | Some x -> x
        | None   -> failwith "PythonMachine not initialised; call PythonMachine.Create()"

    /// Create and start a Python machine.
    /// Mirrors PythonMachine.create(config) in python.ts.
    static member Create(config: MachineConfig) : Promise<PythonMachine> =
        promise {
            let! baseM = Machine.Create(config)
            let pm = PythonMachine(config)
            pm |> (fun x -> inner <- Some baseM)
            return pm
        }

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
        m().Run(image, [| "python"; "-c"; code |], exec)

    /// Run a Python file by path (path must be accessible inside the machine).
    /// Mirrors PythonMachine.runFile(path, options?) in python.ts.
    member _.RunFile(path: string, ?options: CodeOptions) : Promise<ExecResult> =
        let image = options |> Option.bind (fun o -> o.image) |> Option.defaultValue DefaultImage
        let exec: ExecOptions =
            { env     = options |> Option.bind (fun o -> o.env)
              workdir = options |> Option.bind (fun o -> o.workdir)
              timeout = options |> Option.bind (fun o -> o.timeout) }
        m().Run(image, [| "python"; path |], exec)

    /// Install packages via pip.
    /// Mirrors PythonMachine.pip(packages, options?) in python.ts.
    member _.Pip(packages: string[], ?options: ExecOptions) : Promise<ExecResult> =
        m().Run(DefaultImage, Array.append [| "pip"; "install" |] packages, ?options = options)

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
            let! r = m().Run(DefaultImage, [| "pip"; "list"; "--format=freeze" |], ?options = options)
            return r.Stdout.Trim().Split('\n') |> Array.filter (fun s -> s.Length > 0)
        }
