// Centralized event types - must match backend EventType.cs exactly
export const EVENT_TYPES = [
    { value: 'SubscriberRegistered', label: 'Subscriber Registered', icon: '👋' },
    { value: 'PasswordReset', label: 'Password Reset', icon: '🔐' },
    { value: 'EmailVerified', label: 'Email Verified', icon: '✅' },
    { value: 'LoginAlert', label: 'Login Alert', icon: '🔔' },
    { value: 'AccountLocked', label: 'Account Locked', icon: '🔒' },
    { value: 'OrderCreated', label: 'Order Created', icon: '📦' },
    { value: 'OrderShipped', label: 'Order Shipped', icon: '🚚' },
    { value: 'OrderDelivered', label: 'Order Delivered', icon: '✅' },
    { value: 'OrderCancelled', label: 'Order Cancelled', icon: '❌' },
    { value: 'PaymentFailed', label: 'Payment Failed', icon: '💰' },
    { value: 'SubscriptionRenewed', label: 'Subscription Renewed', icon: '🔄' },
    { value: 'Campaign', label: 'Campaign', icon: '📢' },
    { value: 'MonthlyNewsletter', label: 'Monthly Newsletter', icon: '📰' },
    { value: 'SupportTicketUpdated', label: 'Support Ticket Updated', icon: '🎫' },
    { value: 'TestForDevelop', label: 'Test (Developer)', icon: '🧪' },
] as const;

export type EventTypeValue = typeof EVENT_TYPES[number]['value'];
