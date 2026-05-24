
import { ExecResult__AssertSuccess, ExecResult__get_Output, ExecResult__get_Success, ExecResult__get_ExitCode, ExecResult__get_Stdout, ExecResult_$ctor_57DF0130 } from "../../src/Execution.fs.js";
import { ExecResponse } from "../../src/Types.fs.js";
import { Test_test_230D71B4, Test_testSequenced_Z4A41A2F0 } from "./fable_modules/Scriptorium.Quill.0.2.1/DSL.fs.js";
import { snap } from "./Helpers.fs.js";
import { throws, isEqualTo, assertThat } from "./fable_modules/Scriptorium.Nib.0.2.1/Assertion.fs.js";
import { ofArray } from "./fable_modules/fable-library-js.5.0.0/List.js";

function makeResult(stdout, stderr, exitCode) {
    return ExecResult_$ctor_57DF0130(new ExecResponse(exitCode, stdout, stderr));
}

export const tests = Test_testSequenced_Z4A41A2F0("ExecResult", ofArray([Test_test_230D71B4("successful echo result snapshot", (t) => {
    snap(t, makeResult("Hello!\n", "", 0));
}, "/home/cosmos/Projects/smolvm-fable/tests/SmolVm.Fable.Tests/ExecResultTests.fs", 27), Test_test_230D71B4("empty stdout result snapshot", (t_1) => {
    snap(t_1, makeResult("", "", 0));
}, "/home/cosmos/Projects/smolvm-fable/tests/SmolVm.Fable.Tests/ExecResultTests.fs", 34), Test_test_230D71B4("stderr is captured", (t_2) => {
    snap(t_2, makeResult("", "error: file not found\n", 1));
}, "/home/cosmos/Projects/smolvm-fable/tests/SmolVm.Fable.Tests/ExecResultTests.fs", 41), Test_test_230D71B4("non-zero exit code snapshot", (t_3) => {
    snap(t_3, makeResult("", "", 127));
}, "/home/cosmos/Projects/smolvm-fable/tests/SmolVm.Fable.Tests/ExecResultTests.fs", 48), Test_test_230D71B4("stdout equality assertion", (_arg) => {
    assertThat(ExecResult__get_Stdout(makeResult("Hello!\n", "", 0)), isEqualTo("Hello!\n"));
}, "/home/cosmos/Projects/smolvm-fable/tests/SmolVm.Fable.Tests/ExecResultTests.fs", 55), Test_test_230D71B4("exit-code zero means success", (_arg_1) => {
    assertThat(ExecResult__get_ExitCode(makeResult("ok", "", 0)), isEqualTo(0));
}, "/home/cosmos/Projects/smolvm-fable/tests/SmolVm.Fable.Tests/ExecResultTests.fs", 62), Test_test_230D71B4("Success is true for exit code 0", (_arg_2) => {
    assertThat(ExecResult__get_Success(makeResult("ok", "", 0)), isEqualTo(true));
}, "/home/cosmos/Projects/smolvm-fable/tests/SmolVm.Fable.Tests/ExecResultTests.fs", 69), Test_test_230D71B4("Success is false for non-zero exit code", (_arg_3) => {
    assertThat(ExecResult__get_Success(makeResult("", "oops", 1)), isEqualTo(false));
}, "/home/cosmos/Projects/smolvm-fable/tests/SmolVm.Fable.Tests/ExecResultTests.fs", 76), Test_test_230D71B4("Output combines stdout and stderr", (_arg_4) => {
    assertThat(ExecResult__get_Output(makeResult("out", "err", 0)), isEqualTo("out\nerr"));
}, "/home/cosmos/Projects/smolvm-fable/tests/SmolVm.Fable.Tests/ExecResultTests.fs", 83), Test_test_230D71B4("AssertSuccess raises ExecutionError on failure", (_arg_5) => {
    const result_9 = makeResult("", "boom", 2);
    assertThat(() => {
        ExecResult__AssertSuccess(result_9);
    }, throws);
}, "/home/cosmos/Projects/smolvm-fable/tests/SmolVm.Fable.Tests/ExecResultTests.fs", 90), Test_test_230D71B4("AssertSuccess returns self on success", (_arg_6) => {
    assertThat(ExecResult__get_Stdout(ExecResult__AssertSuccess(makeResult("ok", "", 0))), isEqualTo("ok"));
}, "/home/cosmos/Projects/smolvm-fable/tests/SmolVm.Fable.Tests/ExecResultTests.fs", 99), Test_test_230D71B4("multi-line stdout snapshot", (t_4) => {
    snap(t_4, makeResult("line1\nline2\nline3\n", "", 0));
}, "/home/cosmos/Projects/smolvm-fable/tests/SmolVm.Fable.Tests/ExecResultTests.fs", 108)]));

