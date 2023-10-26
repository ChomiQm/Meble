import React, { useState } from 'react';

const Login: React.FC = () => {
    const [email, setEmail] = useState<string>('');
    const [password, setPassword] = useState<string>('');

    const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();

        try {
            const response = await fetch('/login', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ Email: email, Password: password })
            });

            const data = await response.json();

            if (data.status === "Ok") {
                console.log("Logowanie zako�czone sukcesem");
            } else {
                console.error("B��d podczas logowania:", data);
            }
        } catch (error) {
            console.error("B��d podczas komunikacji z API:", error);
        }
    };

    return (
        <form onSubmit={handleSubmit}>
            <input type="email" value={email} onChange={(e) => setEmail(e.target.value)} placeholder="Email" />
            <input type="password" value={password} onChange={(e) => setPassword(e.target.value)} placeholder="Has�o" />
            <button type="submit">Zaloguj si�</button>
        </form>
    );
}

export default Login;
