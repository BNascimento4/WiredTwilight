import React, { useState, useEffect } from 'react';

interface Forum {
    id: number;
    title: string;
    description: string;
}

const ForumList: React.FC = () => {
    const [forums, setForums] = useState<Forum[]>([]);
    const [message, setMessage] = useState<string | null>(null);

    const fetchForums = async () => {
        const token = localStorage.getItem('authToken'); // Recupera o token do localStorage
        if (!token) {
            setMessage('Erro: Você precisa estar logado para visualizar os fóruns.');
            return;
        }

        try {
            const response = await fetch('http://localhost:5223/forums', {
                method: 'GET',
                headers: {
                    Authorization: `Bearer ${token}`, // Autenticação
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

    useEffect(() => {
        fetchForums(); // Carrega os fóruns ao montar o componente
    }, []);

    return (
        <div>
            <h1>Lista de Fóruns</h1>
            {message && <p>{message}</p>}
            <ul>
                {forums.map((forum) => (
                    <li key={forum.id}>
                        <a href={`/forum/${forum.id}`}>{forum.title}</a>
                        <p>{forum.description}</p>
                    </li>
                ))}
            </ul>
        </div>
    );
};

export default ForumList;

