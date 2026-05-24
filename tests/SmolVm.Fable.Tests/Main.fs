module SmolVm.Fable.Tests.Main

open type Scriptorium.Quill.Runner

// ──────────────────────────────────────────────────────────────────────────────
// Entry point
//
// All test suites are registered here.  Scriptorium's runner discovers focused
// tests (ftest / ftestList) across the full list and skips non-focused ones
// when any focused test is present.  In CI, focused tests cause a non-zero
// exit code so they cannot be accidentally committed.
//
// Tests are run sequentially (testSequenced) inside each suite so snapshot
// files are not written concurrently — important because all snapshots for a
// given test file share a single .snap file on disk.
// ──────────────────────────────────────────────────────────────────────────────

[<EntryPoint>]
let main _ =
    runTests
        [
            MachineConfigTests.tests
            ExecResultTests.tests
            ContainerConfigTests.tests
            ResourceLimitsTests.tests
        ]
