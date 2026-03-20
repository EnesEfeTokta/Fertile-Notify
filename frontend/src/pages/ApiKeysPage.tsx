import { useState, useCallback, useEffect } from 'react';
import { subscriberService } from '../api/subscriberService';
import type { ApiKey, SubscriberProfile } from '../types/subscriber';
import AppShell from '../components/AppShell';
import { useToast } from '../components/Toast';

export default function ApiKeysPage() {
    const [apiKeys, setApiKeys] = useState<ApiKey[]>([]);
    const [targetProfile, setTargetProfile] = useState<SubscriberProfile | null>(null);
    const [loading, setLoading] = useState(true);
    const [updating, setUpdating] = useState(false);
    const [apiKeyName, setApiKeyName] = useState("");
    const [newKeyHash, setNewKeyHash] = useState<string | null>(null);
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

                        <div className="space-y-4">
                            <h3 className="text-xs font-bold uppercase tracking-widest text-muted flex items-center justify-between px-1">
                                Active Keys 
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
                                        <div key={key.id} className="card p-4 flex items-center justify-between group hover:border-accent-primary/30 transition-all bg-secondary/20">
                                            <div className="flex items-center gap-3 overflow-hidden">
                                                <div className="w-8 h-8 rounded-lg bg-primary flex items-center justify-center text-[10px] font-bold text-tertiary border border-primary shrink-0">KEY</div>
                                                <div className="min-w-0">
                                                    <p className="font-bold text-primary text-sm truncate">{key.name}</p>
                                                    <p className="text-[11px] font-mono text-accent-light opacity-80">{key.prefix}••••••••</p>
                                                </div>
                                            </div>
                                            <button
                                                onClick={() => handleDelete(key.id, key.name)}
                                                className="p-2 text-red-400/50 hover:text-red-400 hover:bg-red-500/10 rounded-lg transition-all opacity-0 group-hover:opacity-100"
                                                title="Revoke Key"
                                            >
                                                <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                                                </svg>
                                            </button>
                                        </div>
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
                                    Our REST API is the fastest way to trigger notifications from your backend. 
                                    Integrate with a single endpoint and let us handle the multi-channel delivery.
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
