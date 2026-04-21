import React from 'react';
import { createNativeStackNavigator } from '@react-navigation/native-stack';
import { useAuthStore } from '../store/authStore';
import { RootStackParamList } from './types';
import AuthStack from './AuthStack';
import AppStack from './AppStack';

const Root = createNativeStackNavigator<RootStackParamList>();

export default function RootNavigator() {
  const accessToken = useAuthStore((s) => s.accessToken);
  return (
    <Root.Navigator screenOptions={{ headerShown: false }}>
      {accessToken ? (
        <Root.Screen name="App" component={AppStack} />
      ) : (
        <Root.Screen name="Auth" component={AuthStack} />
      )}
    </Root.Navigator>
  );
}
