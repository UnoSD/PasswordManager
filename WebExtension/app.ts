var trap = getGlobalKeyboardShortcuts();

trap.bind("ctrl+shift+u", (e) => setTextValueFrom(getUserName, e.srcElement));

trap.bind("ctrl+shift+p", (e) => setTextValueFrom(getPassword, e.srcElement));

function getGlobalKeyboardShortcuts() {
    const trap = new Mousetrap(window.frameElement);

    trap.stopCallback = () => false;

    return trap;
}

function setTextValueFrom(getValue: () => string, element: any) {
    const activeElement: any = document.activeElement;

    if (!($(activeElement).is("input") || $(activeElement).is("textarea"))) {
        alert("You must be in the password box to insert the password.");
        return;
    }

    const userName = getValue();

    activeElement.value = userName;
}

function getPassword() {
    return "password";
};

function getUserName() {
    return "username";
};

function callServer () {
    const protocol = "http";
    const host = "localhost";
    const port = 5000;
    const path = "/secrets/http%3A%2F%2Fwww.g";
    const appId = "92C9C40B-BB2F-4813-8FA8-39628AD7EA4E";
    const secretKey = "CB6C5713-E213-4524-B93C-17020F0B5B4B";
    const httpMethod = "GET";
    const nonce = Math.random().toString(36).substring(2, 15);
    const content = "";
    const time = Date.now() / 1000 | 0;

    const dataToHash = `${appId}${httpMethod}${path}${time}${nonce}${content}`;

    const signature = CryptoJS.enc.Base64.stringify(CryptoJS.HmacSHA256(dataToHash, secretKey));

    console.log("Sending the request.");

    $.ajaxSetup({
        headers: { "CustomHeader": "myValue" }
    });

    $.ajax({
        type: httpMethod,
        url: `${protocol}://${host}:${port}${path}`,
        crossDomain: true,
        beforeSend(request) {
            request.setRequestHeader("Authorization", `Hmac ${appId}:${signature}:${nonce}:${time}`);
        },
        headers: {
            "Authorization": `Hmac ${appId}:${signature}:${nonce}:${time}`
        },
        success(msg) {
            console.log(msg);
        },
        error(request, status, error) {
            console.log(error);
        }
    });

    console.log("Request sent.");

    return "";
}