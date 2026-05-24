module SmolVm.Fable.Tests.Helpers

/// Convenience: open Assertion and Snapshot in one place so every test
/// module can just `open SmolVm.Fable.Tests.Helpers`.
open Scriptorium.Nib.Assertion
open Scriptorium.Nib.Snapshot
open Scriptorium.Quill

/// Snapshot a value against the test-context name (path-derived key).
/// Uses the default JSON serialiser — System.Text.Json on .NET, JSON.stringify on JS.
let snap (t: TestContext) value =
    t.snapshot value

/// Snapshot with a custom serialiser.
let snapWith (t: TestContext) (serialize: 'a -> string) value =
    t.snapshotWith (serialize, value)

/// Assert structural equality and emit a diff on failure.
let shouldEqual expected actual =
    assertThat actual (isEqualTo expected)
