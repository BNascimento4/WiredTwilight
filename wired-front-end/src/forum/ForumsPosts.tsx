import React, { useEffect, useState } from 'react';
import { Link, useParams } from 'react-router-dom';

interface Post {
    id: number;
    title: string;
    content: string;
}

const ForumPosts: React.FC = () => {
    const { forumId } = useParams<{ forumId: string }>();
    const [posts, setPosts] = useState<Post[]>([]);
    const [message, setMessage] = useState<string | null>(null);

    const fetchPosts = async () => {
        const token = localStorage.getItem('authToken');
        if (!token) {
            setMessage('Erro: Você precisa estar logado para visualizar os posts.');
            return;
        }

        try {
            const response = await fetch(`http://localhost:5223/forums/${forumId}/posts`, {
                method: 'GET',
                headers: {
                    Authorization: `Bearer ${token}`,
                },
            });

            if (response.ok) {
                const data: Post[] = await response.json();
                setPosts(data);
            } else {
                const errorData = await response.text();
                setMessage(`Erro: ${errorData}`);
            }
        } catch (error) {
            setMessage(`Erro ao conectar com o servidor: ${(error as Error).message}`);
        }
    };

    useEffect(() => {
        fetchPosts();
    }, [forumId]);

    return (
        <div>
            <h1>Posts do Fórum</h1>
            {message && <p>{message}</p>}
            <Link to={`/forum/${forumId}`}>
                <button>Voltar para o Fórum</button>
            </Link>
            <Link to={`/forum/${forumId}/create-post`}>
                <button>Criar Post</button>
            </Link>
            <ul>
                {posts.map((post) => (
                    <li key={post.id}>
                        <h3>{post.title}</h3>
                        <p>{post.content}</p>
                        <Link to={`/forum/${forumId}/post/${post.id}/comments`}>
                            <button>Ver Comentários</button>
                        </Link>
                    </li>
                ))}
            </ul>
        </div>
    );
};

export default ForumPosts;
