
import { class_type } from "../tests/SmolVm.Fable.Tests/fable_modules/fable-library-js.5.0.0/Reflection.js";
import { awaitPromise, startAsPromise } from "../tests/SmolVm.Fable.Tests/fable_modules/fable-library-js.5.0.0/Async.js";
import { singleton } from "../tests/SmolVm.Fable.Tests/fable_modules/fable-library-js.5.0.0/AsyncBuilder.js";
import { machineCreate } from "smolvm";
import { defaultArg, some, map } from "../tests/SmolVm.Fable.Tests/fable_modules/fable-library-js.5.0.0/Option.js";
import { MachineConfig, MachineState } from "./Types.fs.js";
import { containerOptsToJs, logsOptsToJs, execOptsToJs } from "./Internal/Interop.fs.js";
import { ExecResult_$ctor_57DF0130 } from "./Execution.fs.js";
import { Container_$ctor_Z702EEDE6 } from "./Container.fs.js";
import { map as map_1 } from "../tests/SmolVm.Fable.Tests/fable_modules/fable-library-js.5.0.0/Array.js";
import { printf, toText } from "../tests/SmolVm.Fable.Tests/fable_modules/fable-library-js.5.0.0/String.js";

export class Machine {
    constructor(js) {
        this.js = js;
    }
}

export function Machine_$reflection() {
    return class_type("SmolVm.Machine.Machine", undefined, Machine);
}

export function Machine_$ctor_230CC1A(js) {
    return new Machine(js);
}

/**
 * Create and start a machine. Mirrors Machine.create(config) in JS.
 * MachineConfig is [<Pojo>] so Fable passes it as a plain JS object.
 */
export function Machine_Create_Z4C8FDF79(config) {
    return startAsPromise(singleton.Delay(() => singleton.Bind(awaitPromise(machineCreate(config)), (_arg) => singleton.Return(Machine_$ctor_230CC1A(_arg)))));
}

export function Machine__get_Js(_) {
    return _.js;
}

export function Machine__get_Name(_) {
    return _.js.name;
}

export function Machine__get_IsStarted(_) {
    return _.js.isStarted;
}

export function Machine__get_Mounts(_) {
    return _.js.mounts;
}

export function Machine__get_Info(_) {
    return _.js.info;
}

export function Machine__get_State(_) {
    return map((_arg) => {
        switch (_arg) {
            case "created":
                return new MachineState(0, []);
            case "running":
                return new MachineState(1, []);
            case "stopped":
                return new MachineState(2, []);
            default:
                return new MachineState(3, [_arg]);
        }
    }, _.js.state);
}

export function Machine__Start(_) {
    return _.js.start();
}

export function Machine__Stop(_) {
    return _.js.stop();
}

export function Machine__Delete(_) {
    return _.js.delete();
}

export function Machine__Status(_) {
    return _.js.status();
}

export function Machine__Exec_Z27CFC470(_, command, options) {
    return startAsPromise(singleton.Delay(() => {
        let o;
        return singleton.Bind((options != null) ? ((o = options, awaitPromise(_.js.exec(command, some(execOptsToJs(o)))))) : awaitPromise(_.js.exec(command)), (_arg) => singleton.Return(ExecResult_$ctor_57DF0130(_arg)));
    }));
}

export function Machine__Run_442174AB(_, image, command, options) {
    return startAsPromise(singleton.Delay(() => {
        let o;
        return singleton.Bind((options != null) ? ((o = options, awaitPromise(_.js.run(image, command, some(execOptsToJs(o)))))) : awaitPromise(_.js.run(image, command)), (_arg) => singleton.Return(ExecResult_$ctor_57DF0130(_arg)));
    }));
}

export function Machine__Logs_774CA14C(_, options) {
    if (options != null) {
        const o = options;
        return _.js.logs(some(logsOptsToJs(o)));
    }
    else {
        return _.js.logs();
    }
}

export function Machine__CreateContainer_Z3B18001(_, options) {
    return startAsPromise(singleton.Delay(() => singleton.Bind(awaitPromise(_.js.createContainer(containerOptsToJs(options))), (_arg) => singleton.Return(Container_$ctor_Z702EEDE6(_arg)))));
}

export function Machine__ListContainers(_) {
    return startAsPromise(singleton.Delay(() => singleton.Bind(awaitPromise(_.js.listContainers()), (_arg) => singleton.Return(map_1(Container_$ctor_Z702EEDE6, _arg)))));
}

export function Machine__GetContainer_Z721C83C5(_, id) {
    return startAsPromise(singleton.Delay(() => singleton.Bind(awaitPromise(_.js.getContainer(id)), (_arg) => singleton.Return(Container_$ctor_Z702EEDE6(_arg)))));
}

export function Machine__ListImages(_) {
    return _.js.listImages();
}

export function Machine__PullImage_27AED5E3(_, image, ociPlatform) {
    if (ociPlatform == null) {
        return _.js.pullImage(image);
    }
    else {
        const p = ociPlatform;
        return _.js.pullImage(image, p);
    }
}

/**
 * Run `fn` against a freshly created machine, then stop + delete it.
 * Cleanup runs even if `fn` raises; the original exception is re-raised.
 */
export function withMachine(config, fn) {
    return startAsPromise(singleton.Delay(() => singleton.Bind(awaitPromise(Machine_Create_Z4C8FDF79(config)), (_arg) => {
        const m = _arg;
        return singleton.TryWith(singleton.Delay(() => singleton.Bind(awaitPromise(fn(m)), (_arg_1) => singleton.Combine(singleton.TryWith(singleton.Delay(() => singleton.Bind(awaitPromise(Machine__Stop(m)), () => singleton.Return(undefined))), (_arg_3) => {
            return singleton.Zero();
        }), singleton.Delay(() => singleton.Combine(singleton.TryWith(singleton.Delay(() => singleton.Bind(awaitPromise(Machine__Delete(m)), () => singleton.Return(undefined))), (_arg_5) => {
            return singleton.Zero();
        }), singleton.Delay(() => singleton.Return(_arg_1))))))), (_arg_6) => singleton.Combine(singleton.TryWith(singleton.Delay(() => singleton.Bind(awaitPromise(Machine__Stop(m)), () => singleton.Return(undefined))), (_arg_8) => {
            return singleton.Zero();
        }), singleton.Delay(() => singleton.Combine(singleton.TryWith(singleton.Delay(() => singleton.Bind(awaitPromise(Machine__Delete(m)), () => singleton.Return(undefined))), (_arg_10) => {
            return singleton.Zero();
        }), singleton.Delay(() => singleton.Return((() => {
            throw _arg_6;
        })()))))));
    })));
}

export function quickExec(command, config) {
    let arg;
    return withMachine(defaultArg(config, new MachineConfig((arg = (~~Date.now() | 0), toText(printf("quick-exec-%d"))(arg)), undefined, undefined, undefined, undefined)), (m) => Machine__Exec_Z27CFC470(m, command));
}

export function quickRun(image, command, config) {
    let arg;
    return withMachine(defaultArg(config, new MachineConfig((arg = (~~Date.now() | 0), toText(printf("quick-run-%d"))(arg)), undefined, undefined, undefined, undefined)), (m) => Machine__Run_442174AB(m, image, command));
}

