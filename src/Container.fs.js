
import { class_type } from "../demo/fable_modules/fable-library-js.5.0.0/Reflection.js";
import { ContainerState } from "./Types.fs.js";
import { awaitPromise, startAsPromise } from "../demo/fable_modules/fable-library-js.5.0.0/Async.js";
import { singleton } from "../demo/fable_modules/fable-library-js.5.0.0/AsyncBuilder.js";
import { execOptsToJs } from "./Internal/Interop.fs.js";
import { some } from "../demo/fable_modules/fable-library-js.5.0.0/Option.js";
import { ExecResult_$ctor_57DF0130 } from "./Execution.fs.js";

export class Container {
    constructor(js) {
        this.js = js;
    }
}

export function Container_$reflection() {
    return class_type("SmolVm.Container.Container", undefined, Container);
}

export function Container_$ctor_Z702EEDE6(js) {
    return new Container(js);
}

export function Container__get_Id(_) {
    return _.js.id;
}

export function Container__get_Info(_) {
    return _.js.info;
}

export function Container__get_Image(_) {
    return _.js.image;
}

export function Container__get_Command(_) {
    return _.js.command;
}

export function Container__get_CreatedAt(_) {
    return _.js.createdAt;
}

export function Container__get_State(_) {
    const matchValue = _.js.state;
    switch (matchValue) {
        case "created":
            return new ContainerState(0, []);
        case "running":
            return new ContainerState(1, []);
        case "stopped":
            return new ContainerState(2, []);
        default:
            return new ContainerState(3, [matchValue]);
    }
}

export function Container__Start(_) {
    return _.js.start();
}

/**
 * Stop the container. `timeout` is forwarded as `timeoutSecs`.
 */
export function Container__Stop_71136F3F(_, timeout) {
    if (timeout == null) {
        return _.js.stop();
    }
    else {
        const t = timeout | 0;
        return _.js.stop(t);
    }
}

export function Container__Delete_6FCE9E49(_, force) {
    if (force == null) {
        return _.js.delete();
    }
    else {
        const f = force;
        return _.js.delete(f);
    }
}

export function Container__Exec_Z27CFC470(_, command, options) {
    return startAsPromise(singleton.Delay(() => {
        let o;
        return singleton.Bind((options != null) ? ((o = options, awaitPromise(_.js.exec(command, some(execOptsToJs(o)))))) : awaitPromise(_.js.exec(command)), (_arg) => singleton.Return(ExecResult_$ctor_57DF0130(_arg)));
    }));
}

export function Container__Refresh(_) {
    return _.js.refresh();
}

