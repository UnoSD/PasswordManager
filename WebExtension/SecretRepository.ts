class SecretRepository {
    private static emptyContentMd5 = CryptoJS.enc.Base64.stringify(CryptoJS.MD5(""));

    static async getSecret(baseUrl: string) {
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
            const time = Date.now() / 1000 | 0;

            const dataToHash = `${appId}${httpMethod}${path}${time}${nonce}${SecretRepository.emptyContentMd5}`;

            const signature = CryptoJS.enc.Base64.stringify(CryptoJS.HmacSHA256(dataToHash, secretKey));
            const url = `${protocol}://${host}:${port}${path}`;
            const authorization = `Hmac ${appId}:${signature}:${nonce}:${time}`;

            return await ajaxAsync(httpMethod, url, authorization) as ISecret;
        } catch (e) {
            console.error(e);
        }

        return { username: "Error", password: "" };
    }
}