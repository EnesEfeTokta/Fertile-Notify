import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { authService } from "../api/authService";

// ---- Brand panel (left side) ----
const BrandPanel = () => (
    <div className="hidden lg:flex flex-col justify-between h-full px-12 py-12 bg-secondary border-r border-primary relative overflow-hidden">
        {/* Grid background */}
        <div className="absolute inset-0 bg-grid-fine opacity-60 pointer-events-none" />
        <div className="absolute inset-0 pointer-events-none"
            style={{ background: "radial-gradient(ellipse 80% 60% at 20% 80%, rgba(59,130,246,0.08) 0%, transparent 70%)" }}
        />

        {/* Logo */}
        <div className="relative z-10 flex items-center gap-2">
            <div className="w-7 h-7 bg-white rounded-md flex items-center justify-center">
                <span className="text-black font-bold text-[10px] font-mono">FN</span>
            </div>
            <span className="font-display font-bold text-[15px]">
                fertile<span className="text-accent-primary">notify</span>
            </span>
        </div>

        {/* Middle content */}
        <div className="relative z-10 space-y-8">
            <blockquote className="space-y-4">
                <p className="font-display text-2xl font-bold leading-tight tracking-tight">
                    "The notification<br />layer we always<br />needed."
                </p>
                <p className="text-sm text-tertiary">— Developer, shipped with Fertile Notify</p>
            </blockquote>

            <div className="space-y-3">
                {[
                    { icon: "✉️", label: "Multi-channel delivery" },
                    { icon: "⚡", label: "Single API for all channels" },
                    { icon: "📊", label: "Real-time delivery analytics" },
                ].map(f => (
                    <div key={f.label} className="flex items-center gap-3 text-sm text-secondary">
                        <span className="w-7 h-7 rounded-lg bg-elevated flex items-center justify-center text-base">{f.icon}</span>
                        {f.label}
                    </div>
                ))}
            </div>
        </div>

        {/* Bottom */}
        <p className="relative z-10 text-[11px] text-muted">
            © {new Date().getFullYear()} Fertile Notify
        </p>
    </div>
);

