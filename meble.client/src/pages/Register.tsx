import React, { useState } from 'react';

const Register: React.FC = () => {
    const [email, setEmail] = useState<string>('');
    const [password, setPassword] = useState<string>('');

    const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();

        try {
            const response = await fetch('/register', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ Email: email, Password: password })
            });

            const data = await response.json();

            if (data.status === "Ok") {
                console.log("Rejestracja zakoñczona sukcesem");
            } else {
                console.error("B³¹d podczas rejestracji:", data);
            }
        } catch (error) {
            console.error("B³¹d podczas komunikacji z API:", error);
        }
    };

    return (
        <form onSubmit={handleSubmit}>
            <input type="email" value={email} onChange={(e) => setEmail(e.target.value)} placeholder="Email" />
            <input type="password" value={password} onChange={(e) => setPassword(e.target.value)} placeholder="Has³o" />
            <button type="submit">Zarejestruj siê</button>
        </form>
    );
}

export default Register;
