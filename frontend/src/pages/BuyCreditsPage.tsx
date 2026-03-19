import { useState, useCallback, useEffect } from 'react';
import { subscriberService } from '../api/subscriberService';
import type { SubscriberProfile } from '../types/subscriber';
import AppShell from '../components/AppShell';
import { useToast } from '../components/Toast';

export default function BuyCreditsPage() {
    const [profile, setProfile] = useState<SubscriberProfile | null>(null);
    const [loading, setLoading] = useState(true);
    const [purchasing, setPurchasing] = useState(false);
    const { showToast, ToastContainer } = useToast();

    const [customAmount, setCustomAmount] = useState<number>(100);
    const customPrice = Number((customAmount * 0.005).toFixed(2));

    const fetchProfile = useCallback(async () => {
        try {
            const data = await subscriberService.getProfile();
            setProfile(data);
        } catch {
            showToast("Failed to load profile.", "error");
        } finally {
            setLoading(false);
        }
    }, [showToast]);

    useEffect(() => { fetchProfile(); }, [fetchProfile]);

    const handleBuy = async (amount: number, price: number) => {
        if (!window.confirm(`Are you sure you want to purchase ${amount.toLocaleString()} credits for ${price} TL?`)) return;
        setPurchasing(true);
        try {
            await subscriberService.buyCredits(amount);
            showToast(`Successfully purchased ${amount.toLocaleString()} credits!`);
            fetchProfile();
        } catch {
            showToast("Purchase failed.", "error");
        } finally {
            setPurchasing(false);
        }
    };

    return (
        <AppShell title="Buy Credits" companyName={profile?.companyName} plan={profile?.subscription?.plan}>
            <ToastContainer />
            
            <div className="max-w-4xl space-y-8 pb-12">
                <div className="card p-8 bg-gradient-to-br from-secondary/50 to-primary border-accent-primary/20 flex flex-col md:flex-row gap-8 items-center">
                    <div className="flex-1">
                        <h2 className="text-2xl font-display font-bold text-primary mb-2">Lifetime Credits</h2>
                        <p className="text-secondary text-sm leading-relaxed mb-6">
                            Subscription credits do not roll over, but extra purchased credits have <strong>lifetime validity</strong> and are used only when your monthly plan limits are exceeded.
                        </p>
                        
                        <div className="flex items-center gap-4 p-4 rounded-xl bg-primary/50 border border-primary w-fit">
                            <div className="w-12 h-12 rounded-full bg-green-500/10 flex items-center justify-center text-2xl">💰</div>
                            <div>
                                <p className="text-xs font-bold uppercase tracking-widest text-muted">Current Extra Balance</p>
                                <p className="text-2xl font-display font-bold text-green-400">
                                    {loading ? "..." : profile?.extraCredits?.toLocaleString() ?? 0}
                                </p>
                            </div>
                        </div>
                    </div>

                    <div className="w-full md:w-80 p-6 rounded-2xl bg-tertiary/30 border border-primary flex flex-col gap-4">
                        <h4 className="text-xs font-bold uppercase tracking-widest text-muted">Custom Amount</h4>
                        <div>
                            <label className="text-[10px] text-tertiary block mb-1">Enter Credit Amount</label>
                            <input 
                                type="number" 
                                min="100"
                                step="100"
                                value={customAmount}
                                onChange={(e) => setCustomAmount(Math.max(0, parseInt(e.target.value) || 0))}
                                className="w-full bg-primary border border-primary rounded-lg px-4 py-2.5 text-primary text-xl font-bold focus:border-accent-primary outline-none transition-all"
                            />
                        </div>
                        <div className="flex items-center justify-between text-sm">
                            <span className="text-secondary">Price (0.005 TL / credit)</span>
                            <span className="font-bold text-primary">{customPrice.toLocaleString()} TL</span>
                        </div>
                        <button 
                            disabled={purchasing || customAmount < 100}
                            onClick={() => handleBuy(customAmount, customPrice)}
                            className="w-full py-3 bg-accent-primary text-primary font-bold rounded-xl hover:bg-accent-light transition-all disabled:opacity-50"
                        >
                            {purchasing ? "Processing..." : "Purchase Now"}
                        </button>
                    </div>
                </div>

                <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
                    {[
                        { amount: 100, price: 5, icon: "🪙" },
                        { amount: 1000, price: 50, icon: "💰" },
                        { amount: 5000, price: 250, icon: "💎", popular: true }
                    ].map(pkg => (
                        <div key={pkg.amount} className={`card p-6 flex flex-col justify-between ${pkg.popular ? 'border-accent-primary/40 shadow-accent relative overflow-hidden' : ''}`}>
                            {pkg.popular && <div className="absolute top-0 right-0 bg-accent-primary text-primary font-bold text-[10px] px-3 py-1 rounded-bl-lg uppercase tracking-wider">Popular</div>}
                            <div>
                                <div className="text-4xl mb-4">{pkg.icon}</div>
                                <h3 className="text-xl font-bold text-primary mb-1">{pkg.amount.toLocaleString()} Credits</h3>
                                <p className="text-sm text-secondary mb-6">Valid forever, never expires.</p>
                                <p className="text-3xl font-display font-bold text-primary mb-6">{pkg.price} TL</p>
                            </div>
                            <button 
                                disabled={purchasing}
                                onClick={() => handleBuy(pkg.amount, pkg.price)}
                                className={`w-full py-3 rounded-lg font-bold text-sm transition-colors ${pkg.popular ? 'bg-accent-primary text-primary hover:bg-accent-light' : 'bg-tertiary text-primary hover:bg-secondary'}`}
                            >
                                {purchasing ? "Processing..." : `Buy for ${pkg.price} TL`}
                            </button>
                        </div>
                    ))}
                </div>

                <div className="card p-6 border-dashed border-tertiary">
                    <h4 className="text-sm font-bold text-primary mb-3">Expected Costs</h4>
                    <div className="grid grid-cols-2 sm:grid-cols-4 gap-4 text-sm">
                        <div className="flex justify-between p-3 rounded bg-tertiary/50"><span>SMS</span><span className="font-mono text-red-400">-10</span></div>
                        <div className="flex justify-between p-3 rounded bg-tertiary/50"><span>WhatsApp</span><span className="font-mono text-orange-400">-5</span></div>
                        <div className="flex justify-between p-3 rounded bg-tertiary/50"><span>Email</span><span className="font-mono text-green-400">-1</span></div>
                        <div className="flex justify-between p-3 rounded bg-tertiary/50"><span>Others</span><span className="font-mono text-blue-400">-1</span></div>
                    </div>
                </div>
            </div>
        </AppShell>
    );
}
