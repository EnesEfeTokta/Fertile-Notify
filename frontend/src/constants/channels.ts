export interface ChannelMetadata {
    id: string;
    name: string;
    icon: string;
    color: string;
    editorRoute: string;
}

export const NOTIFICATION_CHANNELS: ChannelMetadata[] = [
    { id: 'console', name: 'Console', icon: '🖥️', color: '#3b82f6', editorRoute: '/channel-editor' },
    { id: 'email', name: 'Email', icon: '📧', color: '#8b5cf6', editorRoute: '/email-visual-editor' },
    { id: 'sms', name: 'SMS', icon: '💬', color: '#f59e0b', editorRoute: '/channel-editor' },
    { id: 'whatsapp', name: 'WhatsApp', icon: '📱', color: '#25d366', editorRoute: '/channel-editor' },
    { id: 'telegram', name: 'Telegram', icon: '✈️', color: '#0088cc', editorRoute: '/channel-editor' },
    { id: 'discord', name: 'Discord', icon: '🎮', color: '#5865f2', editorRoute: '/channel-editor' },
    { id: 'slack', name: 'Slack', icon: '⌛', color: '#4a154b', editorRoute: '/channel-editor' },
    { id: 'msteams', name: 'MS Teams', icon: '👥', color: '#6264a7', editorRoute: '/channel-editor' },
    { id: 'firebasepush', name: 'Firebase Push', icon: '🔥', color: '#ffca28', editorRoute: '/channel-editor' },
    { id: 'webpush', name: 'Web Push', icon: '🌐', color: '#ff6b6b', editorRoute: '/channel-editor' },
    { id: 'webhook', name: 'Webhook', icon: '🔗', color: '#10b981', editorRoute: '/channel-editor' },
];

export const getChannelMetadata = (id: string): ChannelMetadata => {
    return NOTIFICATION_CHANNELS.find(c => c.id === id.toLowerCase()) || {
        id: id.toLowerCase(),
        name: id.charAt(0).toUpperCase() + id.slice(1),
        icon: '🔔',
        color: '#6b7280',
        editorRoute: '/channel-editor'
    };
};
