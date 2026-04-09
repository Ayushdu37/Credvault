// Mirrors: CredVault.Shared.Contracts.Notification.Responses.NotificationResponse
// Mirrors: CredVault.Shared.Contracts.Notification.Responses.NotificationPreferenceResponse
// Mirrors: CredVault.Shared.Contracts.Notification.Responses.UnreadCountResponse

export interface NotificationResponse {
  id: string;
  type: string;
  title: string;
  message: string;
  isRead: boolean;
  createdAt: string;
}

export interface NotificationPreferenceResponse {
  emailEnabled: boolean;
  paymentAlerts: boolean;
  billReminders: boolean;
  rewardUpdates: boolean;
}

export interface UnreadCountResponse {
  count: number;
}

// === FRONTEND-FRIENDLY TYPES ===

export interface Notification {
  id: string;
  type: 'payment' | 'security' | 'card' | 'billing' | 'system';
  title: string;
  message: string;
  timestamp: string;
  read: boolean;
  icon: string;
}

// === MAPPERS ===

const typeToIcon: Record<string, string> = {
  PaymentSuccess: 'check-circle',
  PaymentFailed: 'x-circle',
  BillGenerated: 'file-text',
  BillOverdue: 'alert-triangle',
  RewardEarned: 'award',
  RewardRedeemed: 'gift',
  General: 'info',
};

const typeToCategory: Record<string, Notification['type']> = {
  PaymentSuccess: 'payment',
  PaymentFailed: 'payment',
  BillGenerated: 'billing',
  BillOverdue: 'billing',
  RewardEarned: 'system',
  RewardRedeemed: 'system',
  General: 'system',
};

export function mapNotificationResponseToNotification(n: NotificationResponse): Notification {
  return {
    id: n.id,
    type: typeToCategory[n.type] || 'system',
    title: n.title,
    message: n.message,
    timestamp: n.createdAt,
    read: n.isRead,
    icon: typeToIcon[n.type] || 'info',
  };
}

// === REQUEST MODELS ===

export interface UpdatePreferencesRequest {
  emailEnabled: boolean;
  paymentAlerts: boolean;
  billReminders: boolean;
  rewardUpdates: boolean;
}
