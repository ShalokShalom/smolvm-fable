module SmolVm.Fable.Tests.ExecResultTests

open SmolVm.Fable.Tests.Helpers
open SmolVm.Fable
open Scriptorium.Quill
open type Scriptorium.Quill.Test
open Scriptorium.Nib.Assertion

// ──────────────────────────────────────────────────────────────────────────────
// ExecResult snapshot tests
//
// ExecResult is the return type of Machine.exec.  These tests snapshot the
// *construction* of ExecResult values (as if returned by the SDK) to ensure
// the shape stays stable.  They also run lightweight equality assertions so
// failures produce a human-readable diff via Scriptorium.Nib's built-in differ.
// ──────────────────────────────────────────────────────────────────────────────

/// A helper that builds an ExecResult inline (simulating what the SDK returns).
let private makeResult stdout stderr exitCode : ExecResult =
    { Stdout = stdout; Stderr = stderr; ExitCode = exitCode }

let tests =
    testSequenced (
        "ExecResult",
        [

            test (
                "successful echo result snapshot",
                fun (t: TestContext) ->
                    let result = makeResult "Hello!\n" "" 0
                    snap t result
            )

            test (
                "empty stdout result snapshot",
                fun (t: TestContext) ->
                    let result = makeResult "" "" 0
                    snap t result
            )

            test (
                "stderr is captured",
                fun (t: TestContext) ->
                    let result = makeResult "" "error: file not found\n" 1
                    snap t result
            )

            test (
                "non-zero exit code snapshot",
                fun (t: TestContext) ->
                    let result = makeResult "" "" 127
                    snap t result
            )

            test (
                "stdout equality assertion",
                fun _ ->
                    let result = makeResult "Hello!\n" "" 0
                    assertThat result.Stdout (isEqualTo "Hello!\n")
            )

            test (
                "exit-code zero means success",
                fun _ ->
                    let result = makeResult "ok" "" 0
                    assertThat result.ExitCode (isEqualTo 0)
            )

            test (
                "multi-line stdout snapshot",
                fun (t: TestContext) ->
                    let result = makeResult "line1\nline2\nline3\n" "" 0
                    snap t result
            )

        ]
    )
