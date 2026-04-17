import { useState, useEffect } from 'react';
import AppShell from '../components/AppShell';
import { paymentService } from '../api/paymentService';
import type { PaymentLog } from '../types/payment';

// ── helpers ──────────────────────────────────────────────────────────────────

function formatAmount(cents: number): string {
    return new Intl.NumberFormat('en-US', {
        style: 'currency', currency: 'USD', minimumFractionDigits: 2
    }).format(cents / 100);
}

function formatDate(iso: string): string {
    return new Date(iso).toLocaleDateString('en-US', {
        year: 'numeric', month: 'short', day: 'numeric',
        hour: '2-digit', minute: '2-digit'
    });
}

function statusMeta(status: string): { label: string; className: string; dot: string } {
    const s = status.toLowerCase();
    if (s === 'succeeded' || s === 'success' || s === 'completed')
        return { label: 'Succeeded', className: 'bg-emerald-500/10 text-emerald-400 border-emerald-500/25', dot: 'bg-emerald-400' };
    if (s === 'pending' || s === 'processing')
        return { label: 'Pending', className: 'bg-amber-500/10 text-amber-400 border-amber-500/25', dot: 'bg-amber-400' };
    if (s === 'failed' || s === 'canceled' || s === 'cancelled')
        return { label: 'Failed', className: 'bg-red-500/10 text-red-400 border-red-500/25', dot: 'bg-red-400' };
    return { label: status, className: 'bg-secondary text-secondary border-primary', dot: 'bg-secondary' };
}

// ── component ─────────────────────────────────────────────────────────────────

