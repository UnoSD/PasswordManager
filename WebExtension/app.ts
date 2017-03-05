var trap = getGlobalKeyboardShortcuts();
var username: string;

trap.bind("ctrl+shift+u", (e) => setElementValueFromAsync(async () => {
    return (await getBaseUrlSecretAsync()).username;
}, e.target));

trap.bind("ctrl+shift+y", (e) => username = getTextValue(e.target));

trap.bind("ctrl+shift+s", (e) => addBaseUrlSecretAsync(e.target));

trap.bind("ctrl+shift+p", (e) => setElementValueFromAsync(async () => {
    return (await getBaseUrlSecretAsync()).password;
}, e.target));

function getGlobalKeyboardShortcuts() {
    const trap = new Mousetrap(window.frameElement);

    trap.stopCallback = () => false;

    return trap;
}

function getTextValue(element: any) {
    if (!($(element).is("input") || $(element).is("textarea"))) {
        alert("You must be in the password box to insert the password.");
        return "";
    }

    return element.value;
}

async function setElementValueFromAsync(getValue: () => Promise<string>, element: any) {
    if (!($(element).is("input") || $(element).is("textarea"))) {
        alert("You must be in the password box to insert the password.");
        return;
    }

    element.value = await getValue();
}

async function addBaseUrlSecretAsync(target) {
    const password = getTextValue(target);

    await SecretRepository.addSecret({
        username: username,
        password: password
    }, getBaseUrl());

    alert("Secret added.");
};

async function getBaseUrlSecretAsync() {
    return await SecretRepository.getSecret(getBaseUrl());
}

function getBaseUrl() {
    return `${window.location.protocol}//${window.location.hostname}`;
}