import { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { templateSevice } from '../api/templateSevice';
import type { CreateOrUpdateCustom } from '../types/template';
import { getChannelMetadata } from '../constants/channels';
import { EVENT_TYPES } from '../constants/eventTypes';

// Console Limits
const limits = { title: 100, body: 2000 };

export default function ConsoleDesignPanelPage() {
    const navigate = useNavigate();
    const location = useLocation();

    // State initialization from location state (for new or existing templates)
    const [channelId] = useState<string>('console');
    const [title, setTitle] = useState('');
    const [message, setMessage] = useState('');
    const [showSaveModal, setShowSaveModal] = useState(false);
    const [templateName, setTemplateName] = useState('');
    const [templateDescription, setTemplateDescription] = useState('');
    const [selectedEventType, setSelectedEventType] = useState('');

    const channelInfo = getChannelMetadata('console');

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

                <div className="flex items-center gap-4">
                    <span className="px-3 py-1 rounded-full bg-blue-500/10 text-blue-400 text-[10px] font-bold uppercase tracking-widest border border-blue-500/20">Development / Testing</span>
                    <button onClick={() => setShowSaveModal(true)} className="btn-primary text-sm">Save</button>
                </div>
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
                        {/* Terminal Mockup */}
                        <div className="relative w-full max-w-2xl h-[500px]">
                            <div className="absolute inset-0 bg-[#0d0d0d] rounded-xl border border-white/10 shadow-2xl overflow-hidden font-mono flex flex-col">
                                {/* Terminal Header */}
                                <div className="h-10 bg-[#1f1f1f] flex items-center px-4 gap-3 border-b border-white/5">
                                    <div className="flex gap-1.5">
                                        <div className="w-3 h-3 rounded-full bg-red-500/80"></div>
                                        <div className="w-3 h-3 rounded-full bg-yellow-500/80"></div>
                                        <div className="w-3 h-3 rounded-full bg-green-500/80"></div>
                                    </div>
                                    <div className="flex-1 text-center text-xs text-white/40 font-sans tracking-wider flex items-center justify-center gap-2">
                                        {channelInfo.icon} bash ~ {channelInfo.name.toLowerCase()}-daemon
                                    </div>
                                </div>
                                {/* Terminal Body */}
                                <div className="p-6 overflow-y-auto text-sm text-gray-300 space-y-4 flex-1">
                                    <div className="flex text-green-400">
                                        <span className="mr-3">❯</span>
                                        <span className="text-white/70">fertile-notify emit --channel console</span>
                                    </div>
                                    <div className="text-blue-400 font-bold mt-6">
                                        [{new Date().toLocaleTimeString()}] INFO: System emitted a new notification event
                                    </div>
                                    <div className="pl-5 border-l-2 border-white/10 space-y-2 py-2">
                                        <div className="text-yellow-300 font-bold">Subject: {title || 'Event Subject'}</div>
                                        <div className="text-white whitespace-pre-wrap leading-relaxed opacity-90">{message || 'Console log output will appear here...'}</div>
                                    </div>
                                    <div className="flex text-green-400 mt-6 pt-4 animate-pulse">
                                        <span className="mr-3">❯</span>
                                        <span className="w-2 h-5 bg-white/50 block"></span>
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
