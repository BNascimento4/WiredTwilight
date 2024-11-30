import React, { useState } from 'react';

const Login: React.FC = () => {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [message, setMessage] = useState<string | null>(null);

    const handleLogin = async () => {
        try {
            const response = await fetch('http://localhost:5223/login', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ Username: username, Password: password }),
            });

            if (response.ok) {
                const data = await response.json();
                const token = data.token; // Ajuste o campo conforme o retorno do backend
                localStorage.setItem('authToken', token); // Salva o token no localStorage
                setMessage('Login realizado com sucesso!');
            } else {
                const errorData = await response.text();
                setMessage(`Erro: ${errorData}`);
            }
        } catch (error) {
            setMessage(`Erro ao conectar com o servidor: ${(error as Error).message}`);
        }
    };

    return (
        <div>
            <h1>Login</h1>
            <form
                onSubmit={(e) => {
                    e.preventDefault();
                    handleLogin();
                }}
            >
                <div>
                    <label>
                        Usuário:
                        <input
                            type="text"
                            value={username}
                            onChange={(e) => setUsername(e.target.value)}
                            required
                        />
                    </label>
                </div>
                <div>
                    <label>
                        Senha:
                        <input
                            type="password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            required
                        />
                    </label>
                </div>
                <button type="submit">Login</button>
            </form>
            {message && <p>{message}</p>}
        </div>
    );
};

export default Login;

