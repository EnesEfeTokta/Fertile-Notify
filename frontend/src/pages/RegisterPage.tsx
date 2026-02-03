import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { authService } from "../api/authService";

export default function RegisterPage() {
    const [companyName, setCompanyName] = useState("");
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [phoneNumber, setPhoneNumber] = useState("");
    const [plan, setPlan] = useState<"Free" | "Pro" | "Enterprise">("Free");
    const [error, setError] = useState<string | null>(null);
    const navigate = useNavigate();

    const handleRegister = async (e: React.FormEvent) => {
        e.preventDefault();
        try {
            await authService.register({ companyName, email, password, phoneNumber: phoneNumber || undefined, plan });
            navigate("/login");
        } catch (error) {
            setError("Registration failed. Please try again.");
            console.error("Registration error:", error);
        }
    };

    return (
        <div className="min-h-screen bg-animated-gradient flex items-center justify-center px-4 py-8 animate-fade-in">
            <div className="glass p-8 md:p-10 w-full max-w-md animate-slide-up clip-sharp border-t-4 border-pink-500">
                {/* Logo/Title */}
                <div className="text-center mb-8">
                    <h2 className="text-4xl font-display font-bold gradient-text mb-2 uppercase tracking-wider">Kayıt Ol</h2>
                    <div className="h-1 w-20 bg-gradient-to-r from-pink-500 to-purple-500 mx-auto mb-3"></div>
                    <p className="text-gray-400 uppercase text-sm tracking-wide">Yeni hesap oluşturun</p>
                </div>

                {/* Error Message */}
                {error && (
                    <div className="bg-red-900/30 border-l-4 border-red-500 text-red-300 p-4 mb-6 clip-sharp-sm animate-slide-up">
                        <p className="font-medium uppercase text-sm">{error}</p>
                    </div>
                )}

                {/* Register Form */}
                <form onSubmit={handleRegister} className="space-y-5">
                    <div>
                        <label className="block text-purple-300 font-semibold mb-2 uppercase text-sm tracking-wide">Firma Adı</label>
                        <input
                            type="text"
                            className="input-modern"
                            value={companyName}
                            onChange={(e) => setCompanyName(e.target.value)}
                            placeholder="Firma Adı"
                            required
                        />
                    </div>

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
                        <label className="block text-purple-300 font-semibold mb-2 uppercase text-sm tracking-wide">Firma Telefon Numarası</label>
                        <input
                            type="tel"
                            className="input-modern"
                            value={phoneNumber}
                            onChange={(e) => setPhoneNumber(e.target.value)}
                            placeholder="(Opsiyonel)"
                        />
                    </div>

                    <div>
                        <label className="block text-purple-300 font-semibold mb-2 uppercase text-sm tracking-wide">Seçilen Plan</label>
                        <select
                            className="input-modern cursor-pointer"
                            value={plan}
                            onChange={(e) => setPlan(e.target.value as "Free" | "Pro" | "Enterprise")}
                        >
                            <option value="Free">Free - Ücretsiz</option>
                            <option value="Pro">Pro - Profesyonel</option>
                            <option value="Enterprise">Enterprise - Kurumsal</option>
                        </select>
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
                        className="btn-gradient w-full text-lg uppercase tracking-wider"
                    >
                        Kayıt Ol
                    </button>
                </form>

                {/* Login Link */}
                <div className="mt-6 text-center">
                    <p className="text-gray-400">
                        Zaten hesabınız var mı?{" "}
                        <button
                            onClick={() => navigate("/login")}
                            className="text-pink-400 font-semibold hover:text-purple-400 transition-colors uppercase text-sm"
                        >
                            Giriş Yap
                        </button>
                    </p>
                </div>
            </div>
        </div>
    );
}