
import { Runner_runTests_Z12A175 } from "./fable_modules/Scriptorium.Quill.0.2.1/Quill.fs.js";
import { tests } from "./MachineConfigTests.fs.js";
import { tests as tests_1 } from "./ExecResponseTests.fs.js";
import { tests as tests_2 } from "./ExecResultTests.fs.js";
import { tests as tests_3 } from "./ContainerConfigTests.fs.js";
import { tests as tests_4 } from "./ContainerOptionsTests.fs.js";
import { tests as tests_5 } from "./ResourceSpecTests.fs.js";
import { tests as tests_6 } from "./ResourceLimitsTests.fs.js";
import { ofArray } from "./fable_modules/fable-library-js.5.0.0/List.js";

(function (_arg) {
    return Runner_runTests_Z12A175(ofArray([tests, tests_1, tests_2, tests_3, tests_4, tests_5, tests_6])) | 0;
})(typeof process === 'object' ? process.argv.slice(2) : []);

