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
    
        if (!newName.trim()) {
            setMessage('Por favor, insira um novo nome de usuário.');
            return;
        }
    
        try {
            const response = await fetch('http://localhost:5223/usuario/atualizar-username', {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${token}`, // Corrigido: Uso correto do template string
                },
                body: JSON.stringify({ newUsername: newName }), // Corrigido: Formato da propriedade
            });
    
            if (response.ok) {
                setMessage('Nome alterado com sucesso!');
                setTimeout(() => {
                    navigate('/'); // Redireciona para outra página
                    alert('Nome atualizado com sucesso!');
                }, 1000);
            } else {
                const errorData = await response.text();
                setMessage(`Erro: ${errorData}`); // Corrigido: Uso do template string
            }
        } catch (error) {
            setMessage(`Erro ao conectar com o servidor: ${(error as Error).message}`); // Corrigido: Uso do template string
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
