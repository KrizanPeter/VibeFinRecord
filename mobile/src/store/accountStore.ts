import { create } from 'zustand';
import { AccountResponse, SnapshotPeriodicity } from '../types';

interface AccountState {
  isSetupComplete: boolean;
  currency: string | null;
  periodicity: SnapshotPeriodicity | null;
  setAccount: (data: AccountResponse) => void;
  clearAccount: () => void;
}

export const useAccountStore = create<AccountState>((set) => ({
  isSetupComplete: false,
  currency: null,
  periodicity: null,
  setAccount: (data) =>
    set({
      isSetupComplete: data.isSetupComplete,
      currency: data.currency,
      periodicity: data.snapshotPeriodicity,
    }),
  clearAccount: () =>
    set({ isSetupComplete: false, currency: null, periodicity: null }),
}));
