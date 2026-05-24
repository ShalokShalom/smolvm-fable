
import { ResourceSpec, MachineConfig } from "../../src/Types.fs.js";
import { Test_test_230D71B4, Test_testSequenced_Z4A41A2F0 } from "./fable_modules/Scriptorium.Quill.0.2.1/DSL.fs.js";
import { snap } from "./Helpers.fs.js";
import { isGreaterThan, assertThat } from "./fable_modules/Scriptorium.Nib.0.2.1/Assertion.fs.js";
import { value } from "./fable_modules/fable-library-js.5.0.0/Option.js";
import { ofArray } from "./fable_modules/fable-library-js.5.0.0/List.js";

function baseCfg(name, resources) {
    return new MachineConfig(name, undefined, undefined, undefined, resources);
}

export const tests = Test_testSequenced_Z4A41A2F0("ResourceSpec", ofArray([Test_test_230D71B4("vcpus only snapshot", (t) => {
    snap(t, baseCfg("vcpu-only", new ResourceSpec(2, undefined)));
}, "/home/cosmos/Projects/smolvm-fable/tests/SmolVm.Fable.Tests/ResourceSpecTests.fs", 26), Test_test_230D71B4("memory only snapshot", (t_1) => {
    snap(t_1, baseCfg("mem-only", new ResourceSpec(undefined, 512)));
}, "/home/cosmos/Projects/smolvm-fable/tests/SmolVm.Fable.Tests/ResourceSpecTests.fs", 32), Test_test_230D71B4("minimum memory (64 MiB) snapshot", (t_2) => {
    snap(t_2, baseCfg("tiny", new ResourceSpec(1, 64)));
}, "/home/cosmos/Projects/smolvm-fable/tests/SmolVm.Fable.Tests/ResourceSpecTests.fs", 38), Test_test_230D71B4("maximum memory (32 GiB) snapshot", (t_3) => {
    snap(t_3, baseCfg("large", new ResourceSpec(8, 32768)));
}, "/home/cosmos/Projects/smolvm-fable/tests/SmolVm.Fable.Tests/ResourceSpecTests.fs", 44), Test_test_230D71B4("combined vcpus and memory snapshot", (t_4) => {
    snap(t_4, baseCfg("full-spec", new ResourceSpec(4, 4096)));
}, "/home/cosmos/Projects/smolvm-fable/tests/SmolVm.Fable.Tests/ResourceSpecTests.fs", 50), Test_test_230D71B4("both fields None snapshot", (t_5) => {
    snap(t_5, baseCfg("defer-all", new ResourceSpec(undefined, undefined)));
}, "/home/cosmos/Projects/smolvm-fable/tests/SmolVm.Fable.Tests/ResourceSpecTests.fs", 56), Test_test_230D71B4("memory value is positive", (_arg) => {
    assertThat(value((new ResourceSpec(2, 128)).memory), isGreaterThan(0));
}, "/home/cosmos/Projects/smolvm-fable/tests/SmolVm.Fable.Tests/ResourceSpecTests.fs", 64), Test_test_230D71B4("vcpu count is positive", (_arg_1) => {
    assertThat(value((new ResourceSpec(4, undefined)).vcpus), isGreaterThan(0));
}, "/home/cosmos/Projects/smolvm-fable/tests/SmolVm.Fable.Tests/ResourceSpecTests.fs", 71)]));

