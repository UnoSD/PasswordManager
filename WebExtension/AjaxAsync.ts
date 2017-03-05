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