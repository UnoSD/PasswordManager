var trap = getGlobalKeyboardShortcuts();

trap.bind("ctrl+shift+u", (e) => setElementValueFromAsync(getUserName, e.target));

trap.bind("ctrl+shift+p", (e) => setElementValueFromAsync(getPassword, e.target));

function getGlobalKeyboardShortcuts() {
    const trap = new Mousetrap(window.frameElement);

    trap.stopCallback = () => false;

    return trap;
}

async function setElementValueFromAsync(getValue: () => Promise<string>, element: any) {
    if (!($(element).is("input") || $(element).is("textarea"))) {
        alert("You must be in the password box to insert the password.");
        return;
    }

    element.value = await getValue();
}

function getBaseUrl() {
    return `${window.location.protocol}//${window.location.hostname}`;
}

async function getPassword() {
    const secret = await getSecret(getBaseUrl());

    return secret.password;
};

async function getUserName() {
    const secret = await getSecret(getBaseUrl());

    return secret.username;
};

interface ISecret { username: string; password: string; };

async function getSecret(baseUrl: string) {
    try {
        const protocol = "http";
        const host = "localhost";
        const port = 5000;
        const source = encodeURIComponent(baseUrl);
        const path = `/secrets/${source}`;
        const appId = "92C9C40B-BB2F-4813-8FA8-39628AD7EA4E";
        const secretKey = "CB6C5713-E213-4524-B93C-17020F0B5B4B";
        const httpMethod = "GET";
        const nonce = Math.random().toString(36).substring(2, 15);
        const content = "";
        const time = Date.now() / 1000 | 0;

        const dataToHash = `${appId}${httpMethod}${path}${time}${nonce}${content}`;

        const signature = CryptoJS.enc.Base64.stringify(CryptoJS.HmacSHA256(dataToHash, secretKey));
        const url = `${protocol}://${host}:${port}${path}`;
        const authorization = `Hmac ${appId}:${signature}:${nonce}:${time}`;

        return await ajaxAsync(httpMethod, url, authorization) as ISecret;
    } catch (e) {
        console.error(e);
    }

    return { username: "Error", password: "" };
}

async function ajaxAsync(httpMethod: string, url: string, authorization: string) {
    return await new Promise((success, error) => 
        $.ajax({
            type: httpMethod,
            url: url,
            crossDomain: true,
            headers: {
                "Authorization": authorization
            },
            success,
            error
        })
    );
}