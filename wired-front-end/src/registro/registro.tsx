import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';

const Registro: React.FC = () => {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [responseMessage, setResponseMessage] = useState<string | null>(null);
    const navigate = useNavigate();

    const handleRegister = async () => {
        try {
            const response = await fetch('http://localhost:5223/registro', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    Username: username,
                    Password: password,
                }),
            });

            if (response.ok) {
                setResponseMessage('Usuário registrado com sucesso!');
                navigate('/'); // Redireciona para a tela de login
            } else {
                const errorData = await response.text();
                setResponseMessage(`Erro: ${errorData}`);
            }
        } catch (error) {
            setResponseMessage(`Erro ao conectar com o servidor: ${(error as Error).message}`);
        }
    };

    return (
        <div>
            <h1>Registro de Usuário</h1>
            <form
                onSubmit={(e) => {
                    e.preventDefault();
                    handleRegister();
                }}
            >
                <div>
                    <label>
                        Nome de Usuário:
                        <input
                            type="text"
                            value={username}
                            onChange={(e) => setUsername(e.target.value)}
                            required
                            placeholder="Digite seu usuário"
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
                            placeholder="Digite sua senha"
                        />
                    </label>
                </div>
                <button type="submit">Registrar</button>
            </form>
            <button onClick={() => navigate('/')}>Ir para Login</button>
            {responseMessage && <p>{responseMessage}</p>}
        </div>
    );
};

export default Registro;
