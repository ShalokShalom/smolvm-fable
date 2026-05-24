module SmolVm.Errors

open Fable.Core

// ============================================================================
// Error hierarchy
// Mirrors errors.ts: SmolvmError → NotFoundError | ConflictError | …
// ============================================================================

/// Base error raised by all smolvm SDK operations.
exception SmolvmError of message: string * code: string * statusCode: int

/// Resource not found (HTTP 404). Corresponds to NotFoundError in JS.
exception NotFoundError of message: string

/// Resource conflict (HTTP 409). Corresponds to ConflictError in JS.
exception ConflictError of message: string

/// Bad request (HTTP 400). Corresponds to BadRequestError in JS.
exception BadRequestError of message: string

/// Request or operation timeout (HTTP 408). Corresponds to TimeoutError in JS.
exception TimeoutError of message: string

/// Internal server error (HTTP 500/502/503). Corresponds to InternalError in JS.
exception InternalError of message: string

/// Network or connection failure. Corresponds to ConnectionError in JS.
exception ConnectionError of message: string
