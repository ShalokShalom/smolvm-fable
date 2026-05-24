module SmolVmDemo

open Fable.Core
open Fable.Core.JS
open Fable.Core.JsInterop
open SmolVm.Types
open SmolVm.Execution
open SmolVm.Machine
open SmolVm.Presets.Node

let main () =
    async {
        printfn "🚀 smolvm-fable demo starting..."

        let config: MachineConfig =
            { name      = sprintf "demo-machine-%d" (int (Constructors.Date.now()))
              serverUrl = None
              mounts    = None
              ports     = None
              resources = Some { vcpus = Some 2; memory = Some 512 } }

        printfn "📋 Config created"

        let! m = Machine.Create(config) |> Async.AwaitPromise
        printfn "✅ Machine created: %s" m.Name

        let! result = m.Exec([| "node"; "-e"; "console.log('Hello from smolvm!')" |]) |> Async.AwaitPromise
        printfn "📤 Exit code: %d" result.ExitCode
        printfn "📤 Stdout: %s" result.Stdout

        do! m.Stop() |> Async.AwaitPromise
        do! m.Delete() |> Async.AwaitPromise
        printfn "✅ Demo completed!"
    }

main () |> Async.StartAsPromise |> ignore
