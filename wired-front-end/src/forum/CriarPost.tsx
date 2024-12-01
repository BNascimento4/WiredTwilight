import React, { useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';

const CreatePost: React.FC = () => {
    const { forumId } = useParams<{ forumId: string }>(); // Obtém o ID do fórum da URL
    const [title, setTitle] = useState('');
    const [content, setContent] = useState('');
    const [message, setMessage] = useState<string | null>(null);
    const navigate = useNavigate(); // Hook de navegação

    const handleAddPost = async () => {
        if (!forumId) {
            setMessage('Erro: Não foi possível encontrar o fórum.');
            return;
        }

        const token = localStorage.getItem('authToken'); // Recupera o token

        if (!token) {
            setMessage('Erro: Você precisa estar logado para criar um post.');
            return;
        }

        try {
            const response = await fetch(`http://localhost:5223/forums/${forumId}/posts`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${token}`,
                },
                body: JSON.stringify({ title, content }),
            });

            if (response.ok) {
                setMessage('Post criado com sucesso!');
                navigate(`/forum/${forumId}`); // Redireciona para o fórum
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
            <h1>Criar Post</h1>
            <form
                onSubmit={(e) => {
                    e.preventDefault();
                    handleAddPost();
                }}
            >
                <div>
                    <label>
                        Título:
                        <input
                            type="text"
                            value={title}
                            onChange={(e) => setTitle(e.target.value)}
                            required
                        />
                    </label>
                </div>
                <div>
                    <label>
                        Conteúdo:
                        <textarea
                            value={content}
                            onChange={(e) => setContent(e.target.value)}
                            required
                        />
                    </label>
                </div>
                <button type="submit">Criar Post</button>
            </form>
            {message && <p>{message}</p>}
        </div>
    );
};

export default CreatePost;