export default function PaymentHistoryPage() {
    const [payments, setPayments] = useState<PaymentLog[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError]     = useState<string | null>(null);
    const [copied, setCopied]   = useState<string | null>(null);

    useEffect(() => {
        paymentService.getPaymentHistory()
            .then(setPayments)
            .catch(() => setError('Failed to load payment history. Please try again.'))
            .finally(() => setLoading(false));
    }, []);

    const copyToClipboard = (text: string, id: string) => {
        navigator.clipboard.writeText(text).then(() => {
            setCopied(id);
            setTimeout(() => setCopied(null), 1800);
        });
    };

    // totals
    const totalSpent = payments
        .filter(p => statusMeta(p.status).label === 'Succeeded')
        .reduce((sum, p) => sum + p.amount, 0);

    const succeededCount = payments.filter(p => statusMeta(p.status).label === 'Succeeded').length;

    return (
        <AppShell title="Billing & Payments">
            <div className="max-w-5xl space-y-7 animate-fade-in">

                {/* ── Hero banner ── */}
                <div className="card relative overflow-hidden p-7 border-blue-500/15 bg-gradient-to-br from-blue-500/5 via-transparent to-transparent">
                    {/* Glow */}
                    <div className="absolute -top-20 -right-20 w-60 h-60 rounded-full bg-blue-500/8 blur-3xl pointer-events-none" />

                    <div className="relative flex flex-col md:flex-row md:items-center justify-between gap-6">
                        <div className="flex items-center gap-4">
                            {/* Icon */}
                            <div className="w-12 h-12 rounded-xl bg-blue-500/10 border border-blue-500/20 flex items-center justify-center shrink-0">
                                <svg className="w-6 h-6 text-blue-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.75}
                                        d="M17 9V7a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2m2 4h10a2 2 0 002-2v-6a2 2 0 00-2-2H9a2 2 0 00-2 2v6a2 2 0 002 2zm7-5a2 2 0 11-4 0 2 2 0 014 0z" />
                                </svg>
                            </div>
                            <div>
                                <h2 className="font-display font-bold text-lg text-primary">Payment History</h2>
                                <p className="text-sm text-secondary mt-0.5">
                                    All credit purchases and charges processed securely via Stripe.
                                </p>
                            </div>
                        </div>

                        {/* Summary stats */}
                        {!loading && !error && payments.length > 0 && (
                            <div className="flex items-center gap-6 shrink-0">
                                <div className="text-center">
                                    <p className="text-2xl font-display font-bold text-primary tracking-tight">
                                        {formatAmount(totalSpent)}
                                    </p>
                                    <p className="text-[10px] uppercase tracking-widest text-muted font-bold mt-0.5">Total Spent</p>
                                </div>
                                <div className="w-px h-10 bg-border-primary" />
                                <div className="text-center">
                                    <p className="text-2xl font-display font-bold text-primary tracking-tight">{payments.length}</p>
                                    <p className="text-[10px] uppercase tracking-widest text-muted font-bold mt-0.5">Transactions</p>
                                </div>
                                <div className="w-px h-10 bg-border-primary" />
                                <div className="text-center">
                                    <p className="text-2xl font-display font-bold text-emerald-400 tracking-tight">{succeededCount}</p>
                                    <p className="text-[10px] uppercase tracking-widest text-muted font-bold mt-0.5">Successful</p>
                                </div>
                            </div>
                        )}
                    </div>
                </div>

                {/* ── Loading ── */}
                {loading && (
                    <div className="flex items-center gap-3 py-16 justify-center">
                        <div className="spinner" />
                        <span className="text-sm text-secondary">Loading payment history…</span>
                    </div>
                )}

                {/* ── Error ── */}
                {!loading && error && (
                    <div className="card p-10 text-center border-red-500/20">
                        <div className="w-12 h-12 rounded-full bg-red-500/10 border border-red-500/20 flex items-center justify-center mx-auto mb-4">
                            <svg className="w-6 h-6 text-red-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.75} d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                            </svg>
                        </div>
                        <p className="text-red-400 font-semibold mb-1">Could not load payments</p>
                        <p className="text-sm text-tertiary mb-5">{error}</p>
                        <button className="btn-secondary text-sm" onClick={() => window.location.reload()}>
                            Try again
                        </button>
                    </div>
                )}

                {/* ── Empty ── */}
                {!loading && !error && payments.length === 0 && (
                    <div className="card p-14 text-center">
                        <div className="w-16 h-16 rounded-2xl bg-blue-500/8 border border-blue-500/15 flex items-center justify-center mx-auto mb-5">
                            <svg className="w-8 h-8 text-blue-400/60" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5}
                                    d="M9 14l6-6m-5.5.5h.01m4.99 5h.01M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16l3.5-2 3.5 2 3.5-2 3.5 2z" />
                            </svg>
                        </div>
                        <p className="font-display font-bold text-primary text-lg mb-1">No payments yet</p>
                        <p className="text-sm text-tertiary max-w-sm mx-auto">
                            You haven't made any credit purchases. Head to Buy Credits to add more notification credits to your account.
                        </p>
                        <a href="/buy-credits" className="btn-primary inline-flex mt-6 text-sm">
                            Buy Credits →
                        </a>
                    </div>
                )}

                {/* ── Table ── */}
                {!loading && !error && payments.length > 0 && (
                    <div className="card overflow-hidden">
                        {/* Table header */}
                        <div className="px-6 py-4 border-b border-primary flex items-center justify-between bg-secondary/20">
                            <p className="text-xs font-bold uppercase tracking-widest text-muted">
                                {payments.length} transaction{payments.length !== 1 ? 's' : ''}
                            </p>
                            <div className="flex items-center gap-2">
                                <div className="w-2 h-2 rounded-full bg-emerald-400 animate-pulse" />
                                <span className="text-[11px] text-emerald-400 font-semibold">Secure · Stripe</span>
                            </div>
                        </div>

                        <div className="overflow-x-auto">
                            <table className="w-full text-left border-collapse">
                                <thead>
                                    <tr className="border-b border-primary bg-secondary/10">
                                        <th className="py-3.5 px-6 text-[10px] font-bold text-muted uppercase tracking-wider">Date</th>
                                        <th className="py-3.5 px-6 text-[10px] font-bold text-muted uppercase tracking-wider">Amount</th>
                                        <th className="py-3.5 px-6 text-[10px] font-bold text-muted uppercase tracking-wider">Status</th>
                                        <th className="py-3.5 px-6 text-[10px] font-bold text-muted uppercase tracking-wider">Stripe Payment ID</th>
                                        <th className="py-3.5 px-6 text-[10px] font-bold text-muted uppercase tracking-wider">Invoice</th>
                                    </tr>
                                </thead>
                                <tbody className="divide-y divide-primary/40">
                                    {payments.map((payment, idx) => {
                                        const meta = statusMeta(payment.status);
                                        const isCopied = copied === payment.id;
                                        return (
                                            <tr
                                                key={payment.id}
                                                className="hover:bg-secondary/20 transition-colors group"
                                                style={{ animationDelay: `${idx * 40}ms` }}
                                            >
                                                {/* Date */}
                                                <td className="py-4 px-6">
                                                    <span className="text-sm text-secondary font-mono">
                                                        {formatDate(payment.createdAt)}
                                                    </span>
                                                </td>

                                                {/* Amount */}
                                                <td className="py-4 px-6">
                                                    <span className="text-base font-display font-bold text-primary">
                                                        {formatAmount(payment.amount)}
                                                    </span>
                                                </td>

                                                {/* Status badge */}
                                                <td className="py-4 px-6">
                                                    <span className={`inline-flex items-center gap-1.5 text-[11px] font-bold px-2.5 py-1 rounded-full border ${meta.className}`}>
                                                        <span className={`w-1.5 h-1.5 rounded-full ${meta.dot}`} />
                                                        {meta.label}
                                                    </span>
                                                </td>

                                                {/* Stripe ID */}
                                                <td className="py-4 px-6">
                                                    <div className="flex items-center gap-2 max-w-[220px]">
                                                        <span className="text-xs font-mono text-tertiary truncate">
                                                            {payment.stripePaymentIntentId}
                                                        </span>
                                                        <button
                                                            title="Copy payment ID"
                                                            onClick={() => copyToClipboard(payment.stripePaymentIntentId, payment.id)}
                                                            className="shrink-0 w-6 h-6 rounded-md border border-primary bg-elevated flex items-center justify-center opacity-0 group-hover:opacity-100 transition-all hover:border-blue-500/40 hover:text-blue-400 text-tertiary"
                                                        >
                                                            {isCopied ? (
                                                                <svg className="w-3 h-3 text-emerald-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2.5} d="M5 13l4 4L19 7" />
                                                                </svg>
                                                            ) : (
                                                                <svg className="w-3 h-3" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.75} d="M8 16H6a2 2 0 01-2-2V6a2 2 0 012-2h8a2 2 0 012 2v2m-6 12h8a2 2 0 002-2v-8a2 2 0 00-2-2h-8a2 2 0 00-2 2v8a2 2 0 002 2z" />
                                                                </svg>
                                                            )}
                                                        </button>
                                                    </div>
                                                </td>

                                                {/* Invoice download (Stripe dashboard link) */}
                                                <td className="py-4 px-6">
                                                    <a
                                                        href={`https://dashboard.stripe.com/payments/${payment.stripePaymentIntentId}`}
                                                        target="_blank"
                                                        rel="noopener noreferrer"
                                                        className="inline-flex items-center gap-1.5 text-xs font-medium text-accent-light hover:text-accent-primary transition-colors group/link"
                                                    >
                                                        <svg className="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.75} d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                                                        </svg>
                                                        View Invoice
                                                        <svg className="w-3 h-3 opacity-0 group-hover/link:opacity-100 -translate-x-1 group-hover/link:translate-x-0 transition-all" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M10 6H6a2 2 0 00-2 2v10a2 2 0 002 2h10a2 2 0 002-2v-4M14 4h6m0 0v6m0-6L10 14" />
                                                        </svg>
                                                    </a>
                                                </td>
                                            </tr>
                                        );
                                    })}
                                </tbody>
                            </table>
                        </div>

                        {/* Footer note */}
                        <div className="px-6 py-4 border-t border-primary bg-secondary/10 flex items-center gap-2">
                            <svg className="w-3.5 h-3.5 text-muted shrink-0" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.75} d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                            </svg>
                            <p className="text-[11px] text-muted">
                                Payments are processed securely by Stripe. Amounts are shown in USD cents converted to dollars.
                                Click <span className="text-secondary">View Invoice</span> to open the official Stripe receipt.
                            </p>
                        </div>
                    </div>
                )}
            </div>
        </AppShell>
    );
}
