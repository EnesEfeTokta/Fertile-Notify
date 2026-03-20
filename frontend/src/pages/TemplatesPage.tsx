import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { templateSevice } from '../api/templateSevice';
import { NOTIFICATION_CHANNELS, getChannelMetadata, getString } from '../constants/channels';
import AppShell from '../components/AppShell';

interface Template {
    id: string;
    name: string;
    description: string;
    type: string;
    channelId: string;
    eventType: string;
    source: 'Default' | 'Custom';
    updatedAt: string;
    subject: string;
    body: string;
}

const capitalizeChannel = (channel: string): string => getChannelMetadata(channel).name;

export default function TemplatesPage() {
    const navigate = useNavigate();
    const [search, setSearch] = useState('');
    const [filterType, setFilterType] = useState('All');
    const [filterSource, setFilterSource] = useState('All');
    const [showCreateDropdown, setShowCreateDropdown] = useState(false);
    const [templates, setTemplates] = useState<Template[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [expandedId, setExpandedId] = useState<string | null>(null);

    useEffect(() => {
        (async () => {
            try {
                setLoading(true);
                const data = await templateSevice.getAllTemplates();
                if (!Array.isArray(data)) {
                    console.error("Templates API returned non-array data:", data);
                    setTemplates([]);
                    return;
                }
                setTemplates(data.map((t: any, index: number) => ({
                    id: getString(t.id || t.Id) || `template-${index}`,
                    name: getString(t.name || t.Name),
                    description: getString(t.description || t.Description || 'No description available'),
                    type: capitalizeChannel(getString(t.channel || t.Channel)),
                    channelId: getString(t.channel || t.Channel || 'sms').toLowerCase(),
                    eventType: getString(t.eventType || t.EventType || t.event || t.Event || 'Unknown'),
                    source: (getString(t.source || t.Source) as 'Default' | 'Custom'),
                    updatedAt: new Date().toISOString().split('T')[0],
                    subject: getString(t.subject || t.Subject || ''),
                    body: getString(t.body || t.Body || ''),
                })));
            } catch (err) {
                console.error("Mapping error:", err);
                setError('Failed to load templates.');
            } finally {
                setLoading(false);
            }
        })();
    }, []);

    const filtered = templates.filter(t => {
        const q = search.toLowerCase();
        return (t.name.toLowerCase().includes(q) || t.description.toLowerCase().includes(q))
            && (filterType === 'All' || t.type === filterType)
            && (filterSource === 'All' || t.source === filterSource);
    });

    const topBarActions = (
        <div className="flex items-center gap-2">
            <div className="relative">
                <svg className="absolute left-2.5 top-1/2 -translate-y-1/2 w-3 h-3 text-muted pointer-events-none" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
                </svg>
                <input
                    type="text"
                    placeholder="Search…"
                    className="input-modern text-xs py-1.5 pl-7 pr-3 w-36"
                    value={search}
                    onChange={e => setSearch(e.target.value)}
                />
            </div>
            <select
                className="input-modern text-xs py-1.5 cursor-pointer w-32 bg-secondary"
                value={filterType}
                onChange={e => setFilterType(e.target.value)}
            >
                <option value="All">All channels</option>
                {NOTIFICATION_CHANNELS.map(c => <option key={c.id} value={c.name}>{c.name}</option>)}
            </select>
            <select
                className="input-modern text-xs py-1.5 cursor-pointer w-28 bg-secondary"
                value={filterSource}
                onChange={e => setFilterSource(e.target.value)}
            >
                <option value="All">All sources</option>
                <option value="Default">System</option>
                <option value="Custom">Custom</option>
            </select>
            <div className="relative">
                <button className="btn-primary text-xs px-3.5 py-1.5 flex items-center gap-1.5 whitespace-nowrap" onClick={() => setShowCreateDropdown(v => !v)}>
                    <svg className="w-3 h-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v16m8-8H4" />
                    </svg>
                    New Template
                    <svg className={`w-3 h-3 transition-transform ${showCreateDropdown ? 'rotate-180' : ''}`} fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7" />
                    </svg>
                </button>
                {showCreateDropdown && (
                    <div className="absolute right-0 mt-2 w-60 card-elevated z-50 overflow-hidden shadow-2xl animate-slide-up">
                        <div className="px-3 py-2 border-b border-primary">
                            <p className="text-[10px] font-bold uppercase tracking-widest text-muted">Email editors</p>
                        </div>
                        <button className="w-full px-4 py-3 text-left text-sm hover:bg-tertiary flex items-center gap-3 transition-colors"
                            onClick={() => { navigate("/email-visual-editor"); setShowCreateDropdown(false); }}>
                            <span>🎨</span> Visual Designer
                        </button>
                        <button className="w-full px-4 py-3 text-left text-sm hover:bg-tertiary flex items-center gap-3 transition-colors border-b border-primary"
                            onClick={() => { navigate("/email-advanced-editor"); setShowCreateDropdown(false); }}>
                            <span>⌨️</span> Advanced (HTML)
                        </button>
                        <div className="px-3 py-2 border-b border-primary">
                            <p className="text-[10px] font-bold uppercase tracking-widest text-muted">Other channels</p>
                        </div>
                        {NOTIFICATION_CHANNELS.filter(c => c.id !== 'email').map(channel => (
                            <button key={channel.id}
                                className="w-full px-4 py-3 text-left text-sm hover:bg-tertiary flex items-center gap-3 border-b border-primary/5 last:border-0 transition-colors"
                                onClick={() => { navigate(channel.editorRoute, { state: { channel: channel.id } }); setShowCreateDropdown(false); }}>
                                <span className="w-5 text-center">{channel.icon}</span>
                                <div>
                                    <p className="font-medium">{channel.name}</p>
                                    <p className="text-[10px] text-tertiary">Dedicated editor</p>
                                </div>
                            </button>
                        ))}
                    </div>
                )}
            </div>
        </div>
    );

    return (
        <AppShell title="Templates" actions={topBarActions}>
            {/* Close dropdown on outside click */}
            {showCreateDropdown && <div className="fixed inset-0 z-30" onClick={() => setShowCreateDropdown(false)} />}

            {/* Loading */}
            {loading && (
                <div className="flex items-center gap-3 py-16">
                    <div className="spinner" />
                    <span className="text-secondary text-sm">Loading templates…</span>
                </div>
            )}

            {/* Error */}
            {error && !loading && (
                <div className="card p-8 text-center space-y-3 border-red-500/20">
                    <p className="text-red-400 font-medium text-sm">{error}</p>
                    <button className="btn-secondary text-sm" onClick={() => window.location.reload()}>Retry</button>
                </div>
            )}

            {/* Template grid */}
            {!loading && !error && (
                <>
                    {filtered.length === 0 ? (
                        <div className="card p-12 text-center space-y-2">
                            <p className="font-semibold text-primary">No templates found</p>
                            <p className="text-sm text-tertiary">Try adjusting your search or filters.</p>
                        </div>
                    ) : (
                        <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-5">
                            {filtered.map(template => {
                                const chMeta = getChannelMetadata(template.channelId);
                                const isExpanded = expandedId === template.id;
                                return (
                                    <div key={template.id} className="card p-5 flex flex-col gap-4 hover:border-hover transition-all group">
                                        {/* Header */}
                                        <div className="flex items-start justify-between">
                                            <div className="flex items-center gap-2.5">
                                                <div
                                                    className="w-8 h-8 rounded-lg flex items-center justify-center text-base shrink-0"
                                                    style={{ background: `${chMeta.color}18`, border: `1px solid ${chMeta.color}25` }}
                                                >
                                                    {chMeta.icon}
                                                </div>
                                                <div>
                                                    <span className={`text-[10px] font-bold px-2 py-0.5 rounded-full border ${template.source === 'Default'
                                                        ? 'bg-blue-500/10 text-blue-400 border-blue-500/20'
                                                        : 'bg-purple-500/10 text-purple-400 border-purple-500/20'
                                                        }`}>
                                                        {template.source === 'Default' ? 'System' : 'Custom'}
                                                    </span>
                                                </div>
                                            </div>
                                            <span className="text-[10px] text-muted font-mono">{template.updatedAt}</span>
                                        </div>

                                        {/* Name + desc */}
                                        <div>
                                            <h3 className="font-bold text-primary tracking-tight mb-1">{template.name}</h3>
                                            <p className="text-xs text-secondary line-clamp-2 leading-relaxed">{template.description}</p>
                                        </div>

                                        {/* Event tag */}
                                        <div className="flex items-center justify-between">
                                            <span className="inline-flex items-center gap-1.5 px-2 py-1 bg-tertiary border border-primary rounded-md text-[10px] font-mono text-accent-light">
                                                <svg className="w-2.5 h-2.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 10V3L4 14h7v7l9-11h-7z" />
                                                </svg>
                                                {template.eventType}
                                            </span>
                                            <button
                                                onClick={() => setExpandedId(isExpanded ? null : template.id)}
                                                className="text-[11px] text-tertiary hover:text-primary-400 font-medium flex items-center gap-1 transition-colors"
                                            >
                                                {isExpanded ? 'Hide' : 'Preview'}
                                                <svg className={`w-3 h-3 transition-transform ${isExpanded ? 'rotate-180' : ''}`} fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7" />
                                                </svg>
                                            </button>
                                        </div>

                                        {/* Collapsible preview */}
                                        {isExpanded && (
                                            <div className="space-y-3 border-t border-primary pt-4 animate-fade-in">
                                                <div>
                                                    <p className="text-[10px] font-bold uppercase tracking-widest text-muted mb-1.5">Subject</p>
                                                    <p className="text-xs text-primary bg-elevated p-2.5 rounded-lg border border-primary font-mono">
                                                        {template.subject || <span className="text-muted italic">No subject</span>}
                                                    </p>
                                                </div>
                                                <div>
                                                    <p className="text-[10px] font-bold uppercase tracking-widest text-muted mb-1.5">Body</p>
                                                    <div className="text-xs text-secondary bg-elevated p-2.5 rounded-lg border border-primary font-mono max-h-28 overflow-y-auto whitespace-pre-wrap">
                                                        {template.body || <span className="text-muted italic">No body content</span>}
                                                    </div>
                                                </div>
                                            </div>
                                        )}

                                        {/* Actions */}
                                        <div className="mt-auto pt-4 border-t border-primary">
                                            {template.source === 'Custom' ? (
                                                <div className="flex gap-2">
                                                    {template.type === 'Email' ? (
                                                        <>
                                                            <button className="btn-secondary flex-1 text-xs py-1.5 h-auto" onClick={() => navigate('/email-visual-editor', { state: { template } })}>Visual</button>
                                                            <button className="btn-secondary flex-1 text-xs py-1.5 h-auto" onClick={() => navigate('/email-advanced-editor', { state: { template } })}>HTML</button>
                                                        </>
                                                    ) : (
                                                        <button className="btn-secondary flex-1 text-xs py-1.5 h-auto" onClick={() => navigate(getChannelMetadata(template.channelId).editorRoute, { state: { template } })}>
                                                            Edit {template.type}
                                                        </button>
                                                    )}
                                                    <button className="btn-secondary px-3 py-1.5 h-auto text-red-400 hover:border-red-500/30">
                                                        <svg className="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                                                        </svg>
                                                    </button>
                                                </div>
                                            ) : (
                                                <div className="flex items-center justify-center gap-2 py-2 text-[10px] font-bold uppercase tracking-widest text-muted">
                                                    <svg className="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z" />
                                                    </svg>
                                                    System Template
                                                </div>
                                            )}
                                        </div>
                                    </div>
                                );
                            })}
                        </div>
                    )}
                </>
            )}
        </AppShell>
    );
}