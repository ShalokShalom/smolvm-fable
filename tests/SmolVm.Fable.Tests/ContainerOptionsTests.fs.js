
import { MachineConfig, MountSpec, ContainerMount, ContainerOptions } from "../../src/Types.fs.js";
import { Test_test_230D71B4, Test_testSequenced_Z4A41A2F0 } from "./fable_modules/Scriptorium.Quill.0.2.1/DSL.fs.js";
import { snap } from "./Helpers.fs.js";
import { ofList } from "./fable_modules/fable-library-js.5.0.0/Map.js";
import { ofArray } from "./fable_modules/fable-library-js.5.0.0/List.js";
import { comparePrimitives } from "./fable_modules/fable-library-js.5.0.0/Util.js";

const minimalOpts = new ContainerOptions("alpine:3.19", undefined, undefined, undefined, undefined);

export const tests = Test_testSequenced_Z4A41A2F0("ContainerOptions", ofArray([Test_test_230D71B4("minimal options snapshot", (t) => {
    snap(t, minimalOpts);
}, "/home/cosmos/Projects/smolvm-fable/tests/SmolVm.Fable.Tests/ContainerOptionsTests.fs", 34), Test_test_230D71B4("options with command", (t_1) => {
    snap(t_1, new ContainerOptions(minimalOpts.image, ["echo", "hello"], minimalOpts.env, minimalOpts.workdir, minimalOpts.mounts));
}, "/home/cosmos/Projects/smolvm-fable/tests/SmolVm.Fable.Tests/ContainerOptionsTests.fs", 40), Test_test_230D71B4("options with env vars", (t_2) => {
    snap(t_2, new ContainerOptions("ubuntu:22.04", minimalOpts.command, ofList(ofArray([["PATH", "/usr/local/bin:/usr/bin"], ["HOME", "/root"]]), {
        Compare: (x, y) => (comparePrimitives(x, y) | 0),
    }), minimalOpts.workdir, minimalOpts.mounts));
}, "/home/cosmos/Projects/smolvm-fable/tests/SmolVm.Fable.Tests/ContainerOptionsTests.fs", 47), Test_test_230D71B4("options with workdir", (t_3) => {
    snap(t_3, new ContainerOptions(minimalOpts.image, minimalOpts.command, minimalOpts.env, "/app", minimalOpts.mounts));
}, "/home/cosmos/Projects/smolvm-fable/tests/SmolVm.Fable.Tests/ContainerOptionsTests.fs", 55), Test_test_230D71B4("options with volume mount", (t_4) => {
    snap(t_4, new ContainerOptions(minimalOpts.image, minimalOpts.command, minimalOpts.env, minimalOpts.workdir, [new ContainerMount("data", "/mnt/data", true)]));
}, "/home/cosmos/Projects/smolvm-fable/tests/SmolVm.Fable.Tests/ContainerOptionsTests.fs", 62), Test_test_230D71B4("machine config with mounts feeding a container", (t_5) => {
    snap(t_5, {
        container: new ContainerOptions("busybox", ["ls", "/mnt/data"], undefined, undefined, [new ContainerMount("data", "/mnt/data", true)]),
        machine: new MachineConfig("runner", undefined, [new MountSpec("/host/data", "data", false)], undefined, undefined),
    });
}, "/home/cosmos/Projects/smolvm-fable/tests/SmolVm.Fable.Tests/ContainerOptionsTests.fs", 75)]));

