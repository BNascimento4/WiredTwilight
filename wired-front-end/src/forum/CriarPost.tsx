import React, { useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { addCommentToPost } from './adicionarComentario'; // Importe a função aqui

const CreatePost: React.FC = () => {
    const { forumId, postId } = useParams<{ forumId: string, postId: string }>(); // Obtém o ID do fórum e do post da URL
    const [content, setContent] = useState('');
    const [message, setMessage] = useState<string | null>(null);
    const navigate = useNavigate(); // Hook de navegação

    const handleAddComment = async () => {
        if (!forumId || !postId) {
            setMessage('Erro: Não foi possível encontrar o fórum ou post.');
            return;
        }

        const token = localStorage.getItem('authToken'); // Recupera o token

        if (!token) {
            setMessage('Erro: Você precisa estar logado para adicionar um comentário.');
            return;
        }

        try {
            const response = await addCommentToPost(Number(forumId), Number(postId), content);

            if (response) {
                setMessage('Comentário adicionado com sucesso!');
            } else {
                setMessage('Erro: Não foi possível adicionar o comentário.');
            }
        } catch (error) {
            setMessage(`Erro ao conectar com o servidor: ${(error as Error).message}`);
        }
    };

    // Função para voltar à lista de fóruns
    const handleGoBack = () => {
        navigate('/forums'); // Navega para a página de lista de fóruns
    };

    return (
        <div>
            <h1>Adicionar Comentário ao Post</h1>
            <form
                onSubmit={(e) => {
                    e.preventDefault();
                    handleAddComment();
                }}
            >
                <div>
                    <label>
                        Comentário:
                        <textarea
                            value={content}
                            onChange={(e) => setContent(e.target.value)}
                            required
                        />
                    </label>
                </div>
                <button type="submit">Adicionar Comentário</button>
            </form>
            {message && <p>{message}</p>}

            {/* Botão de Voltar */}
            <button onClick={handleGoBack}>Voltar</button>
        </div>
    );
};

export default CreatePost;
