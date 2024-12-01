import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';

interface Forum {
    id: number;
    title: string;
    description: string;
}

const ForumList: React.FC = () => {
    const [forums, setForums] = useState<Forum[]>([]);
    const [message, setMessage] = useState<string | null>(null);
    const [username, setUsername] = useState<string | null>(null);

    const fetchForums = async () => {
        const token = localStorage.getItem('authToken');
        if (!token) {
            setMessage('Erro: Você precisa estar logado para visualizar os fóruns.');
            return;
        }

        try {
            // Buscando os fóruns
            const response = await fetch('http://localhost:5223/forums', {
                method: 'GET',
                headers: {
                    Authorization: `Bearer ${token}`,
                },
            });

            if (response.ok) {
                const data: Forum[] = await response.json();
                setForums(data);
            } else {
                const errorData = await response.text();
                setMessage(`Erro: ${errorData}`);
            }
        } catch (error) {
            setMessage(`Erro ao conectar com o servidor: ${(error as Error).message}`);
        }
    };

    const fetchUsername = async () => {
        const token = localStorage.getItem('authToken');
        if (!token) {
            setMessage('Erro: Você precisa estar logado para visualizar seu nome.');
            return;
        }

        try {
            // Buscando o nome do usuário
            const response = await fetch('http://localhost:5223/usuario/info', {
                method: 'GET',
                headers: {
                    Authorization: `Bearer ${token}`,
                },
            });

            if (response.ok) {
                const data = await response.json();
                setUsername(data.username); // Certifique-se de que a API retorna o campo "username"
            } else {
                const errorData = await response.text();
                setMessage(`Erro: ${errorData}`);
            }
        } catch (error) {
            setMessage(`Erro ao conectar com o servidor: ${(error as Error).message}`);
        }
    };

    useEffect(() => {
        fetchForums();
        fetchUsername();
    }, []);

    return (
        <div>
            <nav>
                <h1>Lista de Fóruns</h1>
                {username && <p>Bem-vindo, {username}!</p>} {/* Nome do usuário */}
                <div style={{ float: 'right' }}>
                    <Link to="/Atualizar-username">
                        <button>Configurar Usuário</button>
                    </Link>
                </div>
                <Link to="/forum/criar">
                    <button>Criar Fórum</button>
                </Link>
            </nav>
            <ul>
                {forums.length === 0 ? (
                    <p>Não há fóruns disponíveis.</p>
                ) : (
                    forums.map((forum) => (
                        <li key={forum.id}>
                            <Link to={`/forum/${forum.id}`}>
                                <h2>{forum.title}</h2>
                            </Link>
                            <p>{forum.description}</p>
                            <Link to={`/forum/${forum.id}/posts`}>
                                <button>Ver Posts</button>
                            </Link>
                        </li>
                    ))
                )}
            </ul>
            {message && <p>{message}</p>}
        </div>
    );
};

export default ForumList;
