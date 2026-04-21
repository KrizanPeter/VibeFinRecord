import { apiClient } from './client';

export interface SnapshotStatus {
  missingDates: string[];
  nextExpectedDate: string | null;
}

export async function getSnapshotStatus(): Promise<SnapshotStatus> {
  const { data } = await apiClient.get<SnapshotStatus>('/api/v1/snapshots/status');
  return data;
}
