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
        <div className="min-h-screen flex items-center justify-center bg-gray-100">
            <div className="bg-white p-8 rounded-lg shadow-md w-96">
                <h2 className="text-2xl font-bold mb-6 text-center text-gray-800">Giriş Yap</h2>

                {error && <div className="bg-red-100 text-red-700 p-2 mb-4 rounded">{error}</div>}

                <form onSubmit={handleLogin} className="space-y-4">
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
                        Giriş Yap
                    </button>
                </form>
            </div>
        </div>
    );
}