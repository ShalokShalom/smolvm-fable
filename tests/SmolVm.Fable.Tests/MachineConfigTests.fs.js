
import { PortSpec, MountSpec, ResourceSpec, MachineConfig } from "../../src/Types.fs.js";
import { Test_test_230D71B4, Test_testSequenced_Z4A41A2F0 } from "./fable_modules/Scriptorium.Quill.0.2.1/DSL.fs.js";
import { snapWith, snap } from "./Helpers.fs.js";
import { printf, toText } from "./fable_modules/fable-library-js.5.0.0/String.js";
import { ofArray } from "./fable_modules/fable-library-js.5.0.0/List.js";

const minimalConfig = new MachineConfig("my-machine", undefined, undefined, undefined, undefined);

export const tests = Test_testSequenced_Z4A41A2F0("MachineConfig", ofArray([Test_test_230D71B4("minimal config snapshot", (t) => {
    snap(t, minimalConfig);
}, "/home/cosmos/Projects/smolvm-fable/tests/SmolVm.Fable.Tests/MachineConfigTests.fs", 29), Test_test_230D71B4("config with custom serverUrl", (t_1) => {
    snap(t_1, new MachineConfig(minimalConfig.name, "http://192.168.1.100:8080", minimalConfig.mounts, minimalConfig.ports, minimalConfig.resources));
}, "/home/cosmos/Projects/smolvm-fable/tests/SmolVm.Fable.Tests/MachineConfigTests.fs", 35), Test_test_230D71B4("config with resource limits", (t_2) => {
    snap(t_2, new MachineConfig("constrained", minimalConfig.serverUrl, minimalConfig.mounts, minimalConfig.ports, new ResourceSpec(2, 512)));
}, "/home/cosmos/Projects/smolvm-fable/tests/SmolVm.Fable.Tests/MachineConfigTests.fs", 42), Test_test_230D71B4("config with memory-only resources", (t_3) => {
    snap(t_3, new MachineConfig("memory-only", minimalConfig.serverUrl, minimalConfig.mounts, minimalConfig.ports, new ResourceSpec(undefined, 256)));
}, "/home/cosmos/Projects/smolvm-fable/tests/SmolVm.Fable.Tests/MachineConfigTests.fs", 52), Test_test_230D71B4("config with mounts", (t_4) => {
    snap(t_4, new MachineConfig(minimalConfig.name, minimalConfig.serverUrl, [new MountSpec("/data", "data", true)], minimalConfig.ports, minimalConfig.resources));
}, "/home/cosmos/Projects/smolvm-fable/tests/SmolVm.Fable.Tests/MachineConfigTests.fs", 62), Test_test_230D71B4("config with port mappings", (t_5) => {
    snap(t_5, new MachineConfig(minimalConfig.name, minimalConfig.serverUrl, minimalConfig.mounts, [new PortSpec(8080, 80)], minimalConfig.resources));
}, "/home/cosmos/Projects/smolvm-fable/tests/SmolVm.Fable.Tests/MachineConfigTests.fs", 73), Test_test_230D71B4("round-trip sprintf representation snapshot", (t_6) => {
    let clo;
    const cfg_5 = new MachineConfig("round-trip", minimalConfig.serverUrl, minimalConfig.mounts, minimalConfig.ports, new ResourceSpec(1, 128));
    snapWith(t_6, (clo = toText(printf("%A")), clo), cfg_5);
}, "/home/cosmos/Projects/smolvm-fable/tests/SmolVm.Fable.Tests/MachineConfigTests.fs", 81)]));

