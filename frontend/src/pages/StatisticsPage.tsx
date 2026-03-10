import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { statisticService } from '../api/statisticService';
import type { StatisticsResponse } from '../types/statistic';
import { getChannelMetadata } from '../constants/channels';
import AppShell from '../components/AppShell';

type Period = 'Daily' | 'Weekly' | '1Month' | '3Months' | '6Months' | '1Year';

interface PeriodConfig { key: Period; label: string; shortLabel: string; }

const ALL_PERIODS: PeriodConfig[] = [
    { key: 'Daily', label: 'Daily', shortLabel: '1D' },
    { key: 'Weekly', label: 'Weekly', shortLabel: '1W' },
    { key: '1Month', label: '1 Month', shortLabel: '1M' },
    { key: '3Months', label: '3 Months', shortLabel: '3M' },
    { key: '6Months', label: '6 Months', shortLabel: '6M' },
    { key: '1Year', label: '1 Year', shortLabel: '1Y' },
];

function getAllowedPeriods(plan: string | undefined): Period[] {
    switch (plan?.toLowerCase()) {
        case 'enterprise': return ['Daily', 'Weekly', '1Month', '3Months', '6Months', '1Year'];
        case 'pro': return ['Daily', 'Weekly', '1Month', '3Months'];
        default: return ['Daily', 'Weekly'];
    }
}

function getChannelColor(channel: string): string {
    return getChannelMetadata(channel).color;
}

function getEventColor(index: number): string {
    const palette = ['#3b82f6', '#8b5cf6', '#f59e0b', '#10b981', '#ef4444', '#ec4899', '#06b6d4', '#84cc16'];
    return palette[index % palette.length];
}

// ---- Sub-components ----

const KpiCard = ({ label, value, sub, color, icon }: {
    label: string; value: string | number; sub?: string; color: string; icon: React.ReactNode;
}) => (
    <div className="card p-5 space-y-3">
        <div className="flex items-center justify-between">
            <span className="text-xs font-bold uppercase tracking-widest text-muted">{label}</span>
            <div className={`w-8 h-8 rounded-lg flex items-center justify-center`} style={{ background: `${color}18` }}>
                <span style={{ color }}>{icon}</span>
            </div>
        </div>
        <p className="font-display font-bold text-3xl tracking-tight" style={{ color }}>{value}</p>
        {sub && <p className="text-xs text-tertiary">{sub}</p>}
    </div>
);

