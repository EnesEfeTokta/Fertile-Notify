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

            <div className="max-w-4xl space-y-8">
                <div className="card p-6 border-accent-primary/20 bg-accent-dim/10">
                    <div className="flex items-start gap-4">
                        <div className="w-10 h-10 rounded-lg bg-accent-primary/10 flex items-center justify-center text-accent-primary text-xl shrink-0">🛠️</div>
                        <div className="flex-1">
                            <h3 className="font-bold text-primary">Developer Credentials</h3>
                            <p className="text-xs text-secondary mb-4 leading-relaxed">
                                Use these keys to authenticate your requests to the Fertile Notify API.
                                Treat keys as passwords - never commit them to version control.
                            </p>
                            <div className="flex gap-2">
                                <input
                                    placeholder="Key name (e.g. Production)"
                                    className="input-modern"
                                    value={apiKeyName}
                                    onChange={e => setApiKeyName(e.target.value)}
                                />
                                <button
                                    className="btn-primary px-6"
                                    onClick={handleCreate}
                                    disabled={updating || !apiKeyName.trim()}
                                >
                                    {updating ? "Creating..." : "Generate Key"}
                                </button>
                            </div>
                        </div>
                    </div>
                </div>

                <div className="space-y-4">
                    <h3 className="text-xs font-bold uppercase tracking-widest text-muted">Active Keys ({apiKeys.length})</h3>
                    {loading ? (
                        <div className="spinner mt-4" />
                    ) : apiKeys.length === 0 ? (
                        <div className="card p-8 text-center text-sm text-tertiary border-dashed">No active API keys found.</div>
                    ) : (
                        <div className="grid gap-3">
                            {apiKeys.map(key => (
                                <div key={key.id} className="card p-4 flex items-center justify-between group hover:border-hover transition-colors">
                                    <div className="flex items-center gap-4">
                                        <div className="w-10 h-10 rounded-full bg-tertiary flex items-center justify-center text-xs font-bold text-tertiary">KEY</div>
                                        <div>
                                            <p className="font-bold text-primary">{key.name}</p>
                                            <div className="flex items-center gap-2 mt-0.5">
                                                <span className="text-xs font-mono text-tertiary">{key.prefix}••••••••</span>
                                                <span className="text-[10px] text-muted">• Created {new Date(key.createdAt).toLocaleDateString()}</span>
                                            </div>
                                        </div>
                                    </div>
                                    <button
                                        onClick={() => handleDelete(key.id, key.name)}
                                        className="btn-ghost text-red-400 opacity-0 group-hover:opacity-100 transition-opacity"
                                    >
                                        Revoke
                                    </button>
                                </div>
                            ))}
                        </div>
                    )}
                </div>
            </div>
        </AppShell>
    );
}
