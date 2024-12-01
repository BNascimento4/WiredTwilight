import React, { useEffect, useState } from 'react';
import { Link, useParams } from 'react-router-dom';

interface Post {
    id: number;
    title: string;
    content: string;
    createdByUserId: number;
}

const ForumPosts: React.FC = () => {
    const { forumId } = useParams<{ forumId: string }>();
    const [posts, setPosts] = useState<Post[]>([]);
    const [message, setMessage] = useState<string | null>(null);
    const [currentUser, setCurrentUser] = useState<number | null>(null); // ID do usuário logado
    const [showDeleteForumModal, setShowDeleteForumModal] = useState(false);
    const [password, setPassword] = useState('');

    // Função para buscar posts e usuário atual
    const fetchPosts = async () => {
        const token = localStorage.getItem('authToken');
        if (!token) {
            setMessage('Erro: Você precisa estar logado para visualizar os posts.');
            return;
        }

        try {
            const postsResponse = await fetch(`http://localhost:5223/forums/${forumId}/posts`, {
                method: 'GET',
                headers: {
                    Authorization: `Bearer ${token}`,
                },
            });

            const userResponse = await fetch(`http://localhost:5223/auth/me`, {
                method: 'GET',
                headers: {
                    Authorization: `Bearer ${token}`,
                },
            });

            if (postsResponse.ok && userResponse.ok) {
                const postsData: Post[] = await postsResponse.json();
                const userData = await userResponse.json();
                setPosts(postsData);
                setCurrentUser(userData.id); // Assume que a resposta traz o ID do usuário
            } else {
                const errorData = await postsResponse.text();
                setMessage(`Erro ao carregar dados: ${errorData}`);
            }
        } catch (error) {
            setMessage(`Erro ao conectar com o servidor: ${(error as Error).message}`);
        }
    };

    const deletePost = async (postId: number) => {
        const token = localStorage.getItem('authToken');
        if (!token) {
            setMessage('Erro: Você precisa estar logado para deletar posts.');
            return;
        }

        try {
            const response = await fetch(`http://localhost:5223/forums/${forumId}/post/${postId}`, {
                method: 'DELETE',
                headers: {
                    Authorization: `Bearer ${token}`,
                },
            });

            if (response.ok) {
                setPosts(posts.filter((post) => post.id !== postId));
            } else {
                const errorData = await response.text();
                setMessage(`Erro ao deletar post: ${errorData}`);
            }
        } catch (error) {
            setMessage(`Erro ao conectar com o servidor: ${(error as Error).message}`);
        }
    };

    const deleteForum = async () => {
        const token = localStorage.getItem('authToken');
        if (!token) {
            setMessage('Erro: Você precisa estar logado para deletar o fórum.');
            return;
        }

        try {
            const response = await fetch(`http://localhost:5223/forums/${forumId}`, {
                method: 'DELETE',
                headers: {
                    Authorization: `Bearer ${token}`,
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ password }),
            });

            if (response.ok) {
                alert('Fórum deletado com sucesso!');
                // Redirecionar para a página inicial ou outra ação
            } else {
                const errorData = await response.text();
                setMessage(`Erro ao deletar fórum: ${errorData}`);
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
                        {currentUser === post.createdByUserId && (
                            <button onClick={() => deletePost(post.id)}>Deletar Post</button>
                        )}
                    </li>
                ))}
            </ul>
            <button onClick={() => setShowDeleteForumModal(true)}>Deletar Fórum</button>
            {showDeleteForumModal && (
                <div>
                    <h2>Confirmar Exclusão do Fórum</h2>
                    <p>Digite sua senha para confirmar a exclusão:</p>
                    <input
                        type="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        placeholder="Senha"
                    />
                    <button onClick={deleteForum}>Confirmar</button>
                    <button onClick={() => setShowDeleteForumModal(false)}>Cancelar</button>
                </div>
            )}
        </div>
    );
};

export default ForumPosts;
