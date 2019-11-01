var util = require('util');
var path = require('path');
var exec = util.promisify(require('child_process').exec);
module.exports = function (callback, rut) {
    var rb = path.resolve('src/scripts/node/ps/SII.rb');
    exec(rb + " " + rut).then(function (out) {
        callback(null, out.stdout);
    }).catch(function (error) {
        callback(error, null);
    });
};
//# sourceMappingURL=SII.js.map