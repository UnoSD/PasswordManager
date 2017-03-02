using System.Net;
using Microsoft.AspNetCore.Http;

namespace WebServer
{
    class HmacHasher
    {
        /*
Postman pre-request script:
        
var appId = '92C9C40B-BB2F-4813-8FA8-39628AD7EA4E';
var secretKey = 'CB6C5713-E213-4524-B93C-17020F0B5B4B';
var httpMethod = 'GET';
var encodedUri = 'http%3A%2F%2Flocalhost%3A5000%2Fsecrets%2Fhttp%3A%25252F%25252Fwww.g';
var nonce = 'nonce';
var content = '';
var time = new Date() / 1000 | 0;

var dataToHash = appId + httpMethod + encodedUri + time + nonce + content;

var enc = CryptoJS.enc.Base64.stringify(CryptoJS.HmacSHA256(dataToHash, secretKey));

postman.setGlobalVariable("hmac", 'Hmac ' + appId + ':' + enc + ':' + nonce + ':' + time);
         */

        public static string GetDataToHash(HttpRequest request, string appId, string nonce, string requestTimeStamp, string content)
        {
            var uri = request.GetAbsoluteUri();

            var encodedUri = WebUtility.UrlEncode(uri);

            return $"{appId}{request.Method}{encodedUri}{requestTimeStamp}{nonce}{content}";
        }
    }
}