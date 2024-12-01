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

    const fetchForums = async () => {
        const token = localStorage.getItem('authToken');
        if (!token) {
            setMessage('Erro: Você precisa estar logado para visualizar os fóruns.');
            return;
        }

        try {
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

    const deleteForum = async (id: number) => {
        const token = localStorage.getItem('authToken');
        if (!token) {
            setMessage('Erro: Você precisa estar logado para deletar um fórum.');
            return;
        }

        try {
            const response = await fetch(`http://localhost:5223/forums/${id}`, {
                method: 'DELETE',
                headers: {
                    Authorization: `Bearer ${token}`,
                },
            });

            if (response.ok) {
                setForums(forums.filter((forum) => forum.id !== id));
                setMessage('Fórum deletado com sucesso.');
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
    }, []);

    return (
        <div>
            <nav>
                <img alt="Logo Wired Twilight" />
                <h1>Lista de Fóruns</h1>
                <Link to="/forum/criar">
                    <button>Criar Fórum</button>
                </Link>
            </nav>
            
            {message && <p>{message}</p>}
            <ul>
                {forums.map((forum) => (
                    <li key={forum.id}>
                        <Link to={`/forum/${forum.id}`}>{forum.title}</Link>
                        <p>{forum.description}</p>
                        <button onClick={() => deleteForum(forum.id)}>Deletar</button>
                    </li>
                ))}
            </ul>
        </div>
    );
};

export default ForumList;
