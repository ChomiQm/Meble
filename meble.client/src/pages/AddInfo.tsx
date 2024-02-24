import React, { useContext, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import AuthContext from '../services/AuthContext';
import fetchWithAuth from '../components/utils/fetchWithAuth'

const AddInfo: React.FC = () => {
    const [userData, setUserData] = useState({
        UserFirstName: '',
        UserSurname: '',
        UserCountry: '',
        UserTown: '',
        UserStreet: '',
        UserHomeNumber: '',
        UserFlatNumber: ''
    });
    const [loading, setLoading] = useState<boolean>(false);
    const [error, setError] = useState<string | null>(null);
    const { accessToken, refreshAccessToken, logout } = useContext(AuthContext);
    const navigate = useNavigate();

      const handleHomeNumberChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setUserData(prev => ({
            ...prev,
            UserHomeNumber: e.target.value,
            UserFlatNumber: ''
        }));
    };

    const handleFlatNumberChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setUserData(prev => ({
            ...prev,
            UserFlatNumber: e.target.value,
            UserHomeNumber: '' // Reset the UserHomeNumber if UserFlatNumber is changed
        }));
    };


    const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        setLoading(true);
        setError(null);

        // Check if both UserHomeNumber and UserFlatNumber are provided
        if (userData.UserHomeNumber && userData.UserFlatNumber) {
            setError('Wybierz numer domu lub numer mieszkania');
            setLoading(false);
            return;
        }

        try {
            const response = await fetchWithAuth(
                'https://localhost:7197/info/addInfo',
                {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': `Bearer ${accessToken}` // użyj accessToken z AuthContext
                    },
                    body: JSON.stringify(userData)
                },
                refreshAccessToken, // funkcja odświeżająca token
                logout // funkcja wylogowująca użytkownika
            );

            setLoading(false);
            if (response.ok) {
                navigate('/'); 
            } else {
                setError('Wystąpił błąd w wprowadzaniu danych. Proszę spróbować ponownie.');
            }
        } catch (error) {
            setLoading(false);
            setError('Wystąpił błąd. Proszę spróbować ponownie.');
        }
    };


    return (
        <div className="flex items-center justify-center min-h-screen">
            <div className="w-full max-w-md px-8 py-6 bg-white bg-opacity-60 backdrop-filter backdrop-blur-lg border border-gray-200 rounded-lg shadow-lg">
                <h2 className="text-2xl font-bold text-center text-gray-800">Podaj informacje osobiste</h2>
                {error && <p className="text-red-500 text-center mt-4">{error}</p>}
                <form onSubmit={handleSubmit} className="mt-8">
                    <div className="grid grid-cols-1 gap-6">
                        <div className="flex flex-col">
                            <input
                                type="text"
                                value={userData.UserFirstName}
                                onChange={(e) => setUserData(prev => ({ ...prev, UserFirstName: e.target.value }))}
                                placeholder="Imię (z dużej litery)"
                                className="w-full px-4 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                            />
                            <span className="text-xs text-gray-500 mt-1">Wprowadź imię z dużej litery</span>
                        </div>
                        <div className="flex flex-col">
                            <input
                                type="text"
                                value={userData.UserSurname}
                                onChange={(e) => setUserData(prev => ({ ...prev, UserSurname: e.target.value }))}
                                placeholder="Nazwisko (z dużej litery)"
                                className="w-full px-4 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                            />
                            <span className="text-xs text-gray-500 mt-1">Wprowadź nazwisko z dużej litery</span>
                        </div>
                        <div className="flex flex-col">
                            <input
                                type="text"
                                value={userData.UserCountry}
                                onChange={(e) => setUserData(prev => ({ ...prev, UserCountry: e.target.value }))}
                                placeholder="Kraj (z dużej litery)"
                                className="w-full px-4 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                            />
                            <span className="text-xs text-gray-500 mt-1">Wprowadź kraj z dużej litery</span>
                        </div>
                        <div className="flex flex-col">
                            <input
                                type="text"
                                value={userData.UserTown}
                                onChange={(e) => setUserData(prev => ({ ...prev, UserTown: e.target.value }))}
                                placeholder="Miasto (z dużej litery)"
                                className="w-full px-4 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                            />
                            <span className="text-xs text-gray-500 mt-1">Wprowadź miasto z dużej litery</span>
                        </div>
                        <div className="flex flex-col">
                            <input
                                type="text"
                                value={userData.UserStreet}
                                onChange={(e) => setUserData(prev => ({ ...prev, UserStreet: e.target.value }))}
                                placeholder="Ulica (z dużej litery)"
                                className="w-full px-4 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                            />
                            <span className="text-xs text-gray-500 mt-1">Wprowadź ulicę z dużej litery</span>
                        </div>
                        <div className="flex flex-col">
                            <input
                                type="text"
                                value={userData.UserHomeNumber}
                                onChange={handleHomeNumberChange}
                                placeholder="Numer domu (Nie używaj znaków)"
                                className="w-full px-4 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                disabled={!!userData.UserFlatNumber}
                            />
                            <span className="text-xs text-gray-500 mt-1">Wprowadź numer domu</span>
                        </div>
                        <div className="flex flex-col">
                            <input
                                type="text"
                                value={userData.UserFlatNumber}
                                onChange={handleFlatNumberChange}
                                placeholder="Numer mieszkania (Numer bloku + numer mieszkania)"
                                className="w-full px-4 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                disabled={!!userData.UserHomeNumber}
                            />
                            <span className="text-xs text-gray-500 mt-1">Wprowadź numer mieszkania</span>
                        </div>
                        <button
                            type="submit"
                            className={`w-full px-4 py-2 text-white bg-blue-600 rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-opacity-50 ${loading ? 'opacity-50 cursor-not-allowed' : ''}`}
                            disabled={loading}
                        >
                            {loading ? 'Dodawanie informacji...' : 'Potwierdź'}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );

}

export default AddInfo;