import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import '..//Styles//Login.css';

const Login: React.FC = () => {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [message, setMessage] = useState<string | null>(null);
    const navigate = useNavigate();

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
                navigate('/forums'); // Redireciona para os fóruns
            } else {
                const errorData = await response.text();
                setMessage(`Erro: ${errorData}`);
            }
        } catch (error) {
            setMessage(`Erro ao conectar com o servidor: ${(error as Error).message}`);
        }
    };

    return (
        <div className="Login">
            <h1>Login</h1>
            <form
                className="LoginForm"
                onSubmit={(e) => {
                    e.preventDefault();
                    handleLogin();
                }}
            >
                <div className="InputGroup">
                    <label>
                        Usuário:
                        <input
                            type="text"
                            value={username}
                            onChange={(e) => setUsername(e.target.value)}
                            required
                            placeholder="Digite seu usuário"
                        />
                    </label>
                </div>
                <div className="InputGroup">
                    <label>
                        Senha:
                        <input
                            type="password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            required
                            placeholder="Digite sua senha"
                        />
                    </label>
                </div>
                <div className="ButtonGroup">
                    <button type="submit">Concluir</button>
                    <button
                        type="button"
                        onClick={() => navigate('/registro')}
                    >Registrar-se</button>
                </div>
                {message && <p>{message}</p>}
            </form>
        </div>
    );
};

export default Login;
