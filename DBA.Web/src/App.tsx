import { Container, Title } from '@mantine/core';
import { DeviceBookingsPage } from './features/device-bookings/DeviceBookingsPage';

export default function App() {
  return (
    <Container size="xl" py="xl">
      <Title order={2} mb="lg">
        企業內部設備借用管理系統
      </Title>
      <DeviceBookingsPage />
    </Container>
  );
}
