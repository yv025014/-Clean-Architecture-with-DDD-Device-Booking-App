import {
  Badge,
  Button,
  Center,
  Group,
  Modal,
  Skeleton,
  Stack,
  Table,
  Tabs,
  Text,
} from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
import { useState } from 'react';
import dayjs from 'dayjs';
import type { DeviceBookingDto } from '../../../api/generated-client';
import { useReturnBooking } from '../hooks/useBookingMutations';
import { ApproveModal } from './ApproveModal';
import { RejectModal } from './RejectModal';

const STATUS_MAP: Record<number, { label: string; color: string }> = {
  1: { label: '審核中', color: 'yellow' },
  2: { label: '已借出', color: 'blue' },
  3: { label: '已歸還', color: 'green' },
  4: { label: '已拒絕', color: 'red' },
};

interface ActionCellProps {
  booking: DeviceBookingDto;
}

function ActionCell({ booking }: ActionCellProps) {
  const [approveOpened, { open: openApprove, close: closeApprove }] = useDisclosure(false);
  const [rejectOpened, { open: openReject, close: closeReject }] = useDisclosure(false);
  const returnMutation = useReturnBooking();

  const id = booking.id ?? '';
  const status = booking.status ?? 0;

  if (status === 1) {
    return (
      <>
        <Group gap="xs">
          <Button size="xs" variant="filled" color="blue" onClick={openApprove}>
            同意
          </Button>
          <Button size="xs" variant="outline" color="red" onClick={openReject}>
            拒絕
          </Button>
        </Group>

        <Modal opened={approveOpened} onClose={closeApprove} title="確認同意借用" centered>
          <ApproveModal bookingId={id} onClose={closeApprove} />
        </Modal>

        <Modal opened={rejectOpened} onClose={closeReject} title="確認拒絕申請" centered>
          <RejectModal bookingId={id} onClose={closeReject} />
        </Modal>
      </>
    );
  }

  if (status === 2) {
    return (
      <Button
        size="xs"
        variant="outline"
        color="green"
        loading={returnMutation.isPending}
        loaderProps={{ type: 'dots' }}
        onClick={() => returnMutation.mutate(id)}
      >
        歸還設備
      </Button>
    );
  }

  return null;
}

interface BookingTableProps {
  bookings: DeviceBookingDto[];
  isLoading: boolean;
}

function BookingTable({ bookings, isLoading }: BookingTableProps) {
  if (isLoading) {
    return (
      <Stack gap="xs">
        {Array.from({ length: 5 }).map((_, i) => (
          <Skeleton key={i} height={48} radius="sm" />
        ))}
      </Stack>
    );
  }

  if (bookings.length === 0) {
    return (
      <Center style={{ height: 200 }}>
        <Stack align="center" gap="xs">
          <Text size="xl" c="dimmed">📋</Text>
          <Text c="dimmed">目前尚無任何借用申請紀錄</Text>
        </Stack>
      </Center>
    );
  }

  return (
    <Table withTableBorder withColumnBorders highlightOnHover>
      <Table.Thead>
        <Table.Tr>
          <Table.Th>設備名稱</Table.Th>
          <Table.Th>借用人</Table.Th>
          <Table.Th>預計歸還時間</Table.Th>
          <Table.Th>狀態</Table.Th>
          <Table.Th>動作</Table.Th>
        </Table.Tr>
      </Table.Thead>
      <Table.Tbody>
        {bookings.map((b) => {
          const statusInfo = STATUS_MAP[b.status ?? 0] ?? { label: '未知', color: 'gray' };
          return (
            <Table.Tr key={b.id}>
              <Table.Td>{b.deviceName}</Table.Td>
              <Table.Td>{b.applicant}</Table.Td>
              <Table.Td>
                {b.expectedReturn
                  ? dayjs(b.expectedReturn).format('YYYY/MM/DD HH:mm')
                  : '-'}
              </Table.Td>
              <Table.Td>
                <Badge color={statusInfo.color}>{statusInfo.label}</Badge>
              </Table.Td>
              <Table.Td>
                <ActionCell booking={b} />
              </Table.Td>
            </Table.Tr>
          );
        })}
      </Table.Tbody>
    </Table>
  );
}

interface BookingListProps {
  bookings: DeviceBookingDto[];
  isLoading: boolean;
}

export function BookingList({ bookings, isLoading }: BookingListProps) {
  const [activeTab, setActiveTab] = useState<string | null>('all');

  const filtered: Record<string, DeviceBookingDto[]> = {
    all: bookings,
    pending: bookings.filter((b) => b.status === 1),
    approved: bookings.filter((b) => b.status === 2),
    closed: bookings.filter((b) => b.status === 3 || b.status === 4),
  };

  return (
    <Tabs value={activeTab} onChange={setActiveTab}>
      <Tabs.List mb="md">
        <Tabs.Tab value="all">全部 ({filtered.all.length})</Tabs.Tab>
        <Tabs.Tab value="pending">審核中 ({filtered.pending.length})</Tabs.Tab>
        <Tabs.Tab value="approved">已借出 ({filtered.approved.length})</Tabs.Tab>
        <Tabs.Tab value="closed">已結束 ({filtered.closed.length})</Tabs.Tab>
      </Tabs.List>

      {(['all', 'pending', 'approved', 'closed'] as const).map((key) => (
        <Tabs.Panel key={key} value={key}>
          <BookingTable bookings={filtered[key]} isLoading={isLoading && key === 'all'} />
        </Tabs.Panel>
      ))}
    </Tabs>
  );
}
