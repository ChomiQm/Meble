import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import '../layouts/registerLayout.css';
import RegisterInput from '../components/input/RegisterInput';
import RegisterButton from '../components/button/RegisterButton';

const Register: React.FC = () => {
    const [email, setEmail] = useState<string>('');
    const [password, setPassword] = useState<string>('');
    const [confirmPassword, setConfirmPassword] = useState<string>('');
    const [loading, setLoading] = useState<boolean>(false);
    const [error, setError] = useState<string | null>(null);

    const navigate = useNavigate();

    const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/;

    const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        setLoading(true);
        setError(null);

        if (!passwordRegex.test(password)) {
            setError('Password must contain at least 8 characters, one uppercase, one lowercase, one number, and one special character.');
            setLoading(false);
            return;
        }

        if (password !== confirmPassword) {
            setError('Passwords do not match.');
            setPassword('');
            setConfirmPassword('');
            setLoading(false);
            return;
        }

        await fetch('https://localhost:7197/register', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                Email: email,
                Password: password
            })
        }).then(async (response) => {
            setLoading(false);
            if (response.ok) {
                navigate('/login');
            } else {
                setError('Registration failed. Please check your input.');
            }
        }).catch(() => {
            setLoading(false);
            setError('An error occurred. Please try again.');
        });
    };

    return (
        <div className='flex px-4 justify-center mb-10'>
            <div className='flex md:flex-row flex-col'>
                <div className="flex justify-center items-center flex-col p-20 bg-white rounded">
                    <h1 className="text-2xl font-bold font-size pb-10">Meble</h1>
                    <h2 className='py-4'>Zarejestruj się.</h2>
                    {error && <p className="error">{error}</p>}
                    <form className='flex flex-col justify-center flex-1' onSubmit={handleSubmit}>
                        <RegisterInput
                            type="email"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            placeholder="Email"
                            maxLength={40}
                        />
                        <RegisterInput
                            type="password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            placeholder="Hasło"
                            maxLength={24}
                            pattern={passwordRegex.source}
                        />
                        <RegisterInput
                            type="password"
                            value={confirmPassword}
                            onChange={(e) => setConfirmPassword(e.target.value)}
                            placeholder="Potwierdź hasło"
                            maxLength={24}
                        />
                        <RegisterButton isLoading={loading}>Zarejestruj się</RegisterButton>
                    </form>
                </div>

                <div className='rounded background flex p-20 md:p-10 flex-col justify-center items-center gap-10 max-w-full md:max-w-[500px] text-white'>
                    <h1 className="text-xl font-bold">MebloArt - działalność rodzinna!</h1>
                    <p className="text-base">Witamy w panelu rejestracji, aby korzystać ze sklepu prosze się zarejestrować</p>
                    <div className="flex flex-wrap justify-center gap-4 mt-4">
                        <div className="w-full md:w-1/2">
                            <h2 className="text-center font-bold text-lg">Wymagania do hasła:</h2>
                            <ul className="list-disc list-inside text-left text-sm">
                                <li>Co najmniej 8 znaków</li>
                                <li>Przynajmniej jedna duża litera (A-Z)</li>
                                <li>Przynajmniej jedna mała litera (a-z)</li>
                                <li>Przynajmniej jedna cyfra (0-9)</li>
                                <li>Przynajmniej jeden znak specjalny (!, @)</li>
                            </ul>
                        </div>
                        <div className="w-full md:w-1/2">
                            <h2 className="text-center font-bold text-lg">Formaty numeru telefonu:</h2>
                            <ul className="list-disc list-inside text-left text-sm">
                                <li>+XX XXX-XXX-XXX</li>
                                <li>+XXX XXX-XXX-XXX</li>
                            </ul>
                            <p className="text-xs mt-2">Gdzie "X" przedstawia cyfrę.</p>
                        </div>
                    </div>
                </div>

            </div>
        </div>
    );
}

export default Register;
