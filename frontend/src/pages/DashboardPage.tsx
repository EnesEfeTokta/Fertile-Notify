import { useState, useCallback, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { subscriberService } from '../api/subscriberService';
import type { SubscriberProfile } from '../types/subscriber';
import ConsoleLogsModal from '../components/ConsoleLogsModal';
import ChannelSettingsModal from '../components/ChannelSettingsModal';
import { NOTIFICATION_CHANNELS } from '../constants/channels';
import AppShell from '../components/AppShell';
import { useToast } from '../components/Toast';

export default function DashboardPage() {
    const navigate = useNavigate();
    const [profile, setProfile] = useState<SubscriberProfile | null>(null);
    const [loading, setLoading] = useState(true);
    const [updating, setUpdating] = useState(false);
    const [showConsoleModal, setShowConsoleModal] = useState(false);
    const [configChannel, setConfigChannel] = useState<string | null>(null);
    const { showToast, ToastContainer } = useToast();

    const fetchData = useCallback(async () => {
        try {
            setLoading(true);
            const data = await subscriberService.getProfile();
            setProfile(data);
        } catch {
            showToast("Failed to sync dashboard.", "error");
        } finally {
            setLoading(false);
        }
    }, [showToast]);

    useEffect(() => { fetchData(); }, [fetchData]);

    const toggleChannel = async (id: string, currentlyActive: boolean) => {
        setUpdating(true);
        try {
            await subscriberService.setChannel({ channel: id, enable: !currentlyActive });
            showToast(`${id} ${!currentlyActive ? 'enabled' : 'disabled'}`);
            fetchData();
        } catch { showToast("Action failed.", "error"); }
        finally { setUpdating(false); }
    };

    const usedPct = profile?.subscription
        ? Math.min(100, ((profile.subscription.usedThisMonth ?? 0) / (profile.subscription.monthlyLimit ?? 1)) * 100)
        : 0;

    const topActions = (
        <button onClick={() => setShowConsoleModal(true)} className="btn-secondary text-xs px-3 py-1.5 flex items-center gap-2">
            <span className="w-2 h-2 rounded-full bg-green-500 animate-pulse" />
            Live Console
        </button>
    );

    return (
        <AppShell title="Dashboard" actions={topActions} companyName={profile?.companyName} plan={profile?.subscription?.plan}>
            <ToastContainer />

            {loading ? (
                <div className="flex items-center gap-3 py-16"><div className="spinner" /></div>
            ) : (
                <div className="grid grid-cols-12 gap-6 pb-12">
                    {/* --- LARGE WIDGET: SUB SUMMARY --- */}
                    <div className="col-span-12 xl:col-span-8">
                        <div className="card p-8 h-full flex flex-col justify-between group overflow-hidden relative">
                            <div className="absolute top-0 right-0 w-64 h-64 bg-accent-primary/5 blur-[100px] -mr-32 -mt-32 pointer-events-none" />
                            <div className="relative z-10">
                                <h3 className="text-xs font-bold uppercase tracking-widest text-muted mb-6">Service Status</h3>
                                <div className="grid grid-cols-1 sm:grid-cols-4 gap-8">
                                    <div>
                                        <p className="text-sm text-secondary mb-1">Monthly Usage</p>
                                        <div className="flex items-baseline gap-2">
                                            <span className="text-4xl font-display font-bold text-primary">{(profile?.subscription?.usedThisMonth ?? 0).toLocaleString()}</span>
                                            <span className="text-tertiary text-sm">/ {(profile?.subscription?.monthlyLimit ?? 0).toLocaleString()}</span>
                                        </div>
                                        <div className="mt-4 h-2 bg-tertiary rounded-full overflow-hidden w-full max-w-[200px]">
                                            <div className="h-full bg-accent-primary transition-all duration-1000" style={{ width: `${usedPct}%` }} />
                                        </div>
                                        <p className="text-[10px] text-tertiary mt-2">Monthly subscription credits</p>
                                    </div>
                                    <div>
                                        <p className="text-sm text-secondary mb-1">Extra Credits</p>
                                        <div className="flex items-baseline gap-2">
                                            <span className="text-4xl font-display font-bold text-accent-primary">{(profile?.extraCredits ?? 0).toLocaleString()}</span>
                                            <span className="text-tertiary text-sm">Available</span>
                                        </div>
                                        <button 
                                            onClick={() => navigate("/buy-credits")}
                                            className="mt-4 text-[11px] font-bold text-accent-primary hover:underline flex items-center gap-1"
                                        >
                                            Buy More Credits →
                                        </button>
                                    </div>
                                    <div>
                                        <p className="text-sm text-secondary mb-1">Current Plan</p>
                                        <p className="text-3xl font-display font-bold text-primary capitalize">{profile?.subscription?.plan}</p>
                                        <p className="text-xs text-accent-primary mt-2 flex items-center gap-1">
                                            <svg className="w-3 h-3" fill="currentColor" viewBox="0 0 20 20"><path d="M5 4a2 2 0 012-2h6a2 2 0 012 2v14l-5-2.5L5 18V4z" /></svg>
                                            Active Subscriber
                                        </p>
                                    </div>
                                    <div>
                                        <p className="text-sm text-secondary mb-1">Renews In</p>
                                        <p className="text-3xl font-display font-bold text-primary">
                                            {profile?.subscription?.expiresAt ? Math.ceil((new Date(profile.subscription.expiresAt).getTime() - Date.now()) / (1000 * 60 * 60 * 24)) : 0} Days
                                        </p>
                                        <p className="text-[11px] text-tertiary mt-2">Cycle ends on {profile?.subscription?.expiresAt ? new Date(profile.subscription.expiresAt).toLocaleDateString() : 'N/A'}</p>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    {/* --- SIDE WIDGET: QUICK STATS --- */}
                    <div className="col-span-12 xl:col-span-4">
                        <div className="card p-6 h-full bg-gradient-to-br from-secondary/50 to-primary border-accent-primary/10">
                            <h3 className="text-xs font-bold uppercase tracking-widest text-muted mb-4">Quick Health</h3>
                            <div className="space-y-4">
                                <div className="flex items-center justify-between p-3 rounded-lg bg-tertiary/50 border border-primary">
                                    <span className="text-sm text-secondary">Active Channels</span>
                                    <span className="text-sm font-bold text-primary">{profile?.activeChannels.length} / {NOTIFICATION_CHANNELS.length}</span>
                                </div>
                                <div className="flex items-center justify-between p-3 rounded-lg bg-tertiary/50 border border-primary">
                                    <span className="text-sm text-secondary">System Latency</span>
                                    <span className="text-sm font-bold text-green-400">14ms</span>
                                </div>
                                <div className="flex items-center justify-between p-3 rounded-lg bg-tertiary/50 border border-primary text-tertiary">
                                    <span className="text-sm border-b border-dashed border-tertiary cursor-help" title="Rolling 30-day average">Uptime</span>
                                    <span className="text-sm font-bold">99.99%</span>
                                </div>
                            </div>
                            <div className="flex flex-col gap-2 mt-6">
                                <button onClick={() => navigate("/statistics")} className="w-full py-2 bg-tertiary border border-primary rounded-lg text-xs font-bold hover:bg-hover transition-colors">
                                    View Full Analytics →
                                </button>
                                <button onClick={() => navigate("/logs")} className="w-full py-2 border border-accent-primary/20 rounded-lg text-xs font-bold text-accent-primary hover:bg-accent-primary/5 transition-colors">
                                    Check Delivery Logs →
                                </button>
                            </div>
                        </div>
                    </div>

                    {/* --- FULL WIDTH: CHANNELS GRID --- */}
                    <div className="col-span-12">
                        <div className="flex items-end justify-between mb-4 mt-4">
                            <div>
                                <h3 className="text-xs font-bold uppercase tracking-widest text-muted">Delivery Channels</h3>
                                <p className="text-sm text-secondary mt-1">Configure and monitor your active notification pipelines</p>
                            </div>
                        </div>
                        <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-6 gap-4">
                            {NOTIFICATION_CHANNELS.map(channel => {
                                const active = profile?.activeChannels.includes(channel.id);
                                return (
                                    <div key={channel.id} className={`card p-5 group flex flex-col items-center text-center gap-3 transition-all relative overflow-hidden ${active ? 'border-green-500/30 bg-green-500/5' : 'hover:border-hover'}`}>
                                        <div className="text-3xl filter saturate-50 group-hover:saturate-100 transition-all">{channel.icon}</div>
                                        <div>
                                            <p className="text-xs font-bold text-primary truncate w-full">{channel.name}</p>
                                            <p className={`text-[10px] font-bold mt-1 ${active ? 'text-green-400' : 'text-tertiary'}`}>
                                                {active ? 'CONNECTED' : 'INACTIVE'}
                                            </p>
                                        </div>
                                        <div className="flex gap-2 w-full mt-2">
                                            <button
                                                disabled={updating}
                                                onClick={() => toggleChannel(channel.id, !!active)}
                                                className={`flex-1 py-1.5 rounded-md text-[10px] font-bold transition-colors ${active ? 'bg-red-500/10 text-red-400 hover:bg-red-500/20' : 'bg-green-500/10 text-green-400 hover:bg-green-500/20'}`}
                                            >
                                                {active ? 'Disable' : 'Enable'}
                                            </button>
                                            <button
                                                onClick={() => setConfigChannel(channel.id)}
                                                className="px-2 py-1.5 bg-tertiary rounded-md hover:text-primary transition-colors"
                                            >
                                                <svg className="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z" /><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                                                </svg>
                                            </button>
                                        </div>
                                    </div>
                                );
                            })}
                        </div>
                    </div>
                </div>
            )}

            <ChannelSettingsModal isOpen={!!configChannel} onClose={() => setConfigChannel(null)} channelId={configChannel || ''} />
            <ConsoleLogsModal isOpen={showConsoleModal} onClose={() => setShowConsoleModal(false)} />
        </AppShell>
    );
}