module SmolVm.Fable.Tests.Helpers

open Scriptorium.Nib.Assertion
open Scriptorium.Nib.Snapshot
open Scriptorium.Quill

/// Snapshot a value against the test-context name (path-derived key).
let snap (t: TestContext) value =
    t.snapshot value

/// Snapshot with a custom serialiser.
let snapWith (t: TestContext) (serialize: 'a -> string) value =
    t.snapshotWith (serialize, value)

/// Assert structural equality and emit a diff on failure.
let shouldEqual expected actual =
    assertThat actual (isEqualTo expected)
