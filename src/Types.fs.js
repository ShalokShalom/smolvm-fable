
import { Union, Record } from "../demo/fable_modules/fable-library-js.5.0.0/Types.js";
import { float64_type, int64_type, class_type, union_type, array_type, option_type, bool_type, string_type, record_type, int32_type } from "../demo/fable_modules/fable-library-js.5.0.0/Reflection.js";

export class PortSpec extends Record {
    constructor(host, guest) {
        super();
        this.host = (host | 0);
        this.guest = (guest | 0);
    }
}

export function PortSpec_$reflection() {
    return record_type("SmolVm.Types.PortSpec", [], PortSpec, () => [["host", int32_type], ["guest", int32_type]]);
}

export class MountSpec extends Record {
    constructor(hostPath, tag, readOnly) {
        super();
        this.hostPath = hostPath;
        this.tag = tag;
        this.readOnly = readOnly;
    }
}

export function MountSpec_$reflection() {
    return record_type("SmolVm.Types.MountSpec", [], MountSpec, () => [["hostPath", string_type], ["tag", string_type], ["readOnly", option_type(bool_type)]]);
}

export class ResourceSpec extends Record {
    constructor(vcpus, memory) {
        super();
        this.vcpus = vcpus;
        this.memory = memory;
    }
}

export function ResourceSpec_$reflection() {
    return record_type("SmolVm.Types.ResourceSpec", [], ResourceSpec, () => [["vcpus", option_type(int32_type)], ["memory", option_type(int32_type)]]);
}

/**
 * Passed directly to Machine.create() in JS — Fable serialises records to
 * plain JS objects by default, so no manual conversion is needed.
 */
export class MachineConfig extends Record {
    constructor(name, serverUrl, mounts, ports, resources) {
        super();
        this.name = name;
        this.serverUrl = serverUrl;
        this.mounts = mounts;
        this.ports = ports;
        this.resources = resources;
    }
}

export function MachineConfig_$reflection() {
    return record_type("SmolVm.Types.MachineConfig", [], MachineConfig, () => [["name", string_type], ["serverUrl", option_type(string_type)], ["mounts", option_type(array_type(MountSpec_$reflection()))], ["ports", option_type(array_type(PortSpec_$reflection()))], ["resources", option_type(ResourceSpec_$reflection())]]);
}

export class MachineState extends Union {
    constructor(tag, fields) {
        super();
        this.tag = tag;
        this.fields = fields;
    }
    cases() {
        return ["MachineCreated", "MachineRunning", "MachineStopped", "MachineUnknown"];
    }
}

export function MachineState_$reflection() {
    return union_type("SmolVm.Types.MachineState", [], MachineState, () => [[], [], [], [["Item", string_type]]]);
}

export class ContainerState extends Union {
    constructor(tag, fields) {
        super();
        this.tag = tag;
        this.fields = fields;
    }
    cases() {
        return ["ContainerCreated", "ContainerRunning", "ContainerStopped", "ContainerUnknown"];
    }
}

export function ContainerState_$reflection() {
    return union_type("SmolVm.Types.ContainerState", [], ContainerState, () => [[], [], [], [["Item", string_type]]]);
}

export class ExecOptions extends Record {
    constructor(env, workdir, timeout) {
        super();
        this.env = env;
        this.workdir = workdir;
        this.timeout = timeout;
    }
}

export function ExecOptions_$reflection() {
    return record_type("SmolVm.Types.ExecOptions", [], ExecOptions, () => [["env", option_type(class_type("Microsoft.FSharp.Collections.FSharpMap`2", [string_type, string_type]))], ["workdir", option_type(string_type)], ["timeout", option_type(int32_type)]]);
}

/**
 * Mirrors LogsQuery used by client.ts streamLogs.
 * `tail` is supported by the HTTP client even though SDK types.ts omits it.
 */
export class LogsOptions extends Record {
    constructor(since, follow, tail) {
        super();
        this.since = since;
        this.follow = follow;
        this.tail = tail;
    }
}

export function LogsOptions_$reflection() {
    return record_type("SmolVm.Types.LogsOptions", [], LogsOptions, () => [["since", option_type(string_type)], ["follow", option_type(bool_type)], ["tail", option_type(int32_type)]]);
}

/**
 * Mirrors ContainerMountSpec: tag is the virtiofs tag,
 * target is the container-side mount path.
 */
export class ContainerMount extends Record {
    constructor(tag, target, readOnly) {
        super();
        this.tag = tag;
        this.target = target;
        this.readOnly = readOnly;
    }
}

export function ContainerMount_$reflection() {
    return record_type("SmolVm.Types.ContainerMount", [], ContainerMount, () => [["tag", string_type], ["target", string_type], ["readOnly", option_type(bool_type)]]);
}

/**
 * SDK-level container creation options.
 */
export class ContainerOptions extends Record {
    constructor(image, command, env, workdir, mounts) {
        super();
        this.image = image;
        this.command = command;
        this.env = env;
        this.workdir = workdir;
        this.mounts = mounts;
    }
}

