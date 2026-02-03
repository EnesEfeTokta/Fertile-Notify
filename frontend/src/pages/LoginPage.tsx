import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { authService } from "../api/authService";

export default function LoginPage() {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState<string | null>(null);
    const navigate = useNavigate();

    const handleLogin = async (e: React.FormEvent) => {
        e.preventDefault();
        try {
            const response = await authService.login({ email, password });
            localStorage.setItem("token", response.token);
            navigate("/dashboard");
        } catch (err) {
            setError("Invalid email or password");
            console.error("Login error:", err);
        }
    };

    return (
        <div className="min-h-screen bg-animated-gradient flex items-center justify-center px-4 animate-fade-in">
            <div className="glass p-8 md:p-10 w-full max-w-md animate-slide-up clip-sharp border-t-4 border-purple-500">
                {/* Logo/Title */}
                <div className="text-center mb-8">
                    <h2 className="text-4xl font-display font-bold gradient-text mb-2 uppercase tracking-wider">Giriş Yap</h2>
                    <div className="h-1 w-20 bg-gradient-to-r from-purple-500 to-pink-500 mx-auto mb-3"></div>
                    <p className="text-gray-400 uppercase text-sm tracking-wide">Hesabınıza erişim sağlayın</p>
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
                        <label className="block text-purple-300 font-semibold mb-2 uppercase text-sm tracking-wide">Firma E-Postası</label>
                        <input
                            type="email"
                            className="input-modern"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            placeholder="firma@ornek.com"
                            required
                        />
                    </div>

                    <div>
                        <label className="block text-purple-300 font-semibold mb-2 uppercase text-sm tracking-wide">Şifre</label>
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
                        Giriş Yap
                    </button>
                </form>

                {/* Register Link */}
                <div className="mt-6 text-center">
                    <p className="text-gray-400">
                        Hesabınız yok mu?{" "}
                        <button
                            onClick={() => navigate("/register")}
                            className="text-purple-400 font-semibold hover:text-pink-400 transition-colors uppercase text-sm"
                        >
                            Kayıt Ol
                        </button>
                    </p>
                </div>
            </div>
        </div>
    );
}