import { Button, Card, Stack, TextInput } from '@mantine/core';
import { DateTimePicker } from '@mantine/dates';
import { useForm } from '@mantine/form';
import dayjs from 'dayjs';
import { useCreateBooking } from '../hooks/useBookingMutations';

interface FormValues {
  deviceName: string;
  applicant: string;
  expectedReturn: string | null;
}

export function BookingForm() {
  const mutation = useCreateBooking();

  const form = useForm<FormValues>({
    initialValues: {
      deviceName: '',
      applicant: '',
      expectedReturn: null,
    },
    validate: {
      deviceName: (v) => {
        if (!v.trim()) return '設備名稱為必填';
        if (v.length > 20) return '設備名稱不可超過 20 字';
        return null;
      },
      applicant: (v) => {
        if (!v.trim()) return '借用人為必填';
        if (v.length > 20) return '借用人不可超過 20 字';
        return null;
      },
      expectedReturn: (v) => {
        if (!v) return '請選擇預計歸還時間';
        if (dayjs(v, 'YYYY-MM-DD HH:mm:ss').toDate() <= new Date())
          return '預計歸還時間不可早於當下時間';
        return null;
      },
    },
  });

  const handleSubmit = (values: FormValues) => {
    if (!values.expectedReturn) return;
    const isoDate = dayjs(values.expectedReturn, 'YYYY-MM-DD HH:mm:ss').toISOString();
    mutation.mutate(
      { deviceName: values.deviceName, applicant: values.applicant, expectedReturn: isoDate },
      { onSuccess: () => form.reset() }
    );
  };

  return (
    <Card withBorder shadow="sm" radius="md" p="xl">
      <form onSubmit={form.onSubmit(handleSubmit)}>
        <Stack gap="md">
          <TextInput
            label="設備名稱"
            placeholder="請輸入設備名稱"
            required
            {...form.getInputProps('deviceName')}
          />
          <TextInput
            label="借用人"
            placeholder="請輸入借用人姓名"
            required
            {...form.getInputProps('applicant')}
          />
          <DateTimePicker
            label="預計歸還時間"
            placeholder="請選擇預計歸還日期與時間"
            required
            valueFormat="YYYY/MM/DD HH:mm"
            minDate={new Date()}
            {...form.getInputProps('expectedReturn')}
          />
          <Button
            type="submit"
            variant="filled"
            color="blue"
            loading={mutation.isPending}
            loaderProps={{ type: 'dots' }}
          >
            送出申請
          </Button>
        </Stack>
      </form>
    </Card>
  );
}
