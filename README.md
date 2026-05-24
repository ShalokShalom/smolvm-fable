# smolvm-fable

Idiomatic **F# / [Fable](https://fable.io)** bindings for the
[smolvm Node.js SDK](https://github.com/smol-machines/smolvm-sdk/tree/main/smolvm-node).

This library is a **Fable-compatible wrapper** around the upstream `smolvm`
npm package. It exposes the core `smolvm-node` API as typed F# with
idiomatic naming conventions and no unsafe JS-object leakage, plus a
Node.js-oriented convenience preset for common JavaScript workloads.

## Status

| Area | State |
|---|---|
| Core binding (`Types`, `Execution`, `Container`, `Machine`) | ✅ Stable |
| Node preset (`NodeMachine`) | ✅ Stable |
| Error hierarchy (`SmolvmError` … `ExecutionError`) | ✅ Stable |
| NuGet package | 🚧 Not yet published |

After applying, run a quick sanity review for:
1. F# compile order in `SmolVm.Fable.fsproj`
2. no remaining Python mentions in README
3. `MachineUnknown` and `ContainerUnknown` wired correctly
4. `Machine.fs` and `Container.fs` both use shared interop helpers
5. no accidental changes to `src/Presets/Node.fs`
