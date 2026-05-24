
import { map } from "../../demo/fable_modules/fable-library-js.5.0.0/Array.js";
import { toArray } from "../../demo/fable_modules/fable-library-js.5.0.0/Map.js";
import { jsOptions, defaultOf } from "../../demo/fable_modules/fable-library-js.5.0.0/Util.js";
import { map as map_1, toNullable } from "../../demo/fable_modules/fable-library-js.5.0.0/Option.js";

/**
 * Convert an optional env map to the EnvVar[] shape the smolvm API expects.
 */
export function toEnvVars(env) {
    if (env != null) {
        return map((tupledArg) => ({
            name: tupledArg[0],
            value: tupledArg[1],
        }), toArray(env));
    }
    else {
        return defaultOf();
    }
}

/**
 * Serialise ExecOptions to the JS object shape accepted by the API.
 * Field names match ExecRequest / RunRequest:
 * env -> EnvVar[], workdir -> string?, timeoutSecs -> number?
 */
export function execOptsToJs(o) {
    return jsOptions((x) => {
        x.env = toEnvVars(o.env);
        x.workdir = toNullable(o.workdir);
        x.timeoutSecs = toNullable(o.timeout);
    });
}

/**
 * Serialise LogsOptions to the LogsQuery shape used by client.ts streamLogs.
 * `tail` is supported by the HTTP client even though SDK types.ts omits it.
 */
export function logsOptsToJs(o) {
    return jsOptions((x) => {
        x.follow = toNullable(o.follow);
        x.since = toNullable(o.since);
        x.tail = toNullable(o.tail);
    });
}

/**
 * Serialise ContainerOptions to the CreateContainerRequest shape.
 * ContainerMount.tag maps to source (the virtiofs tag used as host-side key).
 */
export function containerOptsToJs(options) {
    const mounts = toNullable(map_1((ms) => map((m) => ({
        readonly: toNullable(m.readOnly),
        source: m.tag,
        target: m.target,
    }), ms), options.mounts));
    return jsOptions((x) => {
        x.image = options.image;
        x.command = toNullable(options.command);
        x.env = toEnvVars(options.env);
        x.workdir = toNullable(options.workdir);
        x.mounts = mounts;
    });
}

