module SmolVm.Execution

open Fable.Core
open SmolVm.Types

// ============================================================================
// ExecResult  —  mirrors execution.ts : ExecResult class
// ============================================================================

/// Thrown by ExecResult.assertSuccess when the command exits with a non-zero code.
/// Mirrors ExecutionError in execution.ts.
exception ExecutionError of exitCode: int * stdout: string * stderr: string

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
    /// Mirrors the `success` getter in execution.ts.
    member this.Success : bool = this.ExitCode = 0

    /// Combined stdout + stderr output.
    /// Mirrors the `output` getter in execution.ts.
    member this.Output : string =
        match this.Stdout, this.Stderr with
        | s, e when s <> "" && e <> "" -> s + "\n" + e
        | s, _ when s <> ""            -> s
        | _, e                         -> e

    /// Assert that the command succeeded (exit code 0).
    /// Raises ExecutionError if the exit code is non-zero.
    /// Returns self for chaining — mirrors assertSuccess() in execution.ts.
    member this.AssertSuccess() : ExecResult =
        if not this.Success then
            raise (ExecutionError (this.ExitCode, this.Stdout, this.Stderr))
        this
