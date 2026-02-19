import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { templateSevice } from '../api/templateSevice';
import type { Template as ApiTemplate } from '../types/template';

interface Template {
    id: string;
    name: string;
    description: string;
    type: 'Email' | 'SMS' | 'Console';
    eventType: string;
    source: 'Default' | 'Custom';
    updatedAt: string;
    subject: string;
    body: string;
}

const capitalizeChannel = (channel: string): 'Email' | 'SMS' | 'Console' => {
    if (channel === 'email') return 'Email';
    if (channel === 'sms') return 'SMS';
    if (channel === 'console') return 'Console';
    return 'Email'; // fallback
};

export default function TemplatesPage() {
    const navigate = useNavigate();
    const [search, setSearch] = useState('');
    const [filterType, setFilterType] = useState('All');
    const [filterSource, setFilterSource] = useState('All');
    const [showCreateDropdown, setShowCreateDropdown] = useState(false);
    const [templates, setTemplates] = useState<Template[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [expandedTemplateId, setExpandedTemplateId] = useState<string | null>(null);

    useEffect(() => {
        const fetchTemplates = async () => {
            try {
                setLoading(true);
                setError(null);
                const data = await templateSevice.getAllTemplates();

                const mappedTemplates: Template[] = data.map((t: ApiTemplate) => ({
                    id: t.id,
                    name: t.name,
                    description: t.description || 'No description available',
                    type: capitalizeChannel(t.channel),
                    eventType: t.event,
                    source: t.source,
                    updatedAt: new Date().toISOString().split('T')[0],
                    subject: t.subject || '',
                    body: t.body || ''
                }));

                setTemplates(mappedTemplates);
            } catch (err) {
                console.error('Failed to fetch templates:', err);
                setError('Failed to load templates. Please try again later.');
            } finally {
                setLoading(false);
            }
        };

        fetchTemplates();
    }, []);

    const toggleTemplate = (id: string) => {
        setExpandedTemplateId(expandedTemplateId === id ? null : id);
    };

    const filteredTemplates = templates.filter(t => {
        const matchesSearch = t.name.toLowerCase().includes(search.toLowerCase()) ||
            t.description.toLowerCase().includes(search.toLowerCase());
        const matchesType = filterType === 'All' || t.type === filterType;
        const matchesSource = filterSource === 'All' || t.source === filterSource;
        return matchesSearch && matchesType && matchesSource;
    });

    return (
        <div className="min-h-screen bg-primary text-primary flex flex-col items-center py-12 px-4 animate-fade-in relative">
            {/* Back Button */}
            <button
                onClick={() => navigate("/dashboard")}
                className="absolute top-8 left-8 text-secondary hover:text-primary transition-smooth flex items-center gap-2 text-sm"
            >
                <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M10 19l-7-7m0 0l7-7m-7 7h18" />
                </svg>
                Dashboard
            </button>

            <div className="max-w-6xl w-full space-y-8 pt-8">
                {/* Header Section */}
                <div className="flex flex-col md:flex-row md:items-end justify-between gap-6">
                    <div className="space-y-2">
                        <h1 className="text-3xl font-display font-bold tracking-tight text-primary">Templates</h1>
                        <p className="text-secondary text-sm">Manage and customize your notification blueprints.</p>
                    </div>

                    <div className="relative">
                        <button
                            className="btn-primary flex items-center gap-2 text-sm px-6"
                            onClick={() => setShowCreateDropdown(!showCreateDropdown)}
                        >
                            <span>Create New</span>
                            <svg className={`w-4 h-4 transition-transform ${showCreateDropdown ? 'rotate-180' : ''}`} fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7" />
                            </svg>
                        </button>

                        {showCreateDropdown && (
                            <div className="absolute right-0 mt-2 w-56 card-elevated z-50 overflow-hidden animate-slide-up shadow-2xl">
                                <button className="w-full px-4 py-3 text-left text-sm hover:bg-tertiary transition-colors flex items-center gap-3"
                                    onClick={() => navigate("/email-visual-editor")}
                                >
                                    <span className="text-primary-400">📧</span> Email Visual Editor
                                </button>
                                <button className="w-full px-4 py-3 text-left text-sm hover:bg-tertiary transition-colors flex items-center gap-3"
                                    onClick={() => navigate("/email-advanced-editor")}
                                >
                                    <span className="text-primary-400">📧</span> Email Advanced Editor
                                </button>
                                <button className="w-full px-4 py-3 text-left text-sm hover:bg-tertiary transition-colors flex items-center gap-3 border-t border-primary"
                                    onClick={() => navigate("/sms-editor")}>
                                    <span className="text-primary-400">💬</span> SMS Template
                                </button>
                                <button className="w-full px-4 py-3 text-left text-sm hover:bg-tertiary transition-colors flex items-center gap-3 border-t border-primary"
                                    onClick={() => navigate("/console-editor")}>
                                    <span className="text-primary-400">🖥️</span> Console Template
                                </button>
                            </div>
                        )}
                    </div>
                </div>

                {/* Filters Row */}
                <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
                    <div className="md:col-span-2">
                        <input
                            type="text"
                            placeholder="Search templates..."
                            className="input-modern w-full"
                            value={search}
                            onChange={(e) => setSearch(e.target.value)}
                        />
                    </div>
                    <select
                        className="input-modern cursor-pointer"
                        value={filterType}
                        onChange={(e) => setFilterType(e.target.value)}
                    >
                        <option value="All">All Methods</option>
                        <option value="Email">Email</option>
                        <option value="SMS">SMS</option>
                        <option value="Console">Console</option>
                    </select>
                    <select
                        className="input-modern cursor-pointer"
                        value={filterSource}
                        onChange={(e) => setFilterSource(e.target.value)}
                    >
                        <option value="All">All Creators</option>
                        <option value="Default">Default (System)</option>
                        <option value="Custom">Custom (User)</option>
                    </select>
                </div>

                {/* Loading State */}
                {loading && (
                    <div className="card p-12 text-center space-y-4">
                        <div className="flex justify-center">
                            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-400"></div>
                        </div>
                        <p className="text-secondary text-sm">Loading templates...</p>
                    </div>
                )}

                {/* Error State */}
                {error && !loading && (
                    <div className="card p-12 text-center space-y-4 border-red-500/30">
                        <h3 className="text-red-400 font-medium">Error Loading Templates</h3>
                        <p className="text-secondary text-sm max-w-xs mx-auto">{error}</p>
                        <button
                            className="btn-secondary text-sm"
                            onClick={() => window.location.reload()}
                        >
                            Retry
                        </button>
                    </div>
                )}

                {/* Templates Grid */}
                {!loading && !error && (
                    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                        {filteredTemplates.map(template => (
                            <div key={template.id} className="card p-6 flex flex-col justify-between group hover:border-hover transition-smooth relative overflow-hidden">
                                {template.source === 'Default' && (
                                    <div className="absolute top-0 right-0 p-2 opacity-50">
                                        <svg className="w-16 h-16 text-primary-500/5 -rotate-12 transform translate-x-4 -translate-y-4 pointer-events-none" fill="currentColor" viewBox="0 0 24 24">
                                            <path d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z" />
                                        </svg>
                                    </div>
                                )}

                                <div className="space-y-4">
                                    <div className="flex justify-between items-start relative z-10">
                                        <div className="flex items-center gap-2">
                                            <span className="text-xl">
                                                {template.type === 'Email' ? '📧' : template.type === 'SMS' ? '💬' : '🖥️'}
                                            </span>
                                            <span className={`badge ${template.source === 'Default' ? 'text-blue-400 border-blue-500/20' : 'text-purple-400 border-purple-500/20'}`}>
                                                {template.source}
                                            </span>
                                        </div>
                                        <span className="text-[10px] text-tertiary uppercase font-medium tracking-widest">{template.updatedAt}</span>
                                    </div>

                                    <div>
                                        <h3 className="text-lg font-semibold text-primary group-hover:text-primary-400 transition-colors uppercase tracking-tight">
                                            {template.name}
                                        </h3>
                                        <p className="text-secondary text-sm mt-1 line-clamp-2 leading-relaxed">
                                            {template.description}
                                        </p>
                                    </div>

                                    <div className="pt-2 flex items-center justify-between">
                                        <div className="inline-flex items-center gap-1.5 px-2 py-1 bg-tertiary border border-primary rounded text-[11px] text-primary-400 font-mono">
                                            <svg className="w-3 h-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 10V3L4 14h7v7l9-11h-7z" />
                                            </svg>
                                            {template.eventType}
                                        </div>
                                        <button
                                            onClick={() => toggleTemplate(template.id)}
                                            className="text-xs text-secondary hover:text-primary-400 transition-colors flex items-center gap-1 font-medium"
                                        >
                                            {expandedTemplateId === template.id ? 'Hide Details' : 'View Details'}
                                            <svg className={`w-3 h-3 transition-transform ${expandedTemplateId === template.id ? 'rotate-180' : ''}`} fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7" />
                                            </svg>
                                        </button>
                                    </div>

                                    {/* Collapsible Details */}
                                    {expandedTemplateId === template.id && (
                                        <div className="mt-4 p-4 bg-tertiary/50 rounded-lg animate-fade-in border border-primary-500/10 relative z-10 transition-all">
                                            <div className="mb-3">
                                                <h4 className="text-[10px] font-bold text-primary-400 uppercase mb-1 tracking-wider">Subject / Title</h4>
                                                <p className="text-xs text-primary font-medium p-2 bg-primary/50 rounded border border-primary-500/10">
                                                    {template.subject || <span className="text-secondary italic">No subject defined</span>}
                                                </p>
                                            </div>
                                            <div>
                                                <h4 className="text-[10px] font-bold text-primary-400 uppercase mb-1 tracking-wider">Content Template</h4>
                                                <div className="text-xs text-secondary bg-primary/50 p-3 rounded border border-primary-500/10 max-h-32 overflow-y-auto whitespace-pre-wrap font-mono relative">
                                                    {template.description.length > 500 ? (
                                                        template.body || <span className="text-secondary italic">No content template defined</span>
                                                    ) : (
                                                        template.body || <span className="text-secondary italic">No content template defined</span>
                                                    )}
                                                </div>
                                            </div>
                                        </div>
                                    )}
                                </div>

                                <div className="flex flex-col gap-2 mt-8 pt-4 border-t border-primary relative z-10">
                                    {template.source === 'Custom' ? (
                                        <>
                                            {template.type === "Email" ? (
                                                <div className="flex gap-2 w-full">
                                                    <button
                                                        className="btn-secondary flex-1 text-[10px] py-2 h-auto"
                                                        onClick={() => navigate("/email-visual-editor", { state: { template } })}
                                                    >
                                                        Visual Editor
                                                    </button>
                                                    <button
                                                        className="btn-secondary flex-1 text-[10px] py-2 h-auto"
                                                        onClick={() => navigate("/email-advanced-editor", { state: { template } })}
                                                    >
                                                        Advanced
                                                    </button>
                                                </div>
                                            ) : (
                                                <button
                                                    className="btn-secondary flex-1 text-xs py-2 h-auto"
                                                    onClick={() => navigate(template.type === "SMS" ? "/sms-editor" : "/console-editor", { state: { template } })}
                                                >
                                                    Edit {template.type}
                                                </button>
                                            )}
                                            <button className="btn-secondary text-xs py-2 h-auto px-3 hover:text-red-400 hover:border-red-500/30 flex items-center justify-center gap-2">
                                                <svg className="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                                                </svg>
                                                <span className="text-[10px] uppercase font-bold tracking-wider">Delete</span>
                                            </button>
                                        </>
                                    ) : (
                                        <div className="py-2 text-center">
                                            <span className="text-xs text-secondary flex items-center justify-center gap-1.5 opacity-60 cursor-not-allowed">
                                                <svg className="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z" />
                                                </svg>
                                                System Template (Read Only)
                                            </span>
                                        </div>
                                    )}
                                </div>
                            </div>
                        ))}
                    </div>
                )}

                {!loading && !error && filteredTemplates.length === 0 && (
                    <div className="card p-12 text-center space-y-4">
                        <h3 className="text-primary font-medium">No templates found</h3>
                        <p className="text-secondary text-sm max-w-xs mx-auto text-balance">
                            We couldn't find any templates matching your search or filters. Try adjusting them!
                        </p>
                    </div>
                )}
            </div>
        </div>
    );
}