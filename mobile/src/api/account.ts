import { apiClient } from './client';
import { AccountResponse, SnapshotPeriodicity } from '../types';

interface UpdateAccountRequest {
  currency: string;
  snapshotStartDate: string;
  snapshotPeriodicity: SnapshotPeriodicity;
}

export async function getAccount(): Promise<AccountResponse> {
  const { data } = await apiClient.get<AccountResponse>('/api/v1/account');
  return data;
}

export async function updateAccount(body: UpdateAccountRequest): Promise<AccountResponse> {
  const { data } = await apiClient.put<AccountResponse>('/api/v1/account', body);
  return data;
}
