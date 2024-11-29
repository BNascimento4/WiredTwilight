import React, { useState } from 'react';
import { loginUser } from './services/loginService';
import { useAuth } from './contexts/AuthContext';
import './styles/LoginPage.css';

const LoginPage: React.FC = () => {
    const { login } = useAuth(); // Acesso ao contexto de autenticação
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState<string | null>(null);
    const [isLoading, setIsLoading] = useState(false);

    const handleLogin = async (event: React.FormEvent) => {
        event.preventDefault();
        setError(null);
        setIsLoading(true);

        try {
            const token = await loginUser({ email, password }); // Chama o serviço de login
            login(token); // Atualiza o contexto com o token recebido
            alert('Login realizado com sucesso!');
        } catch (err: any) {
            setError(err.message || 'Erro ao fazer login.');
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div className="login-container">
            <h1>Login</h1>
            <form onSubmit={handleLogin}>
                <div className="form-group">
                    <label>Email</label>
                    <input
                        type="email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        placeholder="Digite seu email"
                        required
                    />
                </div>
                <div className="form-group">
                    <label>Senha</label>
                    <input
                        type="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        placeholder="Digite sua senha"
                        required
                    />
                </div>
                {error && <p className="error">{error}</p>}
                <button type="submit" disabled={isLoading}>
                    {isLoading ? 'Carregando...' : 'Entrar'}
                </button>
            </form>
        </div>
    );
};

export default LoginPage;

