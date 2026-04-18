import { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { templateSevice } from '../api/templateSevice';
import type { CreateOrUpdateCustom } from '../types/template';
import { getChannelMetadata } from '../constants/channels';
import { EVENT_TYPES } from '../constants/eventTypes';

const limits = { title: 100, body: 1024 };

export default function WhatsappDesignPanelPage() {
    const navigate = useNavigate();
    const location = useLocation();

    const [channelId] = useState<string>('whatsapp');
    const [title, setTitle] = useState('');
    const [message, setMessage] = useState('');
    const [showSaveModal, setShowSaveModal] = useState(false);
    const [templateName, setTemplateName] = useState('');
    const [templateDescription, setTemplateDescription] = useState('');
    const [selectedEventType, setSelectedEventType] = useState('');

    const channelInfo = getChannelMetadata('whatsapp');

    useEffect(() => {
        if (location.state?.template) {
            const { template } = location.state;
            setTitle(template.subject || '');
            setMessage(template.body || '');
            setTemplateName(template.name || '');
            setTemplateDescription(template.description || '');
            setSelectedEventType(template.eventType || template.event || '');
        }
    }, [location.state]);

    const isTitleTooLong = title.length > limits.title;
    const isMessageTooLong = message.length > limits.body;
    const showWarning = isTitleTooLong || isMessageTooLong;

    const truncateText = (text: string, maxLength: number) => {
        if (text.length <= maxLength) return text;
        return text.slice(0, maxLength) + '...';
    };

    const handleSave = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!message.includes('{{UnsubscriberLink}}')) {
            alert('Template message body must contain the {{UnsubscriberLink}} variable.');
            return;
        }
        try {
            const request: CreateOrUpdateCustom = {
                name: templateName,
                description: templateDescription,
                eventType: selectedEventType,
                channel: channelId,
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
    };

    return (
        <div className="flex flex-col h-screen overflow-hidden bg-primary text-primary">
            {/* Save Modal */}
            {showSaveModal && (
                <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/80 backdrop-blur-sm">
                    <div className="w-full max-w-md card-elevated p-6">
                        <h2 className="text-xl font-semibold text-primary mb-4">Save {channelInfo.name} Template</h2>

                        <div className="space-y-4">
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
                                    <div className="absolute inset-y-0 right-0 flex items-center pr-3 pointer-events-none text-secondary">
                                        <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
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
                            <button onClick={() => setShowSaveModal(false)} className="btn-secondary text-sm">Cancel</button>
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
                        onClick={() => navigate('/templates')}
                        className="p-2 rounded-md bg-tertiary hover:bg-elevated text-secondary hover:text-primary transition-colors"
                    >
                        <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M10 19l-7-7m0 0l7-7m-7 7h18" />
                        </svg>
                    </button>

                    <div className="flex items-center gap-3">
                        <div className="w-10 h-10 rounded-md bg-primary-500 flex items-center justify-center shadow-lg shadow-primary-500/10 text-xl">
                            {channelInfo.icon}
                        </div>
                        <div>
                            <h1 className="text-base font-semibold text-primary">{channelInfo.name} Editor</h1>
                            <p className="text-xs text-secondary tracking-tight">Create Subject & Body Template</p>
                        </div>
                    </div>
                </div>

                <button onClick={() => setShowSaveModal(true)} className="btn-primary text-sm">Save</button>
            </header>

            {/* Warning Banner */}
            {showWarning && (
                <div className="px-6 py-2 bg-yellow-500/10 border-b border-yellow-500/20 flex items-center gap-2">
                    <svg className="w-4 h-4 text-yellow-500 shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
                    </svg>
                    <p className="text-[11px] text-yellow-300">
                        <span className="font-medium">Warning:</span> Message or subject exceeds recommended length for {channelInfo.name}.
                    </p>
                </div>
            )}

            {/* Content area */}
            <div className="flex flex-1 overflow-hidden">
                {/* Editor Panel */}
                <div className="flex-1 flex flex-col border-r border-primary overflow-y-auto">
                    <div className="px-6 py-3 border-b border-primary bg-secondary/50">
                        <h2 className="text-[10px] font-bold text-secondary uppercase tracking-widest">Template Editor</h2>
                    </div>

                    <div className="p-6 space-y-6">
                        <div>
                            <div className="flex items-center justify-between mb-2">
                                <label className="block text-sm font-semibold text-primary">Subject / Title</label>
                                <span className={`text-[10px] font-mono ${isTitleTooLong ? 'text-red-500' : 'text-secondary'}`}>
                                    {title.length} / {limits.title}
                                </span>
                            </div>
                            <input
                                type="text"
                                value={title}
                                onChange={(e) => setTitle(e.target.value)}
                                placeholder="Enter subject..."
                                className={`input-modern ${isTitleTooLong ? 'border-red-500/50 focus:border-red-500' : ''}`}
                            />
                        </div>

                        <div>
                            <div className="flex items-center justify-between mb-2">
                                <label className="block text-sm font-semibold text-primary">Message Body</label>
                                <span className={`text-[10px] font-mono ${isMessageTooLong ? 'text-red-500' : 'text-secondary'}`}>
                                    {message.length} / {limits.body}
                                </span>
                            </div>
                            <textarea
                                value={message}
                                onChange={(e) => setMessage(e.target.value)}
                                placeholder="Enter message body..."
                                rows={10}
                                className={`input-modern resize-none ${isMessageTooLong ? 'border-red-500/50 focus:border-red-500' : ''}`}
                            />
                        </div>

                        <div className="card-elevated p-4 !bg-primary-500/5 !border-primary-500/10">
                            <p className="text-xs text-secondary leading-relaxed">
                                <span className="font-bold text-primary-400">Pro Tip:</span> Use <code>{'{{VariableName}}'}</code> tag to inject dynamic data from your backend. Available tags depend on the event type selected. You MUST include the <code className="text-red-400">{'{{UnsubscriberLink}}'}</code> tag in the body.
                            </p>
                        </div>
                    </div>
                </div>

                {/* Preview Panel */}
                <div className="flex-1 flex flex-col bg-[#050505] overflow-hidden relative">
                    <div className="px-6 py-3 border-b border-white/5 bg-[#0a0a0a]">
                        <h2 className="text-[10px] font-bold text-secondary uppercase tracking-widest">Mockup Preview</h2>
                    </div>

                    <div className="flex-1 flex items-center justify-center p-8">
                        {/* WhatsApp Mobile Mockup */}
                        <div className="relative w-[280px] h-[580px]">
                            <div className="absolute inset-0 bg-black rounded-[3.2rem] border-[8px] border-[#1f1f1f] shadow-2xl overflow-hidden ring-1 ring-white/20 flex flex-col">
                                {/* Notch Area */}
                                <div className="absolute top-0 left-1/2 -translate-x-1/2 w-28 h-6 bg-black rounded-b-2xl z-30"></div>

                                {/* Status Bar */}
                                <div className="h-10 flex items-center justify-between px-6 pt-2 bg-[#075e54] text-white z-20">
                                    <span className="text-[10px] font-bold">9:41</span>
                                    <div className="flex items-center gap-1 text-[10px]">
                                        <svg className="w-3 h-3" fill="currentColor" viewBox="0 0 24 24"><path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm-1 14H9v-2h2v2zm0-4H9V7h2v5z" /></svg>
                                    </div>
                                </div>

                                {/* WhatsApp Header */}
                                <div className="h-14 bg-[#075e54] flex items-center px-4 gap-3 shadow-md z-10 shrink-0">
                                    <div className="w-8 h-8 rounded-full bg-slate-300 flex items-center justify-center overflow-hidden">
                                        <div className="w-full h-full bg-white flex items-center justify-center text-sm">{channelInfo.icon}</div>
                                    </div>
                                    <div className="flex-1">
                                        <h3 className="text-white text-sm font-semibold leading-tight">Fertile Notify</h3>
                                        <p className="text-white/80 text-[10px]">Business Account</p>
                                    </div>
                                    <div className="flex gap-4 text-white">
                                        <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 5a2 2 0 012-2h3.28a1 1 0 01.948.684l1.498 4.493a1 1 0 01-.502 1.21l-2.257 1.13a11.042 11.042 0 005.516 5.516l1.13-2.257a1 1 0 011.21-.502l4.493 1.498a1 1 0 01.684.949V19a2 2 0 01-2 2h-1C9.716 21 3 14.284 3 6V5z" /></svg>
                                    </div>
                                </div>

                                {/* Chat Background */}
                                <div className="flex-1 overflow-y-auto bg-[#ece5dd] p-4 relative" style={{ backgroundImage: 'radial-gradient(#d3cbc1 1px, transparent 1px)', backgroundSize: '20px 20px' }}>

                                    {/* Date Bubble */}
                                    <div className="flex justify-center mb-4">
                                        <div className="bg-[#e1f3fb] text-[#4a4a4a] text-[10px] px-3 py-1 rounded-lg shadow-sm">
                                            TODAY
                                        </div>
                                    </div>

                                    {/* Message Bubble */}
                                    <div className="flex mb-4">
                                        <div className="bg-white rounded-lg rounded-tl-none p-2 shadow-sm text-black max-w-[85%] relative border border-gray-100">
                                            {/* Top left tail */}
                                            <svg className="absolute -left-2 top-0 w-2 h-3 text-white" fill="currentColor" viewBox="0 0 8 13">
                                                <path d="M5.188 1H0v11.156l5.188-5.188a3 3 0 00.879-2.121V3.121a3 3 0 00-.879-2.121z" />
                                            </svg>

                                            {title && (
                                                <div className="text-[12px] font-bold text-black mb-1 leading-tight break-words">
                                                    {truncateText(title, limits.title)}
                                                </div>
                                            )}
                                            <div className="text-[13px] leading-snug whitespace-pre-wrap break-words text-[#111111] pb-3">
                                                {truncateText(message || 'Your WhatsApp message will appear here...', limits.body)}
                                            </div>
                                            <div className="absolute bottom-1 right-2 text-[9px] text-gray-500 font-medium">
                                                {new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
                                            </div>
                                        </div>
                                    </div>

                                </div>

                                {/* Imposed Input Area */}
                                <div className="h-14 bg-[#f0f0f0] flex items-center px-2 gap-2 shrink-0 border-t border-gray-300">
                                    <div className="w-6 text-gray-500 text-center">+</div>
                                    <div className="flex-1 h-9 bg-white rounded-full px-4 flex items-center text-gray-400 text-xs border border-gray-200">
                                        Message
                                    </div>
                                    <div className="w-8 h-8 rounded-full bg-[#075e54] text-white flex items-center justify-center">
                                        <svg className="w-4 h-4 ml-1" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 19l9 2-9-18-9 18 9-2zm0 0v-8" /></svg>
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
