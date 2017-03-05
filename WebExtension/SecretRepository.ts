class SecretRepository {
    private static emptyContentMd5 = CryptoJS.enc.Base64.stringify(CryptoJS.MD5(""));

    static async getSecret(baseUrl: string) {
        try {
            const method = "GET";
            const path = `/secrets/${encodeURIComponent(baseUrl)}`;

            return await SecretRepository.sendRequestWithoutContent(method, path) as ISecret;
        } catch (e) {
            console.error(e);
        }

        return { username: "Error", password: "" };
    }

    static async addSecret(secret: ISecret, baseUrl: string) {
        try {
            // Change to PUT and amend existing instead of adding.
            const method = "POST";
            const path = `/secrets/${encodeURIComponent(baseUrl)}`;
            const object = {
                Secrets: {}
            };

            object.Secrets[secret.username] = secret.password;

            const content = JSON.stringify(object);

            // Encrypt content as well.
            await SecretRepository.sendRequest(method, path, content);
        } catch (e) {
            console.error(e);
        }
    }

    private static async sendRequestWithoutContent(method: string, path: string) {
        return await SecretRepository.sendRequest(method, path, "");
    }

    private static async sendRequest(method: string, path: string, content: string) {
        try {
            let contentMd5: string;

            if (content) {
                contentMd5 = CryptoJS.enc.Base64.stringify(CryptoJS.MD5(content));
            } else {
                contentMd5 = SecretRepository.emptyContentMd5;
            }

            const url = SecretRepository.getFullUrl(path);
            const authorization = SecretRepository.getHmacAuthorizationHeaderValue(path, method, contentMd5);

            return await ajaxAsync(method, url, authorization, content);
        } catch (e) {
            console.error(e);
        }

        return null;
    }

    private static getFullUrl(path: string) {
        const protocol = "http";
        const host = "localhost";
        const port = 5000;

        return `${protocol}://${host}:${port}${path}`;
    }

    private static getHmacAuthorizationHeaderValue(path: string, httpMethod: string, contentMd5: string) {
        const appId = "92C9C40B-BB2F-4813-8FA8-39628AD7EA4E";
        const secretKey = "CB6C5713-E213-4524-B93C-17020F0B5B4B";
        const nonce = Math.random().toString(36).substring(2, 15);
        const time = Date.now() / 1000 | 0;

        const dataToHash = `${appId}${httpMethod}${path}${time}${nonce}${contentMd5}`;

        const signature = CryptoJS.enc.Base64.stringify(CryptoJS.HmacSHA256(dataToHash, secretKey));

        return `Hmac ${appId}:${signature}:${nonce}:${time}`;
    }
}