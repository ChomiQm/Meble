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
            setIsRefreshing(false);
            setAccessToken(null);
            setRefreshToken(null);
            return null;
        }

        try {
            const response = await fetch('https://localhost:7197/refresh', {
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
                setAccessToken(null);
                setRefreshToken(null);
                const data = await response.text();
                throw new Error(`B³¹d podczas odœwie¿ania tokena. Status: ${response.status}, Tekst: ${data || 'Nie mo¿na odœwie¿yæ tokena.'}`);
            }
        } catch (err) {
            const errorMsg = (err instanceof Error) ? err.message : 'Nieznany b³¹d';
            setError(new Error(errorMsg));
            setAccessToken(null);
            setRefreshToken(null);
            return null;
        } finally {
            setIsRefreshing(false);
        }
    }, [refreshToken, setAccessToken, setRefreshToken]);

    return { refreshAccessToken, isRefreshing, error, accessToken, setAccessToken };
}

export default useRefreshToken;
