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
        } catch (err) {
            setError("Invalid email or password");
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
        } catch (err) {
            setError("Invalid OTP code");
            console.error("OTP verification error:", err);
        }
    };

    return (
        <div className="min-h-screen bg-animated-gradient flex items-center justify-center px-4 animate-fade-in">
            {!showOtpForm ? (
                // Login Form
                <div className="glass p-8 md:p-10 w-full max-w-md animate-slide-up clip-sharp border-t-4 border-purple-500">
                    {/* Logo/Title */}
                    <div className="text-center mb-8">
                        <h2 className="text-4xl font-display font-bold gradient-text mb-2 uppercase tracking-wider">Login</h2>
                        <div className="h-1 w-20 bg-gradient-to-r from-purple-500 to-pink-500 mx-auto mb-3"></div>
                        <p className="text-gray-400 uppercase text-sm tracking-wide">Access Your Account</p>
                    </div>

                    {/* Error Message */}
                    {error && (
                        <div className="bg-red-900/30 border-l-4 border-red-500 text-red-300 p-4 mb-6 clip-sharp-sm animate-slide-up">
                            <p className="font-medium uppercase text-sm">{error}</p>
                        </div>
                    )}

                    {/* Login Form */}
                    <form onSubmit={handleLogin} className="space-y-5">
                        <div>
                            <label className="block text-purple-300 font-semibold mb-2 uppercase text-sm tracking-wide">Company Email</label>
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
                            <label className="block text-purple-300 font-semibold mb-2 uppercase text-sm tracking-wide">Password</label>
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
                            className="btn-gradient w-full text-lg uppercase tracking-wider mt-6"
                        >
                            Login
                        </button>
                    </form>

                    {/* Register Link */}
                    <div className="mt-6 text-center">
                        <p className="text-gray-400">
                            Don't have an account?{" "}
                            <button
                                onClick={() => navigate("/register")}
                                className="text-purple-400 font-semibold hover:text-pink-400 transition-colors uppercase text-sm"
                            >
                                Sign Up
                            </button>
                        </p>
                    </div>
                </div>
            ) : (
                // OTP Verify Form
                <div className="glass p-8 md:p-10 w-full max-w-md animate-slide-up clip-sharp border-t-4 border-purple-500">
                    <div className="text-center mb-8">
                        <h2 className="text-4xl font-display font-bold gradient-text mb-2 uppercase tracking-wider">Verify OTP</h2>
                        <div className="h-1 w-20 bg-gradient-to-r from-purple-500 to-pink-500 mx-auto mb-3"></div>
                        <p className="text-gray-400 uppercase text-sm tracking-wide">Enter the OTP sent to your email</p>
                    </div>

                    {/* Timer Display */}
                    <div className="mb-6 text-center">
                        <div className={`inline-block px-6 py-3 rounded-lg ${
                            timeLeft > 60 ? 'bg-green-900/30 border border-green-500' : 'bg-red-900/30 border border-red-500'
                        }`}>
                            <p className="text-sm text-gray-400 mb-1">Code expires in</p>
                            <p className={`text-3xl font-bold font-mono ${
                                timeLeft > 60 ? 'text-green-400' : 'text-red-400'
                            }`}>
                                {formatTime(timeLeft)}
                            </p>
                        </div>
                    </div>

                    {/* Error Message */}
                    {error && (
                        <div className="bg-red-900/30 border-l-4 border-red-500 text-red-300 p-4 mb-6 clip-sharp-sm animate-slide-up">
                            <p className="font-medium uppercase text-sm">{error}</p>
                        </div>
                    )}

                    <form onSubmit={handleVerifyOtp} className="space-y-5">
                        <div>
                            <label className="block text-purple-300 font-semibold mb-2 uppercase text-sm tracking-wide">OTP Code</label>
                            <input
                                type="text"
                                className="input-modern text-center text-2xl tracking-widest"
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
                            className="btn-gradient w-full text-lg uppercase tracking-wider mt-6"
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
                            className="text-purple-400 font-semibold hover:text-pink-400 transition-colors uppercase text-sm"
                        >
                            Back to Login
                        </button>
                    </div>
                </div>
            )}
        </div>
    );
}