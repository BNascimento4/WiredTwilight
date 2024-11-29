import { LoginRequest } from '../contexts/AuthContext';

export async function loginUser(data: LoginRequest): Promise<string> {
    const response = await fetch('/api/login', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(data),
    });

    if (!response.ok) {
        const errorResponse = await response.json();
        throw new Error(errorResponse.message || 'Erro ao fazer login.');
    }

    const { token } = await response.json();
    return token;
}
