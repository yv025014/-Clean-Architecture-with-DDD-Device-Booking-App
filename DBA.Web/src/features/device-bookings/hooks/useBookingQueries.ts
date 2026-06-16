import { useQuery } from '@tanstack/react-query';
import { deviceBookingsApi } from '../../../api/apiClient';

export const BOOKING_QUERY_KEY = ['device-bookings'] as const;

export function useDeviceBookings() {
  return useQuery({
    queryKey: BOOKING_QUERY_KEY,
    queryFn: deviceBookingsApi.getAll,
  });
}
