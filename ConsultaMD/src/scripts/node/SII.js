const util = require('util');
const path = require('path');
const exec = util.promisify(require('child_process').exec);
var rb = path.resolve('SII.rb');
exec(`${rb} ${rut}`).then((out: { stdout: string, stderr: string }) => {
    console.log(out.stdout);
}).catch((error: string) => {
    console.error(error);
});