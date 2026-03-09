import React, { useState, useEffect } from 'react';
import { subscriberService } from '../api/subscriberService';
import { getChannelMetadata } from '../constants/channels';

interface ChannelSettingsModalProps {
    isOpen: boolean;
    onClose: () => void;
    channelId: string;
}

const CONFIG_FIELDS: Record<string, { label: string; key: string; type: 'text' | 'password' | 'textarea' }[]> = {
    telegram: [
        { label: 'Bot Token', key: 'Telegram_BotToken', type: 'password' },
    ],
    discord: [
        { label: 'Webhook URL', key: 'Discord_WebhookUrl', type: 'text' },
    ],
    whatsapp: [
        { label: 'Twilio SID', key: 'WhatsApp_TwilioSid', type: 'text' },
        { label: 'Twilio Token', key: 'WhatsApp_TwilioToken', type: 'password' },
        { label: 'Twilio From Number', key: 'WhatsApp_TwilioFrom', type: 'text' },
    ],
    slack: [
        { label: 'Access Token', key: 'Slack_AccessToken', type: 'password' },
    ],
    msteams: [
        { label: 'Webhook URL', key: 'MSTeams_WebhookUrl', type: 'text' },
    ],
    firebasepush: [
        { label: 'Service Account JSON', key: 'Firebase_ServiceAccountJson', type: 'textarea' },
    ],
    webpush: [
        { label: 'VAPID Public Key', key: 'WebPush_VapidPublicKey', type: 'text' },
        { label: 'VAPID Private Key', key: 'WebPush_VapidPrivateKey', type: 'password' },
        { label: 'Owner Email', key: 'WebPush_OwnerEmail', type: 'text' },
        { label: 'Icon URL', key: 'WebPush_Icon', type: 'text' },
        { label: 'Web URL', key: 'WebPush_WebUrl', type: 'text' },
    ],
    webhook: [
        { label: 'Secret', key: 'Webhook_Secret', type: 'password' },
    ],
    email: [
        { label: 'SMTP Host', key: 'SMTP_Host', type: 'text' },
        { label: 'SMTP Port', key: 'SMTP_Port', type: 'text' },
        { label: 'SMTP Email', key: 'SMTP_Email', type: 'text' },
        { label: 'SMTP Password', key: 'SMTP_Password', type: 'password' },
        { label: 'Sender Name', key: 'SMTP_OwnerName', type: 'text' },
    ],
};

export default function ChannelSettingsModal({ isOpen, onClose, channelId }: ChannelSettingsModalProps) {
    const [settings, setSettings] = useState<Record<string, string>>({});
    const [loading, setLoading] = useState(false);
    const [saving, setSaving] = useState(false);
    const channelInfo = getChannelMetadata(channelId);
    const fields = CONFIG_FIELDS[channelId.toLowerCase()] || [];

    useEffect(() => {
        if (isOpen && channelId) {
            fetchSettings();
        }
    }, [isOpen, channelId]);

    const fetchSettings = async () => {
        try {
            setLoading(true);
            const data = await subscriberService.getChannelSetting(channelId);
            setSettings(data.settings || {});
        } catch (err) {
            console.error('Error fetching settings:', err);
            // If 404, we just have empty settings
            setSettings({});
        } finally {
            setLoading(false);
        }
    };

    const handleSave = async (e: React.FormEvent) => {
        e.preventDefault();
        try {
            setSaving(true);
            await subscriberService.setChannelSetting({
                channel: channelId,
                settings: settings
            });
            alert('Settings saved successfully!');
            onClose();
        } catch (err) {
            console.error('Error saving settings:', err);
            alert('Failed to save settings.');
        } finally {
            setSaving(false);
        }
    };

    if (!isOpen) return null;

    return (
        <div className="fixed inset-0 bg-black/80 backdrop-blur-sm flex items-center justify-center z-[100] px-4">
            <div className="card-elevated max-w-lg w-full p-8 animate-slide-up">
                <div className="flex items-center gap-4 mb-6">
                    <div className="w-12 h-12 rounded-xl bg-primary-500/10 flex items-center justify-center text-2xl">
                        {channelInfo.icon}
                    </div>
                    <div>
                        <h2 className="text-xl font-display font-semibold text-primary">
                            {channelInfo.name} Configuration
                        </h2>
                        <p className="text-sm text-secondary">Set up your credentials</p>
                    </div>
                </div>

                {loading ? (
                    <div className="py-12 flex flex-col items-center justify-center gap-3">
                        <div className="spinner h-8 w-8"></div>
                        <p className="text-sm text-secondary">Loading settings...</p>
                    </div>
                ) : (
                    <form onSubmit={handleSave} className="space-y-4">
                        {fields.length > 0 ? (
                            fields.map((field) => (
                                <div key={field.key}>
                                    <label className="block text-xs font-semibold text-secondary uppercase tracking-wider mb-2">
                                        {field.label}
                                    </label>
                                    {field.type === 'textarea' ? (
                                        <textarea
                                            value={settings[field.key] || ''}
                                            onChange={(e) => setSettings({ ...settings, [field.key]: e.target.value })}
                                            className="input-modern w-full h-32 font-mono text-xs"
                                            placeholder={`Paste ${field.label} here...`}
                                        />
                                    ) : (
                                        <input
                                            type={field.type}
                                            value={settings[field.key] || ''}
                                            onChange={(e) => setSettings({ ...settings, [field.key]: e.target.value })}
                                            className="input-modern w-full"
                                            placeholder={`Enter ${field.label}`}
                                        />
                                    )}
                                </div>
                            ))
                        ) : (
                            <div className="py-8 text-center bg-tertiary rounded-lg border border-dashed border-secondary">
                                <p className="text-sm text-secondary">
                                    No configurable settings for this channel.
                                </p>
                            </div>
                        )}

                        <div className="flex gap-3 pt-6 border-t border-primary">
                            <button
                                type="button"
                                onClick={onClose}
                                className="flex-1 btn-secondary py-2.5"
                                disabled={saving}
                            >
                                Cancel
                            </button>
                            <button
                                type="submit"
                                className="flex-1 btn-primary py-2.5 flex items-center justify-center gap-2"
                                disabled={saving || fields.length === 0}
                            >
                                {saving ? <><div className="spinner h-4 w-4 border-2"></div> Saving...</> : 'Save Configuration'}
                            </button>
                        </div>
                    </form>
                )}
            </div>
        </div>
    );
}
