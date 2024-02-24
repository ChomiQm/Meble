import React, { createContext, ReactNode, useState, useCallback, useContext, useMemo, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useLocalStorage } from '../hooks/useLocalStorage';
import useRefreshToken from '../hooks/useRefreshToken';
import fetchWithAuth from '../components/utils/fetchWithAuth';

export interface AuthContextProps {
    isLoggedIn: boolean;
    hasData: boolean;
    roles: string[];
    setHasData: React.Dispatch<React.SetStateAction<boolean>>;
    accessToken: string | null;
    refreshToken: string | null;
    setLoginStatus: React.Dispatch<React.SetStateAction<boolean>>;
    login: (accessToken: string, refreshToken: string) => void;
    logout: () => void;
    checkUserData: () => Promise<void>;
    refreshAccessToken: () => Promise<string | null>;
}

export const AuthContext = createContext<AuthContextProps>({
    isLoggedIn: false,
    hasData: false,
    roles: [],
    setHasData: () => { },
    accessToken: null,
    refreshToken: null,
    setLoginStatus: () => { },
    login: () => { },
    logout: () => { },
    checkUserData: async () => { },
    refreshAccessToken: async () => null,
});

export const AuthProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
    const navigate = useNavigate();
    const [isLoggedIn, setLoginStatus] = useState<boolean>(!!localStorage.getItem('accessToken'));
    const [hasData, setHasData] = useState<boolean>(false);
    const [roles, setRoles] = useState<string[]>([]);
    const [accessToken, setAccessToken] = useLocalStorage<string | null>('accessToken', null);
    const [refreshToken, setRefreshToken] = useLocalStorage<string | null>('refreshToken', null);
    const { refreshAccessToken } = useRefreshToken();

    const login = useCallback((newAccessToken: string, newRefreshToken: string) => {
        setAccessToken(newAccessToken);
        setRefreshToken(newRefreshToken);
        setLoginStatus(true);
        navigate('/');
    }, [setAccessToken, setRefreshToken, navigate]);

    const logout = useCallback(() => {
        setAccessToken(null);
        setRefreshToken(null);
        setLoginStatus(false);
        setHasData(false);
        setRoles([]);
        navigate('/');
    }, [setAccessToken, setRefreshToken, navigate]);

    const checkUserData = useCallback(async () => {
        if (!accessToken) {
            console.error("Access token not found");
            logout();
            return;
        }

        try {
            const userDataResponse = await fetchWithAuth(
                'https://localhost:7197/info/hasUserData',
                {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': `Bearer ${accessToken}`,
                    },
                },
                refreshAccessToken,
                logout
            );

            if (!userDataResponse.ok) {
                throw new Error(`Failed to verify user data: ${userDataResponse.status}`);
            }

            const hasUserData = await userDataResponse.json();
            setHasData(hasUserData);

            const userInfoResponse = await fetchWithAuth(
                'https://localhost:7197/info/me',
                {
                    method: 'GET',
                    headers: {
                        'Authorization': `Bearer ${accessToken}`,
                    },
                },
                refreshAccessToken,
                logout
            );

            if (!userInfoResponse.ok) {
                throw new Error(`Failed to get user info: ${userInfoResponse.status}`);
            }

            const userInfo = await userInfoResponse.json();
            setRoles(userInfo.roles || []);

            if (!userInfo.roles.includes('Admin') && !hasUserData) {
                navigate('/addInfo');
            } 

        } catch (error) {
            console.error("There was an error:", error);
            logout();
        }
    }, [accessToken, logout, refreshAccessToken, navigate, setHasData, setRoles]);

    useEffect(() => {
        if (isLoggedIn) {
            checkUserData();
        }
    }, [isLoggedIn, checkUserData]);

    const contextValue = useMemo(() => ({
        isLoggedIn,
        hasData,
        roles,
        accessToken,
        refreshToken,
        setLoginStatus,
        setHasData,
        login,
        logout,
        checkUserData,
        refreshAccessToken,
    }), [
        isLoggedIn,
        hasData,
        roles,
        accessToken,
        refreshToken,
        setLoginStatus,
        setHasData,
        login,
        logout,
        checkUserData,
        refreshAccessToken,
    ]);

    return (
        <AuthContext.Provider value={contextValue}>
            {children}
        </AuthContext.Provider>
    );
};

export const useAuth = () => useContext(AuthContext);

export default AuthContext;
