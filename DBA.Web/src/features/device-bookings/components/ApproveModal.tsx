import { Button, Group, Stack, TextInput } from '@mantine/core';
import { useForm } from '@mantine/form';
import { useApproveBooking } from '../hooks/useBookingMutations';

interface ApproveModalProps {
  bookingId: string;
  onClose: () => void;
}

export function ApproveModal({ bookingId, onClose }: ApproveModalProps) {
  const mutation = useApproveBooking();

  const form = useForm({
    initialValues: { approver: '' },
    validate: {
      approver: (v) => {
        if (!v.trim()) return '簽核人為必填';
        if (v.length > 20) return '簽核人不可超過 20 字';
        return null;
      },
    },
  });

  const handleSubmit = (values: { approver: string }) => {
    mutation.mutate({ id: bookingId, approver: values.approver }, { onSuccess: onClose });
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
            確認同意
          </Button>
        </Group>
      </Stack>
    </form>
  );
}
