import { Box, Card, LoadingOverlay, Stack, Title } from '@mantine/core';
import { useDeviceBookings } from './hooks/useBookingQueries';
import { BookingForm } from './components/BookingForm';
import { BookingList } from './components/BookingList';

export function DeviceBookingsPage() {
  const { data = [], isLoading } = useDeviceBookings();

  return (
    <Stack gap="xl">
      <Stack gap="xs">
        <Title order={4}>申請借用設備</Title>
        <BookingForm />
      </Stack>

      <Stack gap="xs">
        <Title order={4}>借用申請清單</Title>
        <Card withBorder shadow="sm" radius="md" p="xl">
          <Box pos="relative">
            <LoadingOverlay
              visible={isLoading}
              overlayProps={{ blur: 2 }}
            />
            <BookingList bookings={data} isLoading={isLoading} />
          </Box>
        </Card>
      </Stack>
    </Stack>
  );
}
