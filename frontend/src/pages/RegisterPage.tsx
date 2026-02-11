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
        <div className="min-h-screen bg-primary flex items-center justify-center px-4 py-8 animate-fade-in">
            <div className="card w-full max-w-md p-8">
                {/* Logo/Title */}
                <div className="text-center mb-8">
                    <h2 className="text-3xl font-display font-semibold text-primary mb-2">Sign Up</h2>
                    <p className="text-sm text-secondary">Create new account</p>
                </div>

                {/* Error Message */}
                {error && (
                    <div className="bg-red-500/10 border border-red-500/20 text-red-400 p-3 mb-6 rounded-md text-sm">
                        {error}
                    </div>
                )}

                {/* Register Form */}
                <form onSubmit={handleRegister} className="space-y-4">
                    <div>
                        <label className="block text-sm font-medium text-secondary mb-2">Company Name</label>
                        <input
                            type="text"
                            className="input-modern"
                            value={companyName}
                            onChange={(e) => setCompanyName(e.target.value)}
                            placeholder="Company Name"
                            required
                        />
                    </div>

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
                        <label className="block text-sm font-medium text-secondary mb-2">Phone Number</label>
                        <input
                            type="tel"
                            className="input-modern"
                            value={phoneNumber}
                            onChange={(e) => setPhoneNumber(e.target.value)}
                            placeholder="Optional"
                        />
                    </div>

                    <div>
                        <label className="block text-sm font-medium text-secondary mb-2">Select Plan</label>
                        <select
                            className="input-modern cursor-pointer"
                            value={plan}
                            onChange={(e) => setPlan(e.target.value as "Free" | "Pro" | "Enterprise")}
                        >
                            <option value="Free">Free - Basic</option>
                            <option value="Pro">Pro - Professional</option>
                            <option value="Enterprise">Enterprise - Business</option>
                        </select>
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
                        Sign Up
                    </button>
                </form>

                {/* Login Link */}
                <div className="mt-6 text-center">
                    <p className="text-sm text-secondary">
                        Already have an account?{" "}
                        <button
                            onClick={() => navigate("/login")}
                            className="text-primary-500 hover:text-primary-400 transition-colors font-medium"
                        >
                            Login
                        </button>
                    </p>
                </div>
            </div>
        </div>
    );
}