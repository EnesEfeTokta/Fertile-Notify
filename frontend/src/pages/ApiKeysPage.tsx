import { useState, useCallback, useEffect } from 'react';
import { subscriberService } from '../api/subscriberService';
import type { ApiKey, SubscriberProfile } from '../types/subscriber';
import AppShell from '../components/AppShell';
import { useToast } from '../components/Toast';

const API_KEY_SCOPES = [
    { id: 'notifications:send', label: 'Send Notifications', description: 'Allows this API key to trigger multi-channel notifications.' },
    { id: 'workflow:trigger', label: 'Trigger Workflows', description: 'Allows this API key to start automation and workflow flows.' },
    { id: 'mcp:usage', label: 'Use MCP', description: 'Allows this API key to use MCP-based integration calls.' },
];

const normalizeScopes = (scopes?: string | null) =>
    (scopes ?? '')
        .split(',')
        .map(scope => scope.trim())
        .filter(Boolean);

export default function ApiKeysPage() {
    const [apiKeys, setApiKeys] = useState<ApiKey[]>([]);
    const [targetProfile, setTargetProfile] = useState<SubscriberProfile | null>(null);
    const [loading, setLoading] = useState(true);
    const [updating, setUpdating] = useState(false);
    const [apiKeyName, setApiKeyName] = useState("");
    const [newKeyHash, setNewKeyHash] = useState<string | null>(null);
    const [selectedApiKeyId, setSelectedApiKeyId] = useState<string | null>(null);
    const [editingScopes, setEditingScopes] = useState<string[]>([]);
    const [editingIsActive, setEditingIsActive] = useState(true);
    const [savingKeyId, setSavingKeyId] = useState<string | null>(null);
    const { showToast, ToastContainer } = useToast();

    const loadData = useCallback(async () => {
        try {
            setLoading(true);
            const [keys, profile] = await Promise.all([
                subscriberService.getApiKeys(),
                subscriberService.getProfile()
            ]);
            setApiKeys(keys);
            setTargetProfile(profile);
        } catch {
            showToast("Failed to sync API keys.", "error");
        } finally {
            setLoading(false);
        }
    }, [showToast]);

    useEffect(() => { loadData(); }, [loadData]);

    useEffect(() => {
        if (apiKeys.length === 0) return;

        const selectedExists = selectedApiKeyId
            ? apiKeys.some(key => key.id === selectedApiKeyId)
            : false;

        if (!selectedApiKeyId || !selectedExists) {
            setSelectedApiKeyId(apiKeys[0].id);
        }
    }, [apiKeys, selectedApiKeyId]);

    useEffect(() => {
        const selectedKey = apiKeys.find(key => key.id === selectedApiKeyId);
        if (!selectedKey) return;

        setEditingScopes(normalizeScopes(selectedKey.scopes));
        setEditingIsActive(selectedKey.isActive);
    }, [apiKeys, selectedApiKeyId]);

    const handleCreate = async () => {
        if (!apiKeyName.trim()) return;
        setUpdating(true);
        try {
            const resp = await subscriberService.setApikey({ name: apiKeyName });
            setNewKeyHash(resp.apiKey);
            setApiKeyName("");
            loadData();
        } catch { showToast("Error creating key.", "error"); }
        finally { setUpdating(false); }
    };

    const handleDelete = async (id: string, name: string) => {
        if (!window.confirm(`Revoke key "${name}"?`)) return;
        try {
            await subscriberService.deleteApiKey(id);
            showToast("Key revoked.");
            loadData();
        } catch { showToast("Error deleting key.", "error"); }
    };

    const handleSaveSelectedKey = async () => {
        const selectedKey = apiKeys.find(key => key.id === selectedApiKeyId);
        if (!selectedKey) return;

        setSavingKeyId(selectedKey.id);
        try {
            await Promise.all([
                subscriberService.updateApiKeyScopes(selectedKey.id, { scopes: editingScopes }),
                subscriberService.updateApiKeyStatus(selectedKey.id, { isActive: editingIsActive }),
            ]);
            showToast("API key updated.");
            await loadData();
        } catch {
            showToast("Failed to update API key.", "error");
        } finally {
            setSavingKeyId(null);
        }
    };

    const toggleScope = (scopeId: string) => {
        setEditingScopes(current =>
            current.includes(scopeId)
                ? current.filter(scope => scope !== scopeId)
                : [...current, scopeId]
        );
    };

    const selectedApiKey = apiKeys.find(key => key.id === selectedApiKeyId) ?? null;

    return (
        <AppShell title="API Keys" companyName={targetProfile?.companyName} plan={targetProfile?.subscription?.plan}>
            <ToastContainer />

            {newKeyHash && (
                <div className="fixed inset-0 bg-black/80 backdrop-blur-sm flex items-center justify-center z-50 px-4">
                    <div className="card max-w-lg w-full p-8 space-y-6 animate-slide-up">
                        <div className="text-center">
                            <div className="w-12 h-12 rounded-xl bg-green-500/10 border border-green-500/20 flex items-center justify-center mx-auto mb-4 text-2xl">🔑</div>
                            <h2 className="font-display font-bold text-xl">API Key Created</h2>
                            <p className="text-sm text-secondary mt-1">Copy this key now. It will not be shown again.</p>
                        </div>
                        <div className="code-block p-4 text-sm font-mono break-all text-accent-light select-all">
                            {newKeyHash}
                        </div>
                        <div className="flex gap-3">
                            <button onClick={() => { navigator.clipboard.writeText(newKeyHash); showToast("Copied!"); }} className="btn-primary flex-1 py-3">Copy Key</button>
                            <button onClick={() => setNewKeyHash(null)} className="btn-secondary flex-1">I've saved it</button>
                        </div>
                    </div>
                </div>
            )}

            <div className="max-w-[1600px] pb-20">
                <div className="flex flex-col xl:flex-row gap-12 items-start">
                    
                    {/* LEFT SIDE: Management */}
                    <div className="xl:w-[420px] shrink-0 space-y-8">
                        <div>
                            <h2 className="text-2xl font-display font-bold text-primary">API Access</h2>
                            <p className="text-sm text-secondary mt-1 leading-relaxed">
                                Manage your security credentials for external integrations.
                            </p>
                        </div>

                        <div className="card p-6 border-accent-primary/20 bg-accent-dim/10 relative overflow-hidden group">
                            <div className="absolute top-0 right-0 p-4 opacity-10 select-none pointer-events-none text-6xl group-hover:scale-110 transition-transform">🔑</div>
                            <h3 className="font-bold text-primary text-sm mb-4">Generate New Key</h3>
                            <div className="space-y-4">
                                <div className="space-y-1.5">
                                    <label className="text-[10px] font-bold uppercase tracking-widest text-muted ml-1">Key Label</label>
                                    <input
                                        placeholder="e.g. Production Mobile App"
                                        className="input-modern w-full"
                                        value={apiKeyName}
                                        onChange={e => setApiKeyName(e.target.value)}
                                    />
                                </div>
                                <button
                                    className="btn-primary w-full py-3 shadow-lg shadow-accent-primary/20"
                                    onClick={handleCreate}
                                    disabled={updating || !apiKeyName.trim()}
                                >
                                    {updating ? "Creating..." : "Generate Secret Key"}
                                </button>
                                <p className="text-[10px] text-center text-tertiary">
                                    Keys grant full access. Never share them publicly.
                                </p>
                            </div>
                        </div>

                        <div className="card p-6 space-y-5 border-primary/70 bg-secondary/30">
                            <div className="flex items-start justify-between gap-4">
                                <div>
                                    <h3 className="font-bold text-primary text-sm">Selected Key Controls</h3>
                                    <p className="text-[11px] text-tertiary mt-1">
                                        Choose a key below and update its scopes or active state.
                                    </p>
                                </div>
                                <div className="text-right">
                                    <p className="text-[10px] uppercase tracking-widest text-muted">Current</p>
                                    <p className="text-xs font-mono text-accent-light">{selectedApiKey ? selectedApiKey.prefix : '—'}</p>
                                </div>
                            </div>

                            <div className="space-y-1.5">
                                <label className="text-[10px] font-bold uppercase tracking-widest text-muted ml-1">Select API Key</label>
                                <select
                                    className="input-modern w-full"
                                    value={selectedApiKeyId ?? ''}
                                    onChange={e => setSelectedApiKeyId(e.target.value)}
                                    disabled={apiKeys.length === 0}
                                >
                                    {apiKeys.map(key => (
                                        <option key={key.id} value={key.id}>
                                            {key.name} {key.isActive ? '(Active)' : '(Inactive)'}
                                        </option>
                                    ))}
                                </select>
                            </div>

                            {selectedApiKey ? (
                                <div className="space-y-4">
                                    <div className="rounded-xl border border-primary bg-primary/10 p-4 space-y-3">
                                        <div className="flex items-center justify-between gap-3">
                                            <div className="min-w-0">
                                                <p className="text-sm font-bold text-primary truncate">{selectedApiKey.name}</p>
                                                <p className="text-[11px] font-mono text-accent-light break-all">{selectedApiKey.prefix}••••••••</p>
                                            </div>
                                            <button
                                                onClick={() => handleDelete(selectedApiKey.id, selectedApiKey.name)}
                                                className="text-[11px] px-3 py-1.5 rounded-lg border border-red-500/20 text-red-400 hover:bg-red-500/10 transition-colors"
                                            >
                                                Delete
                                            </button>
                                        </div>

                                        <label className="flex items-center justify-between gap-3 rounded-lg bg-secondary/60 border border-primary px-3 py-2 cursor-pointer">
                                            <div>
                                                <p className="text-sm font-semibold text-primary">Active</p>
                                                <p className="text-[11px] text-tertiary">Inactive keys cannot authenticate requests.</p>
                                            </div>
                                            <input
                                                type="checkbox"
                                                checked={editingIsActive}
                                                onChange={e => setEditingIsActive(e.target.checked)}
                                                className="w-5 h-5 accent-[var(--color-accent-primary)]"
                                            />
                                        </label>
                                    </div>

                                    <div className="space-y-3">
                                        <div>
                                            <h4 className="text-[10px] font-bold uppercase tracking-widest text-muted">Scopes</h4>
                                            <p className="text-[11px] text-tertiary mt-1">
                                                Set what this API key is allowed to do.
                                            </p>
                                        </div>

                                        <div className="space-y-2">
                                            {API_KEY_SCOPES.map(scope => {
                                                const checked = editingScopes.includes(scope.id);
                                                return (
                                                    <label
                                                        key={scope.id}
                                                        className={`block rounded-xl border p-3 cursor-pointer transition-colors ${checked ? 'border-accent-primary bg-accent-dim/20' : 'border-primary bg-secondary/20 hover:border-accent-primary/30'}`}
                                                    >
                                                        <div className="flex items-start gap-3">
                                                            <input
                                                                type="checkbox"
                                                                checked={checked}
                                                                onChange={() => toggleScope(scope.id)}
                                                                className="mt-1 w-4 h-4 accent-[var(--color-accent-primary)]"
                                                            />
                                                            <div className="min-w-0">
                                                                <p className="text-sm font-semibold text-primary">{scope.label}</p>
                                                                <p className="text-[11px] text-tertiary leading-relaxed mt-1">{scope.description}</p>
                                                                <p className="text-[10px] font-mono text-accent-light mt-2">{scope.id}</p>
                                                            </div>
                                                        </div>
                                                    </label>
                                                );
                                            })}
                                        </div>

                                        <button
                                            className="btn-primary w-full py-3 shadow-lg shadow-accent-primary/20"
                                            onClick={handleSaveSelectedKey}
                                            disabled={savingKeyId === selectedApiKey.id}
                                        >
                                            {savingKeyId === selectedApiKey.id ? 'Saving...' : 'Save Scopes & Status'}
                                        </button>

                                        <div className="rounded-xl bg-black/20 border border-primary p-3">
                                            <p className="text-[10px] uppercase tracking-widest text-muted mb-2">Current Scopes</p>
                                            <div className="flex flex-wrap gap-2">
                                                {normalizeScopes(selectedApiKey.scopes).length > 0 ? normalizeScopes(selectedApiKey.scopes).map(scope => (
                                                    <span key={scope} className="px-2.5 py-1 rounded-full bg-accent-dim/20 text-accent-light text-[10px] font-bold border border-accent-primary/20">
                                                        {API_KEY_SCOPES.find(item => item.id === scope)?.label ?? scope}
                                                    </span>
                                                )) : (
                                                    <span className="text-[11px] text-tertiary">No scopes configured yet.</span>
                                                )}
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            ) : (
                                <div className="rounded-xl border border-dashed border-primary p-6 text-center">
                                    <p className="text-sm text-tertiary">Select a key from the list or dropdown to edit its scopes and status.</p>
                                </div>
                            )}
                        </div>

                        <div className="space-y-4">
                            <h3 className="text-xs font-bold uppercase tracking-widest text-muted flex items-center justify-between px-1">
                                Your API Keys 
                                <span className="bg-primary/50 px-2 py-0.5 rounded text-[10px] font-mono">{apiKeys.length}</span>
                            </h3>
                            {loading ? (
                                <div className="flex items-center gap-3 py-10 justify-center">
                                    <div className="spinner" />
                                    <span className="text-sm text-secondary">Loading...</span>
                                </div>
                            ) : apiKeys.length === 0 ? (
                                <div className="card p-12 text-center border-dashed border-primary/40 bg-transparent">
                                    <p className="text-sm text-tertiary">No keys found.</p>
                                </div>
                            ) : (
                                <div className="space-y-3">
                                    {apiKeys.map(key => (
                                        <button
                                            key={key.id}
                                            onClick={() => setSelectedApiKeyId(key.id)}
                                            className={`card w-full p-4 flex items-center justify-between group hover:border-accent-primary/30 transition-all text-left ${selectedApiKeyId === key.id ? 'border-accent-primary bg-accent-dim/15' : ''} ${key.isActive ? "bg-secondary/20" : "bg-secondary/5 opacity-60 grayscale-[0.5]"}`}
                                        >
                                            <div className="flex items-center gap-3 overflow-hidden min-w-0">
                                                <div className={`w-8 h-8 rounded-lg flex items-center justify-center text-[10px] font-bold border shrink-0 ${key.isActive ? "bg-primary text-tertiary border-primary" : "bg-secondary text-muted border-secondary"}`}>
                                                    {key.isActive ? "ACT" : "PAS"}
                                                </div>
                                                <div className="min-w-0">
                                                    <div className="flex items-center gap-2 flex-wrap">
                                                        <p className="font-bold text-primary text-sm truncate">{key.name}</p>
                                                        {!key.isActive && <span className="text-[9px] px-1.5 py-0.5 rounded-full bg-red-500/10 text-red-500 font-bold border border-red-500/20">INACTIVE</span>}
                                                        {key.isActive && <span className="text-[9px] px-1.5 py-0.5 rounded-full bg-green-500/10 text-green-500 font-bold border border-green-500/20 uppercase tracking-tighter">Active</span>}
                                                    </div>
                                                    <p className="text-[11px] font-mono text-accent-light opacity-80">{key.prefix}••••••••</p>
                                                    <div className="flex flex-wrap gap-1.5 mt-2">
                                                        {normalizeScopes(key.scopes).length > 0 ? normalizeScopes(key.scopes).map(scope => (
                                                            <span key={scope} className="text-[9px] px-2 py-0.5 rounded-full bg-black/20 text-accent-light border border-primary">
                                                                {API_KEY_SCOPES.find(item => item.id === scope)?.label ?? scope}
                                                            </span>
                                                        )) : (
                                                            <span className="text-[10px] text-tertiary">No scopes</span>
                                                        )}
                                                    </div>
                                                </div>
                                            </div>
                                            <div className="flex items-center gap-2">
                                                <span className="text-[10px] text-muted uppercase tracking-widest hidden sm:inline">Manage</span>
                                                <svg className="w-4 h-4 text-muted group-hover:text-primary transition-colors" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5l7 7-7 7" />
                                                </svg>
                                            </div>
                                        </button>
                                    ))}
                                </div>
                            )}
                        </div>
                    </div>

                    {/* RIGHT SIDE: Documentation */}
                    <div className="flex-1 space-y-8 xl:border-l xl:border-primary/30 xl:pl-12">
                        <section className="space-y-6">
                            <div className="max-w-3xl">
                                <h2 className="text-2xl font-display font-bold text-primary">Integration Guide</h2>
                                    <p className="text-sm text-secondary mt-2 leading-relaxed">
                                    Manage notification sending, workflow triggering, and MCP integrations from the REST API.
                                    Control API key scopes and active state from one panel.
                                </p>
                            </div>

                            <div className="space-y-6">
                                {/* Base details */}
                                <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                                    <div className="space-y-3">
                                        <h4 className="text-[11px] font-bold text-muted uppercase tracking-widest">Authentication</h4>
                                        <div className="card-elevated p-4 bg-black/40 border-primary/50 relative group">
                                            <code className="text-[12px] text-accent-light font-mono break-all leading-relaxed font-bold">
                                                Authorization: Bearer <span className="text-white">YOUR_API_KEY</span>
                                            </code>
                                        </div>
                                        <p className="text-[11px] text-tertiary">All requests must be made over HTTPS.</p>
                                    </div>
                                    <div className="space-y-3">
                                        <h4 className="text-[11px] font-bold text-muted uppercase tracking-widest">Base Endpoint</h4>
                                        <div className="flex items-center gap-3">
                                            <span className="px-2 py-1 bg-green-500/10 text-green-400 text-[10px] font-bold rounded border border-green-500/20">POST</span>
                                            <code className="text-[13px] text-primary font-mono font-bold">/api/notifications</code>
                                        </div>
                                        <p className="text-[11px] text-tertiary">Primary endpoint for sending notifications.</p>
                                    </div>
                                </div>

                                <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                                    {API_KEY_SCOPES.map(scope => (
                                        <div key={scope.id} className="p-4 rounded-xl bg-secondary/20 border border-primary space-y-2">
                                            <p className="text-sm font-semibold text-primary">{scope.label}</p>
                                            <p className="text-[11px] text-tertiary leading-relaxed">{scope.description}</p>
                                            <p className="text-[10px] font-mono text-accent-light">{scope.id}</p>
                                        </div>
                                    ))}
                                </div>

                                {/* Main Docs Content */}
                                <div className="space-y-4">
                                    <div className="flex items-center justify-between">
                                        <h4 className="text-[11px] font-bold text-muted uppercase tracking-widest">Example Payload</h4>
                                        <button 
                                            onClick={() => {
                                                const json = JSON.stringify({
                                                    eventType: "OrderCreated",
                                                    parameters: { AppName: "MyApp", Username: "Alex", Code: "#5090" },
                                                    to: [
                                                        { channel: "email", recipients: ["user1@test.com"] },
                                                        { channel: "sms", recipients: ["+012222222222"] }
                                                    ]
                                                }, null, 2);
                                                navigator.clipboard.writeText(json);
                                                showToast('JSON Copied!');
                                            }}
                                            className="text-[11px] text-accent-primary hover:text-accent-light font-bold uppercase tracking-widest transition-colors flex items-center gap-1.5"
                                        >
                                            <svg className="w-3 h-3" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path d="M8 5H6a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2v-1M8 5a2 2 0 002 2h2a2 2 0 002-2M8 5a2 2 0 012-2h2a2 2 0 012 2m0 0h2a2 2 0 012 2v3m2 4H10m0 0l3-3m-3 3l3 3" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/></svg>
                                            Copy JSON
                                        </button>
                                    </div>
                                    
                                    <div className="grid grid-cols-1 lg:grid-cols-12 gap-6">
                                        {/* Code Block */}
                                        <div className="lg:col-span-7">
                                            <div className="code-block p-6 text-[12px] bg-black/60 rounded-2xl border border-primary shadow-inner">
                                                <pre className="font-mono leading-6 text-accent-light whitespace-pre-wrap">
{`{
  "eventType": "OrderCreated",
  "parameters": {
    "AppName": "MyApp",
    "Username": "Alex",
    "Code": "#5090"
  },
  "to": [
    {
      "channel": "email",
      "recipients": [
        "user1@test.com", "user2@abc.com"
      ]
    },
    {
      "channel": "sms",
      "recipients": [
        "+011111111111", "+012222222222"
      ]
    }
  ]
}`}
                                                </pre>
                                            </div>
                                        </div>

                                        {/* Explanations */}
                                        <div className="lg:col-span-5 space-y-4">
                                            <div className="p-4 rounded-xl bg-primary/20 border border-primary/50 group hover:border-accent-primary/50 transition-colors">
                                                <h5 className="text-[10px] font-bold text-accent-light uppercase tracking-widest mb-1">eventType</h5>
                                                <p className="text-[11px] text-secondary leading-relaxed">
                                                    Maps to the <span className="text-primary font-bold">Event Type</span> in your templates. 
                                                    Determines which template content will be used.
                                                </p>
                                            </div>
                                            <div className="p-4 rounded-xl bg-primary/20 border border-primary/50 group hover:border-accent-primary/50 transition-colors">
                                                <h5 className="text-[10px] font-bold text-accent-light uppercase tracking-widest mb-1">parameters</h5>
                                                <p className="text-[11px] text-secondary leading-relaxed">
                                                    Dictionary of dynamic variables (e.g. <code>{"{{AppName}}"}</code>). 
                                                    Key names must match the placeholders in your template.
                                                </p>
                                            </div>
                                            <div className="p-4 rounded-xl bg-primary/20 border border-primary/50 group hover:border-accent-primary/50 transition-colors">
                                                <h5 className="text-[10px] font-bold text-accent-light uppercase tracking-widest mb-1">to (Targeting)</h5>
                                                <p className="text-[11px] text-secondary leading-relaxed">
                                                    Specify an array of targets. Each target has a <code>channel</code> (email, sms, etc.) 
                                                    and an array of <code>recipients</code>.
                                                </p>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </section>
                    </div>
                </div>
            </div>
        </AppShell>
    );
}
