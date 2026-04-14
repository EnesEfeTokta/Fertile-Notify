import { useState, useCallback, useEffect } from 'react';
import {
    Elements,
    PaymentElement,
    useElements,
    useStripe,
} from '@stripe/react-stripe-js';
import { loadStripe } from '@stripe/stripe-js';
import type { StripeElementsOptions } from '@stripe/stripe-js';
import { paymentService } from '../api/paymentService';
import { subscriberService } from '../api/subscriberService';
import type { SubscriberProfile } from '../types/subscriber';
import type { ExtraCreditPaymentIntent } from '../types/payment';
import AppShell from '../components/AppShell';
import { useToast } from '../components/Toast';

const stripePublishableKey = import.meta.env.VITE_STRIPE_PUBLISHABLE_KEY ?? '';
const stripePromise = stripePublishableKey ? loadStripe(stripePublishableKey) : null;

/* ================================================================
   Stripe Checkout Form — rendered inside Elements provider
   ================================================================ */
function StripeCheckoutForm({
    checkoutIntent,
    onSuccess,
    onCancel,
    showToast,
}: {
    checkoutIntent: ExtraCreditPaymentIntent;
    onSuccess: () => Promise<void>;
    onCancel: () => void;
    showToast: (msg: string, type?: 'success' | 'error' | 'info') => void;
}) {
    const stripe = useStripe();
    const elements = useElements();
    const [confirming, setConfirming] = useState(false);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!stripe || !elements) return;

        setConfirming(true);
        try {
            const result = await stripe.confirmPayment({
                elements,
                redirect: 'if_required',
            });

            if (result.error) {
                showToast(result.error.message ?? 'Payment failed.', 'error');
                return;
            }

            if (result.paymentIntent?.status === 'succeeded') {
                await onSuccess();
                return;
            }

            showToast('Payment is being processed. Credits will update shortly.', 'info');
            await onSuccess();
        } catch {
            showToast('Payment failed. Please try again.', 'error');
        } finally {
            setConfirming(false);
        }
    };

    const amountUsd = (checkoutIntent.amountInCents / 100).toFixed(2);

    return (
        <form onSubmit={handleSubmit} className="flex flex-col gap-5">
            {/* Order summary strip */}
            <div className="flex items-center justify-between px-4 py-3 rounded-xl bg-blue-500/8 border border-blue-500/20">
                <div className="flex items-center gap-3">
                    <span className="text-2xl">💎</span>
                    <div>
                        <p className="text-sm font-semibold text-primary">
                            {checkoutIntent.credits.toLocaleString()} Credits
                        </p>
                        <p className="text-xs text-secondary">Lifetime validity — never expires</p>
                    </div>
                </div>
                <div className="text-right">
                    <p className="text-xl font-display font-bold text-primary">
                        ${amountUsd}
                    </p>
                    <p className="text-[10px] uppercase tracking-wider text-muted">
                        {checkoutIntent.currency.toUpperCase()}
                    </p>
                </div>
            </div>

            {/* Stripe Elements */}
            <div className="stripe-element-wrapper">
                <PaymentElement
                    options={{
                        layout: 'accordion',
                    }}
                />
            </div>

            {/* Actions */}
            <div className="flex gap-3 pt-1">
                <button
                    type="button"
                    onClick={onCancel}
                    disabled={confirming}
                    className="flex-1 py-3 rounded-xl border border-primary text-secondary font-semibold text-sm hover:border-secondary hover:text-primary transition-all disabled:opacity-40"
                >
                    Cancel
                </button>
                <button
                    type="submit"
                    disabled={!stripe || !elements || confirming}
                    className="flex-[2] py-3 rounded-xl bg-blue-500 text-white font-bold text-sm hover:bg-blue-400 transition-all disabled:opacity-40 flex items-center justify-center gap-2"
                >
                    {confirming ? (
                        <>
                            <span className="w-4 h-4 border-2 border-white/30 border-t-white rounded-full animate-spin" />
                            Confirming…
                        </>
                    ) : (
                        <>
                            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" className="w-4 h-4">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z" />
                            </svg>
                            Pay ${amountUsd} {checkoutIntent.currency.toUpperCase()}
                        </>
                    )}
                </button>
            </div>

            <p className="text-center text-[11px] text-muted flex items-center justify-center gap-1.5">
                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" className="w-3.5 h-3.5 shrink-0">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z" />
                </svg>
                Secured by Stripe · PCI DSS Compliant
            </p>
        </form>
    );
}

