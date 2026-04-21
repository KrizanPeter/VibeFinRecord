import { NavigatorScreenParams } from '@react-navigation/native';

export type AuthStackParamList = {
  Login: undefined;
  Register: undefined;
};

export type MainTabParamList = {
  Dashboard: undefined;
  Assets: undefined;
  Groups: undefined;
  Goals: undefined;
};

export type AppStackParamList = {
  AccountSetup: undefined;
  SnapshotGate: undefined;
  Main: NavigatorScreenParams<MainTabParamList>;
};

export type RootStackParamList = {
  Auth: NavigatorScreenParams<AuthStackParamList>;
  App: NavigatorScreenParams<AppStackParamList>;
};
