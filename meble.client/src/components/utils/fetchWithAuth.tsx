async function fetchWithAuth(url: string, options: any, refreshAccessTokenFunc: () => Promise<string | null>, logoutFunc: () => void) {
    const accessToken = localStorage.getItem("accessToken");
    if (!options.headers) {
        options.headers = {};
    }
    if (accessToken) {
        options.headers["Authorization"] = `Bearer ${JSON.parse(accessToken)}`;
    }

    const originalResponse = await fetch(url, options);

    if (originalResponse.status === 401) {
        localStorage.removeItem("accessToken");
        localStorage.removeItem("refreshToken");

        const newToken = await refreshAccessTokenFunc();

        if (newToken) {
            localStorage.setItem("accessToken", JSON.stringify(newToken));
            options.headers["Authorization"] = `Bearer ${newToken}`;
            const retriedResponse = await fetch(url, options);

            if (retriedResponse.status !== 401) {
                return retriedResponse;
            }
        }

        logoutFunc();
        throw new Error("Sesja wygas³a. Zaloguj siê ponownie.");
    }

    return originalResponse;
}

export default fetchWithAuth;
