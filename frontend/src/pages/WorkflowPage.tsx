import { useState, useEffect, useCallback } from 'react';
import AppShell from '../components/AppShell';
import { workflowService } from '../api/workflowService';
import { NOTIFICATION_CHANNELS } from '../constants/channels';
import { EVENT_TYPES } from '../constants/eventTypes';
import type { Workflow, CreateWorkflowRequest, UpdateWorkflowRequest } from '../types/workflow';

/* ─── helpers ─────────────────────────────────────────── */
const fmtDate = (iso: string) => {
    try { return new Date(iso).toLocaleDateString('en-GB', { day: '2-digit', month: 'short', year: 'numeric' }); }
    catch { return iso; }
};

const EMPTY_FORM: CreateWorkflowRequest = {
    name: '',
    description: '',
    eventType: '',
    channels: 'email',
    eventTrigger: '',
    cronExpression: '',
    subject: '',
    body: '',
    to: [],
};

/* ─── sub-components ──────────────────────────────────── */
function StatusBadge({ active }: { active: boolean }) {
    return (
        <span className={`inline-flex items-center gap-1.5 px-2 py-0.5 rounded-full text-[10px] font-bold uppercase tracking-wider border ${active
            ? 'bg-green-500/10 text-green-400 border-green-500/20'
            : 'bg-zinc-500/10 text-zinc-400 border-zinc-500/20'}`}>
            <span className={`w-1.5 h-1.5 rounded-full ${active ? 'bg-green-400' : 'bg-zinc-500'}`} />
            {active ? 'Active' : 'Inactive'}
        </span>
    );
}

function TriggerBadge({ label }: { label: string }) {
    const isCron = label?.startsWith('* ') || /^[\d*\/,\-]/.test(label || '');
    return (
        <span className={`inline-flex items-center gap-1 px-2 py-0.5 rounded-md text-[10px] font-mono border ${isCron
            ? 'bg-orange-500/10 text-orange-400 border-orange-500/20'
            : 'bg-blue-500/10 text-blue-400 border-blue-500/20'}`}>
            {isCron ? '⏰' : '⚡'} {label || '—'}
        </span>
    );
}

