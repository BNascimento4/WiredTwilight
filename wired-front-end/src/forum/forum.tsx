import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';

// Modal de confirmação (sem estilização)
const ConfirmModal: React.FC<{
  showModal: boolean;
  onCancel: () => void;
  onConfirm: () => void;
}> = ({ showModal, onCancel, onConfirm }) => {
  if (!showModal) return null;

  return (
    <div>
      <div>
        <h2>Tem certeza?</h2>
        <p>Você tem dados não salvos. Deseja voltar sem salvar?</p>
        <button onClick={onConfirm}>Sim, voltar</button>
        <button onClick={onCancel}>Cancelar</button>
      </div>
    </div>
  );
};

const CreateForum: React.FC = () => {
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [responseMessage, setResponseMessage] = useState<string | null>(null);
  const [showModal, setShowModal] = useState(false); // Controle do modal
  const [isFormDirty, setIsFormDirty] = useState(false); // Para verificar se o formulário foi alterado
  const navigate = useNavigate();

  const handleCreateForum = async () => {
    const token = localStorage.getItem('authToken');

    if (!token) {
      setResponseMessage('Erro: Você precisa estar logado para criar um fórum.');
      return;
    }

    try {
      const response = await fetch('http://localhost:5223/forum', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({
          Title: title,
          Description: description,
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

  // Função para mostrar o modal de confirmação
  const handleGoBack = () => {
    if (isFormDirty) {
      setShowModal(true);
    } else {
      navigate('/forums');
    }
  };

  // Confirmar o retorno
  const handleConfirmReturn = () => {
    // Apaga os campos
    setTitle('');
    setDescription('');
    setShowModal(false);
    navigate('/forums');
  };

  // Cancelar o retorno
  const handleCancelReturn = () => {
    setShowModal(false);
  };

  const handleInputChange = () => {
    setIsFormDirty(true); // Marca o formulário como modificado
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
              onChange={(e) => { setTitle(e.target.value); handleInputChange(); }}
              required
            />
          </label>
        </div>
        <div>
          <label>
            Descrição:
            <textarea
              value={description}
              onChange={(e) => { setDescription(e.target.value); handleInputChange(); }}
              required
            />
          </label>
        </div>
        <button type="submit">Criar Fórum</button>
      </form>
      {responseMessage && <p>{responseMessage}</p>}

      {/* Botão de Voltar */}
      <button onClick={handleGoBack}>Voltar</button>

      {/* Modal de confirmação */}
      <ConfirmModal
        showModal={showModal}
        onCancel={handleCancelReturn}
        onConfirm={handleConfirmReturn}
      />
    </div>
  );
};

export default CreateForum;
