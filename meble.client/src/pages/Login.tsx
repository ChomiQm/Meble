import React, { useState } from 'react';
import '../layouts/loginLayout.css';
import LoginButton from '../components/button/LoginButton';
import LoginInput from '../components/input/LoginInput';
import { useAuth } from '../services/AuthContext';
import { Link } from 'react-router-dom';

const Login: React.FC = () => {
    const [email, setEmail] = useState<string>('');
    const [password, setPassword] = useState<string>('');
    const [loading, setLoading] = useState<boolean>(false);
    const [error, setError] = useState<string | null>(null);
    const { login } = useAuth();

    const passwordPattern = "^(?=.*[A-Z])(?=.*[a-z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$";

    const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        setLoading(true);
        setError(null);

        try {
            const response = await fetch('https://localhost:7197/login', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    Email: email,
                    Password: password
                })
            });

            const data = await response.json();

            if (response.ok) {
                login(data.accessToken, data.refreshToken);
            } else {
                console.error("Login Response Status:", response.status, response.statusText);
                setError('Login failed. Please check your credentials.');
            }
        } catch (error) {
            console.error("Error during fetch:", error);
            setError("An unexpected error occurred.");
        }

        setLoading(false);
    };

    return (
        <div className='flex px-4 justify-center'>
            <div className='flex md:flex-row flex-col'>
                <div className="flex justify-center items-center flex-col p-10 md:p-20 bg-white rounded">
                    <h1 className="text-2xl font-bold font-size pb-10">Meble</h1>
                    <h2 className='py-4'>Zaloguj się.</h2>
                    {error && <p className="error">{error}</p>}
                    <form className='flex flex-col justify-center flex-1 mt-4' onSubmit={handleSubmit}>
                        <LoginInput
                            type="email"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            placeholder="Email"
                            maxLength={40}
                        />
                        <LoginInput
                            type="password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            placeholder="Hasło"
                            maxLength={24}
                            pattern={passwordPattern}
                        />
                        <LoginButton isLoading={loading}>Zaloguj się</LoginButton>
                    </form>
                </div>

                <div className='text-lg rounded background flex p-20 md:p-10 text-center flex-col justify-center items-center gap-10 max-w-full md:max-w-[500px] text-white'>
                    <h1>MebloArt - działalność rodzinna!</h1>
                    <p>Jeśli jeszcze nie masz konta, <Link to="/register" className="text-blue-200 hover:text-blue-100">zarejestruj się <span className="underline">teraz</span></Link>.</p>
                    <div className="text-sm mt-6">
                        <h2 className="text-center font-bold">Przypomnienie o wymaganiach hasła:</h2>
                        <ul className="list-disc list-inside text-left">
                            <li>Co najmniej 8 znaków</li>
                            <li>Przynajmniej jedna duża litera (A-Z)</li>
                            <li>Przynajmniej jedna mała litera (a-z)</li>
                            <li>Przynajmniej jedna cyfra (0-9)</li>
                            <li>Przynajmniej jeden znak specjalny (np. !, @, #)</li>
                        </ul>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default Login;
