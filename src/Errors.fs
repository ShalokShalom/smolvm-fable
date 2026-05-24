module SmolVm.Errors

open Fable.Core.JsInterop

// ============================================================================
// JS error interop
//
// The upstream SDK throws JS class instances (SmolvmError, NotFoundError, …)
// which extend Error. F# `exception` declarations produce .NET exception
// objects and cannot be matched against JS class instances in a `try/with`.
//
// Correct approach:
//   - Catch `exn` (the raw JS Error object on the Fable/JS side)
//   - Inspect e?name / e?code via Fable dynamic access
//   - Use the active patterns below for ergonomic matching
//
// Example:
//   try ... with
//   | NotFoundErr e  -> printfn "Not found: %s" e.message
//   | TimeoutErr  e  -> printfn "Timed out: %s" e.message
//   | SmolvmErr   e  -> printfn "SDK error %s (%s)" e.message e.code
// ============================================================================

/// Fields common to every smolvm SDK error class.
type SmolvmErrorInfo =
    { name       : string
      message    : string
      code       : string
      statusCode : int }

/// Try to extract smolvm error metadata from a raw JS exception.
let tryParseSmolvmError (e: exn) : SmolvmErrorInfo option =
    let js = box e
    match js?name with
    | "SmolvmError"
    | "NotFoundError"
    | "ConflictError"
    | "BadRequestError"
    | "TimeoutError"
    | "InternalError"
    | "ConnectionError" ->
        Some { name       = js?name
               message    = js?message
               code       = js?code
               statusCode = js?statusCode }
    | _ -> None

// Active patterns ─────────────────────────────────────────────────────────────

/// Matches any smolvm SDK error.
let (|SmolvmErr|_|)     (e: exn) = tryParseSmolvmError e

/// Matches NotFoundError (HTTP 404).
let (|NotFoundErr|_|)   (e: exn) =
    match tryParseSmolvmError e with
    | Some i when i.name = "NotFoundError"   -> Some i
    | _                                       -> None

/// Matches ConflictError (HTTP 409).
let (|ConflictErr|_|)   (e: exn) =
    match tryParseSmolvmError e with
    | Some i when i.name = "ConflictError"   -> Some i
    | _                                       -> None

/// Matches BadRequestError (HTTP 400).
let (|BadRequestErr|_|) (e: exn) =
    match tryParseSmolvmError e with
    | Some i when i.name = "BadRequestError" -> Some i
    | _                                       -> None

/// Matches TimeoutError (HTTP 408 / request timeout).
let (|TimeoutErr|_|)    (e: exn) =
    match tryParseSmolvmError e with
    | Some i when i.name = "TimeoutError"    -> Some i
    | _                                       -> None

/// Matches InternalError (HTTP 500/502/503).
let (|InternalErr|_|)   (e: exn) =
    match tryParseSmolvmError e with
    | Some i when i.name = "InternalError"   -> Some i
    | _                                       -> None

/// Matches ConnectionError (network / ECONNREFUSED).
let (|ConnectionErr|_|) (e: exn) =
    match tryParseSmolvmError e with
    | Some i when i.name = "ConnectionError" -> Some i
    | _                                       -> None
