import React, { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';

interface Post {
    id: number;
    title: string;
    content: string;
}

const CreatePost: React.FC = () => {
    const { forumId } = useParams<{ forumId: string }>(); // Obtém o ID do fórum da URL
    const [title, setTitle] = useState('');
    const [content, setContent] = useState('');
    const [message, setMessage] = useState<string | null>(null);
    const [posts, setPosts] = useState<Post[]>([]); // Estado para armazenar os posts criados
    const [forumDetails, setForumDetails] = useState<any>(null); // Para armazenar os detalhes do fórum
    const [loading, setLoading] = useState(false); // Para indicar que a requisição está em andamento
    const navigate = useNavigate(); // Hook de navegação

    // Carregar detalhes do fórum e os posts
    useEffect(() => {
        if (forumId) {
            // Carregar os detalhes do fórum
            fetch(`http://localhost:5223/forum/${forumId}`)
                .then((response) => response.json())
                .then((data) => setForumDetails(data))
                .catch((error) => setMessage('Erro ao carregar os detalhes do fórum'));

            // Carregar os posts do fórum
            fetch(`http://localhost:5223/forum/${forumId}/posts`)
                .then((response) => response.json())
                .then((data) => setPosts(data))
                .catch((error) => setMessage('Erro ao carregar os posts'));
        }
    }, [forumId]);

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

        setLoading(true); // Ativa o estado de carregamento

        try {
            const response = await fetch(`http://localhost:5223/forum/${forumId}/post`, { // Alterado para a URL correta
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${token}`,
                },
                body: JSON.stringify({ title, content }),
            });

            if (response.ok) {
                const newPost = await response.json();
                setPosts([newPost, ...posts]); // Adiciona o novo post na lista
                setTitle(''); // Limpa os campos de título
                setContent(''); // Limpa os campos de conteúdo
                setMessage('Post criado com sucesso!');
            } else {
                const errorData = await response.text();
                setMessage(`Erro: ${errorData}`);
            }
        } catch (error) {
            setMessage(`Erro ao conectar com o servidor: ${(error as Error).message}`);
        } finally {
            setLoading(false); // Finaliza o estado de carregamento
        }
    };

    return (
        <div>
            {/* Detalhes do Fórum */}
            {forumDetails && (
                <>
                    <h1>{forumDetails.title}</h1>
                    <p>{forumDetails.description}</p>
                    <p><strong>Criador:</strong> {forumDetails.creatorName}</p>
                    <hr />
                </>
            )}

            {/* Formulário para Criar um Post */}
            <div>
                <h2>Criar Novo Post</h2>
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
                    <button type="submit" disabled={loading}>
                        {loading ? 'Criando...' : 'Criar Post'}
                    </button>
                </form>
            </div>

            {/* Exibindo os posts criados abaixo do formulário */}
            <div>
                {posts.length > 0 ? (
                    posts.map((post, index) => (
                        <div key={post.id}>
                            <h3>{post.title}</h3>
                            <p>{post.content}</p>
                            {index < posts.length - 1 && <hr />} {/* Separador entre os posts */}
                        </div>
                    ))
                ) : (
                    <p>Não há posts neste fórum.</p>
                )}
            </div>
        </div>
    );
};

export default CreatePost;
