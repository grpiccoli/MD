var exec = require('child_process').exec;
module.exports = function (callback, rut) {
    exec('SII.rb ' + rut, function (error, stdout, stderror) {
        callback(error, stdout);
    });
};
//# sourceMappingURL=SII.js.map