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
        <div className="min-h-screen flex items-center justify-center bg-gray-100">
            <div className="bg-white p-8 rounded-lg shadow-md w-96">
                <h2 className="text-2xl font-bold mb-6 text-center text-gray-800">Kayıt Ol</h2>

                {error && <div className="bg-red-100 text-red-700 p-2 mb-4 rounded">{error}</div>}

                <form onSubmit={handleRegister} className="space-y-4">
                    <div>
                        <label className="block text-gray-700 mb-1">Firma Adı</label>
                        <input
                            type="text"
                            className="w-full border p-2 rounded focus:outline-none focus:ring-2 focus:ring-blue-500"
                            value={companyName}
                            onChange={(e) => setCompanyName(e.target.value)}
                            placeholder="Firma Adı"
                            required
                        />
                    </div>
                    <div>
                        <label className="block text-gray-700 mb-1">Firma E-Postası</label>
                        <input
                            type="email"
                            className="w-full border p-2 rounded focus:outline-none focus:ring-2 focus:ring-blue-500"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            placeholder="firma@ornek.com"
                            required
                        />
                    </div>
                    <div>
                        <label className="block text-gray-700 mb-1">Firma Telefon Numarası</label>
                        <input
                            type="tel"
                            className="w-full border p-2 rounded focus:outline-none focus:ring-2 focus:ring-blue-500"
                            value={phoneNumber}
                            onChange={(e) => setPhoneNumber(e.target.value)}
                            placeholder="Firma Telefon Numarası (opsiyonel)"
                        />
                    </div>
                    <div>
                        <label className="block text-gray-700 mb-1">Seçilen Plan</label>
                        <select
                            className="w-full border p-2 rounded focus:outline-none focus:ring-2 focus:ring-blue-500"
                            value={plan}
                            onChange={(e) => setPlan(e.target.value as "Free" | "Pro" | "Enterprise")}>
                            <option value="Free">Free</option>
                            <option value="Pro">Pro</option>
                            <option value="Enterprise">Enterprise</option>
                        </select>
                    </div>
                    <div>
                        <label className="block text-gray-700 mb-1">Şifre</label>
                        <input
                            type="password"
                            className="w-full border p-2 rounded focus:outline-none focus:ring-2 focus:ring-blue-500"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            placeholder="Şifrenizi girin"
                            required
                        />
                    </div>
                    <button
                        type="submit"
                        className="w-full bg-blue-600 text-white py-2 rounded hover:bg-blue-700 transition">
                        Kayıt Ol
                    </button>
                </form>
            </div>
        </div>
    );
}