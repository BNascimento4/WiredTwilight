interface AddCommentResponse {
    id: number;
    content: string;
    createdAt: string;
}

export async function addCommentToPost(
    forumId: number,
    postId: number,
    commentContent: string
): Promise<AddCommentResponse | void> {
    const token = localStorage.getItem('authToken'); // Recupera o token do localStorage

    if (!token) {
        console.error("Token não encontrado. Faça login novamente.");
        return;
    }

    const url = `http://localhost:5223/forum/${forumId}/post/${postId}/comment`;

    const payload = { Content: commentContent };

    try {
        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                Authorization: `Bearer ${token}`,
            },
            body: JSON.stringify(payload),
        });

        if (!response.ok) {
            const error = await response.text();
            console.error("Erro ao adicionar comentário:", response.status, error);
            return;
        }

        const result: AddCommentResponse = await response.json();
        console.log("Comentário adicionado com sucesso:", result);
        return result;
    } catch (err) {
        console.error("Erro na requisição:", err);
    }
}
