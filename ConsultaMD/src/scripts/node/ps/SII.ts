const util = require('util');
const path = require('path')
const exec = util.promisify(require('child_process').exec);
module.exports = (callback: any, rut: string) => {
    var rb = path.resolve('src/scripts/node/ps/SII.rb');
    exec(`${rb} ${rut}`).then((out: { stdout: string, stderr: string }) => {
        callback(null, out.stdout);
    }).catch((error: string) => {
        callback(error, null);
    });
};