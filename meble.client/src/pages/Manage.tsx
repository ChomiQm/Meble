import { useState, useContext, useEffect } from 'react';
import UserNavbar from '../components/navbar/UserNavbar';
import fetchWithAuth from '../components/utils/fetchWithAuth';
import { AuthContext } from '../services/AuthContext';

interface UserData {
    userFirstName: string;
    userSurname: string;
    userCountry: string;
    userTown: string;
    userStreet?: string;
    userHomeNumber?: number;
    userFlatNumber?: string;
}

interface User {
    phoneNumber: string;
    email: string;
}

const Manage = () => {
    const { accessToken, refreshAccessToken, logout } = useContext(AuthContext);
    const [userData, setUserData] = useState<UserData | null>(null);
    const [user, setUser] = useState<User | null>(null); // Tu było błędnie null zamiast {}
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const fetchUserData = async () => {
            try {
                const response = await fetchWithAuth('https://localhost:7197/info/getData', {
                    method: 'GET',
                    headers: {
                        'Authorization': `Bearer ${accessToken}`,
                    },
                }, refreshAccessToken, logout);

                if (response.ok) {
                    const data: UserData = await response.json();
                    setUserData(data);
                } else {
                    setError('Błąd podczas pobierania danych UserData');
                }
            } catch (error) {
                setError('Wystąpił błąd podczas ładowania danych UserData');
                logout();
            }
        };

        fetchUserData();
    }, [accessToken, refreshAccessToken, logout]);

    useEffect(() => {
        const fetchCurrentUserData = async () => {
            try {
                const response = await fetchWithAuth('https://localhost:7197/account/currentUser', {
                    method: 'GET',
                    headers: {
                        'Authorization': `Bearer ${accessToken}`,
                    },
                }, refreshAccessToken, logout);

                if (response.ok) {
                    const currentUserData: User = await response.json();
                    setUser(currentUserData);
                } else {
                    setError('Błąd podczas ładowania aktualnych danych User');
                }
            } catch (error) {
                setError('Wystąpił błąd podczas ładowania danych User');
                logout();
            }
        };

        fetchCurrentUserData();
    }, [accessToken, refreshAccessToken, logout]);

    return (
        <div className='flex flex-col justify-center items-center min-h-screen'>
            <div className='flex flex-col md:flex-row glass rounded-lg gap-10 p-8 max-w-4xl w-full mx-auto mt-0 md:mt-4'>
                <UserNavbar />

                {error && <div className="text-red-500">Błąd: {error}</div>}

                {userData && user ? (
                    <div className='flex flex-col justify-center items-center text-center gap-4 white-bg p-4 rounded-lg shadow-lg'>
                        <h1 className='text-2xl font-semibold mt-2 mb-4'>Zarządzanie Kontem Użytkownika</h1>
                        <p><strong>Email: </strong><br /> {user.email}</p>
                        <p><strong>Numer telefonu: </strong><br /> {user.phoneNumber}</p>
                        <p><strong>Imię: </strong> <br /> {userData.userFirstName}</p>
                        <p><strong>Nazwisko: </strong><br /> {userData.userSurname}</p>
                        <p><strong>Kraj: </strong><br /> {userData.userCountry}</p>
                        <p><strong>Miasto: </strong><br /> {userData.userTown}</p>
                        {userData.userStreet && <p><strong>Ulica: </strong> {userData.userStreet}</p>}
                        {userData.userHomeNumber && <p><strong>Numer domu: </strong> {userData.userHomeNumber}</p>}
                        {userData.userFlatNumber && <p><strong>Numer mieszkania: </strong> {userData.userFlatNumber}</p>}
                    </div>
                ) : (
                    <p>Ładowanie danych użytkownika...</p>
                )}
            </div>
        </div>
    );

};

export default Manage;
