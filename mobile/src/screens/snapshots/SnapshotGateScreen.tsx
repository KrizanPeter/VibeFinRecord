import React from 'react';
import { StyleSheet, Text, View } from 'react-native';

export default function SnapshotGateScreen() {
  return (
    <View style={styles.container}>
      <Text style={styles.label}>Snapshot Gate</Text>
    </View>
  );
}

const styles = StyleSheet.create({
  container: { flex: 1, alignItems: 'center', justifyContent: 'center', backgroundColor: '#F7F8FA' },
  label: { fontSize: 18, color: '#0F1720' },
});
