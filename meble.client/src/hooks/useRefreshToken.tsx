import { useState, useCallback } from 'react';
import { useLocalStorage } from './useLocalStorage';

function useRefreshToken() {
    const [isRefreshing, setIsRefreshing] = useState(false);
    const [error, setError] = useState<Error | null>(null);
    const [refreshToken, setRefreshToken] = useLocalStorage<string | null>('refreshToken', null);
    const [accessToken, setAccessToken] = useLocalStorage<string | null>('accessToken', null);

    const refreshAccessToken = useCallback(async (): Promise<string | null> => {
        setIsRefreshing(true);

        if (!refreshToken) {
            localStorage.removeItem("accessToken");
            localStorage.removeItem("refreshToken");
            setIsRefreshing(false);
            return null;
        }

        try {
            const response = await fetch('https://mebloartbackend.azurewebsites.net/refresh', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ refreshToken })
            });

            if (response.ok) {
                const data = await response.json();
                const newAccessToken = data.accessToken;
                const newRefreshToken = data.refreshToken;

                setAccessToken(newAccessToken);
                setRefreshToken(newRefreshToken);

                return newAccessToken;
            } else {
                localStorage.removeItem("accessToken");
                localStorage.removeItem("refreshToken");
                const data = await response.text();
                throw new Error(`B��d podczas od�wie�ania tokena. Status: ${response.status}, Tekst: ${data || 'Nie mo�na od�wie�y� tokena.'}`);
            }
        } catch (err) {
            const errorMsg = (err instanceof Error) ? err.message : 'Nieznany b��d';
            setError(new Error(errorMsg));
            localStorage.removeItem("accessToken");
            localStorage.removeItem("refreshToken");
            return null;
        } finally {
            setIsRefreshing(false);
        }
    }, [refreshToken, setAccessToken, setRefreshToken]);

    return { refreshAccessToken, isRefreshing, error, accessToken, setAccessToken };
}

export default useRefreshToken;
