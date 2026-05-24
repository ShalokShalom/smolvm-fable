
import { isEqualTo, assertThat } from "./fable_modules/Scriptorium.Nib.0.2.1/Assertion.fs.js";

/**
 * Snapshot a value against the test-context name (path-derived key).
 */
export function snap(t, value) {
    throw 1;
}

/**
 * Snapshot with a custom serialiser.
 */
export function snapWith(t, serialize, value) {
    throw 1;
}

/**
 * Assert structural equality and emit a diff on failure.
 */
export function shouldEqual(expected, actual) {
    assertThat(actual, isEqualTo(expected));
}

