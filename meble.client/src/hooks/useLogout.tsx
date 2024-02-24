import { useNavigate } from 'react-router-dom';
import { useAuth } from '../services/AuthContext';
import fetchWithAuth from '../components/utils/fetchWithAuth';

const useLogout = () => {
    const { setLoginStatus, accessToken, refreshAccessToken, logout } = useAuth();
    const navigate = useNavigate();

    const handleLogout = async () => {
        try {
            const response = await fetchWithAuth('https://localhost:7197/account/logout', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': accessToken ? `Bearer ${accessToken}` : '', 
                },
            },
                refreshAccessToken,
                logout
            );

            if (!response.ok) {
                throw new Error(`B³¹d podczas wylogowywania. Status: ${response.status}`);
            }

            window.localStorage.removeItem('accessToken');
            window.localStorage.removeItem('refreshToken');
            setLoginStatus(false);
            navigate('/');
        } catch (error) {
            console.error('B³¹d podczas wylogowywania:', error);
        }
    };

    return handleLogout;
};

export default useLogout;
