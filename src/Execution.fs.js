
import { class_type } from "../demo/fable_modules/fable-library-js.5.0.0/Reflection.js";
import { ExecutionError } from "smolvm";

/**
 * Active pattern to catch a JS ExecutionError thrown by the smolvm SDK.
 * Matches any exn whose JS `name` property is "ExecutionError".
 */
export function $007CExecutionErr$007C_$007C(e) {
    let exitCode, stdout;
    const js = e;
    if (js.name === "ExecutionError") {
        return (exitCode = js.exitCode, (stdout = js.stdout, {
            exitCode: exitCode,
            stderr: js.stderr,
            stdout: stdout,
        }));
    }
    else {
        return undefined;
    }
}

/**
 * Rich result of a command execution.
 * Mirrors the ExecResult class from execution.ts.
 */
export class ExecResult {
    constructor(response) {
        this.response = response;
    }
}

export function ExecResult_$reflection() {
    return class_type("SmolVm.Execution.ExecResult", undefined, ExecResult);
}

export function ExecResult_$ctor_57DF0130(response) {
    return new ExecResult(response);
}

/**
 * Exit code returned by the command.
 */
export function ExecResult__get_ExitCode(_) {
    return _.response.exitCode | 0;
}

/**
 * Standard output captured from the command.
 */
export function ExecResult__get_Stdout(_) {
    return _.response.stdout;
}

/**
 * Standard error captured from the command.
 */
export function ExecResult__get_Stderr(_) {
    return _.response.stderr;
}

/**
 * True when the exit code is 0.
 */
export function ExecResult__get_Success(this$) {
    return ExecResult__get_ExitCode(this$) === 0;
}

/**
 * Combined stdout + stderr output.
 */
export function ExecResult__get_Output(this$) {
    const matchValue = ExecResult__get_Stdout(this$);
    const matchValue_1 = ExecResult__get_Stderr(this$);
    if ((matchValue !== "") && (matchValue_1 !== "")) {
        return (matchValue + "\n") + matchValue_1;
    }
    else if (matchValue !== "") {
        return matchValue;
    }
    else {
        return matchValue_1;
    }
}

/**
 * Assert that the command succeeded (exit code 0).
 * Raises the JS ExecutionError class so that catch clauses in both
 * F# and JS code see the same error type.
 * Returns self for chaining.
 */
export function ExecResult__AssertSuccess(this$) {
    if (!ExecResult__get_Success(this$)) {
        throw ExecutionError(ExecResult__get_ExitCode(this$), ExecResult__get_Stdout(this$), ExecResult__get_Stderr(this$));
    }
    return this$;
}

