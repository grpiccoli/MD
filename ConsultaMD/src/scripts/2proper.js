"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
function toTitleCase(str) {
    return str.replace(/(?:^|\s)\w/g, function (match) {
        return match.toUpperCase();
    });
}
exports.toTitleCase = toTitleCase;
//# sourceMappingURL=2proper.js.map