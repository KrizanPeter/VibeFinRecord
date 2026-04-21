import { apiClient } from './client';
import { AssetResponse } from '../types';

interface AssetRequest {
  name: string;
  institution: string | null;
}

export async function listAssets(): Promise<AssetResponse[]> {
  const { data } = await apiClient.get<AssetResponse[]>('/api/v1/assets');
  return data;
}

export async function getAsset(id: string): Promise<AssetResponse> {
  const { data } = await apiClient.get<AssetResponse>(`/api/v1/assets/${id}`);
  return data;
}

export async function createAsset(body: AssetRequest): Promise<AssetResponse> {
  const { data } = await apiClient.post<AssetResponse>('/api/v1/assets', body);
  return data;
}

export async function updateAsset(id: string, body: AssetRequest): Promise<AssetResponse> {
  const { data } = await apiClient.put<AssetResponse>(`/api/v1/assets/${id}`, body);
  return data;
}

export async function deleteAsset(id: string): Promise<void> {
  await apiClient.delete(`/api/v1/assets/${id}`);
}
