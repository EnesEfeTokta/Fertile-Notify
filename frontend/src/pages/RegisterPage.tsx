import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { authService } from "../api/authService";

/* ─── Brand Panel (left column) ─────────────────────────────────────────── */
const BrandPanel = () => (
    <div className="hidden lg:flex flex-col justify-between h-full px-12 py-12 bg-secondary border-r border-primary relative overflow-hidden">
        <div className="absolute inset-0 bg-grid-fine opacity-60 pointer-events-none" />
        <div className="absolute inset-0 pointer-events-none"
            style={{ background: "radial-gradient(ellipse 80% 60% at 80% 20%, rgba(59,130,246,0.08) 0%, transparent 70%)" }}
        />
        <div className="relative z-10 flex items-center gap-2">
            <div className="w-7 h-7 bg-white rounded-md flex items-center justify-center">
                <span className="text-black font-bold text-[10px] font-mono">FN</span>
            </div>
            <span className="font-display font-bold text-[15px]">
                fertile<span className="text-accent-primary">notify</span>
            </span>
        </div>

        <div className="relative z-10 space-y-6">
            <h2 className="font-display font-bold text-2xl leading-tight tracking-tight">
                Notifications that<br />
                <span className="text-accent-light">just work.</span>
            </h2>
            <p className="text-sm text-secondary leading-relaxed">
                Set up in 5 minutes. Reach your users across Email, SMS, Slack,
                Push, and more with a single API call.
            </p>
            <div className="space-y-3">
                {[
                    { icon: "🚀", label: "2,000 notifications free, no credit card" },
                    { icon: "🔧", label: "One API for all channels" },
                    { icon: "📊", label: "Real-time delivery tracking" },
                ].map(f => (
                    <div key={f.label} className="flex items-center gap-3 text-sm text-secondary">
                        <span className="w-7 h-7 rounded-lg bg-elevated flex items-center justify-center">{f.icon}</span>
                        {f.label}
                    </div>
                ))}
            </div>
        </div>
        <p className="relative z-10 text-[11px] text-muted">© {new Date().getFullYear()} Fertile Notify</p>
    </div>
);

/* ─── Password rules ─────────────────────────────────────────────────────── */
const passwordRules = [
    { label: "At least 8 characters", test: (p: string) => p.length >= 8 },
    { label: "Upper & lower case letter", test: (p: string) => /[a-z]/.test(p) && /[A-Z]/.test(p) },
    { label: "At least one digit", test: (p: string) => /\d/.test(p) },
    { label: "Special character (@, #, $, &…)", test: (p: string) => /[^A-Za-z0-9]/.test(p) },
];

/* ─── Step progress indicator ────────────────────────────────────────────── */
const STEPS = [
    { label: "Company", icon: "🏢" },
    { label: "Account",  icon: "👤" },
    { label: "Plan",     icon: "⚡" },
];

const StepIndicator = ({ current }: { current: number }) => (
    <div className="flex items-center gap-0 mb-8">
        {STEPS.map((step, i) => {
            const done    = i < current;
            const active  = i === current;
            return (
                <React.Fragment key={i}>
                    <div className="flex flex-col items-center gap-1.5">
                        <div className={`w-8 h-8 rounded-full flex items-center justify-center text-sm border transition-all duration-300 ${
                            done    ? "bg-accent-primary border-accent-primary text-white"
                            : active ? "bg-accent-dim border-blue-500/40 text-accent-primary"
                            : "bg-tertiary border-primary text-muted"
                        }`}>
                            {done ? (
                                <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2.5} d="M5 13l4 4L19 7" />
                                </svg>
                            ) : (
                                <span>{i + 1}</span>
                            )}
                        </div>
                        <span className={`text-[10px] font-semibold uppercase tracking-wider transition-colors ${
                            active ? "text-accent-primary" : done ? "text-secondary" : "text-muted"
                        }`}>{step.label}</span>
                    </div>
                    {i < STEPS.length - 1 && (
                        <div className={`flex-1 h-px mx-2 mb-5 transition-all duration-500 ${i < current ? "bg-accent-primary" : "bg-primary"}`} />
                    )}
                </React.Fragment>
            );
        })}
    </div>
);

/* ─── Shared field helpers ───────────────────────────────────────────────── */
const FieldLabel = ({ children, optional }: { children: React.ReactNode; optional?: boolean }) => (
    <label className="block text-xs font-semibold uppercase tracking-wider text-tertiary mb-2">
        {children}{optional && <span className="text-muted normal-case font-normal tracking-normal ml-1">(optional)</span>}
    </label>
);

