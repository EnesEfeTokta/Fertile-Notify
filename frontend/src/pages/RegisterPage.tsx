import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { authService } from "../api/authService";

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

const passwordRules = [
    { label: "At least 8 characters", test: (p: string) => p.length >= 8 },
    { label: "Upper & lower case letter", test: (p: string) => /[a-z]/.test(p) && /[A-Z]/.test(p) },
    { label: "At least one digit", test: (p: string) => /\d/.test(p) },
    { label: "Special character (@, #, $, &…)", test: (p: string) => /[^A-Za-z0-9]/.test(p) },
];

export default function RegisterPage() {
    const [companyName, setCompanyName] = useState("");
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [showPassword, setShowPassword] = useState(false);
    const [phoneNumber, setPhoneNumber] = useState("");
    const [plan, setPlan] = useState<"Free" | "Pro" | "Enterprise">("Free");
    const [error, setError] = useState<string | null>(null);
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);

    const isPasswordValid = passwordRules.every(r => r.test(password));

    const handleRegister = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!isPasswordValid) {
            setError("Please fulfill all password requirements.");
            return;
        }
        setLoading(true);
        setError(null);
        try {
            await authService.register({ companyName, email, password, phoneNumber: phoneNumber || undefined, plan });
            navigate("/login");
        } catch (err) {
            setError("Registration failed. Please try again or contact support.");
            console.error("Registration error:", err);
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

                <div className="relative z-10 w-full max-w-[400px] space-y-7 mt-4">
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

                    {error && (
                        <div className="bg-error-dim border border-red-500/20 text-red-400 p-3.5 rounded-lg text-sm flex items-start gap-3">
                            <svg className="w-4 h-4 shrink-0 mt-0.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                            </svg>
                            {error}
                        </div>
                    )}

                    <form onSubmit={handleRegister} className="space-y-4">
                        <div>
                            <label className="block text-xs font-semibold uppercase tracking-wider text-tertiary mb-2">Company name</label>
                            <input
                                type="text"
                                className="input-modern"
                                value={companyName}
                                onChange={e => setCompanyName(e.target.value)}
                                placeholder="Acme Corp"
                                required
                            />
                        </div>

                        <div>
                            <label className="block text-xs font-semibold uppercase tracking-wider text-tertiary mb-2">Email address</label>
                            <input
                                type="email"
                                className="input-modern"
                                value={email}
                                onChange={e => setEmail(e.target.value)}
                                placeholder="you@company.com"
                                required
                            />
                        </div>

                        <div>
                            <label className="block text-xs font-semibold uppercase tracking-wider text-tertiary mb-2">
                                Phone <span className="text-muted normal-case font-normal tracking-normal">(optional)</span>
                            </label>
                            <input
                                type="tel"
                                className="input-modern"
                                value={phoneNumber}
                                onChange={e => setPhoneNumber(e.target.value)}
                                placeholder="+1 555 000 0000"
                            />
                        </div>

                        {/* Plan picker */}
                        <div>
                            <label className="block text-xs font-semibold uppercase tracking-wider text-tertiary mb-2">Plan</label>
                            <div className="grid grid-cols-3 gap-2">
                                {(["Free", "Pro", "Enterprise"] as const).map(p => (
                                    <button
                                        key={p}
                                        type="button"
                                        onClick={() => setPlan(p)}
                                        className={`py-2 px-3 text-xs font-semibold rounded-lg border transition-all ${plan === p
                                                ? "bg-accent-dim border-blue-500/30 text-accent-primary"
                                                : "bg-transparent border-primary text-tertiary hover:border-hover"
                                            }`}
                                    >
                                        {p}
                                    </button>
                                ))}
                            </div>
                        </div>

                        {/* Password */}
                        <div className="space-y-3">
                            <label className="block text-xs font-semibold uppercase tracking-wider text-tertiary">Password</label>
                            <div className="relative">
                                <input
                                    type={showPassword ? "text" : "password"}
                                    className="input-modern pr-11"
                                    value={password}
                                    onChange={e => setPassword(e.target.value)}
                                    placeholder="••••••••"
                                    required
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

                            {/* Password strength indicator */}
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

                        <button
                            type="submit"
                            className="btn-primary w-full py-3 mt-2 text-[15px]"
                            disabled={loading || !isPasswordValid}
                        >
                            {loading ? (
                                <span className="flex items-center justify-center gap-2">
                                    <span className="spinner w-4 h-4" />
                                    Creating account…
                                </span>
                            ) : (
                                "Create account"
                            )}
                        </button>
                    </form>

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