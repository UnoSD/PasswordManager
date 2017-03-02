const cryptoJs = require("crypto-js");
const http = require("http");

const host = "localhost";
const port = 5000;
const path = "/secrets/http%3A%2F%2Fwww.g";

const appId = "92C9C40B-BB2F-4813-8FA8-39628AD7EA4E";
const secretKey = "CB6C5713-E213-4524-B93C-17020F0B5B4B";
const httpMethod = "GET";
const encodedUri = encodeURIComponent(`http://${host}:${port}${path}`);
const nonce = Math.random().toString(36).substring(2, 15);
const content = "";
const time = Date.now() / 1000 | 0;

const dataToHash = `${appId}${httpMethod}${encodedUri}${time}${nonce}${content}`;

const signature = cryptoJs.enc.Base64.stringify(cryptoJs.HmacSHA256(dataToHash, secretKey));

const options = {
    host: host,
    port: port,
    path: path,
    method: httpMethod,
    headers: {
        'Authorization': `Hmac ${appId}:${signature}:${nonce}:${time}`
    }
};

http.request(options, res => {
    console.log(`Status code: ${res.statusCode}`);
    console.log(`Headers    : ${JSON.stringify(res.headers)}`);

    res.setEncoding("utf8");

    res.on("data", chunk => { console.log(`Body       : ${chunk}`); });
}).end();