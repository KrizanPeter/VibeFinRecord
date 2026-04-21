import { create } from 'zustand';
import { persist, createJSONStorage } from 'zustand/middleware';
import * as SecureStore from 'expo-secure-store';

interface AuthState {
  accessToken: string | null;
  refreshToken: string | null;
  accountId: string | null;
  needsAccountSetup: boolean;
  setTokens: (accessToken: string, refreshToken: string, accountId: string) => void;
  clearTokens: () => void;
  setNeedsAccountSetup: (value: boolean) => void;
}

const secureStorage = createJSONStorage(() => ({
  getItem: (name: string) => SecureStore.getItemAsync(name),
  setItem: (name: string, value: string) => SecureStore.setItemAsync(name, value),
  removeItem: (name: string) => SecureStore.deleteItemAsync(name),
}));

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      accessToken: null,
      refreshToken: null,
      accountId: null,
      needsAccountSetup: false,
      setTokens: (accessToken, refreshToken, accountId) =>
        set({ accessToken, refreshToken, accountId, needsAccountSetup: false }),
      clearTokens: () =>
        set({ accessToken: null, refreshToken: null, accountId: null, needsAccountSetup: false }),
      setNeedsAccountSetup: (value) => set({ needsAccountSetup: value }),
    }),
    {
      name: 'auth-store',
      storage: secureStorage,
      partialize: (state) => ({
        accessToken: state.accessToken,
        refreshToken: state.refreshToken,
        accountId: state.accountId,
      }),
    },
  ),
);