/* ─── main page ───────────────────────────────────────── */
export default function WorkflowPage() {
    const [workflows, setWorkflows] = useState<Workflow[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [toast, setToast] = useState<{ msg: string; type: 'success' | 'error' } | null>(null);

    // Modal state
    const [modalOpen, setModalOpen] = useState(false);
    const [editTarget, setEditTarget] = useState<Workflow | null>(null);
    const [form, setForm] = useState<CreateWorkflowRequest>(EMPTY_FORM);
    const [saving, setSaving] = useState(false);

    // Trigger modal state
    const [triggerModalOpen, setTriggerModalOpen] = useState(false);
    const [triggerKey, setTriggerKey] = useState('');
    const [triggering, setTriggering] = useState(false);

    // Recipient field helpers (comma-separated per channel)
    const [recipientInputs, setRecipientInputs] = useState<{ channel: string; value: string }[]>([]);

    const showToast = (msg: string, type: 'success' | 'error' = 'success') => {
        setToast({ msg, type });
        setTimeout(() => setToast(null), 3500);
    };

    const load = useCallback(async () => {
        try {
            setLoading(true);
            setError(null);
            const data = await workflowService.listWorkflows();
            setWorkflows(Array.isArray(data) ? data : []);
        } catch {
            setError('Failed to load workflows.');
        } finally {
            setLoading(false);
        }
    }, []);

    useEffect(() => { load(); }, [load]);

    /* ── open modal ── */
    const openCreate = () => {
        setEditTarget(null);
        setForm(EMPTY_FORM);
        setRecipientInputs([]);
        setModalOpen(true);
    };

    const openEdit = (w: Workflow) => {
        setEditTarget(w);
        setForm({
            name: w.name,
            description: w.description,
            eventType: w.eventType ?? '',
            channels: w.channel,
            eventTrigger: w.eventTrigger,
            cronExpression: w.cronExpression,
            subject: w.subject,
            body: w.body,
            to: w.recipients ?? [],
        });
        setRecipientInputs((w.recipients ?? []).map(r => ({ channel: r.channel, value: r.recipients.join(', ') })));
        setModalOpen(true);
    };

    /* ── save (create or update) ── */
    const handleSave = async () => {
        if (!form.name.trim()) return showToast('Name is required.', 'error');
        if (!form.eventType.trim()) return showToast('Event type is required.', 'error');
        setSaving(true);
        try {
            // Build recipients from inputs
            const to = recipientInputs
                .filter(r => r.value.trim())
                .map(r => ({ channel: r.channel, recipients: r.value.split(',').map(s => s.trim()).filter(Boolean) }));

            if (editTarget) {
                const req: UpdateWorkflowRequest = {
                    id: editTarget.id,
                    name: form.name,
                    description: form.description,
                    eventType: form.eventType,
                    channel: form.channels,
                    eventTrigger: form.eventTrigger,
                    cronExpression: form.cronExpression,
                    subject: form.subject,
                    body: form.body,
                    to,
                };
                await workflowService.updateWorkflow(req);
                showToast('Workflow updated.');
            } else {
                await workflowService.addWorkflow({ ...form, to });
                showToast('Workflow created.');
            }
            setModalOpen(false);
            load();
        } catch {
            showToast('Save failed. Please try again.', 'error');
        } finally {
            setSaving(false);
        }
    };

    /* ── delete ── */
    const handleDelete = async (w: Workflow) => {
        if (!confirm(`Delete workflow "${w.name}"?`)) return;
        try {
            await workflowService.deleteWorkflow(w.id);
            showToast('Workflow deleted.');
            load();
        } catch {
            showToast('Delete failed.', 'error');
        }
    };

    /* ── toggle active ── */
    const handleToggleActive = async (w: Workflow) => {
        try {
            if (w.isActive) {
                await workflowService.deactivateWorkflow(w.id);
                showToast(`"${w.name}" deactivated.`);
            } else {
                await workflowService.activateWorkflow(w.id);
                showToast(`"${w.name}" activated.`);
            }
            load();
        } catch {
            showToast('Status update failed.', 'error');
        }
    };

    /* ── trigger ── */
    const handleTrigger = async () => {
        if (!triggerKey.trim()) return showToast('Enter an event trigger key.', 'error');
        setTriggering(true);
        try {
            const res = await workflowService.triggerWorkflow(triggerKey.trim());
            showToast(`Triggered! ${res?.count ?? 0} notification(s) queued.`);
            setTriggerModalOpen(false);
            setTriggerKey('');
        } catch {
            showToast('Trigger failed.', 'error');
        } finally {
            setTriggering(false);
        }
    };

    /* ── recipient helpers ── */
    const addRecipientRow = () => setRecipientInputs(p => [...p, { channel: NOTIFICATION_CHANNELS[0].id, value: '' }]);
    const removeRecipientRow = (i: number) => setRecipientInputs(p => p.filter((_, idx) => idx !== i));
    const updateRecipientRow = (i: number, field: 'channel' | 'value', val: string) =>
        setRecipientInputs(p => p.map((r, idx) => idx === i ? { ...r, [field]: val } : r));

    /* ─────── topbar actions ─────── */
    const topBarActions = (
        <div className="flex items-center gap-2">
            <button
                className="btn-secondary text-xs px-3.5 py-1.5 flex items-center gap-1.5"
                onClick={() => setTriggerModalOpen(true)}
            >
                <svg className="w-3 h-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 10V3L4 14h7v7l9-11h-7z" />
                </svg>
                Fire Trigger
            </button>
            <button className="btn-primary text-xs px-3.5 py-1.5 flex items-center gap-1.5" onClick={openCreate}>
                <svg className="w-3 h-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v16m8-8H4" />
                </svg>
                New Workflow
            </button>
        </div>
    );

    /* ─────── render ─────── */
    return (
        <AppShell title="Workflows" actions={topBarActions}>

            {/* Toast */}
            {toast && (
                <div className={`fixed bottom-6 right-6 z-[999] flex items-center gap-2.5 px-4 py-3 rounded-xl border text-sm font-medium shadow-2xl animate-fade-in-up ${toast.type === 'error'
                    ? 'bg-red-500/10 border-red-500/30 text-red-400'
                    : 'bg-green-500/10 border-green-500/30 text-green-400'}`}>
                    {toast.type === 'error'
                        ? <svg className="w-4 h-4 shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>
                        : <svg className="w-4 h-4 shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" /></svg>
                    }
                    {toast.msg}
                </div>
            )}

            {/* Loading */}
            {loading && (
                <div className="flex items-center gap-3 py-16">
                    <div className="spinner" />
                    <span className="text-secondary text-sm">Loading workflows…</span>
                </div>
            )}

            {/* Error */}
            {error && !loading && (
                <div className="card p-8 text-center space-y-3 border-red-500/20">
                    <p className="text-red-400 font-medium text-sm">{error}</p>
                    <button className="btn-secondary text-sm" onClick={load}>Retry</button>
                </div>
            )}

            {/* Content */}
            {!loading && !error && (
                <>
                    {/* Info bar */}
                    <div className="flex items-center justify-between mb-6">
                        <p className="text-sm text-secondary">
                            <span className="text-primary font-semibold">{workflows.length}</span> workflow{workflows.length !== 1 ? 's' : ''} configured
                        </p>
                    </div>

                    {/* Empty */}
                    {workflows.length === 0 && (
                        <div className="card p-14 text-center space-y-4 border-dashed">
                            <div className="w-12 h-12 mx-auto rounded-2xl bg-accent-dim border border-blue-500/15 flex items-center justify-center text-2xl">⚡</div>
                            <div>
                                <p className="font-semibold text-primary mb-1">No workflows yet</p>
                                <p className="text-sm text-tertiary">Create your first automated notification workflow.</p>
                            </div>
                            <button className="btn-primary text-sm mx-auto" onClick={openCreate}>New Workflow</button>
                        </div>
                    )}

                    {/* Grid */}
                    {workflows.length > 0 && (
                        <div className="grid grid-cols-1 lg:grid-cols-2 xl:grid-cols-3 gap-5">
                            {workflows.map(w => (
                                <div key={w.id} className="card p-5 flex flex-col gap-4 hover:border-hover transition-all group">
                                    {/* Header row */}
                                    <div className="flex items-start justify-between gap-2">
                                        <div className="flex-1 min-w-0">
                                            <h3 className="font-bold text-primary tracking-tight truncate">{w.name}</h3>
                                            <p className="text-xs text-secondary mt-0.5 line-clamp-2 leading-relaxed">{w.description || 'No description'}</p>
                                        </div>
                                        <StatusBadge active={w.isActive} />
                                    </div>

                                    {/* Meta row */}
                                    <div className="flex flex-wrap gap-2">
                                        <span className="inline-flex items-center gap-1 px-2 py-0.5 bg-tertiary border border-primary rounded-md text-[10px] font-semibold text-secondary uppercase tracking-wider">
                                            {NOTIFICATION_CHANNELS.find(c => c.id === w.channel?.toLowerCase())?.icon ?? '🔔'} {w.channel}
                                        </span>
                                        {w.eventType && (
                                            <span className="inline-flex items-center gap-1 px-2 py-0.5 bg-indigo-500/10 border border-indigo-500/20 rounded-md text-[10px] font-semibold text-indigo-300">
                                                {EVENT_TYPES.find(et => et.value === w.eventType)?.icon ?? '🏷️'} {EVENT_TYPES.find(et => et.value === w.eventType)?.label ?? w.eventType}
                                            </span>
                                        )}
                                        {(w.eventTrigger || w.cronExpression) && (
                                            <TriggerBadge label={w.eventTrigger || w.cronExpression} />
                                        )}
                                    </div>

                                    {/* Subject/body preview */}
                                    {w.subject && (
                                        <div className="text-[11px] text-tertiary bg-elevated border border-primary rounded-lg px-3 py-2 font-mono truncate">
                                            {w.subject}
                                        </div>
                                    )}

                                    {/* Footer */}
                                    <div className="mt-auto border-t border-primary pt-4 flex items-center justify-between gap-2">
                                        <span className="text-[10px] text-muted font-mono">{fmtDate(w.createdAt)}</span>
                                        <div className="flex items-center gap-1.5">
                                            {/* Toggle active */}
                                            <button
                                                title={w.isActive ? 'Deactivate' : 'Activate'}
                                                onClick={() => handleToggleActive(w)}
                                                className={`p-1.5 rounded-md border transition-all text-xs ${w.isActive
                                                    ? 'border-green-500/20 text-green-400 hover:bg-green-500/10'
                                                    : 'border-primary text-muted hover:text-green-400 hover:border-green-500/20'}`}
                                            >
                                                <svg className="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5.636 18.364a9 9 0 010-12.728m12.728 0a9 9 0 010 12.728M12 12v.01" />
                                                </svg>
                                            </button>
                                            {/* Edit */}
                                            <button
                                                title="Edit"
                                                onClick={() => openEdit(w)}
                                                className="p-1.5 rounded-md border border-primary text-muted hover:text-primary hover:border-hover transition-all"
                                            >
                                                <svg className="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
                                                </svg>
                                            </button>
                                            {/* Delete */}
                                            <button
                                                title="Delete"
                                                onClick={() => handleDelete(w)}
                                                className="p-1.5 rounded-md border border-primary text-muted hover:text-red-400 hover:border-red-500/20 transition-all"
                                            >
                                                <svg className="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                                                </svg>
                                            </button>
                                        </div>
                                    </div>
                                </div>
                            ))}
                        </div>
                    )}
                </>
            )}

            {/* ─── Create / Edit Modal ─── */}
            {modalOpen && (
                <>
                    <div className="fixed inset-0 z-40 bg-black/60 backdrop-blur-sm animate-fade-in" onClick={() => setModalOpen(false)} />
                    <div className="fixed inset-0 z-50 flex items-center justify-center p-4">
                        <div className="card-elevated w-full max-w-2xl max-h-[90vh] overflow-y-auto animate-fade-in-up shadow-2xl">
                            {/* Modal header */}
                            <div className="flex items-center justify-between px-6 py-5 border-b border-primary">
                                <div>
                                    <h2 className="font-bold text-lg tracking-tight">{editTarget ? 'Edit Workflow' : 'New Workflow'}</h2>
                                    <p className="text-xs text-secondary mt-0.5">Configure automated notification rules</p>
                                </div>
                                <button onClick={() => setModalOpen(false)} className="p-2 rounded-lg hover:bg-tertiary text-muted hover:text-primary transition-all">
                                    <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                                    </svg>
                                </button>
                            </div>

                            {/* Modal body */}
                            <div className="px-6 py-5 space-y-5">
                                {/* Name + Description */}
                                <div className="grid grid-cols-2 gap-4">
                                    <div>
                                        <label className="text-[11px] font-bold uppercase tracking-widest text-muted block mb-1.5">Name <span className="text-red-400">*</span></label>
                                        <input
                                            className="input-modern text-sm"
                                            placeholder="e.g. Welcome Email"
                                            value={form.name}
                                            onChange={e => setForm(f => ({ ...f, name: e.target.value }))}
                                        />
                                    </div>
                                    <div>
                                        <label className="text-[11px] font-bold uppercase tracking-widest text-muted block mb-1.5">Channel</label>
                                        <select
                                            className="input-modern text-sm bg-primary cursor-pointer"
                                            value={form.channels}
                                            onChange={e => setForm(f => ({ ...f, channels: e.target.value }))}
                                        >
                                            {NOTIFICATION_CHANNELS.map(c => (
                                                <option key={c.id} value={c.id}>{c.icon} {c.name}</option>
                                            ))}
                                        </select>
                                    </div>
                                </div>

                                <div>
                                    <label className="text-[11px] font-bold uppercase tracking-widest text-muted block mb-1.5">Description</label>
                                    <input
                                        className="input-modern text-sm"
                                        placeholder="Short description of this workflow"
                                        value={form.description}
                                        onChange={e => setForm(f => ({ ...f, description: e.target.value }))}
                                    />
                                </div>

                                <div>
                                    <label className="text-[11px] font-bold uppercase tracking-widest text-muted block mb-1.5">Event Type <span className="text-red-400">*</span></label>
                                    <select
                                        className="input-modern text-sm bg-primary cursor-pointer"
                                        value={form.eventType}
                                        onChange={e => setForm(f => ({ ...f, eventType: e.target.value }))}
                                    >
                                        <option value="">Select event type</option>
                                        {EVENT_TYPES.map(et => (
                                            <option key={et.value} value={et.value}>{et.icon} {et.label}</option>
                                        ))}
                                    </select>
                                </div>

                                {/* Trigger section */}
                                <div className="bg-elevated border border-primary rounded-xl p-4 space-y-4">
                                    <p className="text-[11px] font-bold uppercase tracking-widest text-muted">Trigger</p>
                                    <div className="grid grid-cols-2 gap-4">
                                        <div>
                                            <label className="text-xs font-medium text-secondary block mb-1.5 flex items-center gap-1.5">
                                                <span className="text-blue-400">⚡</span> Event Trigger Key
                                            </label>
                                            <input
                                                className="input-modern text-sm font-mono"
                                                placeholder="e.g. user.registered"
                                                value={form.eventTrigger}
                                                onChange={e => setForm(f => ({ ...f, eventTrigger: e.target.value }))}
                                            />
                                        </div>
                                        <div>
                                            <label className="text-xs font-medium text-secondary block mb-1.5 flex items-center gap-1.5">
                                                <span className="text-orange-400">⏰</span> Cron Expression
                                            </label>
                                            <input
                                                className="input-modern text-sm font-mono"
                                                placeholder="e.g. 0 9 * * 1"
                                                value={form.cronExpression}
                                                onChange={e => setForm(f => ({ ...f, cronExpression: e.target.value }))}
                                            />
                                        </div>
                                    </div>
                                    <p className="text-[10px] text-muted">Provide an event trigger key, a cron expression, or both.</p>
                                </div>

                                {/* Notification content */}
                                <div>
                                    <label className="text-[11px] font-bold uppercase tracking-widest text-muted block mb-1.5">Subject</label>
                                    <input
                                        className="input-modern text-sm"
                                        placeholder="Notification subject line"
                                        value={form.subject}
                                        onChange={e => setForm(f => ({ ...f, subject: e.target.value }))}
                                    />
                                </div>

                                <div>
                                    <label className="text-[11px] font-bold uppercase tracking-widest text-muted block mb-1.5">Body</label>
                                    <textarea
                                        className="input-modern text-sm font-mono resize-none h-28"
                                        placeholder="Notification body / template…"
                                        value={form.body}
                                        onChange={e => setForm(f => ({ ...f, body: e.target.value }))}
                                    />
                                </div>

                                {/* Recipients */}
                                <div>
                                    <div className="flex items-center justify-between mb-2">
                                        <label className="text-[11px] font-bold uppercase tracking-widest text-muted">Recipients</label>
                                        <button className="text-xs text-accent-primary hover:underline" onClick={addRecipientRow}>+ Add group</button>
                                    </div>
                                    {recipientInputs.length === 0 && (
                                        <p className="text-xs text-muted italic">No recipient groups. Click "+ Add group" to add.</p>
                                    )}
                                    <div className="space-y-2">
                                        {recipientInputs.map((row, i) => (
                                            <div key={i} className="flex gap-2 items-center">
                                                <select
                                                    className="input-modern text-xs w-36 shrink-0 bg-primary cursor-pointer"
                                                    value={row.channel}
                                                    onChange={e => updateRecipientRow(i, 'channel', e.target.value)}
                                                >
                                                    {NOTIFICATION_CHANNELS.map(c => (
                                                        <option key={c.id} value={c.id}>{c.name}</option>
                                                    ))}
                                                </select>
                                                <input
                                                    className="input-modern text-xs flex-1 font-mono"
                                                    placeholder="addr1@x.com, addr2@x.com"
                                                    value={row.value}
                                                    onChange={e => updateRecipientRow(i, 'value', e.target.value)}
                                                />
                                                <button
                                                    onClick={() => removeRecipientRow(i)}
                                                    className="p-1.5 rounded-md text-muted hover:text-red-400 transition-colors shrink-0"
                                                >
                                                    <svg className="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                                                    </svg>
                                                </button>
                                            </div>
                                        ))}
                                    </div>
                                </div>
                            </div>

                            {/* Modal footer */}
                            <div className="px-6 py-4 border-t border-primary flex justify-end gap-2">
                                <button className="btn-secondary text-sm" onClick={() => setModalOpen(false)}>Cancel</button>
                                <button
                                    className="btn-primary text-sm"
                                    onClick={handleSave}
                                    disabled={saving}
                                >
                                    {saving ? (
                                        <span className="flex items-center gap-2"><span className="spinner w-3.5 h-3.5 border-white/30 border-t-white" />{editTarget ? 'Saving…' : 'Creating…'}</span>
                                    ) : (editTarget ? 'Save Changes' : 'Create Workflow')}
                                </button>
                            </div>
                        </div>
                    </div>
                </>
            )}

            {/* ─── Fire Trigger Modal ─── */}
            {triggerModalOpen && (
                <>
                    <div className="fixed inset-0 z-40 bg-black/60 backdrop-blur-sm animate-fade-in" onClick={() => setTriggerModalOpen(false)} />
                    <div className="fixed inset-0 z-50 flex items-center justify-center p-4">
                        <div className="card-elevated w-full max-w-md animate-fade-in-up shadow-2xl">
                            <div className="flex items-center justify-between px-6 py-5 border-b border-primary">
                                <div>
                                    <h2 className="font-bold text-base tracking-tight flex items-center gap-2">
                                        <span className="text-blue-400">⚡</span> Fire Event Trigger
                                    </h2>
                                    <p className="text-xs text-secondary mt-0.5">Manually send all workflows matching this key</p>
                                </div>
                                <button onClick={() => setTriggerModalOpen(false)} className="p-2 rounded-lg hover:bg-tertiary text-muted hover:text-primary transition-all">
                                    <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                                    </svg>
                                </button>
                            </div>
                            <div className="px-6 py-5 space-y-3">
                                <label className="text-[11px] font-bold uppercase tracking-widest text-muted block">Event Trigger Key</label>
                                <input
                                    autoFocus
                                    className="input-modern text-sm font-mono"
                                    placeholder="e.g. user.registered"
                                    value={triggerKey}
                                    onChange={e => setTriggerKey(e.target.value)}
                                    onKeyDown={e => { if (e.key === 'Enter') handleTrigger(); }}
                                />
                                <p className="text-[11px] text-muted">All active workflows with matching event trigger key will be queued.</p>
                            </div>
                            <div className="px-6 py-4 border-t border-primary flex justify-end gap-2">
                                <button className="btn-secondary text-sm" onClick={() => setTriggerModalOpen(false)}>Cancel</button>
                                <button
                                    className="btn-primary text-sm flex items-center gap-1.5"
                                    onClick={handleTrigger}
                                    disabled={triggering}
                                >
                                    {triggering ? (
                                        <span className="flex items-center gap-2"><span className="spinner w-3.5 h-3.5 border-white/30 border-t-white" />Firing…</span>
                                    ) : (
                                        <>
                                            <svg className="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 10V3L4 14h7v7l9-11h-7z" />
                                            </svg>
                                            Fire
                                        </>
                                    )}
                                </button>
                            </div>
                        </div>
                    </div>
                </>
            )}
        </AppShell>
    );
}
