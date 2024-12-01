import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { addCommentToPost } from './adicionarComentario'; // Função de adicionar comentário

interface Comment {
    id: number;
    content: string;
    createdAt: string;
}

const PostComments: React.FC = () => {
    const { forumId, postId } = useParams<{ forumId: string, postId: string }>();
    const [comments, setComments] = useState<Comment[]>([]);
    const [content, setContent] = useState('');
    const [message, setMessage] = useState<string | null>(null);

    const fetchComments = async () => {
        const token = localStorage.getItem('authToken');
        if (!token) {
            setMessage('Erro: Você precisa estar logado para ver os comentários.');
            return;
        }

        try {
            const response = await fetch(`http://localhost:5223/posts/${postId}/comments`, {
                method: 'GET',
                headers: {
                    Authorization: `Bearer ${token}`,
                },
            });

            if (response.ok) {
                const data: Comment[] = await response.json();
                setComments(data);
            } else {
                const errorData = await response.text();
                setMessage(`Erro: ${errorData}`);
            }
        } catch (error) {
            setMessage(`Erro ao conectar com o servidor: ${(error as Error).message}`);
        }
    };

    const handleAddComment = async () => {
        if (!forumId || !postId) {
            setMessage('Erro: Não foi possível encontrar o fórum ou post.');
            return;
        }

        const commentResponse = await addCommentToPost(Number(forumId), Number(postId), content);
        if (commentResponse) {
            setMessage('Comentário adicionado com sucesso!');
            fetchComments(); // Atualiza a lista de comentários
        } else {
            setMessage('Erro: Não foi possível adicionar o comentário.');
        }
    };

    useEffect(() => {
        fetchComments();
    }, [forumId, postId]);

    return (
        <div>
            <h1>Comentários do Post</h1>
            {message && <p>{message}</p>}
            <textarea
                value={content}
                onChange={(e) => setContent(e.target.value)}
                required
            />
            <button onClick={handleAddComment}>Adicionar Comentário</button>
            <ul>
                {comments.map((comment) => (
                    <li key={comment.id}>
                        <p>{comment.content}</p>
                        <p><em>{new Date(comment.createdAt).toLocaleString()}</em></p>
                    </li>
                ))}
            </ul>
        </div>
    );
};

export default PostComments;