/* ================================================================
   Checkout Modal Overlay
   ================================================================ */
function CheckoutModal({
    checkoutIntent,
    stripeOptions,
    onSuccess,
    onClose,
    showToast,
}: {
    checkoutIntent: ExtraCreditPaymentIntent;
    stripeOptions: StripeElementsOptions;
    onSuccess: () => Promise<void>;
    onClose: () => void;
    showToast: (msg: string, type?: 'success' | 'error' | 'info') => void;
}) {
    // Close on Escape
    useEffect(() => {
        const handler = (e: KeyboardEvent) => { if (e.key === 'Escape') onClose(); };
        window.addEventListener('keydown', handler);
        return () => window.removeEventListener('keydown', handler);
    }, [onClose]);

    return (
        <div
            className="fixed inset-0 z-[999] flex items-center justify-center p-4"
            style={{ backgroundColor: 'rgba(0,0,0,0.75)', backdropFilter: 'blur(6px)' }}
            onClick={(e) => { if (e.target === e.currentTarget) onClose(); }}
        >
            <div
                className="relative w-full max-w-md bg-[#0d0d0d] border border-[#1f1f1f] rounded-2xl shadow-2xl animate-scale-in overflow-hidden"
                onClick={(e) => e.stopPropagation()}
            >
                {/* Header */}
                <div className="flex items-center justify-between px-6 pt-6 pb-4 border-b border-[#1f1f1f]">
                    <div className="flex items-center gap-3">
                        <div className="w-9 h-9 rounded-xl bg-blue-500/10 border border-blue-500/20 flex items-center justify-center">
                            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" className="w-5 h-5 text-blue-400">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.75} d="M3 10h18M7 15h1m4 0h1m-7 4h12a3 3 0 003-3V8a3 3 0 00-3-3H6a3 3 0 00-3 3v8a3 3 0 003 3z" />
                            </svg>
                        </div>
                        <div>
                            <h2 className="text-base font-bold text-[#ededed]">Secure Checkout</h2>
                            <p className="text-xs text-[#888]">Powered by Stripe</p>
                        </div>
                    </div>
                    <button
                        onClick={onClose}
                        className="w-8 h-8 flex items-center justify-center rounded-lg text-[#555] hover:text-[#ededed] hover:bg-[#111] transition-all"
                    >
                        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" className="w-4 h-4">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                        </svg>
                    </button>
                </div>

                {/* Body */}
                <div className="px-6 py-5">
                    {stripePromise ? (
                        <Elements stripe={stripePromise} options={stripeOptions}>
                            <StripeCheckoutForm
                                checkoutIntent={checkoutIntent}
                                onSuccess={onSuccess}
                                onCancel={onClose}
                                showToast={showToast}
                            />
                        </Elements>
                    ) : (
                        <div className="text-center py-8 text-[#888] text-sm">
                            Stripe is not configured. Please set VITE_STRIPE_PUBLISHABLE_KEY.
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
}

/* ================================================================
   Success Modal
   ================================================================ */
function SuccessModal({ credits, onClose }: { credits: number; onClose: () => void }) {
    return (
        <div
            className="fixed inset-0 z-[999] flex items-center justify-center p-4"
            style={{ backgroundColor: 'rgba(0,0,0,0.75)', backdropFilter: 'blur(6px)' }}
        >
            <div className="relative w-full max-w-sm bg-[#0d0d0d] border border-[#1f1f1f] rounded-2xl shadow-2xl animate-scale-in p-8 flex flex-col items-center text-center gap-4">
                <div className="w-16 h-16 rounded-full bg-green-500/10 border border-green-500/20 flex items-center justify-center text-3xl">
                    ✓
                </div>
                <div>
                    <h2 className="text-xl font-bold text-[#ededed] mb-1">Payment Successful!</h2>
                    <p className="text-[#888] text-sm">
                        <strong className="text-green-400">{credits.toLocaleString()} credits</strong> have been added to your account.
                    </p>
                </div>
                <button
                    onClick={onClose}
                    className="w-full py-3 rounded-xl bg-blue-500 text-white font-bold text-sm hover:bg-blue-400 transition-all mt-2"
                >
                    Continue
                </button>
            </div>
        </div>
    );
}

/* ================================================================
   Credit Package Card
   ================================================================ */
type CreditPackage = {
    amount: number;
    priceUsd: number;
    icon: string;
    label: string;
    popular?: boolean;
    badge?: string;
};

const PACKAGES: CreditPackage[] = [
    { amount: 100,  priceUsd: 1,   icon: '🪙', label: 'Starter',     badge: '100 credits' },
    { amount: 1000, priceUsd: 10,  icon: '💰', label: 'Growth',      badge: '1,000 credits' },
    { amount: 5000, priceUsd: 50,  icon: '💎', label: 'Pro',         badge: '5,000 credits', popular: true },
    { amount: 20000, priceUsd: 200, icon: '🚀', label: 'Enterprise',  badge: '20,000 credits' },
];

function PackageCard({
    pkg,
    onSelect,
    disabled,
}: {
    pkg: CreditPackage;
    onSelect: () => void;
    disabled: boolean;
}) {
    return (
        <button
            onClick={onSelect}
            disabled={disabled}
            className={`relative w-full text-left p-5 rounded-2xl border transition-all duration-200 disabled:opacity-40 disabled:cursor-not-allowed group
                ${pkg.popular
                    ? 'border-blue-500/40 bg-blue-500/5 hover:bg-blue-500/10 hover:border-blue-500/60'
                    : 'border-[#1f1f1f] bg-[#0d0d0d] hover:bg-[#111] hover:border-[#3a3a3a]'
                }`}
        >
            {pkg.popular && (
                <span className="absolute top-3 right-3 text-[10px] font-bold uppercase tracking-wider px-2 py-0.5 rounded-full bg-blue-500/20 text-blue-400 border border-blue-500/30">
                    Most Popular
                </span>
            )}
            <div className="text-3xl mb-3">{pkg.icon}</div>
            <p className="text-xs font-bold uppercase tracking-widest text-[#555] mb-1">{pkg.label}</p>
            <p className="text-lg font-bold text-[#ededed] mb-0.5">{pkg.amount.toLocaleString()} Credits</p>
            <p className="text-xs text-[#888] mb-4">Lifetime validity</p>
            <div className="flex items-baseline gap-1">
                <span className="text-2xl font-display font-bold text-[#ededed]">${pkg.priceUsd}</span>
                <span className="text-xs text-[#555]">USD</span>
            </div>
            <div className={`mt-4 w-full py-2.5 rounded-xl text-sm font-semibold text-center transition-all
                ${pkg.popular
                    ? 'bg-blue-500 text-white group-hover:bg-blue-400'
                    : 'bg-[#161616] text-[#ededed] border border-[#2a2a2a] group-hover:border-[#3a3a3a]'
                }`}
            >
                {disabled ? 'Processing…' : 'Buy Now'}
            </div>
        </button>
    );
}

/* ================================================================
   Main Page
   ================================================================ */
export default function BuyCreditsPage() {
    const [profile, setProfile] = useState<SubscriberProfile | null>(null);
    const [loading, setLoading] = useState(true);
    const [purchasing, setPurchasing] = useState(false);
    const [checkoutIntent, setCheckoutIntent] = useState<ExtraCreditPaymentIntent | null>(null);
    const [successCredits, setSuccessCredits] = useState<number | null>(null);
    const { showToast, ToastContainer } = useToast();

    const [customAmount, setCustomAmount] = useState<number>(500);
    // 1 credit = $0.01 USD (100 cents per credit, per backend config)
    const customPriceUsd = Number((customAmount * 0.01).toFixed(2));

    const fetchProfile = useCallback(async () => {
        try {
            const data = await subscriberService.getProfile();
            setProfile(data);
        } catch {
            showToast('Failed to load profile.', 'error');
        } finally {
            setLoading(false);
        }
    }, [showToast]);

    useEffect(() => { fetchProfile(); }, [fetchProfile]);

    const initiateCheckout = async (amount: number) => {
        if (!stripePromise) {
            showToast('Stripe is not configured (VITE_STRIPE_PUBLISHABLE_KEY is missing).', 'error');
            return;
        }
        setPurchasing(true);
        try {
            const intent = await paymentService.createExtraCreditPaymentIntent({ credits: amount });
            setCheckoutIntent(intent);
        } catch {
            showToast('Could not initialize payment. Please try again.', 'error');
        } finally {
            setPurchasing(false);
        }
    };

    const handlePaymentSuccess = useCallback(async () => {
        const credits = checkoutIntent?.credits ?? 0;
        setCheckoutIntent(null);
        await new Promise(r => setTimeout(r, 1000));
        await fetchProfile();
        setSuccessCredits(credits);
    }, [checkoutIntent, fetchProfile]);

    const stripeOptions: StripeElementsOptions | undefined = checkoutIntent
        ? {
            clientSecret: checkoutIntent.clientSecret,
            appearance: {
                theme: 'night',
                variables: {
                    colorPrimary: '#3b82f6',
                    colorBackground: '#0d0d0d',
                    colorText: '#ededed',
                    colorDanger: '#ef4444',
                    fontFamily: 'Inter, system-ui, sans-serif',
                    borderRadius: '10px',
                },
                rules: {
                    '.Input': {
                        backgroundColor: '#000',
                        border: '1px solid #1f1f1f',
                        color: '#ededed',
                    },
                    '.Input:focus': {
                        border: '1px solid #3b82f6',
                        boxShadow: '0 0 0 3px rgba(59,130,246,0.12)',
                    },
                    '.Label': {
                        color: '#888',
                        fontSize: '12px',
                    },
                },
            },
        }
        : undefined;

    return (
        <AppShell title="Buy Credits" companyName={profile?.companyName} plan={profile?.subscription?.plan}>
            <ToastContainer />

            {/* Checkout modal */}
            {checkoutIntent && stripeOptions && (
                <CheckoutModal
                    checkoutIntent={checkoutIntent}
                    stripeOptions={stripeOptions}
                    onSuccess={handlePaymentSuccess}
                    onClose={() => setCheckoutIntent(null)}
                    showToast={showToast}
                />
            )}

            {/* Success modal */}
            {successCredits !== null && (
                <SuccessModal
                    credits={successCredits}
                    onClose={() => setSuccessCredits(null)}
                />
            )}

            <div className="max-w-5xl space-y-8 pb-12 animate-fade-in-up">

                {/* ---- Hero banner ---- */}
                <div className="relative overflow-hidden rounded-2xl border border-[#1f1f1f] bg-gradient-to-br from-[#0d0d0d] via-[#0a0f1e] to-[#0d0d0d] p-8">
                    <div className="absolute inset-0 bg-[radial-gradient(ellipse_80%_60%_at_60%_50%,rgba(59,130,246,0.08),transparent)]" />
                    <div className="relative flex flex-col md:flex-row gap-8 items-start md:items-center">
                        <div className="flex-1">
                            <div className="inline-flex items-center gap-2 px-3 py-1 rounded-full bg-blue-500/10 border border-blue-500/20 text-xs font-semibold text-blue-400 uppercase tracking-wider mb-4">
                                <span className="w-1.5 h-1.5 rounded-full bg-blue-400 animate-pulse" />
                                Extra Credits
                            </div>
                            <h2 className="text-3xl font-display font-bold text-[#ededed] mb-2 tracking-tight">
                                Top up your balance
                            </h2>
                            <p className="text-[#888] text-sm leading-relaxed max-w-md">
                                Extra credits never expire and are used only when your monthly plan quota is exceeded. Buy once, use forever.
                            </p>
                        </div>

                        <div className="flex items-center gap-4 px-6 py-5 rounded-2xl bg-[#000] border border-[#1f1f1f] shrink-0">
                            <div className="w-12 h-12 rounded-xl bg-green-500/10 border border-green-500/20 flex items-center justify-center text-2xl">
                                💰
                            </div>
                            <div>
                                <p className="text-[10px] font-bold uppercase tracking-widest text-[#555] mb-0.5">
                                    Extra Balance
                                </p>
                                <p className="text-3xl font-display font-bold text-green-400">
                                    {loading ? (
                                        <span className="inline-block w-16 h-7 rounded bg-[#111] animate-pulse" />
                                    ) : (
                                        (profile?.extraCredits ?? 0).toLocaleString()
                                    )}
                                </p>
                                <p className="text-[11px] text-[#555] mt-0.5">credits available</p>
                            </div>
                        </div>
                    </div>
                </div>

                {/* ---- Package grid ---- */}
                <div>
                    <h3 className="text-sm font-bold uppercase tracking-widest text-[#555] mb-4">
                        Choose a Package
                    </h3>
                    <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
                        {PACKAGES.map(pkg => (
                            <PackageCard
                                key={pkg.amount}
                                pkg={pkg}
                                onSelect={() => initiateCheckout(pkg.amount)}
                                disabled={purchasing}
                            />
                        ))}
                    </div>
                </div>

                {/* ---- Custom amount ---- */}
                <div className="rounded-2xl border border-[#1f1f1f] bg-[#0d0d0d] p-6">
                    <div className="flex flex-col sm:flex-row gap-6 items-start sm:items-end">
                        <div className="flex-1">
                            <h3 className="text-sm font-bold uppercase tracking-widest text-[#555] mb-4">
                                Custom Amount
                            </h3>
                            <label className="text-xs text-[#888] block mb-2">
                                Number of credits (min. 100)
                            </label>
                            <div className="relative">
                                <input
                                    type="number"
                                    min="100"
                                    step="100"
                                    value={customAmount}
                                    onChange={(e) => setCustomAmount(Math.max(100, parseInt(e.target.value) || 100))}
                                    className="w-full bg-[#000] border border-[#1f1f1f] rounded-xl px-4 py-3 text-[#ededed] text-xl font-bold focus:border-blue-500 focus:ring-0 outline-none transition-all placeholder-[#333]"
                                />
                                <div className="absolute right-4 top-1/2 -translate-y-1/2 flex flex-col gap-0.5">
                                    <button
                                        onClick={() => setCustomAmount(a => a + 100)}
                                        className="text-[#555] hover:text-[#ededed] transition-colors leading-none text-xs"
                                    >▲</button>
                                    <button
                                        onClick={() => setCustomAmount(a => Math.max(100, a - 100))}
                                        className="text-[#555] hover:text-[#ededed] transition-colors leading-none text-xs"
                                    >▼</button>
                                </div>
                            </div>
                        </div>

                        <div className="flex flex-col gap-3 sm:w-56">
                            <div className="flex items-center justify-between text-sm">
                                <span className="text-[#888]">Unit price</span>
                                <span className="text-[#ededed] font-mono">$0.01 / credit</span>
                            </div>
                            <div className="h-px bg-[#1f1f1f]" />
                            <div className="flex items-center justify-between">
                                <span className="text-[#888] text-sm">Total</span>
                                <span className="text-2xl font-display font-bold text-[#ededed]">
                                    ${customPriceUsd.toFixed(2)}
                                    <span className="text-xs text-[#555] ml-1">USD</span>
                                </span>
                            </div>
                            <button
                                disabled={purchasing || customAmount < 100}
                                onClick={() => initiateCheckout(customAmount)}
                                className="w-full py-3 rounded-xl bg-blue-500 text-white font-bold text-sm hover:bg-blue-400 transition-all disabled:opacity-40 disabled:cursor-not-allowed flex items-center justify-center gap-2"
                            >
                                {purchasing ? (
                                    <>
                                        <span className="w-4 h-4 border-2 border-white/30 border-t-white rounded-full animate-spin" />
                                        Processing…
                                    </>
                                ) : (
                                    'Purchase Custom Amount'
                                )}
                            </button>
                        </div>
                    </div>
                </div>

                {/* ---- Credit cost reference ---- */}
                <div className="rounded-2xl border border-dashed border-[#1f1f1f] p-6">
                    <h4 className="text-xs font-bold uppercase tracking-widest text-[#555] mb-4">
                        Credit Cost Per Channel
                    </h4>
                    <div className="grid grid-cols-2 sm:grid-cols-4 gap-3">
                        {[
                            { channel: 'SMS',       cost: 10, color: 'text-red-400',    icon: '📱' },
                            { channel: 'WhatsApp',  cost: 5,  color: 'text-green-400',  icon: '💬' },
                            { channel: 'Email',     cost: 1,  color: 'text-blue-400',   icon: '✉️' },
                            { channel: 'Push / Other', cost: 1, color: 'text-purple-400', icon: '🔔' },
                        ].map(item => (
                            <div key={item.channel} className="flex items-center gap-3 p-4 rounded-xl bg-[#0a0a0a] border border-[#1a1a1a]">
                                <span className="text-xl">{item.icon}</span>
                                <div>
                                    <p className="text-xs text-[#888]">{item.channel}</p>
                                    <p className={`text-sm font-bold font-mono ${item.color}`}>
                                        -{item.cost} cr
                                    </p>
                                </div>
                            </div>
                        ))}
                    </div>
                </div>
            </div>
        </AppShell>
    );
}
