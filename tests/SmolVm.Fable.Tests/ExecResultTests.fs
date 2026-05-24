module SmolVm.Fable.Tests.ExecResultTests

open SmolVm.Fable.Tests.Helpers
open SmolVm.Types        // ExecResponse
open SmolVm.Execution    // ExecResult, ExecutionError
open Scriptorium.Quill
open type Scriptorium.Quill.Test
open Scriptorium.Nib.Assertion

// ──────────────────────────────────────────────────────────────────────────────
// ExecResult snapshot tests
//
// ExecResult is a class that wraps an ExecResponse.  We construct it by
// passing an ExecResponse record directly — the same path the SDK takes when
// it receives a real response from the daemon.
// ──────────────────────────────────────────────────────────────────────────────

/// Build an ExecResult the same way the SDK does: wrap an ExecResponse.
let private makeResult stdout stderr exitCode : ExecResult =
    ExecResult { exitCode = exitCode; stdout = stdout; stderr = stderr }

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
                "Success is true for exit code 0",
                fun _ ->
                    let result = makeResult "ok" "" 0
                    assertThat result.Success (isEqualTo true)
            )

            test (
                "Success is false for non-zero exit code",
                fun _ ->
                    let result = makeResult "" "oops" 1
                    assertThat result.Success (isEqualTo false)
            )

            test (
                "Output combines stdout and stderr",
                fun _ ->
                    let result = makeResult "out" "err" 0
                    assertThat result.Output (isEqualTo "out\nerr")
            )

            test (
                "AssertSuccess raises ExecutionError on failure",
                fun _ ->
                    let result = makeResult "" "boom" 2
                    assertThat
                        (fun () -> result.AssertSuccess() |> ignore)
                        (throws<ExecutionError>)
            )

            test (
                "AssertSuccess returns self on success",
                fun _ ->
                    let result = makeResult "ok" "" 0
                    // Should not raise; return value is the same instance.
                    let r2 = result.AssertSuccess()
                    assertThat r2.Stdout (isEqualTo "ok")
            )

            test (
                "multi-line stdout snapshot",
                fun (t: TestContext) ->
                    let result = makeResult "line1\nline2\nline3\n" "" 0
                    snap t result
            )

        ]
    )
