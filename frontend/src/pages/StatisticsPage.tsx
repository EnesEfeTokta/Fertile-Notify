import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { statisticService } from '../api/statisticService';
import { subscriberService } from '../api/subscriberService';
import type { Statistic } from '../types/statistic';
import type { SubscriberProfile } from '../types/subscriber';

type Period = 'Daily' | 'Weekly' | '1Month' | '3Months' | '6Months' | '1Year';

interface PeriodConfig {
    key: Period;
    label: string;
    shortLabel: string;
}

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
        case 'enterprise':
            return ['Daily', 'Weekly', '1Month', '3Months', '6Months', '1Year'];
        case 'pro':
            return ['Daily', 'Weekly', '1Month', '3Months'];
        default: // Free
            return ['Daily', 'Weekly'];
    }
}

function getChannelColor(channel: string): string {
    const map: Record<string, string> = {
        console: '#3b82f6',
        email: '#8b5cf6',
        sms: '#f59e0b',
    };
    return map[channel.toLowerCase()] || '#6b7280';
}

function getEventColor(index: number): string {
    const palette = ['#3b82f6', '#8b5cf6', '#f59e0b', '#10b981', '#ef4444', '#ec4899', '#06b6d4', '#84cc16'];
    return palette[index % palette.length];
}

