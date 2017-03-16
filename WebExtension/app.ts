var trap = getGlobalKeyboardShortcuts();
var username: string;

trap.bind("alt+u", (e) => setElementValueFromAsync(async () => {
    return (await getBaseUrlSecretAsync()).username;
}, e.target));

trap.bind("alt+p", (e) => setElementValueFromAsync(async () => {
    return (await getBaseUrlSecretAsync()).password;
}, e.target));

trap.bind("ctrl+shift+u", (e) => {
    username = getTextValue(e.target);
    notify("Username copied.");
});

trap.bind("ctrl+alt+s", (e) => addBaseUrlSecretAsync(e.target));

function getGlobalKeyboardShortcuts() {
    const trap = new Mousetrap(window.frameElement);

    trap.stopCallback = () => false;

    return trap;
}

function getTextValue(element: any) {
    if (!($(element).is("input") || $(element).is("textarea"))) {
        notify("You must be in the password box to insert the password.");
        return "";
    }

    return element.value;
}

async function setElementValueFromAsync(getValue: () => Promise<string>, element: any) {
    if (!($(element).is("input") || $(element).is("textarea"))) {
        notify("You must be in the password box to insert the password.");
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

    notify("Secret added.");
};

async function getBaseUrlSecretAsync() {
    return await SecretRepository.getSecret(getBaseUrl());
}

function getBaseUrl() {
    return `${window.location.protocol}//${window.location.hostname}`;
}

function notify(message: string) {
    Push.create("Password manager", {
        body: message,
        timeout: 4000,
        onClick: function () {
            window.focus();
            this.close();
        }
    });
}