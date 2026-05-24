
import { unwrap, defaultArg, bind } from "../../demo/fable_modules/fable-library-js.5.0.0/Option.js";
import { ExecOptions } from "../Types.fs.js";
import { class_type } from "../../demo/fable_modules/fable-library-js.5.0.0/Reflection.js";
import { awaitPromise, startAsPromise } from "../../demo/fable_modules/fable-library-js.5.0.0/Async.js";
import { singleton } from "../../demo/fable_modules/fable-library-js.5.0.0/AsyncBuilder.js";
import { Machine__PullImage_27AED5E3, Machine__ListImages, Machine__GetContainer_Z721C83C5, Machine__ListContainers, Machine__CreateContainer_Z3B18001, Machine__Logs_774CA14C, Machine__Run_442174AB, Machine__Exec_Z27CFC470, Machine__Status, Machine__Delete, Machine__Stop, Machine__Start, Machine__get_Info, Machine__get_Mounts, Machine__get_State, Machine__get_IsStarted, Machine__get_Name, Machine_Create_Z4C8FDF79 } from "../Machine.fs.js";
import { append } from "../../demo/fable_modules/fable-library-js.5.0.0/Array.js";
import { ExecResult__get_Stdout } from "../Execution.fs.js";
import { printf, toText } from "../../demo/fable_modules/fable-library-js.5.0.0/String.js";

function toExecOptions(opts) {
    return new ExecOptions(bind((o) => o.env, opts), bind((o_1) => o_1.workdir, opts), bind((o_2) => o_2.timeout, opts));
}

function resolveImage(opts) {
    return defaultArg(bind((o) => o.image, opts), "node:22-alpine");
}

/**
 * A machine specialised for running JavaScript / Node.js code.
 * Delegates lifecycle and execution to an inner Machine.
 * Mirrors the NodeMachine class from presets/node.ts.
 */
export class NodeMachine {
    constructor(inner) {
        this.inner = inner;
    }
}

export function NodeMachine_$reflection() {
    return class_type("SmolVm.Presets.Node.NodeMachine", undefined, NodeMachine);
}

function NodeMachine_$ctor_5331BC83(inner) {
    return new NodeMachine(inner);
}

/**
 * Create and start a Node.js machine.
 * Mirrors NodeMachine.create(config) in node.ts.
 */
export function NodeMachine_Create_Z4C8FDF79(config) {
    return startAsPromise(singleton.Delay(() => singleton.Bind(awaitPromise(Machine_Create_Z4C8FDF79(config)), (_arg) => singleton.Return(NodeMachine_$ctor_5331BC83(_arg)))));
}

export function NodeMachine__get_Name(_) {
    return Machine__get_Name(_.inner);
}

export function NodeMachine__get_IsStarted(_) {
    return Machine__get_IsStarted(_.inner);
}

export function NodeMachine__get_State(_) {
    return Machine__get_State(_.inner);
}

export function NodeMachine__get_Mounts(_) {
    return Machine__get_Mounts(_.inner);
}

export function NodeMachine__get_Info(_) {
    return Machine__get_Info(_.inner);
}

export function NodeMachine__Start(_) {
    return Machine__Start(_.inner);
}

export function NodeMachine__Stop(_) {
    return Machine__Stop(_.inner);
}

export function NodeMachine__Delete(_) {
    return Machine__Delete(_.inner);
}

export function NodeMachine__Status(_) {
    return Machine__Status(_.inner);
}

export function NodeMachine__Exec_Z27CFC470(_, command, options) {
    return Machine__Exec_Z27CFC470(_.inner, command, unwrap(options));
}

export function NodeMachine__Run_442174AB(_, image, command, options) {
    return Machine__Run_442174AB(_.inner, image, command, unwrap(options));
}

export function NodeMachine__Logs_774CA14C(_, options) {
    return Machine__Logs_774CA14C(_.inner, unwrap(options));
}

export function NodeMachine__CreateContainer_Z3B18001(_, options) {
    return Machine__CreateContainer_Z3B18001(_.inner, options);
}

export function NodeMachine__ListContainers(_) {
    return Machine__ListContainers(_.inner);
}

export function NodeMachine__GetContainer_Z721C83C5(_, id) {
    return Machine__GetContainer_Z721C83C5(_.inner, id);
}

export function NodeMachine__ListImages(_) {
    return Machine__ListImages(_.inner);
}

export function NodeMachine__PullImage_27AED5E3(_, image, platform) {
    return Machine__PullImage_27AED5E3(_.inner, image, unwrap(platform));
}

/**
 * Run JavaScript source code directly.
 * Equivalent to `node -e <code>` in a container.
 * Mirrors NodeMachine.runCode(code, options?) in node.ts.
 */
export function NodeMachine__RunCode_Z15F7973(_, code, options) {
    return Machine__Run_442174AB(_.inner, resolveImage(options), ["node", "-e", code], toExecOptions(options));
}

/**
 * Run a JavaScript file by path.
 * Mirrors NodeMachine.runFile(path, options?) in node.ts.
 */
export function NodeMachine__RunFile_Z15F7973(_, path, options) {
    return Machine__Run_442174AB(_.inner, resolveImage(options), ["node", path], toExecOptions(options));
}

/**
 * Run npm commands.
 * Mirrors NodeMachine.npm(args, options?) in node.ts.
 */
export function NodeMachine__Npm_Z27CFC470(_, args, options) {
    return Machine__Run_442174AB(_.inner, "node:22-alpine", append(["npm"], args), unwrap(options));
}

/**
 * Install npm packages.
 * Mirrors NodeMachine.npmInstall(packages, options?) in node.ts.
 */
export function NodeMachine__NpmInstall_Z27CFC470(this$, packages, options) {
    return NodeMachine__Npm_Z27CFC470(this$, append(["install"], packages), unwrap(options));
}

/**
 * Run npx commands.
 * Mirrors NodeMachine.npx(args, options?) in node.ts.
 */
export function NodeMachine__Npx_Z27CFC470(_, args, options) {
    return Machine__Run_442174AB(_.inner, "node:22-alpine", append(["npx"], args), unwrap(options));
}

/**
 * Return the Node.js version string.
 * Mirrors NodeMachine.version(options?) in node.ts.
 */
export function NodeMachine__Version_Z4B0C7AEA(this$, options) {
    return startAsPromise(singleton.Delay(() => singleton.Bind(awaitPromise(NodeMachine__RunCode_Z15F7973(this$, "console.log(process.version)", unwrap(options))), (_arg) => singleton.Return(ExecResult__get_Stdout(_arg).trim()))));
}

/**
 * Run ES module code (passes --input-type=module to node).
 * Mirrors NodeMachine.runESM(code, options?) in node.ts.
 */
export function NodeMachine__RunESM_Z15F7973(_, code, options) {
    return Machine__Run_442174AB(_.inner, resolveImage(options), ["node", "--input-type=module", "-e", code], toExecOptions(options));
}

/**
 * Evaluate a JavaScript expression and return JSON-serialised output.
 * Mirrors NodeMachine.evaluate(expression, options?) in node.ts.
 */
export function NodeMachine__Evaluate_Z15F7973(this$, expression, options) {
    return NodeMachine__RunCode_Z15F7973(this$, toText(printf("console.log(JSON.stringify(%s))"))(expression), unwrap(options));
}

