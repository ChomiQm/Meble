import { useState, useEffect, useCallback } from 'react';

export const useLocalStorage = <T,>(key: string, initialValue: T, refreshFunc?: () => Promise<T | null>) => {
    const [storedValue, setStoredValue] = useState<T>(() => {
        try {
            const item = window.localStorage.getItem(key);
            return item ? JSON.parse(item) : initialValue;
        } catch (error) {
            console.error(error);
            return initialValue;
        }
    });

    const setValue = useCallback((value: T) => {
        try {
            setStoredValue(value);
            window.localStorage.setItem(key, JSON.stringify(value));
        } catch (error) {
            console.error(error);
        }
    }, [key]);

    useEffect(() => {
        if (refreshFunc) {
            const interval = setInterval(() => {
                refreshFunc().then(newToken => {
                    if (newToken) {
                        setValue(newToken);
                    }
                });
            }, 1000 * 60 * 15);

            return () => clearInterval(interval);
        }
    }, [refreshFunc, setValue]);

    return [storedValue, setValue] as const;
};

export default useLocalStorage;
