import React, { createContext, useState, useContext, useEffect } from 'react';
import axiosClient from '../AxiosClient';
import { User } from '../types';

interface AuthContextType {
  user: User | null;
  loading: boolean;
  login: (username: string, password: string) => Promise<{ success: boolean; error?: string }>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType>({
  user: null,
  loading: true,
  login: async () => ({ success: false }),
  logout: () => {},
});

export const useAuth = () => useContext(AuthContext);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);

  // Keep original flag but we can support session restoration if the user wants to stay logged in
  const ALWAYS_SHOW_LOGIN = false; // Set to false to allow persistent sessions on Web

  useEffect(() => {
    if (ALWAYS_SHOW_LOGIN) {
      setLoading(false);
    } else {
      checkUser();
    }
  }, []);

  const checkUser = () => {
    try {
      const storedUser = localStorage.getItem('user');
      if (storedUser) {
        setUser(JSON.parse(storedUser));
      }
    } catch (e) {
      console.log('Session check error:', e);
    } finally {
      setLoading(false);
    }
  };

  const login = async (username: string, password: string) => {
    try {
      setLoading(true);
      console.log('🔐 Attempting login for:', username);
      console.log('🌐 Using baseURL:', axiosClient.defaults.baseURL);

      let response;
      try {
        response = await axiosClient.post('/Auth/Login', {
          firstName: username,
          password: password,
        });
        console.log('✅ Login success with /Auth/Login');
      } catch (firstError: any) {
        console.log('⚠️ First endpoint failed, trying second...', firstError.message);
        response = await axiosClient.post('/Auth/Login', {
          username: username,
          password: password,
        });
        console.log('✅ Login success with alternative payload');
      }

      const userData = response.data;
      console.log('📦 User data received:', { ...userData, token: userData.token ? 'present' : 'missing' });

      localStorage.setItem('user', JSON.stringify(userData));
      if (userData.token) {
        localStorage.setItem('token', userData.token);
      } else if (userData.accessToken) {
        localStorage.setItem('token', userData.accessToken);
      }

      setUser(userData);
      return { success: true };
    } catch (err: any) {
      console.log('💥 Login error:', err);

      let errorMessage = 'Login failed';
      if (err.code === 'ECONNABORTED') {
        errorMessage = 'Connection timeout - Server not responding';
      } else if (err.code === 'ERR_NETWORK') {
        errorMessage = 'Network error - Cannot reach server. Please make sure the backend API is online.';
      } else if (err.response) {
        errorMessage = err.response.data?.message || err.response.data?.error || `Server error: ${err.response.status}`;
      } else if (err.request) {
        errorMessage = 'No response from server. Check network connection';
      } else {
        errorMessage = err.message;
      }

      return { success: false, error: errorMessage };
    } finally {
      setLoading(false);
    }
  };

  const logout = () => {
    localStorage.removeItem('user');
    localStorage.removeItem('token');
    setUser(null);
  };

  return (
    <AuthContext.Provider value={{ user, loading, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};
