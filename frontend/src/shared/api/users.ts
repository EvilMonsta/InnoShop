import { usersApi } from './http';


export interface AuthResponse {
  AccessToken: string | null;
  TokenType: string | null;
}

export interface UserResponse {
  Id: string;
  Name: string | null;
  Email: string | null;
  Role: string | null;
  IsActive: boolean;
  EmailConfirmed: boolean;
  CreatedAt: string | null;
}

export interface LoginFormValues {
  email: string;
  password: string;
}

export interface RegisterFormValues {
  name: string;
  email: string;
  password: string;
}

export interface UserFilter {
  q?: string;
  role?: string;
  isActive?: boolean;
  emailConfirmed?: boolean;
  page?: number;
  pageSize?: number;
}

export interface UserPagedResult {
  Items: UserResponse[] | null;
  Total: number;
  Page: number;
  PageSize: number;
}


export async function loginRequest(
  data: LoginFormValues,
): Promise<AuthResponse> {
  const res = await usersApi.post<AuthResponse>('/auth/login', {
    Email: data.email,
    Password: data.password,
  });
  return res.data;
}

export async function registerRequest(
  data: RegisterFormValues,
): Promise<UserResponse> {
  const res = await usersApi.post<UserResponse>('/auth/register', {
    Name: data.name,
    Email: data.email,
    Password: data.password,
  });
  return res.data;
}

export async function getUserById(id: string): Promise<UserResponse> {
  const res = await usersApi.get<UserResponse>(`/users/${id}`);
  return res.data;
}

export async function listUsers(
  filter: UserFilter,
): Promise<UserPagedResult> {
  const res = await usersApi.get<UserPagedResult>('/users', {
    params: {
      q: filter.q,
      role: filter.role,
      isActive: filter.isActive,
      emailConfirmed: filter.emailConfirmed,
      page: filter.page ?? 1,
      pageSize: filter.pageSize ?? 50,
    },
  });
  return res.data;
}

export async function updateUserName(
  id: string,
  name: string,
): Promise<void> {
  await usersApi.put(`/users/${id}`, {
    Name: name,
    Role: undefined,
  });
}

export async function deactivateUser(id: string): Promise<void> {
  await usersApi.post(`/users/${id}/deactivate`);
}

export async function reactivateUser(id: string): Promise<void> {
  await usersApi.post(`/users/${id}/reactivate`);
}

export async function requestEmailConfirmation(userId: string): Promise<void> {
  await usersApi.post('/auth/request-email-confirmation', {
    UserId: userId,
  });
}

export async function requestPasswordReset(email: string): Promise<void> {
  await usersApi.post('/auth/request-password-reset', {
    Email: email,
  });
}

export async function confirmPasswordReset(
  token: string,
  newPassword: string,
): Promise<void> {
  await usersApi.post(`/auth/reset-password/${token}`, {
    NewPassword: newPassword,
  });
}

export async function confirmEmail(token: string): Promise<void> {
  await usersApi.get(`/auth/confirm-email/${token}`);
}

export async function deleteUserAccount(id: string): Promise<void> {
  await usersApi.delete(`/users/${id}`);
}
