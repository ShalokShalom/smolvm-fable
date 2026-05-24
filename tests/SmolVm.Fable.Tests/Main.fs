module SmolVm.Fable.Tests.Main

open type Scriptorium.Quill.Runner

[<EntryPoint>]
let main _ =
    runTests
        [
            MachineConfigTests.tests
            ExecResponseTests.tests
            ContainerOptionsTests.tests
            ResourceSpecTests.tests
        ]
