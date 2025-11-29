import {
  createContext,
  useContext,
  useEffect,
  useState,
} from 'react';
import type { ReactNode } from 'react';

import {
  getUserById,
  loginRequest,
  registerRequest,
} from '../api/users';
import type {
  AuthResponse,
  LoginFormValues,
  RegisterFormValues,
  UserResponse,
} from '../api/users';

interface AuthUser {
  id: string;
  email?: string | null;
  name?: string | null;
  role?: string | null;
  isActive?: boolean;
  emailConfirmed?: boolean;
}

interface AuthContextValue {
  user: AuthUser | null;
  accessToken: string | null;
  isAuthenticated: boolean;
  login: (data: LoginFormValues) => Promise<void>;
  register: (data: RegisterFormValues) => Promise<void>;
  logout: () => void;
  reloadUser: () => Promise<void>;
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

function parseJwt(token: string): any | null {
  try {
    const [, payload] = token.split('.');
    const decoded = atob(payload.replace(/-/g, '+').replace(/_/g, '/'));
    return JSON.parse(decoded);
  } catch {
    return null;
  }
}

function mapUserFromJwtAndApi(
  token: string,
  apiUser?: UserResponse,
): AuthUser | null {
  const payload = parseJwt(token);
  if (!payload) return null;

  const id: string =
    payload.sub || payload.userId || payload.UserId || apiUser?.Id;

  if (!id) return null;

  return {
    id,
    email: apiUser?.Email ?? null,
    name: apiUser?.Name ?? null,
    role: apiUser?.Role ?? (payload.role ?? null),
    isActive: apiUser?.IsActive,
    emailConfirmed: apiUser?.EmailConfirmed,
  };
}

async function loadUserByToken(
  token: string,
  setUser: (user: AuthUser | null) => void,
) {
  const payload = parseJwt(token);
  const id: string =
    payload?.sub || payload?.userId || payload?.UserId || '';

  if (!id) return;

  const apiUser = await getUserById(id);
  setUser(mapUserFromJwtAndApi(token, apiUser));
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [accessToken, setAccessToken] = useState<string | null>(
    () => localStorage.getItem('accessToken'),
  );
  const [user, setUser] = useState<AuthUser | null>(null);

  useEffect(() => {
    if (!accessToken) return;
    void loadUserByToken(accessToken, setUser);
  }, [accessToken]);

  const login = async (data: LoginFormValues) => {
    const res: AuthResponse = await loginRequest(data);
    if (!res.AccessToken) {
      throw new Error('No access token in response');
    }

    localStorage.setItem('accessToken', res.AccessToken);
    setAccessToken(res.AccessToken);
    await loadUserByToken(res.AccessToken, setUser);
  };

  const register = async (data: RegisterFormValues) => {
    await registerRequest(data);
  };

  const logout = () => {
    localStorage.removeItem('accessToken');
    setAccessToken(null);
    setUser(null);
  };

  const reloadUser = async () => {
    if (!accessToken) return;
    await loadUserByToken(accessToken, setUser);
  };

  const value: AuthContextValue = {
    user,
    accessToken,
    isAuthenticated: !!accessToken,
    login,
    register,
    logout,
    reloadUser,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuthInternal() {
  const ctx = useContext(AuthContext);
  if (!ctx) {
    throw new Error('useAuthInternal must be used within AuthProvider');
  }
  return ctx;
}
