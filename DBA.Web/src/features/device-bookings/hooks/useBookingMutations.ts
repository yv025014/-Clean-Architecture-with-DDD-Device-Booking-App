import { useMutation, useQueryClient } from '@tanstack/react-query';
import { notifications } from '@mantine/notifications';
import { deviceBookingsApi, extractErrorDetail } from '../../../api/apiClient';
import { BOOKING_QUERY_KEY } from './useBookingQueries';

export function useCreateBooking() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: deviceBookingsApi.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: BOOKING_QUERY_KEY });
      notifications.show({
        message: '借用申請已成功送出，進入審核流程',
        color: 'green',
      });
    },
    onError: (error) => {
      notifications.show({ message: extractErrorDetail(error), color: 'red' });
    },
  });
}

export function useApproveBooking() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, approver }: { id: string; approver: string }) =>
      deviceBookingsApi.approve(id, approver),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: BOOKING_QUERY_KEY });
      notifications.show({
        message: '該單據已變更為【已借出】狀態',
        color: 'green',
      });
    },
    onError: (error) => {
      notifications.show({ message: extractErrorDetail(error), color: 'red' });
    },
  });
}

export function useRejectBooking() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({
      id,
      approver,
      reason,
    }: {
      id: string;
      approver: string;
      reason: string;
    }) => deviceBookingsApi.reject(id, approver, reason),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: BOOKING_QUERY_KEY });
      notifications.show({ message: '單據已拒絕', color: 'orange' });
    },
    onError: (error) => {
      notifications.show({ message: extractErrorDetail(error), color: 'red' });
    },
  });
}

export function useReturnBooking() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => deviceBookingsApi.returnDevice(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: BOOKING_QUERY_KEY });
      notifications.show({
        message: '設備已順利歸還，單據結案',
        color: 'green',
      });
    },
    onError: (error) => {
      notifications.show({ message: extractErrorDetail(error), color: 'red' });
    },
  });
}
