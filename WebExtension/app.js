var trap = getGlobalKeyboardShortcuts();
trap.bind("ctrl+shift+u", function (e) { return setTextValueFrom(getUserName, e.srcElement); });
trap.bind("ctrl+shift+p", function (e) { return setTextValueFrom(getPassword, e.srcElement); });
function getGlobalKeyboardShortcuts() {
    var trap = new Mousetrap(window.frameElement);
    trap.stopCallback = function () { return false; };
    return trap;
}
function setTextValueFrom(getValue, element) {
    var activeElement = document.activeElement;
    if (!($(activeElement).is("input") || $(activeElement).is("textarea"))) {
        alert("You must be in the password box to insert the password.");
        return;
    }
    var userName = getValue();
    activeElement.value = userName;
}
function getPassword() {
    return "password";
}
;
function getUserName() {
    return "username";
}
;
function callServer() {
    var protocol = "http";
    var host = "localhost";
    var port = 5000;
    var path = "/secrets/http%3A%2F%2Fwww.g";
    var appId = "92C9C40B-BB2F-4813-8FA8-39628AD7EA4E";
    var secretKey = "CB6C5713-E213-4524-B93C-17020F0B5B4B";
    var httpMethod = "GET";
    var nonce = Math.random().toString(36).substring(2, 15);
    var content = "";
    var time = Date.now() / 1000 | 0;
    var dataToHash = "" + appId + httpMethod + path + time + nonce + content;
    var signature = CryptoJS.enc.Base64.stringify(CryptoJS.HmacSHA256(dataToHash, secretKey));
    console.log("Sending the request.");
    $.ajaxSetup({
        headers: { "CustomHeader": "myValue" }
    });
    $.ajax({
        type: httpMethod,
        url: protocol + "://" + host + ":" + port + path,
        crossDomain: true,
        beforeSend: function (request) {
            request.setRequestHeader("Authorization", "Hmac " + appId + ":" + signature + ":" + nonce + ":" + time);
        },
        headers: {
            "Authorization": "Hmac " + appId + ":" + signature + ":" + nonce + ":" + time
        },
        success: function (msg) {
            console.log(msg);
        },
        error: function (request, status, error) {
            console.log(error);
        }
    });
    console.log("Request sent.");
    return "";
}
//# sourceMappingURL=app.js.map