export default function StatisticsPage() {
    const navigate = useNavigate();
    const [stats, setStats] = useState<StatisticsResponse | null>(null);
    const [selectedPeriod, setSelectedPeriod] = useState<Period>('Daily');
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const allowedPeriods = getAllowedPeriods(stats?.subscriptionSummary?.plan);

    const fetchStats = React.useCallback(async (period: Period) => {
        try {
            setLoading(true);
            setError(null);
            const data = await statisticService.getStatistics(period);
            setStats(data);
        } catch {
            setError('Failed to load statistics. Please try again.');
        } finally {
            setLoading(false);
        }
    }, []);

    useEffect(() => { fetchStats(selectedPeriod); }, [selectedPeriod, fetchStats]);

    const handlePeriodChange = (period: Period) => {
        if (allowedPeriods.includes(period)) setSelectedPeriod(period);
    };

    const usageStats = stats?.usageStats;
    const successRate = usageStats && usageStats.totalUsage > 0
        ? ((usageStats.successCount / usageStats.totalUsage) * 100).toFixed(1) : '0.0';
    const failRate = usageStats && usageStats.totalUsage > 0
        ? ((usageStats.failedCount / usageStats.totalUsage) * 100).toFixed(1) : '0.0';
    const maxChannelValue = usageStats ? Math.max(...Object.values(usageStats.statsByChannel), 1) : 1;
    const maxEventValue = usageStats ? Math.max(...Object.values(usageStats.statsByEventType), 1) : 1;

    const periodSelector = (
        <div className="flex gap-1 bg-secondary border border-primary rounded-lg p-1">
            {ALL_PERIODS.map(p => {
                const isAllowed = allowedPeriods.includes(p.key);
                const isActive = selectedPeriod === p.key;
                return (
                    <button
                        key={p.key}
                        onClick={() => handlePeriodChange(p.key)}
                        disabled={!isAllowed}
                        title={!isAllowed ? `Upgrade to access ${p.label}` : p.label}
                        className={`px-3 py-1.5 text-xs font-semibold rounded-md transition-all ${isActive
                                ? 'bg-accent-primary text-white shadow-sm'
                                : isAllowed
                                    ? 'text-secondary hover:text-primary hover:bg-tertiary'
                                    : 'text-muted opacity-40 cursor-not-allowed'
                            }`}
                    >
                        <span className="hidden sm:inline">{p.label}</span>
                        <span className="sm:hidden">{p.shortLabel}</span>
                        {!isAllowed && <span className="ml-1 opacity-60">🔒</span>}
                    </button>
                );
            })}
        </div>
    );

    return (
        <AppShell
            title="Statistics"
            actions={periodSelector}
            plan={stats?.subscriptionSummary?.plan}
        >
            {/* Loading */}
            {loading && (
                <div className="flex items-center gap-3 py-16">
                    <div className="spinner" />
                    <span className="text-secondary text-sm">Fetching statistics…</span>
                </div>
            )}

            {/* Error */}
            {error && !loading && (
                <div className="card p-8 text-center space-y-4">
                    <p className="text-red-400 text-sm">{error}</p>
                    <button onClick={() => fetchStats(selectedPeriod)} className="btn-secondary text-sm">
                        Try again
                    </button>
                </div>
            )}

            {/* Content */}
            {stats && usageStats && !loading && !error && (
                <div className="space-y-6 max-w-5xl">
                    {/* KPI row */}
                    <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                        <KpiCard
                            label="Total Notifications"
                            value={usageStats.totalUsage.toLocaleString()}
                            sub="sent in this period"
                            color="#3b82f6"
                            icon={<svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 7h8m0 0v8m0-8l-8 8-4-4-6 6" /></svg>}
                        />
                        <KpiCard
                            label="Delivered"
                            value={usageStats.successCount.toLocaleString()}
                            sub={`${successRate}% success rate`}
                            color="#10b981"
                            icon={<svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" /></svg>}
                        />
                        <KpiCard
                            label="Failed"
                            value={usageStats.failedCount.toLocaleString()}
                            sub={`${failRate}% fail rate`}
                            color="#ef4444"
                            icon={<svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" /></svg>}
                        />
                    </div>

                    {/* Success / Fail bar */}
                    {usageStats.totalUsage > 0 && (
                        <div className="card p-5 space-y-3">
                            <p className="text-xs font-bold uppercase tracking-widest text-muted">Delivery Rate</p>
                            <div className="h-3 w-full bg-tertiary rounded-full overflow-hidden flex">
                                <div className="h-full bg-green-500 transition-all duration-700" style={{ width: `${successRate}%` }} />
                                <div className="h-full bg-red-500 transition-all duration-700" style={{ width: `${failRate}%` }} />
                            </div>
                            <div className="flex items-center gap-4 text-xs text-tertiary">
                                <span className="flex items-center gap-1.5"><span className="w-2 h-2 rounded-full bg-green-500" /> Delivered ({successRate}%)</span>
                                <span className="flex items-center gap-1.5"><span className="w-2 h-2 rounded-full bg-red-500" /> Failed ({failRate}%)</span>
                            </div>
                        </div>
                    )}

                    {/* Channel breakdown */}
                    <div className="card p-6 space-y-5">
                        <div className="flex items-center justify-between">
                            <h2 className="text-sm font-bold text-primary">By Channel</h2>
                            <span className="text-[10px] uppercase tracking-widest text-muted">{selectedPeriod} period</span>
                        </div>
                        {Object.keys(usageStats.statsByChannel).length === 0 ? (
                            <p className="text-sm text-muted text-center py-4">No channel data for this period.</p>
                        ) : (
                            <div className="space-y-4">
                                {Object.entries(usageStats.statsByChannel).map(([channel, total]) => {
                                    const pct = (total / maxChannelValue) * 100;
                                    const color = getChannelColor(channel);
                                    return (
                                        <div key={channel} className="space-y-1.5">
                                            <div className="flex items-center justify-between text-xs">
                                                <div className="flex items-center gap-2">
                                                    <span className="w-2 h-2 rounded-sm" style={{ backgroundColor: color }} />
                                                    <span className="font-medium text-primary capitalize">{channel}</span>
                                                </div>
                                                <span className="text-secondary font-mono">{total.toLocaleString()}</span>
                                            </div>
                                            <div className="h-2.5 bg-tertiary rounded-full overflow-hidden">
                                                <div className="h-full rounded-full transition-all duration-700" style={{ width: `${pct}%`, backgroundColor: color }} />
                                            </div>
                                        </div>
                                    );
                                })}
                            </div>
                        )}
                    </div>

                    {/* Event type breakdown */}
                    <div className="card p-6 space-y-5">
                        <div className="flex items-center justify-between">
                            <h2 className="text-sm font-bold text-primary">By Event Type</h2>
                            <span className="text-[10px] uppercase tracking-widest text-muted">{selectedPeriod} period</span>
                        </div>
                        {Object.keys(usageStats.statsByEventType).length === 0 ? (
                            <p className="text-sm text-muted text-center py-4">No event data for this period.</p>
                        ) : (
                            <div className="space-y-2">
                                {Object.entries(usageStats.statsByEventType).map(([eventType, total], idx) => {
                                    const pct = (total / maxEventValue) * 100;
                                    const color = getEventColor(idx);
                                    return (
                                        <div key={eventType} className="card-elevated p-4 rounded-xl hover:border-hover transition-all">
                                            <div className="flex items-center justify-between mb-2">
                                                <div className="flex items-center gap-3">
                                                    <div className="w-1 h-7 rounded-full" style={{ backgroundColor: color }} />
                                                    <span className="text-sm font-semibold text-primary">{eventType}</span>
                                                </div>
                                                <span className="text-sm font-bold font-mono" style={{ color }}>{total.toLocaleString()}</span>
                                            </div>
                                            <div className="h-1.5 bg-primary rounded-full overflow-hidden">
                                                <div className="h-full rounded-full transition-all duration-700" style={{ width: `${pct}%`, backgroundColor: color }} />
                                            </div>
                                        </div>
                                    );
                                })}
                            </div>
                        )}
                    </div>

                    {/* Upgrade banner */}
                    {stats.subscriptionSummary.plan && stats.subscriptionSummary.plan.toLowerCase() !== 'enterprise' && (
                        <div className="card p-5 flex items-center justify-between gap-4">
                            <div className="flex items-center gap-4">
                                <div className="w-9 h-9 rounded-xl bg-purple-500/10 border border-purple-500/20 flex items-center justify-center text-purple-400 shrink-0">
                                    <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 3v4M3 5h4M6 17v4m-2-2h4m5-16l2.286 6.857L21 12l-5.714 2.143L13 21l-2.286-6.857L5 12l5.714-2.143L13 3z" />
                                    </svg>
                                </div>
                                <div>
                                    <p className="text-sm font-semibold text-primary">Unlock longer time ranges</p>
                                    <p className="text-xs text-tertiary mt-0.5">
                                        Upgrade to access {stats.subscriptionSummary.plan.toLowerCase() === 'free' ? '1M, 3M, 6M and 1Y' : '6M and 1Y'} statistics.
                                    </p>
                                </div>
                            </div>
                            <button onClick={() => navigate('/pricing')} className="btn-primary text-sm shrink-0">
                                Upgrade Plan
                            </button>
                        </div>
                    )}
                </div>
            )}
        </AppShell>
    );
}