export function ContainerOptions_$reflection() {
    return record_type("SmolVm.Types.ContainerOptions", [], ContainerOptions, () => [["image", string_type], ["command", option_type(array_type(string_type))], ["env", option_type(class_type("Microsoft.FSharp.Collections.FSharpMap`2", [string_type, string_type]))], ["workdir", option_type(string_type)], ["mounts", option_type(array_type(ContainerMount_$reflection()))]]);
}

export class CodeOptions extends Record {
    constructor(env, workdir, timeout, image) {
        super();
        this.env = env;
        this.workdir = workdir;
        this.timeout = timeout;
        this.image = image;
    }
}

export function CodeOptions_$reflection() {
    return record_type("SmolVm.Types.CodeOptions", [], CodeOptions, () => [["env", option_type(class_type("Microsoft.FSharp.Collections.FSharpMap`2", [string_type, string_type]))], ["workdir", option_type(string_type)], ["timeout", option_type(int32_type)], ["image", option_type(string_type)]]);
}

export class MountInfo extends Record {
    constructor(hostPath, tag, readOnly) {
        super();
        this.hostPath = hostPath;
        this.tag = tag;
        this.readOnly = readOnly;
    }
}

export function MountInfo_$reflection() {
    return record_type("SmolVm.Types.MountInfo", [], MountInfo, () => [["hostPath", string_type], ["tag", string_type], ["readOnly", bool_type]]);
}

export class MachineInfo extends Record {
    constructor(name, state, mounts) {
        super();
        this.name = name;
        this.state = state;
        this.mounts = mounts;
    }
}

export function MachineInfo_$reflection() {
    return record_type("SmolVm.Types.MachineInfo", [], MachineInfo, () => [["name", string_type], ["state", string_type], ["mounts", array_type(MountInfo_$reflection())]]);
}

export class ListMachinesResponse extends Record {
    constructor(machines) {
        super();
        this.machines = machines;
    }
}

export function ListMachinesResponse_$reflection() {
    return record_type("SmolVm.Types.ListMachinesResponse", [], ListMachinesResponse, () => [["machines", array_type(MachineInfo_$reflection())]]);
}

export class ExecResponse extends Record {
    constructor(exitCode, stdout, stderr) {
        super();
        this.exitCode = (exitCode | 0);
        this.stdout = stdout;
        this.stderr = stderr;
    }
}

export function ExecResponse_$reflection() {
    return record_type("SmolVm.Types.ExecResponse", [], ExecResponse, () => [["exitCode", int32_type], ["stdout", string_type], ["stderr", string_type]]);
}

export class ImageInfo extends Record {
    constructor(id, name, size) {
        super();
        this.id = id;
        this.name = name;
        this.size = size;
    }
}

export function ImageInfo_$reflection() {
    return record_type("SmolVm.Types.ImageInfo", [], ImageInfo, () => [["id", string_type], ["name", string_type], ["size", int64_type]]);
}

export class ListImagesResponse extends Record {
    constructor(images) {
        super();
        this.images = images;
    }
}

export function ListImagesResponse_$reflection() {
    return record_type("SmolVm.Types.ListImagesResponse", [], ListImagesResponse, () => [["images", array_type(ImageInfo_$reflection())]]);
}

export class PullImageResponse extends Record {
    constructor(image) {
        super();
        this.image = image;
    }
}

export function PullImageResponse_$reflection() {
    return record_type("SmolVm.Types.PullImageResponse", [], PullImageResponse, () => [["image", ImageInfo_$reflection()]]);
}

export class DeleteResponse extends Record {
    constructor(deleted) {
        super();
        this.deleted = deleted;
    }
}

export function DeleteResponse_$reflection() {
    return record_type("SmolVm.Types.DeleteResponse", [], DeleteResponse, () => [["deleted", bool_type]]);
}

export class ContainerInfo extends Record {
    constructor(id, image, command, state, createdAt) {
        super();
        this.id = id;
        this.image = image;
        this.command = command;
        this.state = state;
        this.createdAt = createdAt;
    }
}

export function ContainerInfo_$reflection() {
    return record_type("SmolVm.Types.ContainerInfo", [], ContainerInfo, () => [["id", string_type], ["image", string_type], ["command", array_type(string_type)], ["state", string_type], ["createdAt", float64_type]]);
}

export class ListContainersResponse extends Record {
    constructor(containers) {
        super();
        this.containers = containers;
    }
}

export function ListContainersResponse_$reflection() {
    return record_type("SmolVm.Types.ListContainersResponse", [], ListContainersResponse, () => [["containers", array_type(ContainerInfo_$reflection())]]);
}

export class ApiErrorResponse extends Record {
    constructor(error, code) {
        super();
        this.error = error;
        this.code = code;
    }
}

export function ApiErrorResponse_$reflection() {
    return record_type("SmolVm.Types.ApiErrorResponse", [], ApiErrorResponse, () => [["error", string_type], ["code", option_type(string_type)]]);
}

export class HealthResponse extends Record {
    constructor(status) {
        super();
        this.status = status;
    }
}

export function HealthResponse_$reflection() {
    return record_type("SmolVm.Types.HealthResponse", [], HealthResponse, () => [["status", string_type]]);
}

