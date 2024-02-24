import { useState, useContext, useEffect, ChangeEvent, FormEvent } from 'react';
import { useNavigate } from 'react-router-dom';
import UserNavbar from '../components/navbar/UserNavbar';
import fetchWithAuth from '../components/utils/fetchWithAuth';
import { AuthContext } from '../services/AuthContext';

interface UserData {
    userFirstName: string;
    userSurname: string;
    userCountry: string;
    userTown: string;
    userStreet?: string | null;
    userHomeNumber?: number | null;
    userFlatNumber?: string | null;
}

const Edit = () => {
    const navigate = useNavigate();
    const { accessToken, refreshAccessToken, logout } = useContext(AuthContext);
    const [userData, setUserData] = useState<UserData>({
        userFirstName: '',
        userSurname: '',
        userCountry: '',
        userTown: '',
        userStreet: null,
        userHomeNumber: null,
        userFlatNumber: null,
    });
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const loadUserData = async () => {
            try {
                const response = await fetchWithAuth('https://localhost:7197/info/getData', {
                    method: 'GET',
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': `Bearer ${accessToken}`,
                    },
                }, refreshAccessToken, logout);

                if (response.ok) {
                    const data: UserData = await response.json();
                    setUserData(data);
                } else {
                    setError('Błąd podczas ładowania danych użytkownika');
                }
            } catch (error) {
                setError('Wystąpił błąd podczas ładowania danych użytkownika');
                logout();
            }
        };

        loadUserData();
    }, [accessToken, refreshAccessToken, logout]);

    const handleChange = (e: ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        setUserData(prevState => ({
            ...prevState,
            [name]: value,
        }));
    };

    const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
        e.preventDefault();

        try {
            const response = await fetchWithAuth('https://localhost:7197/info/update', {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${accessToken}`,
                },
                body: JSON.stringify(userData),
            }, refreshAccessToken, logout);

            if (response.ok) {
                navigate('/manage');
            } else {
                setError('Błąd podczas aktualizacji danych użytkownika, wprowadzono zły input');
                logout();
            }
        } catch (error) {
            setError('Wystąpił błąd podczas aktualizacji danych użytkownika');
            logout();
        }
    };

    return (
        <div className='flex flex-col justify-center items-center min-h-screen'>
            <div className='flex flex-col md:flex-row glass rounded-lg gap-10 p-8 max-w-4xl w-full mx-auto mt-0 md:mt-4'>
                <UserNavbar />
                {error &&
                    <div className="bg-red-100 border-l-4 border-red-500 text-red-700 p-4 mt-5 rounded relative" role="alert">
                        <strong className="font-bold block mb-2">Błąd!</strong>
                        <span>{error}</span>
                    </div>
                }
                    <form className='flex justify-center w-full items-center flex-col glass2 rounded-lg gap-4 p-6 shadow-xl backdrop-filter backdrop-blur-md' onSubmit={handleSubmit}>
                    <label className='text-white' htmlFor="userFirstName">Imię - Poprawę zaczynaj od dużej litery</label>
                        <input
                            className='text-black border rounded w-full py-2 px-4'
                            id="userFirstName"
                            type="text"
                            name="userFirstName"
                            placeholder="Imię - Zaczynaj od dużej litery"
                            value={userData.userFirstName}
                            onChange={handleChange}
                        />
                    <label className='text-white' htmlFor="userSurname">Nazwisko - Poprawę zaczynaj od dużej litery</label>
                        <input
                            className='text-black border w-full rounded py-2 px-4'
                            id="userSurname"
                            type="text"
                            name="userSurname"
                            placeholder="Nazwisko - Zaczynaj od dużej litery"
                            value={userData.userSurname}
                            onChange={handleChange}
                        />
                    <label className='text-white' htmlFor="userCountry">Kraj - Poprawę zaczynaj od dużej litery</label>
                        <input
                            className='text-black border w-full rounded py-2 px-4'
                            id="userCountry"
                            type="text"
                            name="userCountry"
                            placeholder="Kraj -  od dużej litery"
                            value={userData.userCountry}
                            onChange={handleChange}
                        />
                    <label className='text-white' htmlFor="userTown">Miasto - Poprawę zaczynaj od dużej litery</label>
                        <input
                            className='text-black border w-full rounded py-2 px-4'
                            id="userTown"
                            type="text"
                            name="userTown"
                            placeholder="Miasto - Zaczynaj od dużej litery"
                            value={userData.userTown}
                            onChange={handleChange}
                        />
                    <label className='text-white' htmlFor="userStreet">Ulica - Poprawę zaczynaj od dużej litery</label>
                        <input
                            className='text-black border w-full rounded py-2 px-4'
                            id="userStreet"
                            type="text"
                            name="userStreet"
                            placeholder="Ulica - Zaczynaj od dużej litery"
                            value={userData.userStreet || ''}
                            onChange={handleChange}
                        />
                    <label className='text-white' htmlFor="userHomeNumber">Numer domu - Podaj tylko cyfry</label>
                        <input
                            className='text-black border w-full rounded py-2 px-4'
                            id="userHomeNumber"
                            type="text"
                            name="userHomeNumber"
                            placeholder="Numer domu - Bez znaków, same cyfry"
                            value={userData.userHomeNumber || ''}
                            onChange={handleChange}
                        />
                    <label className='text-white' htmlFor="userFlatNumber">Numer mieszkania - Podaj Numer bloku/Numer mieszkania</label>
                        <input
                            className='text-black border w-full rounded py-2 px-4'
                            id="userFlatNumber"
                            type="text"
                            name="userFlatNumber"
                            placeholder="Numer mieszkania - Numer Bloku/Numer mieszkania"
                            value={userData.userFlatNumber || ''}
                            onChange={handleChange}
                        />
                        <button className='mt-4 bg-transparent border-2 border-white text-white rounded-full px-4 py-2 hover:bg-white hover:text-gray-800 transition-colors duration-300' type="submit">Zapisz zmiany</button>
                </form>                
            </div>
        </div>
    );

};

export default Edit;
