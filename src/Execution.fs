module SmolVm.Execution

open Fable.Core
open Fable.Core.JsInterop
open SmolVm.Types

// ============================================================================
// ExecutionError
//
// The upstream SDK's ExecutionError is a JS class (extends Error) with
// exitCode, stdout, stderr properties. We import it via [<Import>] so that
// AssertSuccess throws the *same* JS class instance the upstream SDK throws,
// keeping try/with blocks on both sides compatible.
// ============================================================================

[<Import("ExecutionError", "smolvm")>]
type JsExecutionError =
    abstract exitCode : int
    abstract stdout   : string
    abstract stderr   : string

/// Private constructor binding — lets us raise a JS ExecutionError directly.
[<Import("ExecutionError", "smolvm")>]
let private newExecutionError (exitCode: int) (stdout: string) (stderr: string) : JsExecutionError = jsNative

/// Active pattern to catch a JS ExecutionError thrown by the smolvm SDK.
/// Matches any exn whose JS `name` property is "ExecutionError".
let (|ExecutionErr|_|) (e: exn) =
    let js = box e
    if js?name = "ExecutionError" then
        Some {| exitCode = js?exitCode
                stdout   = js?stdout
                stderr   = js?stderr |}
    else None

// ============================================================================
// ExecResult  —  mirrors execution.ts : ExecResult class
// ============================================================================

/// Rich result of a command execution.
/// Mirrors the ExecResult class from execution.ts.
type ExecResult(response: ExecResponse) =

    /// Exit code returned by the command.
    member _.ExitCode : int    = response.exitCode

    /// Standard output captured from the command.
    member _.Stdout   : string = response.stdout

    /// Standard error captured from the command.
    member _.Stderr   : string = response.stderr

    /// True when the exit code is 0.
    member this.Success : bool = this.ExitCode = 0

    /// Combined stdout + stderr output.
    member this.Output : string =
        match this.Stdout, this.Stderr with
        | s, e when s <> "" && e <> "" -> s + "\n" + e
        | s, _ when s <> ""            -> s
        | _, e                         -> e

    /// Assert that the command succeeded (exit code 0).
    /// Raises the JS ExecutionError class so that catch clauses in both
    /// F# and JS code see the same error type.
    /// Returns self for chaining.
    member this.AssertSuccess() : ExecResult =
        if not this.Success then
            raise (!! (newExecutionError this.ExitCode this.Stdout this.Stderr))
        this
