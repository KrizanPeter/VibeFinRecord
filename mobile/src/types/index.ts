export interface ApiError {
  status: number;
  title: string;
  detail: string;
  type?: string;
}

export type SnapshotPeriodicity = 'Monthly' | 'Quarterly' | 'Yearly';

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  accountId: string;
}

export interface AccountResponse {
  id: string;
  currency: string | null;
  snapshotStartDate: string | null;
  snapshotPeriodicity: SnapshotPeriodicity | null;
  isSetupComplete: boolean;
}

export interface AssetResponse {
  id: string;
  name: string;
  institution: string | null;
  createdAt: string;
}
