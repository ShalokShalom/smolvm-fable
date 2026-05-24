
import { ResourceSpec, MachineConfig } from "../../src/Types.fs.js";
import { Test_test_230D71B4, Test_testSequenced_Z4A41A2F0 } from "./fable_modules/Scriptorium.Quill.0.2.1/DSL.fs.js";
import { snap } from "./Helpers.fs.js";
import { isGreaterThan, assertThat } from "./fable_modules/Scriptorium.Nib.0.2.1/Assertion.fs.js";
import { value } from "./fable_modules/fable-library-js.5.0.0/Option.js";
import { ofArray } from "./fable_modules/fable-library-js.5.0.0/List.js";

function cfg(name, res) {
    return new MachineConfig(name, undefined, undefined, undefined, res);
}

export const tests = Test_testSequenced_Z4A41A2F0("ResourceLimits", ofArray([Test_test_230D71B4("minimum memory config snapshot", (t) => {
    snap(t, cfg("tiny", new ResourceSpec(1, 64)));
}, "/home/cosmos/Projects/smolvm-fable/tests/SmolVm.Fable.Tests/ResourceLimitsTests.fs", 34), Test_test_230D71B4("maximum memory config snapshot", (t_1) => {
    snap(t_1, cfg("large", new ResourceSpec(8, 32768)));
}, "/home/cosmos/Projects/smolvm-fable/tests/SmolVm.Fable.Tests/ResourceLimitsTests.fs", 40), Test_test_230D71B4("single vCPU config snapshot", (t_2) => {
    snap(t_2, cfg("single-cpu", new ResourceSpec(1, undefined)));
}, "/home/cosmos/Projects/smolvm-fable/tests/SmolVm.Fable.Tests/ResourceLimitsTests.fs", 46), Test_test_230D71B4("high-vCPU config snapshot", (t_3) => {
    snap(t_3, cfg("high-cpu", new ResourceSpec(16, undefined)));
}, "/home/cosmos/Projects/smolvm-fable/tests/SmolVm.Fable.Tests/ResourceLimitsTests.fs", 52), Test_test_230D71B4("combined resource config snapshot", (t_4) => {
    snap(t_4, cfg("full-spec", new ResourceSpec(8, 4096)));
}, "/home/cosmos/Projects/smolvm-fable/tests/SmolVm.Fable.Tests/ResourceLimitsTests.fs", 58), Test_test_230D71B4("both fields None defers to daemon defaults", (t_5) => {
    snap(t_5, cfg("defer-all", new ResourceSpec(undefined, undefined)));
}, "/home/cosmos/Projects/smolvm-fable/tests/SmolVm.Fable.Tests/ResourceLimitsTests.fs", 64), Test_test_230D71B4("memory value is positive", (_arg) => {
    assertThat(value((new ResourceSpec(2, 128)).memory), isGreaterThan(0));
}, "/home/cosmos/Projects/smolvm-fable/tests/SmolVm.Fable.Tests/ResourceLimitsTests.fs", 70), Test_test_230D71B4("vCPU count is positive", (_arg_1) => {
    assertThat(value((new ResourceSpec(4, undefined)).vcpus), isGreaterThan(0));
}, "/home/cosmos/Projects/smolvm-fable/tests/SmolVm.Fable.Tests/ResourceLimitsTests.fs", 77)]));

