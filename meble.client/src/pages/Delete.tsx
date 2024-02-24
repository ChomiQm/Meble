import { useState, useContext, FormEvent } from 'react';
import { useNavigate } from 'react-router-dom';
import { AuthContext } from '../services/AuthContext';
import fetchWithAuth from '../components/utils/fetchWithAuth';

const Delete = () => {
    const navigate = useNavigate();
    const { accessToken, refreshAccessToken, logout } = useContext(AuthContext);
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');

    const handleDelete = async (e: FormEvent<HTMLFormElement>) => {
        e.preventDefault();

        try {
            const response = await fetchWithAuth('https://localhost:7197/account/validatePassword', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${accessToken}`,
                },
                body: JSON.stringify({ password }),
            }, refreshAccessToken, logout);

            if (response.ok) {
                const deleteResponse = await fetchWithAuth('https://localhost:7197/account/delete', {
                    method: 'DELETE',
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': `Bearer ${accessToken}`,
                    },
                }, refreshAccessToken, logout);

                if (deleteResponse.ok) {
                    logout();
                    navigate('/');
                } else {
                    setError('Nie udało się usunąć konta użytkownika.');
                }
            } else {
                setError('Podano nieprawidłowe hasło.');
            }
        } catch (error) {
            setError('Wystąpił błąd podczas usuwania konta użytkownika.');
            logout();
        }
    };

    return (
        <div className='h-screen'>
            <div className='flex justify-center flex-col gap-4 items-center'>
                {error && <p className="error">{error}</p>}
                <form className='flex min-w-96 min-h-64 justify-center flex-col gap-2 items-center white-bg' onSubmit={handleDelete}>
                <h1 className='text-black'>Usuwanie Konta Użytkownika</h1>
                    <input
                        className='rounded py-1 px-4 border border-gray-500'
                        type="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        placeholder="Wpisz swoje hasło"
                    />
                    <button className='transition-transform duration-500 hover:scale-105 px-3 my-2 p-1 flex justify-center items-center rounded-xl text-white brown' type="submit">Usuń konto</button>
                </form>
            </div>
        </div>
    );
};

export default Delete;
