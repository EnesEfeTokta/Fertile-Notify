import { useState } from 'react';
import { useNavigate } from 'react-router-dom';

// System-defined event types
const EVENT_TYPES = [
    { value: 'SubscriberRegistered', label: 'Subscriber Registered', icon: '👋' },
    { value: 'PasswordReset', label: 'Password Reset', icon: '🔐' },
    { value: 'EmailVerified', label: 'Email Verified', icon: '🛒' },
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
];

// SMS length limits (typical for notifications)
const MAX_TITLE_LENGTH = 40;
const MAX_MESSAGE_LENGTH = 160;

export default function SmsDesignPanelPage() {
    const [title, setTitle] = useState('');
    const [message, setMessage] = useState('');
    const [showSaveModal, setShowSaveModal] = useState(false);
    const [templateName, setTemplateName] = useState('');
    const [templateDescription, setTemplateDescription] = useState('');
    const [selectedEventType, setSelectedEventType] = useState('');
    const navigate = useNavigate();

    const isTitleTooLong = title.length > MAX_TITLE_LENGTH;
    const isMessageTooLong = message.length > MAX_MESSAGE_LENGTH;
    const showWarning = isTitleTooLong || isMessageTooLong;

    const truncateText = (text: string, maxLength: number) => {
        if (text.length <= maxLength) return text;
        return text.slice(0, maxLength) + '...';
    };

    const handleSave = () => {
        console.log('Saving SMS template:', {
            templateName,
            templateDescription,
            selectedEventType,
            title,
            message,
        });
        // TODO: API call to save template
        setShowSaveModal(false);
        setTemplateName('');
        setTemplateDescription('');
        setSelectedEventType('');
    };

    return (
        <div className="flex flex-col h-screen overflow-hidden bg-gradient-to-br from-gray-900 via-purple-900 to-gray-900 text-white">
            {/* Save Modal */}
            {showSaveModal && (
                <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/70 backdrop-blur-sm">
                    <div className="w-full max-w-md bg-gray-900 border border-purple-500/30 rounded-xl shadow-2xl p-6">
                        <h2 className="text-xl font-bold text-white mb-4">Save SMS Template</h2>

                        <div className="space-y-4">
                            {/* Event Type Selection */}
                            <div>
                                <label className="block text-sm text-gray-400 mb-2">Event Type</label>
                                <div className="relative">
                                    <select
                                        value={selectedEventType}
                                        onChange={(e) => setSelectedEventType(e.target.value)}
                                        className="w-full px-4 py-2.5 bg-gray-800 border border-gray-700 rounded-lg text-white appearance-none focus:outline-none focus:border-purple-500 cursor-pointer"
                                    >
                                        <option value="" disabled>Select an event type...</option>
                                        {EVENT_TYPES.map((event) => (
                                            <option key={event.value} value={event.value}>
                                                {event.icon} {event.label}
                                            </option>
                                        ))}
                                    </select>
                                    <div className="absolute inset-y-0 right-0 flex items-center pr-3 pointer-events-none">
                                        <svg className="w-5 h-5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7" />
                                        </svg>
                                    </div>
                                </div>
                            </div>

                            <div>
                                <label className="block text-sm text-gray-400 mb-1">Template Name</label>
                                <input
                                    type="text"
                                    value={templateName}
                                    onChange={(e) => setTemplateName(e.target.value)}
                                    placeholder="Enter template name..."
                                    className="w-full px-4 py-2 bg-gray-800 border border-gray-700 rounded-lg text-white placeholder-gray-500 focus:outline-none focus:border-purple-500"
                                />
                            </div>

                            <div>
                                <label className="block text-sm text-gray-400 mb-1">Description</label>
                                <textarea
                                    value={templateDescription}
                                    onChange={(e) => setTemplateDescription(e.target.value)}
                                    placeholder="Enter description..."
                                    rows={2}
                                    className="w-full px-4 py-2 bg-gray-800 border border-gray-700 rounded-lg text-white placeholder-gray-500 focus:outline-none focus:border-purple-500 resize-none"
                                />
                            </div>
                        </div>

                        <div className="flex justify-end gap-3 mt-6">
                            <button
                                onClick={() => setShowSaveModal(false)}
                                className="px-4 py-2 text-sm text-gray-400 hover:text-white transition-colors"
                            >
                                Cancel
                            </button>
                            <button
                                onClick={handleSave}
                                disabled={!templateName.trim() || !selectedEventType}
                                className="px-6 py-2 bg-gradient-to-r from-purple-600 to-pink-600 text-white rounded-lg font-medium disabled:opacity-50 disabled:cursor-not-allowed"
                            >
                                Save
                            </button>
                        </div>
                    </div>
                </div>
            )}

            {/* Header */}
            <header className="flex items-center justify-between px-6 py-4 bg-gray-900/80 backdrop-blur-md border-b border-purple-500/30">
                <div className="flex items-center gap-4">
                    <button
                        onClick={() => navigate('/dashboard')}
                        className="p-2 rounded-lg bg-gray-800 hover:bg-gray-700 text-gray-400 hover:text-white transition-colors"
                    >
                        <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M10 19l-7-7m0 0l7-7m-7 7h18" />
                        </svg>
                    </button>

                    <div className="flex items-center gap-3">
                        <div className="w-10 h-10 rounded-lg bg-gradient-to-br from-green-500 to-emerald-600 flex items-center justify-center">
                            <svg className="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 10h.01M12 10h.01M16 10h.01M9 16H5a2 2 0 01-2-2V6a2 2 0 012-2h14a2 2 0 012 2v8a2 2 0 01-2 2h-5l-5 5v-5z" />
                            </svg>
                        </div>
                        <div>
                            <h1 className="text-lg font-bold text-white">SMS Designer</h1>
                            <p className="text-xs text-gray-400">Create SMS Templates</p>
                        </div>
                    </div>
                </div>

                <button
                    onClick={() => setShowSaveModal(true)}
                    className="px-5 py-2 bg-gradient-to-r from-purple-600 to-pink-600 text-white rounded-lg font-medium text-sm hover:opacity-90 transition-opacity"
                >
                    Save
                </button>
            </header>

            {/* Info Banner */}
            <div className="px-6 py-2 bg-purple-900/20 border-b border-purple-500/20 flex items-center gap-2">
                <svg className="w-4 h-4 text-purple-400 shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
                <p className="text-xs text-purple-300">
                    <span className="font-medium">Tip:</span> Use <code className="px-1 py-0.5 bg-purple-500/20 rounded text-purple-200">{'{Variable}'}</code> syntax for dynamic data. Example: <code className="px-1 py-0.5 bg-purple-500/20 rounded text-purple-200">{'{Name}'}</code>, <code className="px-1 py-0.5 bg-purple-500/20 rounded text-purple-200">{'{Code}'}</code>
                </p>
            </div>

            {/* Warning Banner */}
            {showWarning && (
                <div className="px-6 py-3 bg-yellow-900/20 border-b border-yellow-500/30 flex items-center gap-2">
                    <svg className="w-5 h-5 text-yellow-400 shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
                    </svg>
                    <p className="text-sm text-yellow-300">
                        <span className="font-medium">Uyarı:</span> Mesaj içeriği ve başlığı ortalama bir mesajdan çok uzun oldu. SMS'i alan kişiler tamamını göremeyebilir.
                    </p>
                </div>
            )}

            {/* Main Content */}
            <div className="flex flex-1 overflow-hidden">
                {/* Left Panel - SMS Form */}
                <div className="flex-1 flex flex-col border-r border-gray-800 overflow-y-auto bg-gray-900/50">
                    <div className="px-6 py-3 border-b border-gray-800 bg-gray-800/50">
                        <h2 className="text-xs font-medium text-gray-300 uppercase tracking-wide">SMS Content</h2>
                    </div>

                    <div className="flex-1 p-6 space-y-6">
                        {/* Title Input */}
                        <div>
                            <div className="flex items-center justify-between mb-2">
                                <label className="block text-sm font-medium text-gray-300">Title</label>
                                <span className={`text-xs ${isTitleTooLong ? 'text-yellow-400 font-medium' : 'text-gray-500'}`}>
                                    {title.length} / {MAX_TITLE_LENGTH}
                                </span>
                            </div>
                            <input
                                type="text"
                                value={title}
                                onChange={(e) => setTitle(e.target.value)}
                                placeholder="Enter SMS title..."
                                className={`w-full px-4 py-3 bg-gray-800 border rounded-lg text-white placeholder-gray-500 focus:outline-none transition-colors ${isTitleTooLong ? 'border-yellow-500 focus:border-yellow-400' : 'border-gray-700 focus:border-purple-500'
                                    }`}
                            />
                            {isTitleTooLong && (
                                <p className="mt-1 text-xs text-yellow-400">
                                    Title exceeds recommended length. It may be truncated in the notification.
                                </p>
                            )}
                        </div>

                        {/* Message Textarea */}
                        <div>
                            <div className="flex items-center justify-between mb-2">
                                <label className="block text-sm font-medium text-gray-300">Message</label>
                                <span className={`text-xs ${isMessageTooLong ? 'text-yellow-400 font-medium' : 'text-gray-500'}`}>
                                    {message.length} / {MAX_MESSAGE_LENGTH}
                                </span>
                            </div>
                            <textarea
                                value={message}
                                onChange={(e) => setMessage(e.target.value)}
                                placeholder="Enter your SMS message here... Use {Variable} for dynamic content."
                                rows={10}
                                className={`w-full px-4 py-3 bg-gray-800 border rounded-lg text-white placeholder-gray-500 focus:outline-none resize-none transition-colors ${isMessageTooLong ? 'border-yellow-500 focus:border-yellow-400' : 'border-gray-700 focus:border-purple-500'
                                    }`}
                            />
                            {isMessageTooLong && (
                                <p className="mt-1 text-xs text-yellow-400">
                                    Message exceeds typical SMS length. Recipients may not see the full content.
                                </p>
                            )}
                        </div>

                        {/* Info Card */}
                        <div className="bg-gray-800/50 border border-gray-700 rounded-lg p-4">
                            <h3 className="text-sm font-medium text-gray-300 mb-2">💡 Best Practices</h3>
                            <ul className="space-y-1 text-xs text-gray-400">
                                <li>• Keep titles under {MAX_TITLE_LENGTH} characters</li>
                                <li>• Keep messages under {MAX_MESSAGE_LENGTH} characters</li>
                                <li>• Use clear and concise language</li>
                                <li>• Include a call-to-action when needed</li>
                                <li>• Test with variables before sending</li>
                            </ul>
                        </div>
                    </div>
                </div>

                {/* Right Panel - Phone Preview */}
                <div className="flex-1 flex flex-col bg-gradient-to-br from-gray-800 via-gray-900 to-black">
                    <div className="px-6 py-3 border-b border-gray-700 bg-gray-800/30">
                        <h2 className="text-xs font-medium text-gray-300 uppercase tracking-wide">Preview</h2>
                    </div>

                    <div className="flex-1 flex items-center justify-center p-6">
                        {/* Phone Mockup */}
                        <div className="relative w-80 h-[640px]">
                            {/* Phone Frame */}
                            <div className="absolute inset-0 bg-gradient-to-b from-gray-700 to-gray-800 rounded-[3rem] border-[12px] border-gray-600 shadow-2xl overflow-hidden">
                                {/* Phone Notch */}
                                <div className="absolute top-0 left-1/2 -translate-x-1/2 w-40 h-7 bg-gray-900 rounded-b-3xl z-10 shadow-lg"></div>

                                {/* Phone Screen */}
                                <div className="h-full bg-gradient-to-br from-sky-100 via-indigo-50 to-purple-100 overflow-hidden">
                                    {/* Status Bar */}
                                    <div className="h-12 bg-gray-900 flex items-center justify-between px-6 pt-2">
                                        <span className="text-white text-xs font-medium">9:41</span>
                                        <div className="flex items-center gap-1">
                                            <svg className="w-3 h-3 text-white" fill="currentColor" viewBox="0 0 20 20">
                                                <path d="M2 11a1 1 0 011-1h2a1 1 0 011 1v5a1 1 0 01-1 1H3a1 1 0 01-1-1v-5zM8 7a1 1 0 011-1h2a1 1 0 011 1v9a1 1 0 01-1 1H9a1 1 0 01-1-1V7zM14 4a1 1 0 011-1h2a1 1 0 011 1v12a1 1 0 01-1 1h-2a1 1 0 01-1-1V4z" />
                                            </svg>
                                            <svg className="w-4 h-4 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M20.618 5.984A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z" />
                                            </svg>
                                            <div className="w-6 h-3 border-2 border-white rounded-sm relative">
                                                <div className="absolute inset-0.5 bg-white rounded-sm"></div>
                                            </div>
                                        </div>
                                    </div>

                                    {/* Notification */}
                                    <div className="mt-4 mx-3">
                                        <div className="bg-white/95 backdrop-blur-xl rounded-2xl shadow-2xl overflow-hidden border border-gray-100">
                                            {/* App Header */}
                                            <div className="flex items-center gap-3 px-4 py-3 bg-gradient-to-r from-white/90 to-white/70 border-b border-gray-100">
                                                <div className="w-8 h-8 rounded-full bg-gradient-to-br from-purple-500 to-pink-500 flex items-center justify-center shrink-0 shadow-md">
                                                    <span className="text-white text-sm font-bold">A</span>
                                                </div>
                                                <div className="flex-1 min-w-0">
                                                    <p className="text-xs font-semibold text-gray-900">Your App Name</p>
                                                    <p className="text-xs text-gray-500">now</p>
                                                </div>
                                            </div>

                                            {/* SMS Content */}
                                            <div className="px-4 py-3 bg-white">
                                                {title ? (
                                                    <h3 className="text-sm font-semibold text-gray-900 mb-1">
                                                        {truncateText(title, MAX_TITLE_LENGTH)}
                                                    </h3>
                                                ) : (
                                                    <h3 className="text-sm font-semibold text-gray-400 mb-1">SMS Title</h3>
                                                )}
                                                {message ? (
                                                    <p className="text-sm text-gray-700 whitespace-pre-wrap break-words leading-relaxed">
                                                        {truncateText(message, MAX_MESSAGE_LENGTH)}
                                                    </p>
                                                ) : (
                                                    <p className="text-sm text-gray-400">Your message will appear here...</p>
                                                )}
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}
