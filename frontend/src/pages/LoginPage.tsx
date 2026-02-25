import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { authService } from "../api/authService";

export default function LoginPage() {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [otpCode, setOtpCode] = useState("");
    const [error, setError] = useState<string | null>(null);
    const [showOtpForm, setShowOtpForm] = useState(false);
    const [timeLeft, setTimeLeft] = useState(300);
    const navigate = useNavigate();

    useEffect(() => {
        if (showOtpForm && timeLeft > 0) {
            const timer = setInterval(() => {
                setTimeLeft((prev) => {
                    if (prev <= 1) {
                        setError("OTP code has expired. Please login again.");
                        return 0;
                    }
                    return prev - 1;
                });
            }, 1000);
            return () => clearInterval(timer);
        }
    }, [showOtpForm, timeLeft]);

    const formatTime = (seconds: number) => {
        const mins = Math.floor(seconds / 60);
        const secs = seconds % 60;
        return `${mins}:${secs.toString().padStart(2, '0')}`;
    };

    const handleLogin = async (e: React.FormEvent) => {
        e.preventDefault();
        setError(null);
        try {
            await authService.login({ email, password });
            setShowOtpForm(true);
            setTimeLeft(300);
        } catch (err: any) {
            const message = err.message || (err.errors && err.errors.length > 0 ? err.errors[0] : "Invalid email or password");
            setError(message);
            console.error("Login error:", err);
        }
    };

    const handleVerifyOtp = async (e: React.FormEvent) => {
        e.preventDefault();
        setError(null);

        if (timeLeft === 0) {
            setError("OTP code has expired. Please login again.");
            return;
        }

        try {
            const response = await authService.verifyOtp({ email, otpCode });
            localStorage.setItem("accessToken", response.accessToken);
            localStorage.setItem("refreshToken", response.refreshToken.token);
            navigate("/dashboard");
        } catch (err: any) {
            const message = err.message || (err.errors && err.errors.length > 0 ? err.errors[0] : "Invalid OTP code");
            setError(message);
            console.error("OTP verification error:", err);
        }
    };

    return (
        <div className="min-h-screen bg-primary flex items-center justify-center px-4 animate-fade-in">
            {!showOtpForm ? (
                // Login Form
                <div className="card w-full max-w-md p-8">
                    {/* Logo/Title */}
                    <div className="text-center mb-8">
                        <h2 className="text-3xl font-display font-semibold text-primary mb-2">Login</h2>
                        <p className="text-sm text-secondary">Access your account</p>
                    </div>

                    {/* Error Message */}
                    {error && (
                        <div className="bg-red-500/10 border border-red-500/20 text-red-400 p-3 mb-6 rounded-md text-sm">
                            {error}
                        </div>
                    )}

                    {/* Login Form */}
                    <form onSubmit={handleLogin} className="space-y-4">
                        <div>
                            <label className="block text-sm font-medium text-secondary mb-2">Email</label>
                            <input
                                type="email"
                                className="input-modern"
                                value={email}
                                onChange={(e) => setEmail(e.target.value)}
                                placeholder="company@example.com"
                                required
                            />
                        </div>

                        <div>
                            <label className="block text-sm font-medium text-secondary mb-2">Password</label>
                            <input
                                type="password"
                                className="input-modern"
                                value={password}
                                onChange={(e) => setPassword(e.target.value)}
                                placeholder="••••••••"
                                required
                            />
                        </div>

                        <button
                            type="submit"
                            className="btn-primary w-full mt-6"
                        >
                            Login
                        </button>
                    </form>

                    {/* Register Link */}
                    <div className="mt-6 text-center">
                        <p className="text-sm text-secondary">
                            Don't have an account?{" "}
                            <button
                                onClick={() => navigate("/register")}
                                className="text-primary-500 hover:text-primary-400 transition-colors font-medium"
                            >
                                Sign Up
                            </button>
                        </p>
                    </div>
                </div>
            ) : (
                // OTP Verify Form
                <div className="card w-full max-w-md p-8">
                    <div className="text-center mb-8">
                        <h2 className="text-3xl font-display font-semibold text-primary mb-2">Verify OTP</h2>
                        <p className="text-sm text-secondary">Enter the code sent to your email</p>
                    </div>

                    {/* Timer Display */}
                    <div className="mb-6 text-center">
                        <div className={`inline-block px-6 py-3 rounded-lg border ${timeLeft > 60 ? 'bg-green-500/10 border-green-500/20' : 'bg-red-500/10 border-red-500/20'
                            }`}>
                            <p className="text-xs text-secondary mb-1">Code expires in</p>
                            <p className={`text-2xl font-mono font-semibold ${timeLeft > 60 ? 'text-green-400' : 'text-red-400'
                                }`}>
                                {formatTime(timeLeft)}
                            </p>
                        </div>
                    </div>

                    {/* Error Message */}
                    {error && (
                        <div className="bg-red-500/10 border border-red-500/20 text-red-400 p-3 mb-6 rounded-md text-sm">
                            {error}
                        </div>
                    )}

                    <form onSubmit={handleVerifyOtp} className="space-y-4">
                        <div>
                            <label className="block text-sm font-medium text-secondary mb-2">OTP Code</label>
                            <input
                                type="text"
                                className="input-modern text-center text-xl tracking-widest font-mono"
                                value={otpCode}
                                onChange={(e) => setOtpCode(e.target.value)}
                                placeholder="000000"
                                required
                                maxLength={6}
                                disabled={timeLeft === 0}
                            />
                        </div>

                        <button
                            type="submit"
                            className="btn-primary w-full mt-6"
                            disabled={timeLeft === 0}
                        >
                            Verify OTP
                        </button>
                    </form>

                    {/* Back to Login */}
                    <div className="mt-6 text-center">
                        <button
                            onClick={() => {
                                setShowOtpForm(false);
                                setError(null);
                                setOtpCode("");
                                setTimeLeft(300);
                            }}
                            className="text-sm text-secondary hover:text-primary transition-colors"
                        >
                            Back to Login
                        </button>
                    </div>
                </div>
            )}
        </div>
    );
}