import React, { useState } from 'react';

const CreateForum: React.FC = () => {
    const [title, setTitle] = useState('');
    const [description, setDescription] = useState('');
    const [isActive, setIsActive] = useState(true);
    const [responseMessage, setResponseMessage] = useState<string | null>(null);

    const handleCreateForum = async () => {
        const token = localStorage.getItem('authToken'); // Recupera o token do localStorage

        if (!token) {
            setResponseMessage('Erro: Você precisa estar logado para criar um fórum.');
            return;
        }

        try {
            const response = await fetch('http://localhost:5223/forum', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${token}`, // Usa o token armazenado
                },
                body: JSON.stringify({
                    Title: title,
                    Description: description,
                    IsActive: isActive,
                }),
            });

            if (response.ok) {
                const data = await response.json();
                setResponseMessage(`Fórum criado com sucesso! ID: ${data.id}`);
            } else {
                const errorData = await response.text();
                setResponseMessage(`Erro: ${errorData}`);
            }
        } catch (error) {
            setResponseMessage(`Erro ao conectar com o servidor: ${(error as Error).message}`);
        }
    };

    return (
        <div>
            <h1>Criar Fórum</h1>
            <form
                onSubmit={(e) => {
                    e.preventDefault();
                    handleCreateForum();
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
                        Descrição:
                        <textarea
                            value={description}
                            onChange={(e) => setDescription(e.target.value)}
                            required
                        />
                    </label>
                </div>
                <div>
                    <label>
                        Ativo:
                        <input
                            type="checkbox"
                            checked={isActive}
                            onChange={(e) => setIsActive(e.target.checked)}
                        />
                    </label>
                </div>
                <button type="submit">Criar Fórum</button>
            </form>
            {responseMessage && <p>{responseMessage}</p>}
        </div>
    );
};

export default CreateForum;