/* ─── Main Component ─────────────────────────────────────────────────────── */
export default function RegisterPage() {
    const navigate = useNavigate();
    const [step, setStep] = useState(0);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    /* Step 1 — Company */
    const [companyName, setCompanyName]             = useState("");
    const [companyDescription, setCompanyDescription] = useState("");
    const [logoUrl, setLogoUrl]                     = useState("");
    const [websiteUrl, setWebsiteUrl]               = useState("");
    const [location, setLocation]                   = useState("");

    /* Step 2 — Account */
    const [email, setEmail]               = useState("");
    const [phoneNumber, setPhoneNumber]   = useState("");
    const [password, setPassword]         = useState("");
    const [showPassword, setShowPassword] = useState(false);

    /* Step 3 — Plan */
    const [plan, setPlan]     = useState<"Free" | "Pro" | "Enterprise">("Free");
    const [agreed, setAgreed] = useState(false);

    const isPasswordValid = passwordRules.every(r => r.test(password));

    /* ── Validation per step ── */
    const canProceedStep0 = companyName.trim().length > 0 && companyDescription.trim().length > 0 && location.trim().length > 0;
    const canProceedStep1 = email.trim().length > 0 && isPasswordValid;

    const handleNext = () => {
        setError(null);
        if (step === 0 && !canProceedStep0) { setError("Company name is required."); return; }
        if (step === 1 && !canProceedStep1) { setError("Please fill in all required fields and meet password requirements."); return; }
        setStep(s => s + 1);
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!agreed) { setError("You must agree to the Terms of Use and Privacy Policy."); return; }
        setLoading(true);
        setError(null);
        try {
            await authService.register({
                companyName,
                companyDescription: companyDescription || undefined,
                logoUrl: logoUrl || undefined,
                websiteUrl: websiteUrl || undefined,
                location: location || undefined,
                email,
                password,
                phoneNumber: phoneNumber || undefined,
                plan,
            });
            navigate("/login");
        } catch {
            setError("Registration failed. Please try again or contact support.");
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="min-h-screen bg-primary flex">
            <div className="w-[420px] shrink-0">
                <BrandPanel />
            </div>

            <div className="flex-1 flex items-start justify-center px-6 py-12 overflow-y-auto relative">
                <div className="fixed inset-0 bg-grid-fine opacity-30 pointer-events-none z-0" />

                <div className="relative z-10 w-full max-w-[440px] space-y-6 mt-4">
                    {/* Back to home */}
                    <button
                        onClick={() => navigate("/")}
                        className="flex items-center gap-1.5 text-xs text-tertiary hover:text-primary transition-colors"
                    >
                        <svg className="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
                        </svg>
                        Back to home
                    </button>

                    <div>
                        <h1 className="font-display font-bold text-2xl tracking-tight">Create your account</h1>
                        <p className="text-sm text-secondary mt-1.5">Start building with Fertile Notify for free</p>
                    </div>

                    {/* Step Indicator */}
                    <StepIndicator current={step} />

                    {/* Error */}
                    {error && (
                        <div className="bg-error-dim border border-red-500/20 text-red-400 p-3.5 rounded-lg text-sm flex items-start gap-3">
                            <svg className="w-4 h-4 shrink-0 mt-0.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                            </svg>
                            {error}
                        </div>
                    )}

                    {/* ── STEP 0: COMPANY ── */}
                    {step === 0 && (
                        <div className="space-y-4 animate-[fadeIn_0.2s_ease]">
                            <div className="card p-1 mb-2">
                                <p className="text-xs text-tertiary px-4 py-2">
                                    Tell us about your company. This information will appear on your profile.
                                </p>
                            </div>

                            <div>
                                <FieldLabel>Company Name <span className="text-red-500">*</span></FieldLabel>
                                <input
                                    type="text"
                                    className="input-modern"
                                    value={companyName}
                                    onChange={e => setCompanyName(e.target.value)}
                                    placeholder="Acme Corp"
                                    autoFocus
                                />
                            </div>

                            <div>
                                <FieldLabel>Company Description <span className="text-red-500">*</span></FieldLabel>
                                <textarea
                                    className="input-modern resize-none"
                                    rows={3}
                                    value={companyDescription}
                                    onChange={e => setCompanyDescription(e.target.value)}
                                    placeholder="A short description of what your company does…"
                                />
                            </div>

                            <div className="grid grid-cols-2 gap-3">
                                <div>
                                    <FieldLabel optional>Logo URL</FieldLabel>
                                    <input
                                        type="url"
                                        className="input-modern"
                                        value={logoUrl}
                                        onChange={e => setLogoUrl(e.target.value)}
                                        placeholder="https://…/logo.png"
                                    />
                                </div>
                                <div>
                                    <FieldLabel optional>Website URL</FieldLabel>
                                    <input
                                        type="url"
                                        className="input-modern"
                                        value={websiteUrl}
                                        onChange={e => setWebsiteUrl(e.target.value)}
                                        placeholder="https://acme.com"
                                    />
                                </div>
                            </div>

                            <div>
                                <FieldLabel>Location <span className="text-red-500">*</span></FieldLabel>
                                <input
                                    type="text"
                                    className="input-modern"
                                    value={location}
                                    onChange={e => setLocation(e.target.value)}
                                    placeholder="San Francisco, CA"
                                />
                            </div>

                            <button
                                onClick={handleNext}
                                disabled={!canProceedStep0}
                                className="btn-primary w-full py-3 mt-2 text-[15px] flex items-center justify-center gap-2 disabled:opacity-40"
                            >
                                Continue
                                <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5l7 7-7 7" />
                                </svg>
                            </button>
                        </div>
                    )}

                    {/* ── STEP 1: ACCOUNT ── */}
                    {step === 1 && (
                        <div className="space-y-4 animate-[fadeIn_0.2s_ease]">
                            <div className="card p-1 mb-2">
                                <p className="text-xs text-tertiary px-4 py-2">
                                    Set up your login credentials. You'll use these to sign in to your account.
                                </p>
                            </div>

                            <div>
                                <FieldLabel>Email Address</FieldLabel>
                                <input
                                    type="email"
                                    className="input-modern"
                                    value={email}
                                    onChange={e => setEmail(e.target.value)}
                                    placeholder="you@company.com"
                                    autoFocus
                                />
                            </div>

                            <div>
                                <FieldLabel optional>Phone</FieldLabel>
                                <input
                                    type="tel"
                                    className="input-modern"
                                    value={phoneNumber}
                                    onChange={e => setPhoneNumber(e.target.value)}
                                    placeholder="+1 555 000 0000"
                                />
                            </div>

                            {/* Password */}
                            <div className="space-y-3">
                                <FieldLabel>Password</FieldLabel>
                                <div className="relative">
                                    <input
                                        type={showPassword ? "text" : "password"}
                                        className="input-modern pr-11"
                                        value={password}
                                        onChange={e => setPassword(e.target.value)}
                                        placeholder="••••••••"
                                    />
                                    <button
                                        type="button"
                                        onClick={() => setShowPassword(!showPassword)}
                                        className="absolute right-3 top-1/2 -translate-y-1/2 text-tertiary hover:text-primary transition-colors"
                                    >
                                        {showPassword ? (
                                            <svg className="w-4.5 h-4.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                                            </svg>
                                        ) : (
                                            <svg className="w-4.5 h-4.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13.875 18.825A10.05 10.05 0 0112 19c-4.478 0-8.268-2.943-9.543-7a9.97 9.97 0 011.563-3.029m5.858.908a3 3 0 114.243 4.243M9.878 9.878l4.242 4.242M9.88 9.88l-3.29-3.29m7.532 7.532l3.29 3.29M3 3l18 18" />
                                            </svg>
                                        )}
                                    </button>
                                </div>

                                {/* Password strength */}
                                {password.length > 0 && (
                                    <div className="grid grid-cols-2 gap-1.5">
                                        {passwordRules.map((rule, i) => {
                                            const met = rule.test(password);
                                            return (
                                                <div key={i} className={`flex items-center gap-1.5 text-[11px] transition-colors ${met ? "text-green-500" : "text-tertiary"}`}>
                                                    <svg className={`w-3 h-3 shrink-0 ${met ? "text-green-500" : "text-muted"}`} fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                                        {met
                                                            ? <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2.5} d="M5 13l4 4L19 7" />
                                                            : <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                                                        }
                                                    </svg>
                                                    {rule.label}
                                                </div>
                                            );
                                        })}
                                    </div>
                                )}
                            </div>

                            <div className="flex gap-3 mt-2">
                                <button
                                    onClick={() => { setStep(0); setError(null); }}
                                    className="btn-secondary py-3 px-5 flex items-center gap-2 text-sm"
                                >
                                    <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
                                    </svg>
                                    Back
                                </button>
                                <button
                                    onClick={handleNext}
                                    disabled={!canProceedStep1}
                                    className="btn-primary flex-1 py-3 text-[15px] flex items-center justify-center gap-2 disabled:opacity-40"
                                >
                                    Continue
                                    <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5l7 7-7 7" />
                                    </svg>
                                </button>
                            </div>
                        </div>
                    )}

                    {/* ── STEP 2: PLAN ── */}
                    {step === 2 && (
                        <form onSubmit={handleSubmit} className="space-y-5 animate-[fadeIn_0.2s_ease]">
                            <div className="card p-1 mb-2">
                                <p className="text-xs text-tertiary px-4 py-2">
                                    Choose a plan. You can always upgrade later from your dashboard.
                                </p>
                            </div>

                            {/* Plan picker */}
                            <div className="grid grid-cols-3 gap-3">
                                {(["Free", "Pro", "Enterprise"] as const).map(p => {
                                    const meta = {
                                        Free:       { badge: "Forever free",  limit: "2,000 / mo",  color: "bg-tertiary border-secondary",           active: "bg-accent-dim border-blue-500/30 text-accent-primary" },
                                        Pro:        { badge: "Most popular",  limit: "50,000 / mo", color: "bg-blue-500/5 border-blue-500/20",       active: "bg-blue-500/15 border-blue-500/40 text-blue-300" },
                                        Enterprise: { badge: "Custom",        limit: "Unlimited",   color: "bg-purple-500/5 border-purple-500/20",   active: "bg-purple-500/15 border-purple-500/40 text-purple-300" },
                                    }[p];
                                    const isSelected = plan === p;
                                    return (
                                        <button
                                            key={p}
                                            type="button"
                                            onClick={() => setPlan(p)}
                                            className={`relative flex flex-col items-center gap-2 p-4 rounded-xl border transition-all duration-200 text-center
                                                ${isSelected ? meta.active : meta.color + " hover:border-hover text-secondary"}`}
                                        >
                                            {isSelected && (
                                                <div className="absolute top-1.5 right-1.5 w-4 h-4 rounded-full bg-accent-primary flex items-center justify-center">
                                                    <svg className="w-2.5 h-2.5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={3} d="M5 13l4 4L19 7" />
                                                    </svg>
                                                </div>
                                            )}
                                            <span className="text-sm font-bold">{p}</span>
                                            <span className="text-[10px] opacity-70">{meta.limit}</span>
                                            <span className="text-[9px] font-semibold uppercase tracking-wider opacity-50">{meta.badge}</span>
                                        </button>
                                    );
                                })}
                            </div>

                            {/* Summary card */}
                            <div className="rounded-xl border border-primary bg-secondary/50 p-4 space-y-2 text-sm">
                                <p className="text-xs font-bold uppercase tracking-widest text-muted mb-3">Registration Summary</p>
                                <div className="flex justify-between text-secondary">
                                    <span>Company</span>
                                    <span className="font-semibold text-primary truncate max-w-[180px]">{companyName}</span>
                                </div>
                                <div className="flex justify-between text-secondary">
                                    <span>Email</span>
                                    <span className="font-semibold text-primary truncate max-w-[180px]">{email}</span>
                                </div>
                                {websiteUrl && (
                                    <div className="flex justify-between text-secondary">
                                        <span>Website</span>
                                        <span className="font-semibold text-primary truncate max-w-[180px]">{websiteUrl}</span>
                                    </div>
                                )}
                                <div className="flex justify-between text-secondary">
                                    <span>Plan</span>
                                    <span className="font-semibold text-accent-primary">{plan}</span>
                                </div>
                            </div>

                            {/* Terms */}
                            <div className="flex items-start gap-3 py-1">
                                <div className="flex h-5 items-center">
                                    <input
                                        id="legal-agreement"
                                        type="checkbox"
                                        required
                                        checked={agreed}
                                        onChange={e => setAgreed(e.target.checked)}
                                        className="h-4 w-4 rounded border-primary bg-secondary text-accent-primary focus:ring-accent-primary accent-accent-primary cursor-pointer"
                                    />
                                </div>
                                <div className="text-[13px] leading-5 text-secondary">
                                    <label htmlFor="legal-agreement" className="cursor-pointer">
                                        I agree to the{" "}
                                        <button type="button" onClick={() => navigate("/terms")} className="text-accent-primary hover:underline font-medium">Terms of Use</button>
                                        {" "}and{" "}
                                        <button type="button" onClick={() => navigate("/privacy")} className="text-accent-primary hover:underline font-medium">Privacy Policy</button>.
                                    </label>
                                </div>
                            </div>

                            <div className="flex gap-3">
                                <button
                                    type="button"
                                    onClick={() => { setStep(1); setError(null); }}
                                    className="btn-secondary py-3 px-5 flex items-center gap-2 text-sm"
                                >
                                    <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
                                    </svg>
                                    Back
                                </button>
                                <button
                                    type="submit"
                                    className="btn-primary flex-1 py-3 text-[15px]"
                                    disabled={loading || !agreed}
                                >
                                    {loading ? (
                                        <span className="flex items-center justify-center gap-2">
                                            <span className="spinner w-4 h-4" />
                                            Creating account…
                                        </span>
                                    ) : (
                                        "Create Account →"
                                    )}
                                </button>
                            </div>
                        </form>
                    )}

                    <p className="text-center text-sm text-secondary pb-6">
                        Already have an account?{" "}
                        <button onClick={() => navigate("/login")} className="text-accent-primary hover:underline font-medium">
                            Sign in
                        </button>
                    </p>
                </div>
            </div>
        </div>
    );
}