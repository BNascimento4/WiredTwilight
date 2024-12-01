import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';

const AtualizarUsername: React.FC = () => {
    const [newName, setNewName] = useState('');
    const [message, setMessage] = useState<string | null>(null);
    const navigate = useNavigate();

    const handleUpdateName = async () => {
        const token = localStorage.getItem('authToken');

        if (!token) {
            setMessage('Você precisa estar logado para alterar seu nome.');
            return;
        }

        if (!newName) {
            setMessage('Por favor, insira um novo nome.');
            return;
        }

        try {
            const response = await fetch('http://localhost:5223/usuario/alterar-nome', {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${token}`,
                },
                body: JSON.stringify({ newName }),
            });

            if (response.ok) {
                setMessage('Nome alterado com sucesso!');
                setTimeout(() => {
                    navigate('/'); // Redireciona para a tela de lista de fóruns
                    alert('Nome atualizado com sucesso!');
                }, 1000);
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
            <h1>Atualizar Nome de Usuário</h1>
            <div>
                <label>
                    Novo Nome:
                    <input
                        type="text"
                        value={newName}
                        onChange={(e) => setNewName(e.target.value)}
                        required
                    />
                </label>
            </div>
            <div>
                <button onClick={handleUpdateName}>Confirmar Alteração</button>
                <button onClick={() => navigate('/')}>Voltar</button>
            </div>
            {message && <p>{message}</p>}
        </div>
    );
};

export default AtualizarUsername;
