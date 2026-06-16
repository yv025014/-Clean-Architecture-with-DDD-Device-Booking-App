import axios from 'axios';
import { z } from 'zod';
import type { DeviceBookingDto } from './generated-client';

const http = axios.create({
  baseURL: '',
  headers: { 'Content-Type': 'application/json' },
});

const ProblemDetailsSchema = z.object({
  detail: z.string().optional(),
});

export const extractErrorDetail = (error: unknown): string => {
  if (axios.isAxiosError(error)) {
    const parsed = ProblemDetailsSchema.safeParse(error.response?.data);
    return parsed.success && parsed.data.detail
      ? parsed.data.detail
      : '發生未預期的錯誤。';
  }
  return '發生未預期的錯誤。';
};

export const deviceBookingsApi = {
  getAll: (): Promise<DeviceBookingDto[]> =>
    http.get<DeviceBookingDto[]>('/api/DeviceBookings').then((r) => r.data),

  create: (payload: { deviceName: string; applicant: string; expectedReturn: string }): Promise<string> =>
    http.post<string>('/api/DeviceBookings', payload).then((r) => r.data),

  approve: (id: string, approver: string): Promise<string> =>
    http.put<string>(`/api/DeviceBookings/${id}/approve`, { approver }).then((r) => r.data),

  reject: (id: string, approver: string, reason: string): Promise<string> =>
    http.put<string>(`/api/DeviceBookings/${id}/reject`, { approver, reason }).then((r) => r.data),

  returnDevice: (id: string): Promise<string> =>
    http.put<string>(`/api/DeviceBookings/${id}/return`).then((r) => r.data),
};
