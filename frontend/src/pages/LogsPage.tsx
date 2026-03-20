import React, { useState, useEffect } from 'react';
import { templateSevice } from '../api/templateSevice';
import type { Notification } from '../types/template';
import AppShell from '../components/AppShell';
import { getChannelMetadata, getString } from '../constants/channels';

export default function LogsPage() {
    const [logs, setLogs] = useState<Notification[]>([]);
    const [loading, setLoading] = useState(true);
    const [limit, setLimit] = useState(50);
    const [error, setError] = useState<string | null>(null);
    const [expandedId, setExpandedId] = useState<string | null>(null);

    useEffect(() => {
        const fetchLogs = async () => {
            try {
                setLoading(true);
                const data = await templateSevice.getNotificationLogs(limit);
                setLogs(data);
            } catch {
                setError('Failed to load notification logs.');
            } finally {
                setLoading(false);
            }
        };
        fetchLogs();
    }, [limit]);

    const getStatusColor = (status: string) => {
        switch (status) {
            case 'Success': return 'bg-green-500/10 text-green-400 border-green-500/20';
            case 'Failed': return 'bg-red-500/10 text-red-400 border-red-500/20';
            case 'Rejected': return 'bg-orange-500/10 text-orange-400 border-orange-500/20';
            default: return 'bg-gray-500/10 text-gray-400 border-gray-500/20';
        }
    };

    return (
        <AppShell title="Notification Logs">
            <div className="max-w-6xl space-y-6">
                <div className="card p-6 border-accent-primary/20 bg-primary/50 relative overflow-hidden flex flex-col md:flex-row justify-between items-start md:items-center gap-6">
                    <div className="flex-1">
                        <h2 className="text-xl font-display font-bold text-primary mb-2">Delivery History</h2>
                        <p className="text-sm text-secondary max-w-2xl leading-relaxed">
                            Review your recent notification deliveries. For privacy and GDPR compliance, full contents 
                            are retained for 7 days. Older logs (up to 30 days) have sensitive data masked.
                        </p>
                    </div>
                    <div className="shrink-0 flex flex-col gap-1.5 min-w-[140px]">
                        <label className="text-[10px] font-bold uppercase tracking-widest text-muted">Logs to fetch</label>
                        <select 
                            value={limit} 
                            onChange={(e) => setLimit(Number(e.target.value))}
                            className="bg-secondary border border-primary rounded-lg px-3 py-2 text-sm text-primary font-bold focus:border-accent-primary outline-none cursor-pointer transition-all"
                        >
                            <option value={10}>Latest 10</option>
                            <option value={50}>Latest 50</option>
                            <option value={100}>Latest 100</option>
                            <option value={500}>Latest 500</option>
                        </select>
                    </div>
                </div>

                {loading ? (
                    <div className="flex items-center gap-3 py-10">
                        <div className="spinner" />
                        <span className="text-sm text-secondary">Loading logs...</span>
                    </div>
                ) : error ? (
                    <div className="card p-8 text-center border-red-500/20">
                        <p className="text-red-400 font-medium text-sm">{error}</p>
                        <button className="btn-secondary mt-3 text-sm" onClick={() => window.location.reload()}>Retry</button>
                    </div>
                ) : logs.length === 0 ? (
                    <div className="card p-12 text-center">
                        <p className="font-semibold text-primary">No logs found</p>
                        <p className="text-sm text-tertiary">You haven't sent any notifications recently.</p>
                    </div>
                ) : (
                    <div className="card overflow-hidden">
                        <div className="overflow-x-auto">
                            <table className="w-full text-left border-collapse">
                                <thead>
                                    <tr className="border-b border-primary bg-secondary/30">
                                        <th className="py-4 px-5 text-xs font-bold text-muted uppercase tracking-wider">Date & Time</th>
                                        <th className="py-4 px-5 text-xs font-bold text-muted uppercase tracking-wider">Recipient</th>
                                        <th className="py-4 px-5 text-xs font-bold text-muted uppercase tracking-wider">Channel</th>
                                        <th className="py-4 px-5 text-xs font-bold text-muted uppercase tracking-wider">Event</th>
                                        <th className="py-4 px-5 text-xs font-bold text-muted uppercase tracking-wider">Status</th>
                                        <th className="py-4 px-5 text-xs font-bold text-muted uppercase tracking-wider">Action</th>
                                    </tr>
                                </thead>
                                <tbody className="divide-y divide-primary/50">
                                    {logs
                                        .filter(log => {
                                            const diffDays = Math.ceil((Date.now() - new Date(log.createdAt).getTime()) / (1000 * 60 * 60 * 24));
                                            return diffDays <= 30;
                                        })
                                        .map((log) => {
                                            const chMeta = getChannelMetadata(log.channel || (log as any).Channel);
                                            const isExpanded = expandedId === log.id;
                                            const diffDays = Math.ceil((Date.now() - new Date(log.createdAt).getTime()) / (1000 * 60 * 60 * 24));
                                            const isMasked = diffDays > 7;

                                            const mask = (text: string | null) => isMasked ? '*** (Protected by GDPR Policy)' : (text || '');
                                            
                                            return (
                                                <React.Fragment key={log.id}>
                                                    <tr className={`hover:bg-tertiary/20 transition-colors ${isExpanded ? 'bg-tertiary/10' : ''}`}>
                                                        <td className="py-4 px-5 text-sm text-secondary font-mono">
                                                            {new Date(log.createdAt).toLocaleString(undefined, { 
                                                                month: 'short', day: 'numeric', hour: '2-digit', minute: '2-digit' 
                                                            })}
                                                        </td>
                                                        <td className="py-4 px-5 text-sm font-medium text-primary">
                                                            {isMasked ? '***' : log.recipient}
                                                        </td>
                                                        <td className="py-4 px-5">
                                                            <div className="flex items-center gap-2">
                                                                <div 
                                                                    className="w-6 h-6 rounded flex items-center justify-center text-[10px]"
                                                                    style={{ background: `${chMeta.color}18`, color: chMeta.color }}
                                                                >
                                                                    {chMeta.icon}
                                                                </div>
                                                                <span className="text-sm text-secondary">{chMeta.name}</span>
                                                            </div>
                                                        </td>
                                                        <td className="py-4 px-5 text-sm text-secondary">
                                                            <span className="px-2 py-1 rounded bg-secondary text-xs font-mono">{getString(log.event || (log as any).Event)}</span>
                                                        </td>
                                                        <td className="py-4 px-5">
                                                            <span className={`text-[10px] font-bold px-2.5 py-1 rounded-full border ${getStatusColor(log.status)}`}>
                                                                {log.status}
                                                            </span>
                                                        </td>
                                                        <td className="py-4 px-5">
                                                            <button 
                                                                onClick={() => setExpandedId(isExpanded ? null : log.id)}
                                                                className="text-xs font-medium text-accent-light hover:text-accent-primary transition-colors flex items-center gap-1"
                                                            >
                                                                {isExpanded ? 'Hide' : 'View'}
                                                                <svg className={`w-3 h-3 transition-transform ${isExpanded ? 'rotate-180' : ''}`} fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7" />
                                                                </svg>
                                                            </button>
                                                        </td>
                                                    </tr>
                                                    {isExpanded && (
                                                        <tr className="bg-elevated/30">
                                                            <td colSpan={6} className="py-4 px-5 border-t border-primary/20">
                                                                <div className="grid grid-cols-1 md:grid-cols-2 gap-6 animate-fade-in text-secondary">
                                                                    {isMasked && (
                                                                        <div className="col-span-1 md:col-span-2 p-3 bg-accent-primary/5 border border-accent-primary/10 rounded-lg flex items-center gap-3">
                                                                            <svg className="w-4 h-4 text-accent-primary" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                                                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z" />
                                                                            </svg>
                                                                            <p className="text-[11px] font-medium">This content is hidden to satisfy GDPR privacy requirements (logs older than 7 days).</p>
                                                                        </div>
                                                                    )}
                                                                    <div className="space-y-4">
                                                                        <div>
                                                                            <h4 className="text-[10px] font-bold text-muted uppercase tracking-widest mb-1.5">Subject</h4>
                                                                            <p className="text-sm text-primary font-mono bg-primary p-3 rounded-lg border border-primary/50 break-words">
                                                                                {mask(log.subject) || <span className="italic text-tertiary">No subject</span>}
                                                                            </p>
                                                                        </div>
                                                                        {log.errorMessage && (
                                                                            <div>
                                                                                <h4 className="text-[10px] font-bold text-red-400/70 uppercase tracking-widest mb-1.5">Error Detail</h4>
                                                                                <p className="text-sm text-red-400 font-mono bg-red-500/5 p-3 rounded-lg border border-red-500/20 break-words">
                                                                                    {log.errorMessage}
                                                                                </p>
                                                                            </div>
                                                                        )}
                                                                    </div>
                                                                    <div>
                                                                        <h4 className="text-[10px] font-bold text-muted uppercase tracking-widest mb-1.5">Body Preview</h4>
                                                                        <div className="text-xs text-secondary font-mono bg-primary p-3 rounded-lg border border-primary/50 h-32 overflow-y-auto whitespace-pre-wrap break-words">
                                                                            {mask(log.body) || <span className="italic text-tertiary">Empty body</span>}
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    )}
                                                </React.Fragment>
                                            );
                                        })}
                                </tbody>
                            </table>
                        </div>
                    </div>
                )}
            </div>
        </AppShell>
    );
}
