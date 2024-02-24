import { useState, useContext, ChangeEvent, FormEvent } from 'react';
import { useNavigate } from 'react-router-dom';
import UserNavbar from '../components/navbar/UserNavbar';
import fetchWithAuth from '../components/utils/fetchWithAuth';
import { AuthContext } from '../services/AuthContext';

const EditUser = () => {
    const navigate = useNavigate();
    const { accessToken, refreshAccessToken, logout } = useContext(AuthContext);
    const [email, setEmail] = useState('');
    const [phoneNumber, setPhoneNumber] = useState('');
    const [currentPassword, setCurrentPassword] = useState('');
    const [newPassword, setNewPassword] = useState('');
    const [error, setError] = useState<string | null>(null);

    const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/;
    const phoneRegex = /^\+\d{2}\s\d{3}-\d{3}-\d{3}$/;

    const handleChange = (e: ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        switch (name) {
            case 'email':
                setEmail(value);
                break;
            case 'phoneNumber':
                setPhoneNumber(value);
                break;
            case 'currentPassword':
                setCurrentPassword(value);
                break;
            case 'newPassword':
                setNewPassword(value);
                break;
            default:
                break;
        }
    };

    const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
        e.preventDefault();

        try {
            const response = await fetchWithAuth('https://localhost:7197/account/manageUser', {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${accessToken}`,
                },
                body: JSON.stringify({
                    email,
                    phoneNumber,
                    currentPassword,
                    newPassword
                }),
            }, refreshAccessToken, logout);

            if (response.ok) {
                navigate('/manage');
            } else {
                setError('Błąd podczas aktualizacji danych użytkownika, wprowadzono czyiś mail, niezgodne hasło, lub niezgony numer telefonu');
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
                <form className='flex flex-col justify-center white-bg gap-5 items-center ' onSubmit={handleSubmit}>
            <h1 className=''>Edycja Danych Użytkownika</h1>
                <input
                    className='border rounded py-1 px-4'
                    type="email"
                    name="email"
                    value={email}
                    onChange={handleChange}
                    placeholder="Nowy email"
                    maxLength={40}
                />
                <input
                    className='border rounded py-1 px-4'
                    type="tel"
                    name="phoneNumber"
                    value={phoneNumber}
                    onChange={handleChange}
                    placeholder="Nowy numer telefonu"
                    maxLength={18}
                    pattern={phoneRegex.source} 
                />
                <input
                    className='border rounded py-1 px-4'
                    type="password"
                    name="currentPassword"
                    value={currentPassword}
                    onChange={handleChange}
                    placeholder="Aktualne hasło"
                    maxLength={24}
                    pattern={passwordRegex.source}
                />
                <input
                    className='border rounded py-1 px-4'
                    type="password"
                    name="newPassword"
                    value={newPassword}
                    onChange={handleChange}
                    placeholder="Nowe hasło"
                    maxLength={24}
                    pattern={passwordRegex.source}
                />
                <button className='transition-transform duration-500 hover:scale-105 px-3 my-2 p-1 flex justify-center items-center rounded-xl text-white brown' type="submit">Zapisz zmiany</button>
            </form>                    
            </div>
        </div>
    );
};

export default EditUser;
