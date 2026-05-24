
import { MachineConfig, ResourceSpec, MountSpec, ContainerMount, ContainerOptions } from "../../src/Types.fs.js";
import { Test_test_230D71B4, Test_testSequenced_Z4A41A2F0 } from "./fable_modules/Scriptorium.Quill.0.2.1/DSL.fs.js";
import { snap } from "./Helpers.fs.js";
import { ofList } from "./fable_modules/fable-library-js.5.0.0/Map.js";
import { singleton, ofArray } from "./fable_modules/fable-library-js.5.0.0/List.js";
import { comparePrimitives } from "./fable_modules/fable-library-js.5.0.0/Util.js";

const defaultOpts = new ContainerOptions("alpine:3.19", undefined, undefined, undefined, undefined);

export const tests = Test_testSequenced_Z4A41A2F0("ContainerConfig", ofArray([Test_test_230D71B4("default container config snapshot", (t) => {
    snap(t, defaultOpts);
}, "/home/cosmos/Projects/smolvm-fable/tests/SmolVm.Fable.Tests/ContainerConfigTests.fs", 34), Test_test_230D71B4("image-only config", (t_1) => {
    snap(t_1, new ContainerOptions("alpine:3.19", defaultOpts.command, defaultOpts.env, defaultOpts.workdir, defaultOpts.mounts));
}, "/home/cosmos/Projects/smolvm-fable/tests/SmolVm.Fable.Tests/ContainerConfigTests.fs", 40), Test_test_230D71B4("config with env vars", (t_2) => {
    snap(t_2, new ContainerOptions("ubuntu:22.04", defaultOpts.command, ofList(ofArray([["PATH", "/usr/local/bin:/usr/bin"], ["HOME", "/root"]]), {
        Compare: (x, y) => (comparePrimitives(x, y) | 0),
    }), defaultOpts.workdir, defaultOpts.mounts));
}, "/home/cosmos/Projects/smolvm-fable/tests/SmolVm.Fable.Tests/ContainerConfigTests.fs", 47), Test_test_230D71B4("config with command and workdir", (t_3) => {
    snap(t_3, new ContainerOptions("python:3.12-slim", ["python", "-c", "print(\'hello\')"], defaultOpts.env, "/app", defaultOpts.mounts));
}, "/home/cosmos/Projects/smolvm-fable/tests/SmolVm.Fable.Tests/ContainerConfigTests.fs", 55), Test_test_230D71B4("config with volume mounts", (t_4) => {
    snap(t_4, new ContainerOptions("busybox", defaultOpts.command, defaultOpts.env, defaultOpts.workdir, [new ContainerMount("data", "/mnt/data", true)]));
}, "/home/cosmos/Projects/smolvm-fable/tests/SmolVm.Fable.Tests/ContainerConfigTests.fs", 66), Test_test_230D71B4("machine config with embedded container options", (t_5) => {
    const machineCfg = new MachineConfig("node-runner", undefined, [new MountSpec("/host/data", "data", false)], undefined, new ResourceSpec(undefined, 256));
    const containerMount = new ContainerMount("data", "/mnt/data", true);
    snap(t_5, {
        container: new ContainerOptions("node:20-alpine", undefined, ofList(singleton(["NODE_ENV", "production"]), {
            Compare: (x_1, y_1) => (comparePrimitives(x_1, y_1) | 0),
        }), undefined, [containerMount]),
        machine: machineCfg,
    });
}, "/home/cosmos/Projects/smolvm-fable/tests/SmolVm.Fable.Tests/ContainerConfigTests.fs", 79)]));

