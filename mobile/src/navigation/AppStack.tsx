import React from 'react';
import { ActivityIndicator, StyleSheet, View } from 'react-native';
import { createNativeStackNavigator } from '@react-navigation/native-stack';
import { useQuery } from '@tanstack/react-query';
import { useAccountStore } from '../store/accountStore';
import { accountApi, snapshotsApi } from '../api';
import { AppStackParamList } from './types';
import AccountSetupScreen from '../screens/account/AccountSetupScreen';
import SnapshotGateScreen from '../screens/snapshots/SnapshotGateScreen';
import MainTabs from './MainTabs';

const Stack = createNativeStackNavigator<AppStackParamList>();

export default function AppStack() {
  const isSetupComplete = useAccountStore((s) => s.isSetupComplete);
  const setAccount = useAccountStore((s) => s.setAccount);

  const { isLoading: isAccountLoading } = useQuery({
    queryKey: ['account'],
    queryFn: async () => {
      const data = await accountApi.getAccount();
      setAccount(data);
      return data;
    },
    staleTime: 5 * 60 * 1000,
  });

  const { data: snapshotStatus, isLoading: isSnapshotLoading } = useQuery({
    queryKey: ['snapshot-status'],
    queryFn: snapshotsApi.getSnapshotStatus,
    enabled: isSetupComplete,
  });

  if (isAccountLoading || (isSetupComplete && isSnapshotLoading)) {
    return (
      <View style={styles.center}>
        <ActivityIndicator size="large" color="#1F8A5E" />
      </View>
    );
  }

  const hasMissingSnapshots = (snapshotStatus?.missingDates?.length ?? 0) > 0;

  return (
    <Stack.Navigator screenOptions={{ headerShown: false }}>
      {!isSetupComplete ? (
        <Stack.Screen name="AccountSetup" component={AccountSetupScreen} />
      ) : hasMissingSnapshots ? (
        <Stack.Screen name="SnapshotGate" component={SnapshotGateScreen} />
      ) : (
        <Stack.Screen name="Main" component={MainTabs} />
      )}
    </Stack.Navigator>
  );
}

const styles = StyleSheet.create({
  center: { flex: 1, alignItems: 'center', justifyContent: 'center', backgroundColor: '#F7F8FA' },
});