export default function LoginPage() {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [showPassword, setShowPassword] = useState(false);
    const [otpCode, setOtpCode] = useState("");
    const [error, setError] = useState<string | null>(null);
    const [showOtpForm, setShowOtpForm] = useState(false);
    const [loading, setLoading] = useState(false);
    const navigate = useNavigate();

    const handleLogin = async (e: React.FormEvent) => {
        e.preventDefault();
        setLoading(true);
        setError(null);
        try {
            await authService.login({ email, password });
            setShowOtpForm(true);
        } catch (err) {
            setError("Invalid email or password. Please try again.");
            console.error("Login error:", err);
        } finally {
            setLoading(false);
        }
    };

    const handleOtpVerification = async (e: React.FormEvent) => {
        e.preventDefault();
        setLoading(true);
        setError(null);
        try {
            const data = await authService.verifyOtp({ email, otpCode });
            localStorage.setItem("accessToken", data.accessToken);
            localStorage.setItem("refreshToken", data.refreshToken.token);
            navigate("/dashboard");
        } catch (err) {
            setError("Invalid verification code. Please try again.");
            console.error("OTP error:", err);
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="min-h-screen bg-primary flex">
            {/* Left Brand Panel */}
            <div className="w-[420px] shrink-0">
                <BrandPanel />
            </div>

            {/* Right Form Panel */}
            <div className="flex-1 flex items-center justify-center px-6 py-12 relative">
                <div className="fixed inset-0 bg-grid-fine opacity-30 pointer-events-none z-0" />

                <div className="relative z-10 w-full max-w-[380px] space-y-8">
                    {/* Back to home */}
                    <button
                        onClick={() => navigate("/")}
                        className="flex items-center gap-1.5 text-xs text-tertiary hover:text-primary transition-colors mb-4"
                    >
                        <svg className="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
                        </svg>
                        Back to home
                    </button>

                    {!showOtpForm ? (
                        <>
                            {/* Header */}
                            <div>
                                <h1 className="font-display font-bold text-2xl tracking-tight">Welcome back</h1>
                                <p className="text-sm text-secondary mt-1.5">Sign in to your Fertile Notify account</p>
                            </div>

                            {error && (
                                <div className="bg-error-dim border border-red-500/20 text-red-400 p-3.5 rounded-lg text-sm flex items-start gap-3">
                                    <svg className="w-4 h-4 shrink-0 mt-0.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                                    </svg>
                                    {error}
                                </div>
                            )}

                            <form onSubmit={handleLogin} className="space-y-4">
                                <div>
                                    <label className="block text-xs font-semibold uppercase tracking-wider text-tertiary mb-2">Email</label>
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
                                    <div className="flex justify-between items-center mb-2">
                                        <label className="text-xs font-semibold uppercase tracking-wider text-tertiary">Password</label>
                                        <button
                                            type="button"
                                            onClick={() => alert("Password reset coming soon. Contact support@fertile-notify.shop")}
                                            className="text-[11px] text-accent-primary hover:underline"
                                        >
                                            Forgot password?
                                        </button>
                                    </div>
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
                                </div>

                                <button
                                    type="submit"
                                    className="btn-primary w-full py-3 mt-2 text-[15px]"
                                    disabled={loading}
                                >
                                    {loading ? (
                                        <span className="flex items-center justify-center gap-2">
                                            <span className="spinner w-4 h-4" />
                                            Signing in…
                                        </span>
                                    ) : (
                                        "Sign in"
                                    )}
                                </button>
                            </form>

                            <p className="text-center text-sm text-secondary">
                                Don't have an account?{" "}
                                <button onClick={() => navigate("/register")} className="text-accent-primary hover:underline font-medium">
                                    Create one
                                </button>
                            </p>
                        </>
                    ) : (
                        <>
                            {/* OTP Header */}
                            <div>
                                <div className="w-11 h-11 bg-accent-dim border border-blue-500/20 rounded-xl flex items-center justify-center mb-4 text-xl">
                                    🔐
                                </div>
                                <h1 className="font-display font-bold text-2xl tracking-tight">Check your email</h1>
                                <p className="text-sm text-secondary mt-1.5">
                                    We sent a verification code to <span className="text-primary font-medium">{email}</span>
                                </p>
                            </div>

                            {error && (
                                <div className="bg-error-dim border border-red-500/20 text-red-400 p-3.5 rounded-lg text-sm">
                                    {error}
                                </div>
                            )}

                            <form onSubmit={handleOtpVerification} className="space-y-4">
                                <div>
                                    <label className="block text-xs font-semibold uppercase tracking-wider text-tertiary mb-2">
                                        Verification code
                                    </label>
                                    <input
                                        type="text"
                                        className="input-modern text-center text-2xl tracking-[0.5em] font-mono"
                                        value={otpCode}
                                        onChange={e => setOtpCode(e.target.value.replace(/\D/g, "").slice(0, 6))}
                                        placeholder="000000"
                                        maxLength={6}
                                        required
                                    />
                                </div>
                                <button
                                    type="submit"
                                    className="btn-primary w-full py-3 text-[15px]"
                                    disabled={loading || otpCode.length < 6}
                                >
                                    {loading ? (
                                        <span className="flex items-center justify-center gap-2">
                                            <span className="spinner w-4 h-4" />
                                            Verifying…
                                        </span>
                                    ) : (
                                        "Verify"
                                    )}
                                </button>
                            </form>

                            <button onClick={() => { setShowOtpForm(false); setError(null); }} className="btn-secondary w-full">
                                ← Go back
                            </button>
                        </>
                    )}
                </div>
            </div>
        </div>
    );
}