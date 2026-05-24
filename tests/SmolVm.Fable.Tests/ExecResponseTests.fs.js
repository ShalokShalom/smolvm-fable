
import { ExecResponse } from "../../src/Types.fs.js";
import { Test_test_230D71B4, Test_testSequenced_Z4A41A2F0 } from "./fable_modules/Scriptorium.Quill.0.2.1/DSL.fs.js";
import { snap } from "./Helpers.fs.js";
import { isEqualTo, assertThat } from "./fable_modules/Scriptorium.Nib.0.2.1/Assertion.fs.js";
import { ofArray } from "./fable_modules/fable-library-js.5.0.0/List.js";

function makeResponse(stdout, stderr, exitCode) {
    return new ExecResponse(exitCode, stdout, stderr);
}

export const tests = Test_testSequenced_Z4A41A2F0("ExecResponse", ofArray([Test_test_230D71B4("successful echo response snapshot", (t) => {
    snap(t, makeResponse("Hello!\n", "", 0));
}, "/home/cosmos/Projects/smolvm-fable/tests/SmolVm.Fable.Tests/ExecResponseTests.fs", 29), Test_test_230D71B4("empty stdout response snapshot", (t_1) => {
    snap(t_1, makeResponse("", "", 0));
}, "/home/cosmos/Projects/smolvm-fable/tests/SmolVm.Fable.Tests/ExecResponseTests.fs", 36), Test_test_230D71B4("stderr is captured", (t_2) => {
    snap(t_2, makeResponse("", "error: file not found\n", 1));
}, "/home/cosmos/Projects/smolvm-fable/tests/SmolVm.Fable.Tests/ExecResponseTests.fs", 43), Test_test_230D71B4("non-zero exit code snapshot", (t_3) => {
    snap(t_3, makeResponse("", "", 127));
}, "/home/cosmos/Projects/smolvm-fable/tests/SmolVm.Fable.Tests/ExecResponseTests.fs", 50), Test_test_230D71B4("stdout equality assertion", (_arg) => {
    assertThat(makeResponse("Hello!\n", "", 0).stdout, isEqualTo("Hello!\n"));
}, "/home/cosmos/Projects/smolvm-fable/tests/SmolVm.Fable.Tests/ExecResponseTests.fs", 57), Test_test_230D71B4("exit code zero means success", (_arg_1) => {
    assertThat(makeResponse("ok", "", 0).exitCode, isEqualTo(0));
}, "/home/cosmos/Projects/smolvm-fable/tests/SmolVm.Fable.Tests/ExecResponseTests.fs", 64), Test_test_230D71B4("multi-line stdout snapshot", (t_4) => {
    snap(t_4, makeResponse("line1\nline2\nline3\n", "", 0));
}, "/home/cosmos/Projects/smolvm-fable/tests/SmolVm.Fable.Tests/ExecResponseTests.fs", 71)]));

