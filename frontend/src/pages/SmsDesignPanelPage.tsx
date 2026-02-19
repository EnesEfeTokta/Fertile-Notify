import { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { templateSevice } from '../api/templateSevice';
import type { CreateOrUpdateCustom } from '../types/template';

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
    const navigate = useNavigate();
    const location = useLocation();
    const [title, setTitle] = useState('');
    const [message, setMessage] = useState('');
    const [showSaveModal, setShowSaveModal] = useState(false);
    const [templateName, setTemplateName] = useState('');
    const [templateDescription, setTemplateDescription] = useState('');
    const [selectedEventType, setSelectedEventType] = useState('');

    useEffect(() => {
        if (location.state && location.state.template) {
            const { template } = location.state;
            setTitle(template.subject || ''); // Using subject as title for SMS
            setMessage(template.body || '');  // Using body as message
            setTemplateName(template.name || '');
            setTemplateDescription(template.description || '');
            setSelectedEventType(template.eventType || '');
        }
    }, [location.state]);

    const isTitleTooLong = title.length > MAX_TITLE_LENGTH;
    const isMessageTooLong = message.length > MAX_MESSAGE_LENGTH;
    const showWarning = isTitleTooLong || isMessageTooLong;

    const truncateText = (text: string, maxLength: number) => {
        if (text.length <= maxLength) return text;
        return text.slice(0, maxLength) + '...';
    };

    const handleSave = async (e: React.FormEvent) => {
        e.preventDefault();
        try {
            const request: CreateOrUpdateCustom = {
                name: templateName,
                description: templateDescription,
                eventType: selectedEventType,
                channel: 'sms',
                subjectTemplate: title,
                bodyTemplate: message
            };
            await templateSevice.createOrUpdateTemplate(request);
            navigate("/templates");
        } catch (error) {
            alert("Template registration failed. Please try again.");
            console.error("Template registration error:", error);
        }
        setShowSaveModal(false);
        setTemplateName('');
        setTemplateDescription('');
        setSelectedEventType('');
    };

    return (
        <div className="flex flex-col h-screen overflow-hidden bg-primary text-primary">
            {/* Save Modal */}
            {showSaveModal && (
                <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/80 backdrop-blur-sm">
                    <div className="w-full max-w-md card-elevated p-6">
                        <h2 className="text-xl font-semibold text-primary mb-4">Save SMS Template</h2>

                        <div className="space-y-4">
                            {/* Event Type Selection */}
                            <div>
                                <label className="block text-sm text-secondary mb-2">Event Type</label>
                                <div className="relative">
                                    <select
                                        value={selectedEventType}
                                        onChange={(e) => setSelectedEventType(e.target.value)}
                                        className="input-modern appearance-none cursor-pointer"
                                    >
                                        <option value="" disabled>Select an event type...</option>
                                        {EVENT_TYPES.map((event) => (
                                            <option key={event.value} value={event.value}>
                                                {event.icon} {event.label}
                                            </option>
                                        ))}
                                    </select>
                                    <div className="absolute inset-y-0 right-0 flex items-center pr-3 pointer-events-none">
                                        <svg className="w-4 h-4 text-secondary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7" />
                                        </svg>
                                    </div>
                                </div>
                            </div>

                            <div>
                                <label className="block text-sm text-secondary mb-2">Template Name</label>
                                <input
                                    type="text"
                                    value={templateName}
                                    onChange={(e) => setTemplateName(e.target.value)}
                                    placeholder="Enter template name..."
                                    className="input-modern"
                                />
                            </div>

                            <div>
                                <label className="block text-sm text-secondary mb-2">Description</label>
                                <textarea
                                    value={templateDescription}
                                    onChange={(e) => setTemplateDescription(e.target.value)}
                                    placeholder="Enter description..."
                                    rows={2}
                                    className="input-modern resize-none"
                                />
                            </div>
                        </div>

                        <div className="flex justify-end gap-3 mt-6">
                            <button
                                onClick={() => setShowSaveModal(false)}
                                className="btn-secondary text-sm"
                            >
                                Cancel
                            </button>
                            <button
                                onClick={handleSave}
                                disabled={!templateName.trim() || !selectedEventType}
                                className="btn-primary text-sm disabled:opacity-50"
                            >
                                Save
                            </button>
                        </div>
                    </div>
                </div>
            )}

            {/* Header */}
            <header className="flex items-center justify-between px-6 py-4 bg-secondary border-b border-primary">
                <div className="flex items-center gap-4">
                    <button
                        onClick={() => navigate('/dashboard')}
                        className="p-2 rounded-md bg-tertiary hover:bg-elevated text-secondary hover:text-primary transition-colors"
                    >
                        <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M10 19l-7-7m0 0l7-7m-7 7h18" />
                        </svg>
                    </button>

                    <div className="flex items-center gap-3">
                        <div className="w-10 h-10 rounded-md bg-primary-500 flex items-center justify-center shadow-lg shadow-primary-500/10">
                            <svg className="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 10h.01M12 10h.01M16 10h.01M9 16H5a2 2 0 01-2-2V6a2 2 0 012-2h14a2 2 0 012 2v8a2 2 0 01-2 2h-5l-5 5v-5z" />
                            </svg>
                        </div>
                        <div>
                            <h1 className="text-base font-semibold text-primary">SMS Designer</h1>
                            <p className="text-xs text-secondary tracking-tight">Create Professional SMS Templates</p>
                        </div>
                    </div>
                </div>

                <button
                    onClick={() => setShowSaveModal(true)}
                    className="btn-primary text-sm"
                >
                    Save
                </button>
            </header>

            {/* Info Banner */}
            <div className="px-6 py-2 bg-primary-500/5 border-b border-primary-500/10 flex items-center gap-2">
                <svg className="w-4 h-4 text-primary-400 shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
                <p className="text-xs text-secondary">
                    <span className="font-medium">Tip:</span> Use <code className="px-1 py-0.5 bg-tertiary rounded text-primary-400">{'{Variable}'}</code> syntax for dynamic data. Example: <code className="px-1 py-0.5 bg-tertiary rounded text-primary-400">{'{Name}'}</code>, <code className="px-1 py-0.5 bg-tertiary rounded text-primary-400">{'{Code}'}</code>
                </p>
            </div>

            {/* Warning Banner */}
            {showWarning && (
                <div className="px-6 py-3 bg-yellow-500/10 border-b border-yellow-500/20 flex items-center gap-2">
                    <svg className="w-5 h-5 text-yellow-500 shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
                    </svg>
                    <p className="text-sm text-yellow-300">
                        <span className="font-medium">Warning:</span> Message content or title is significantly longer than average. Some recipients might not see the full content.
                    </p>
                </div>
            )}

            {/* Main Content */}
            <div className="flex flex-1 overflow-hidden">
                {/* Left Panel - SMS Form */}
                <div className="flex-1 flex flex-col border-r border-[#262626] overflow-y-auto bg-primary">
                    <div className="px-6 py-3 border-b border-[#262626] bg-[#0a0a0a]">
                        <h2 className="text-[10px] font-bold text-secondary uppercase tracking-widest">SMS Content</h2>
                    </div>

                    <div className="flex-1 p-8 space-y-8">
                        {/* Title Input */}
                        <div>
                            <div className="flex items-center justify-between mb-3">
                                <label className="block text-sm font-semibold text-primary">Title</label>
                                <span className={`text-[10px] font-mono ${isTitleTooLong ? 'text-yellow-500 font-bold' : 'text-secondary'}`}>
                                    {title.length} / {MAX_TITLE_LENGTH}
                                </span>
                            </div>
                            <input
                                type="text"
                                value={title}
                                onChange={(e) => setTitle(e.target.value)}
                                placeholder="Enter SMS title..."
                                className={`input-modern !bg-[#050505] !border-[#333] !py-3 ${isTitleTooLong ? 'border-yellow-500/50 focus:border-yellow-500' : 'focus:border-primary-500'}`}
                            />
                            {isTitleTooLong && (
                                <p className="mt-2 text-xs text-yellow-500/80">
                                    Title exceeds recommended length. It may be truncated in the notification.
                                </p>
                            )}
                        </div>

                        {/* Message Textarea */}
                        <div>
                            <div className="flex items-center justify-between mb-3">
                                <label className="block text-sm font-semibold text-primary">Message</label>
                                <span className={`text-[10px] font-mono ${isMessageTooLong ? 'text-yellow-500 font-bold' : 'text-secondary'}`}>
                                    {message.length} / {MAX_MESSAGE_LENGTH}
                                </span>
                            </div>
                            <textarea
                                value={message}
                                onChange={(e) => setMessage(e.target.value)}
                                placeholder="Enter your SMS message here... Use {Variable} for dynamic content."
                                rows={8}
                                className={`input-modern !bg-[#050505] !border-[#333] !py-4 resize-none ${isMessageTooLong ? 'border-yellow-500/50 focus:border-yellow-500' : 'focus:border-primary-500'}`}
                            />
                            {isMessageTooLong && (
                                <p className="mt-2 text-xs text-yellow-500/80">
                                    Message exceeds typical SMS length. Recipients may not see the full content.
                                </p>
                            )}
                        </div>

                        {/* Info Card */}
                        <div className="card-elevated p-6 !border-primary-500/20 !bg-primary-500/[0.03]">
                            <h3 className="text-xs font-bold text-primary-400 mb-4 uppercase tracking-widest">💡 Best Practices</h3>
                            <ul className="space-y-3 text-xs text-secondary">
                                <li className="flex gap-3">
                                    <span className="text-primary-500">•</span>
                                    <span>Keep titles under <strong className="text-primary">{MAX_TITLE_LENGTH}</strong> characters for better visibility.</span>
                                </li>
                                <li className="flex gap-3">
                                    <span className="text-primary-500">•</span>
                                    <span>Keep messages under <strong className="text-primary">{MAX_MESSAGE_LENGTH}</strong> characters to avoid multi-part SMS.</span>
                                </li>
                                <li className="flex gap-3">
                                    <span className="text-primary-500">•</span>
                                    <span>Use clear, action-oriented language to improve engagement.</span>
                                </li>
                            </ul>
                        </div>
                    </div>
                </div>

                {/* Right Panel - Phone Preview */}
                <div className="flex-1 flex flex-col bg-[#050505] overflow-hidden relative">
                    {/* Background Pattern */}
                    <div className="absolute inset-0 opacity-[0.05] pointer-events-none" style={{ backgroundImage: 'radial-gradient(#ffffff 1px, transparent 1px)', backgroundSize: '32px 32px' }}></div>
                    <div className="absolute inset-0 bg-gradient-to-b from-transparent via-primary-500/[0.02] to-primary-500/[0.05]"></div>

                    <div className="px-6 py-3 border-b border-[#262626] bg-[#0a0a0a]/80 backdrop-blur-sm relative z-10">
                        <h2 className="text-[10px] font-bold text-secondary uppercase tracking-widest">Live Preview</h2>
                    </div>

                    <div className="flex-1 flex items-center justify-center p-8 relative z-10">
                        {/* Phone Mockup */}
                        <div className="relative w-[280px] h-[580px] animate-slide-up group">
                            {/* Outer Frame Highlight (Glow) */}
                            <div className="absolute -inset-1 bg-gradient-to-b from-white/10 to-transparent rounded-[3.5rem] blur-sm opacity-50 group-hover:opacity-100 transition-opacity"></div>

                            {/* Phone Frame */}
                            <div className="absolute inset-0 bg-[#000] rounded-[3.2rem] border-[8px] border-[#1f1f1f] shadow-[0_0_40px_-10px_rgba(0,0,0,0.8)] overflow-hidden ring-1 ring-white/20">
                                {/* Phone Notch */}
                                <div className="absolute top-0 left-1/2 -translate-x-1/2 w-28 h-6 bg-[#000] rounded-b-2xl z-20"></div>

                                {/* Phone Screen */}
                                <div className="h-full bg-black overflow-hidden flex flex-col">
                                    {/* Status Bar */}
                                    <div className="h-10 flex items-center justify-between px-8 pt-3">
                                        <span className="text-white text-[10px] font-bold">9:41</span>
                                        <div className="flex items-center gap-1.5">
                                            <div className="flex gap-0.5">
                                                <div className="w-0.5 h-1.5 bg-white rounded-full"></div>
                                                <div className="w-0.5 h-2 bg-white rounded-full"></div>
                                                <div className="w-0.5 h-2.5 bg-white rounded-full"></div>
                                                <div className="w-0.5 h-2.5 bg-white/30 rounded-full"></div>
                                            </div>
                                            <div className="w-4 h-2 rounded-[2px] border border-white/40 relative">
                                                <div className="absolute inset-[1px] bg-white rounded-[1px] w-[80%]"></div>
                                            </div>
                                        </div>
                                    </div>

                                    {/* App Notification Container */}
                                    <div className="mt-6 mx-3">
                                        <div className="bg-[#1c1c1e] rounded-2xl overflow-hidden shadow-2xl border border-white/10">
                                            {/* App Header */}
                                            <div className="flex items-center gap-3 px-4 py-3 bg-white/[0.03] border-b border-white/5">
                                                <div className="w-8 h-8 rounded-lg bg-primary-500 flex items-center justify-center shrink-0 shadow-lg shadow-primary-500/20">
                                                    <svg className="w-4 h-4 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 10h.01M12 10h.01M16 10h.01M9 16H5a2 2 0 01-2-2V6a2 2 0 012-2h14a2 2 0 012 2v8a2 2 0 01-2 2h-5l-5 5v-5z" />
                                                    </svg>
                                                </div>
                                                <div className="flex-1 min-w-0">
                                                    <div className="flex items-center justify-between">
                                                        <p className="text-[11px] font-bold text-white tracking-tight">Fertile-Notify</p>
                                                        <p className="text-[9px] text-white/40 uppercase font-bold tracking-wider">now</p>
                                                    </div>
                                                </div>
                                            </div>

                                            {/* SMS Content */}
                                            <div className="px-4 py-4">
                                                {title ? (
                                                    <h3 className="text-[13px] font-bold text-white mb-2 leading-tight">
                                                        {truncateText(title, MAX_TITLE_LENGTH)}
                                                    </h3>
                                                ) : (
                                                    <h3 className="text-[13px] font-bold text-white/20 mb-2">SMS Title</h3>
                                                )}
                                                {message ? (
                                                    <p className="text-[13px] text-white/80 whitespace-pre-wrap break-words leading-snug tracking-tight">
                                                        {truncateText(message, MAX_MESSAGE_LENGTH)}
                                                    </p>
                                                ) : (
                                                    <p className="text-[13px] text-white/20 italic">Your message will appear here...</p>
                                                )}
                                            </div>
                                        </div>
                                    </div>

                                    {/* Simple Home Indicator */}
                                    <div className="mt-auto mb-2 mx-auto w-32 h-1 bg-white/10 rounded-full"></div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}
