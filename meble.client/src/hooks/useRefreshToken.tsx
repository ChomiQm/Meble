import { useState, useCallback } from 'react';
import { useLocalStorage } from './useLocalStorage';

function useRefreshToken() {
    const [isRefreshing, setIsRefreshing] = useState(false);
    const [error, setError] = useState<Error | null>(null);
    const [refreshToken, setRefreshToken] = useLocalStorage<string | null>('refreshToken', null);
    const [, setAccessToken] = useLocalStorage<string | null>('accessToken', null);

    const refreshAccessToken = useCallback(async (): Promise<string | null> => {
        setIsRefreshing(true);

        try {
            const response = await fetch('https://localhost:7197/refresh', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ refreshToken: refreshToken })
            });

            if (!refreshToken) {
                setIsRefreshing(false);
                return null;
            }

            if (response.ok) {
                const data = await response.json();
                const newAccessToken = data?.accessToken;
                const newRefreshToken = data?.refreshToken;

                // Aktualizacja tokenów przy u¿yciu hooka useLocalStorage
                setAccessToken(newAccessToken);
                setRefreshToken(newRefreshToken);

                setIsRefreshing(false);
                return newAccessToken;
            } else {
                const data = await response.text();
                throw new Error(`B³¹d podczas odœwie¿ania tokena. Status: ${response.status}, Tekst: ${data || 'Nie mo¿na odœwie¿yæ tokena.'}`);
            }
        } catch (err) {
            const errorMsg = (err instanceof Error) ? err.message : 'Nieznany b³¹d';
            setError(new Error(errorMsg));
            setIsRefreshing(false);
            return null;
        }
        finally {
            setIsRefreshing(false);
        }
    }, [refreshToken, setAccessToken, setRefreshToken]);

    return { refreshAccessToken, isRefreshing, error };
}

export default useRefreshToken;
