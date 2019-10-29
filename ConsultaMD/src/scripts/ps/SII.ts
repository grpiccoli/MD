var exec = require('child_process').exec

module.exports = function (callback: any, rut: string) {
    exec('./SII.rb '+rut, function (error: string, stdout: string, stderror: string) {
        callback(error, stdout);
    }).catch(function (err: string) {
        callback(err, rut)
    });
}