export default function StatisticsPage() {
    const navigate = useNavigate();
    const [profile, setProfile] = useState<SubscriberProfile | null>(null);
    const [stats, setStats] = useState<Statistic | null>(null);
    const [selectedPeriod, setSelectedPeriod] = useState<Period>('Daily');
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const allowedPeriods = getAllowedPeriods(profile?.subscription?.plan);

    const fetchProfile = React.useCallback(async () => {
        try {
            const data = await subscriberService.getProfile();
            setProfile(data);
        } catch {
            console.error('Failed to fetch profile');
        }
    }, []);

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

    useEffect(() => {
        fetchProfile();
    }, [fetchProfile]);

    useEffect(() => {
        fetchStats(selectedPeriod);
    }, [selectedPeriod, fetchStats]);

    const handlePeriodChange = (period: Period) => {
        if (allowedPeriods.includes(period)) {
            setSelectedPeriod(period);
        }
    };

    const successRate = stats && stats.totalUsage > 0
        ? ((stats.successCount / stats.totalUsage) * 100).toFixed(1)
        : '0.0';

    const failRate = stats && stats.totalUsage > 0
        ? ((stats.failedCount / stats.totalUsage) * 100).toFixed(1)
        : '0.0';

    const maxChannelValue = stats
        ? Math.max(...Object.values(stats.statsByChannel), 1)
        : 1;

    const maxEventValue = stats
        ? Math.max(...Object.values(stats.statsByEventType), 1)
        : 1;

    return (
        <div className="min-h-screen bg-primary">
            {/* Header */}
            <header className="border-b border-primary bg-secondary/50 backdrop-blur-sm sticky top-0 z-10">
                <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4 flex items-center justify-between">
                    <div className="flex items-center gap-4">
                        <button
                            onClick={() => navigate('/dashboard')}
                            className="text-secondary hover:text-primary transition-colors flex items-center gap-2 text-sm"
                        >
                            <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M10 19l-7-7m0 0l7-7m-7 7h18" />
                            </svg>
                            Dashboard
                        </button>
                        <div className="h-6 w-px bg-[var(--border-primary)]"></div>
                        <h1 className="text-xl font-display font-semibold text-primary flex items-center gap-2">
                            <span>📊</span>
                            Statistics
                        </h1>
                    </div>
                    <div className="flex items-center gap-2">
                        {profile?.subscription?.plan && (
                            <span className="badge text-xs">
                                {profile.subscription.plan} Plan
                            </span>
                        )}
                    </div>
                </div>
            </header>

            <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8 space-y-8">
                {/* Period Selector */}
                <div className="card p-2 inline-flex gap-1">
                    {ALL_PERIODS.map((p) => {
                        const isAllowed = allowedPeriods.includes(p.key);
                        const isActive = selectedPeriod === p.key;
                        return (
                            <button
                                key={p.key}
                                onClick={() => handlePeriodChange(p.key)}
                                disabled={!isAllowed}
                                className={`relative px-4 py-2 text-sm font-medium rounded-md transition-all duration-200 flex items-center gap-1.5 ${isActive
                                        ? 'bg-[var(--accent-primary)] text-white shadow-lg shadow-blue-500/20'
                                        : isAllowed
                                            ? 'text-secondary hover:text-primary hover:bg-[var(--bg-tertiary)]'
                                            : 'text-tertiary opacity-50 cursor-not-allowed'
                                    }`}
                                title={!isAllowed ? `Upgrade to access ${p.label} statistics` : p.label}
                            >
                                <span className="hidden sm:inline">{p.label}</span>
                                <span className="sm:hidden">{p.shortLabel}</span>
                                {!isAllowed && (
                                    <svg className="w-3 h-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z" />
                                    </svg>
                                )}
                            </button>
                        );
                    })}
                </div>

                {/* Loading State */}
                {loading && (
                    <div className="flex justify-center items-center py-20">
                        <div className="spinner"></div>
                        <span className="ml-3 text-secondary text-sm">Fetching statistics...</span>
                    </div>
                )}

                {/* Error State */}
                {error && !loading && (
                    <div className="card p-8 text-center">
                        <p className="text-red-400 text-sm mb-4">{error}</p>
                        <button
                            onClick={() => fetchStats(selectedPeriod)}
                            className="btn-secondary text-sm"
                        >
                            Retry
                        </button>
                    </div>
                )}

                {/* Stats Content */}
                {stats && !loading && !error && (
                    <>
                        {/* KPI Overview Cards */}
                        <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
                            {/* Total Usage */}
                            <div className="card p-6 space-y-3">
                                <div className="flex items-center justify-between">
                                    <span className="text-secondary text-sm font-medium">Total Usage</span>
                                    <div className="w-8 h-8 rounded-lg bg-blue-500/10 flex items-center justify-center">
                                        <svg className="w-4 h-4 text-blue-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 7h8m0 0v8m0-8l-8 8-4-4-6 6" />
                                        </svg>
                                    </div>
                                </div>
                                <div className="text-3xl font-bold text-primary font-display">
                                    {stats.totalUsage.toLocaleString()}
                                </div>
                                <div className="text-xs text-tertiary">
                                    notifications sent in this period
                                </div>
                            </div>

                            {/* Success */}
                            <div className="card p-6 space-y-3">
                                <div className="flex items-center justify-between">
                                    <span className="text-secondary text-sm font-medium">Successful</span>
                                    <div className="w-8 h-8 rounded-lg bg-green-500/10 flex items-center justify-center">
                                        <svg className="w-4 h-4 text-green-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
                                        </svg>
                                    </div>
                                </div>
                                <div className="flex items-baseline gap-3">
                                    <span className="text-3xl font-bold text-green-400 font-display">
                                        {stats.successCount.toLocaleString()}
                                    </span>
                                    <span className="text-sm font-medium text-green-400/60">
                                        {successRate}%
                                    </span>
                                </div>
                                <div className="w-full h-1.5 bg-[var(--bg-tertiary)] rounded-full overflow-hidden">
                                    <div
                                        className="h-full bg-green-500 rounded-full transition-all duration-700 ease-out"
                                        style={{ width: `${successRate}%` }}
                                    ></div>
                                </div>
                            </div>

                            {/* Failed */}
                            <div className="card p-6 space-y-3">
                                <div className="flex items-center justify-between">
                                    <span className="text-secondary text-sm font-medium">Failed</span>
                                    <div className="w-8 h-8 rounded-lg bg-red-500/10 flex items-center justify-center">
                                        <svg className="w-4 h-4 text-red-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                                        </svg>
                                    </div>
                                </div>
                                <div className="flex items-baseline gap-3">
                                    <span className="text-3xl font-bold text-red-400 font-display">
                                        {stats.failedCount.toLocaleString()}
                                    </span>
                                    <span className="text-sm font-medium text-red-400/60">
                                        {failRate}%
                                    </span>
                                </div>
                                <div className="w-full h-1.5 bg-[var(--bg-tertiary)] rounded-full overflow-hidden">
                                    <div
                                        className="h-full bg-red-500 rounded-full transition-all duration-700 ease-out"
                                        style={{ width: `${failRate}%` }}
                                    ></div>
                                </div>
                            </div>
                        </div>

                        {/* Channel Breakdown */}
                        <div className="card p-6 space-y-6">
                            <div className="flex items-center justify-between">
                                <h2 className="text-lg font-semibold text-primary font-display">By Channel</h2>
                                <span className="text-xs text-tertiary">{selectedPeriod} breakdown</span>
                            </div>

                            <div className="space-y-4">
                                {Object.entries(stats.statsByChannel).map(([channel, total]) => {
                                    const success = stats.successByChannel?.[channel] ?? total;
                                    const failed = stats.failedByChannel?.[channel] ?? 0;
                                    const barWidth = (total / maxChannelValue) * 100;
                                    const color = getChannelColor(channel);

                                    return (
                                        <div key={channel} className="space-y-2">
                                            <div className="flex items-center justify-between">
                                                <div className="flex items-center gap-3">
                                                    <div
                                                        className="w-2.5 h-2.5 rounded-sm"
                                                        style={{ backgroundColor: color }}
                                                    ></div>
                                                    <span className="text-sm font-medium text-primary capitalize">
                                                        {channel}
                                                    </span>
                                                </div>
                                                <div className="flex items-center gap-4 text-xs">
                                                    <span className="text-secondary">
                                                        Total: <span className="font-bold text-primary">{total.toLocaleString()}</span>
                                                    </span>
                                                    {stats.successByChannel && (
                                                        <span className="text-green-400/80">
                                                            ✓ {success.toLocaleString()}
                                                        </span>
                                                    )}
                                                    {stats.failedByChannel && (
                                                        <span className="text-red-400/80">
                                                            ✕ {failed.toLocaleString()}
                                                        </span>
                                                    )}
                                                </div>
                                            </div>
                                            <div className="w-full h-3 bg-[var(--bg-tertiary)] rounded-full overflow-hidden flex">
                                                {stats.successByChannel ? (
                                                    <>
                                                        <div
                                                            className="h-full rounded-l-full transition-all duration-700 ease-out"
                                                            style={{
                                                                width: `${(success / maxChannelValue) * 100}%`,
                                                                backgroundColor: color,
                                                                opacity: 0.9,
                                                            }}
                                                        ></div>
                                                        <div
                                                            className="h-full transition-all duration-700 ease-out"
                                                            style={{
                                                                width: `${(failed / maxChannelValue) * 100}%`,
                                                                backgroundColor: '#ef4444',
                                                                opacity: 0.6,
                                                            }}
                                                        ></div>
                                                    </>
                                                ) : (
                                                    <div
                                                        className="h-full rounded-full transition-all duration-700 ease-out"
                                                        style={{
                                                            width: `${barWidth}%`,
                                                            backgroundColor: color,
                                                        }}
                                                    ></div>
                                                )}
                                            </div>
                                        </div>
                                    );
                                })}

                                {Object.keys(stats.statsByChannel).length === 0 && (
                                    <p className="text-sm text-tertiary text-center py-4">No channel data for this period.</p>
                                )}
                            </div>
                        </div>

                        {/* Event Type Breakdown */}
                        <div className="card p-6 space-y-6">
                            <div className="flex items-center justify-between">
                                <h2 className="text-lg font-semibold text-primary font-display">By Event Type</h2>
                                <span className="text-xs text-tertiary">{selectedPeriod} breakdown</span>
                            </div>

                            <div className="space-y-3">
                                {Object.entries(stats.statsByEventType).map(([eventType, total], idx) => {
                                    const success = stats.successByEventType?.[eventType] ?? total;
                                    const failed = stats.failedByEventType?.[eventType] ?? 0;
                                    const barWidth = (total / maxEventValue) * 100;
                                    const color = getEventColor(idx);

                                    return (
                                        <div
                                            key={eventType}
                                            className="group card-elevated p-4 rounded-lg hover:border-hover transition-all"
                                        >
                                            <div className="flex items-center justify-between mb-3">
                                                <div className="flex items-center gap-3">
                                                    <div
                                                        className="w-1 h-8 rounded-full"
                                                        style={{ backgroundColor: color }}
                                                    ></div>
                                                    <div>
                                                        <span className="text-sm font-semibold text-primary">
                                                            {eventType}
                                                        </span>
                                                    </div>
                                                </div>
                                                <div className="flex items-center gap-5 text-xs">
                                                    <div className="text-center">
                                                        <div className="text-secondary font-medium">Total</div>
                                                        <div className="font-bold text-primary text-sm">{total.toLocaleString()}</div>
                                                    </div>
                                                    {stats.successByEventType && (
                                                        <div className="text-center">
                                                            <div className="text-green-400/60 font-medium">Success</div>
                                                            <div className="font-bold text-green-400 text-sm">{success.toLocaleString()}</div>
                                                        </div>
                                                    )}
                                                    {stats.failedByEventType && (
                                                        <div className="text-center">
                                                            <div className="text-red-400/60 font-medium">Failed</div>
                                                            <div className="font-bold text-red-400 text-sm">{failed.toLocaleString()}</div>
                                                        </div>
                                                    )}
                                                </div>
                                            </div>
                                            <div className="w-full h-2 bg-[var(--bg-primary)] rounded-full overflow-hidden flex">
                                                {stats.successByEventType ? (
                                                    <>
                                                        <div
                                                            className="h-full rounded-l-full transition-all duration-700 ease-out"
                                                            style={{
                                                                width: `${(success / maxEventValue) * 100}%`,
                                                                backgroundColor: color,
                                                                opacity: 0.85,
                                                            }}
                                                        ></div>
                                                        <div
                                                            className="h-full transition-all duration-700 ease-out"
                                                            style={{
                                                                width: `${(failed / maxEventValue) * 100}%`,
                                                                backgroundColor: '#ef4444',
                                                                opacity: 0.5,
                                                            }}
                                                        ></div>
                                                    </>
                                                ) : (
                                                    <div
                                                        className="h-full rounded-full transition-all duration-700 ease-out"
                                                        style={{
                                                            width: `${barWidth}%`,
                                                            backgroundColor: color,
                                                        }}
                                                    ></div>
                                                )}
                                            </div>
                                        </div>
                                    );
                                })}

                                {Object.keys(stats.statsByEventType).length === 0 && (
                                    <p className="text-sm text-tertiary text-center py-4">No event type data for this period.</p>
                                )}
                            </div>
                        </div>

                        {/* Upgrade Banner for restricted plans */}
                        {profile?.subscription?.plan && profile.subscription.plan.toLowerCase() !== 'enterprise' && (
                            <div className="card p-6 border-dashed flex items-center justify-between">
                                <div className="flex items-center gap-4">
                                    <div className="w-10 h-10 rounded-xl bg-purple-500/10 flex items-center justify-center">
                                        <svg className="w-5 h-5 text-purple-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 7h8m0 0v8m0-8l-8 8-4-4-6 6" />
                                        </svg>
                                    </div>
                                    <div>
                                        <p className="text-sm font-semibold text-primary">Unlock more time periods</p>
                                        <p className="text-xs text-tertiary">
                                            Upgrade your plan to access {profile.subscription.plan.toLowerCase() === 'free' ? '1M, 3M, 6M, and 1Y' : '6M and 1Y'} statistics.
                                        </p>
                                    </div>
                                </div>
                                <button
                                    onClick={() => navigate('/pricing')}
                                    className="btn-primary text-sm"
                                >
                                    Upgrade
                                </button>
                            </div>
                        )}
                    </>
                )}
            </main>
        </div>
    );
}
