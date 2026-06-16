import { Button, Group, Stack, TextInput } from '@mantine/core';
import { useForm } from '@mantine/form';
import { useRejectBooking } from '../hooks/useBookingMutations';

interface RejectModalProps {
  bookingId: string;
  onClose: () => void;
}

export function RejectModal({ bookingId, onClose }: RejectModalProps) {
  const mutation = useRejectBooking();

  const form = useForm({
    initialValues: { approver: '', reason: '' },
    validate: {
      approver: (v) => {
        if (!v.trim()) return '簽核人為必填';
        if (v.length > 20) return '簽核人不可超過 20 字';
        return null;
      },
      reason: (v) => {
        if (!v.trim()) return '拒絕原因為必填';
        if (v.length > 100) return '拒絕原因不可超過 100 字';
        return null;
      },
    },
  });

  const handleSubmit = (values: { approver: string; reason: string }) => {
    mutation.mutate(
      { id: bookingId, approver: values.approver, reason: values.reason },
      { onSuccess: onClose }
    );
  };

  return (
    <form onSubmit={form.onSubmit(handleSubmit)}>
      <Stack gap="md">
        <TextInput
          label="簽核人"
          placeholder="請輸入簽核人姓名"
          required
          {...form.getInputProps('approver')}
        />
        <TextInput
          label="拒絕原因"
          placeholder="請輸入拒絕原因（最多 100 字）"
          required
          {...form.getInputProps('reason')}
        />
        <Group justify="flex-end" gap="xs">
          <Button variant="subtle" color="gray" onClick={onClose}>
            取消
          </Button>
          <Button
            type="submit"
            variant="filled"
            color="blue"
            loading={mutation.isPending}
            loaderProps={{ type: 'dots' }}
          >
            確認拒絕
          </Button>
        </Group>
      </Stack>
    </form>
  );
}
