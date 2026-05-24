
import { Record } from "../tests/SmolVm.Fable.Tests/fable_modules/fable-library-js.5.0.0/Types.js";
import { record_type, int32_type, string_type } from "../tests/SmolVm.Fable.Tests/fable_modules/fable-library-js.5.0.0/Reflection.js";

/**
 * Fields common to every smolvm SDK error class.
 */
export class SmolvmErrorInfo extends Record {
    constructor(name, message, code, statusCode) {
        super();
        this.name = name;
        this.message = message;
        this.code = code;
        this.statusCode = (statusCode | 0);
    }
}

export function SmolvmErrorInfo_$reflection() {
    return record_type("SmolVm.Errors.SmolvmErrorInfo", [], SmolvmErrorInfo, () => [["name", string_type], ["message", string_type], ["code", string_type], ["statusCode", int32_type]]);
}

/**
 * Try to extract smolvm error metadata from a raw JS exception.
 */
export function tryParseSmolvmError(e) {
    const js = e;
    const matchValue = js.name;
    switch (matchValue) {
        case "SmolvmError":
        case "NotFoundError":
        case "ConflictError":
        case "BadRequestError":
        case "TimeoutError":
        case "InternalError":
        case "ConnectionError":
            return new SmolvmErrorInfo(js.name, js.message, js.code, js.statusCode);
        default:
            return undefined;
    }
}

/**
 * Matches any smolvm SDK error.
 */
export function $007CSmolvmErr$007C_$007C(e) {
    return tryParseSmolvmError(e);
}

/**
 * Matches NotFoundError (HTTP 404).
 */
export function $007CNotFoundErr$007C_$007C(e) {
    const matchValue = tryParseSmolvmError(e);
    let matchResult, i_1;
    if (matchValue != null) {
        if (matchValue.name === "NotFoundError") {
            matchResult = 0;
            i_1 = matchValue;
        }
        else {
            matchResult = 1;
        }
    }
    else {
        matchResult = 1;
    }
    switch (matchResult) {
        case 0:
            return i_1;
        default:
            return undefined;
    }
}

/**
 * Matches ConflictError (HTTP 409).
 */
export function $007CConflictErr$007C_$007C(e) {
    const matchValue = tryParseSmolvmError(e);
    let matchResult, i_1;
    if (matchValue != null) {
        if (matchValue.name === "ConflictError") {
            matchResult = 0;
            i_1 = matchValue;
        }
        else {
            matchResult = 1;
        }
    }
    else {
        matchResult = 1;
    }
    switch (matchResult) {
        case 0:
            return i_1;
        default:
            return undefined;
    }
}

/**
 * Matches BadRequestError (HTTP 400).
 */
export function $007CBadRequestErr$007C_$007C(e) {
    const matchValue = tryParseSmolvmError(e);
    let matchResult, i_1;
    if (matchValue != null) {
        if (matchValue.name === "BadRequestError") {
            matchResult = 0;
            i_1 = matchValue;
        }
        else {
            matchResult = 1;
        }
    }
    else {
        matchResult = 1;
    }
    switch (matchResult) {
        case 0:
            return i_1;
        default:
            return undefined;
    }
}

/**
 * Matches TimeoutError (HTTP 408 / request timeout).
 */
export function $007CTimeoutErr$007C_$007C(e) {
    const matchValue = tryParseSmolvmError(e);
    let matchResult, i_1;
    if (matchValue != null) {
        if (matchValue.name === "TimeoutError") {
            matchResult = 0;
            i_1 = matchValue;
        }
        else {
            matchResult = 1;
        }
    }
    else {
        matchResult = 1;
    }
    switch (matchResult) {
        case 0:
            return i_1;
        default:
            return undefined;
    }
}

/**
 * Matches InternalError (HTTP 500/502/503).
 */
export function $007CInternalErr$007C_$007C(e) {
    const matchValue = tryParseSmolvmError(e);
    let matchResult, i_1;
    if (matchValue != null) {
        if (matchValue.name === "InternalError") {
            matchResult = 0;
            i_1 = matchValue;
        }
        else {
            matchResult = 1;
        }
    }
    else {
        matchResult = 1;
    }
    switch (matchResult) {
        case 0:
            return i_1;
        default:
            return undefined;
    }
}

/**
 * Matches ConnectionError (network / ECONNREFUSED).
 */
export function $007CConnectionErr$007C_$007C(e) {
    const matchValue = tryParseSmolvmError(e);
    let matchResult, i_1;
    if (matchValue != null) {
        if (matchValue.name === "ConnectionError") {
            matchResult = 0;
            i_1 = matchValue;
        }
        else {
            matchResult = 1;
        }
    }
    else {
        matchResult = 1;
    }
    switch (matchResult) {
        case 0:
            return i_1;
        default:
            return undefined;
    }
}

