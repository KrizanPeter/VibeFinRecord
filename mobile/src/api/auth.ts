import axios from 'axios';
import { API_BASE_URL } from '@env';
import { AuthResponse } from '../types';

const BASE = `${API_BASE_URL}/api/v1/auth`;

export async function register(email: string, password: string): Promise<AuthResponse> {
  const { data } = await axios.post<AuthResponse>(`${BASE}/register`, { email, password });
  return data;
}

export async function login(email: string, password: string): Promise<AuthResponse> {
  const { data } = await axios.post<AuthResponse>(`${BASE}/login`, { email, password });
  return data;
}

export async function refresh(
  userId: string,
  refreshToken: string,
): Promise<{ accessToken: string; refreshToken: string }> {
  const { data } = await axios.post<{ accessToken: string; refreshToken: string }>(
    `${BASE}/refresh`,
    { userId, refreshToken },
  );
  return data;
}
