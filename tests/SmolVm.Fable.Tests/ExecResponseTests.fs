module SmolVm.Fable.Tests.ExecResponseTests

open SmolVm.Fable.Tests.Helpers
open SmolVm.Types      // ExecResponse
open Scriptorium.Quill
open type Scriptorium.Quill.Test
open Scriptorium.Nib.Assertion

// ---------------------------------------------------------------------------
// ExecResponse snapshot tests
//
// ExecResponse is the raw API response returned by Machine.exec and
// Machine.run (before being wrapped in the ExecResult class).  Field names
// are camelCase [<Pojo>] to match the JS JSON wire format.
//
// NOTE: the F# wrapper ExecResult (in Execution.fs) is a class that wraps
// ExecResponse, not a record, so it cannot be directly constructed in tests
// without a running machine.  We test ExecResponse (the data shape) here.
// ---------------------------------------------------------------------------

let private makeResponse stdout stderr exitCode : ExecResponse =
    { exitCode = exitCode; stdout = stdout; stderr = stderr }

let tests =
    testSequenced (
        "ExecResponse",
        [

            test (
                "successful echo response snapshot",
                fun (t: TestContext) ->
                    let r = makeResponse "Hello!\n" "" 0
                    snap t r
            )

            test (
                "empty stdout response snapshot",
                fun (t: TestContext) ->
                    let r = makeResponse "" "" 0
                    snap t r
            )

            test (
                "stderr is captured",
                fun (t: TestContext) ->
                    let r = makeResponse "" "error: file not found\n" 1
                    snap t r
            )

            test (
                "non-zero exit code snapshot",
                fun (t: TestContext) ->
                    let r = makeResponse "" "" 127
                    snap t r
            )

            test (
                "stdout equality assertion",
                fun _ ->
                    let r = makeResponse "Hello!\n" "" 0
                    assertThat r.stdout (isEqualTo "Hello!\n")
            )

            test (
                "exit code zero means success",
                fun _ ->
                    let r = makeResponse "ok" "" 0
                    assertThat r.exitCode (isEqualTo 0)
            )

            test (
                "multi-line stdout snapshot",
                fun (t: TestContext) ->
                    let r = makeResponse "line1\nline2\nline3\n" "" 0
                    snap t r
            )

        ]
    )
