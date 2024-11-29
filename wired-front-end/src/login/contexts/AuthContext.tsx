import React, { createContext, useContext, useState } from 'react';

interface AuthContextData {
    token: string | null;
    login: (token: string) => void;
    logout: () => void;
}

const AuthContext = createContext<AuthContextData | undefined>(undefined);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const [token, setToken] = useState<string | null>(null);

    const login = (newToken: string) => {
        setToken(newToken);
        localStorage.setItem('authToken', newToken);
    };

    const logout = () => {
        setToken(null);
        localStorage.removeItem('authToken');
    };

    return (
        <AuthContext.Provider value={{ token, login, logout }}>
            {children}
        </AuthContext.Provider>
    );
};

export const useAuth = (): AuthContextData => {
    const context = useContext(AuthContext);
    if (!context) {
        throw new Error('useAuth deve ser usado dentro de um AuthProvider');
    }
    return context;
};
