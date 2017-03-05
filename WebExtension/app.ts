var trap = getGlobalKeyboardShortcuts();

trap.bind("ctrl+shift+u", (e) => setElementValueFromAsync(async () => {
    return (await getBaseUrlSecretAsync()).username;
}, e.target));

trap.bind("ctrl+shift+p", (e) => setElementValueFromAsync(async () => {
    return (await getBaseUrlSecretAsync()).password;
}, e.target));

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

async function getBaseUrlSecretAsync() {
    const baseUrl = `${window.location.protocol}//${window.location.hostname}`;

    return await SecretRepository.getSecret(baseUrl);
}