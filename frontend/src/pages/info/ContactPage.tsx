import { useState } from "react";
import { InfoPageLayout } from "./InfoPageLayout";

export default function ContactPage() {
    const [submitted, setSubmitted] = useState(false);
    const [form, setForm] = useState({ name: "", email: "", subject: "", message: "" });
    const [loading, setLoading] = useState(false);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setLoading(true);
        await new Promise(r => setTimeout(r, 1000));
        setLoading(false);
        setSubmitted(true);
    };

    return (
        <InfoPageLayout
            badge="Contact"
            title="Get in touch"
            subtitle="Have a question, bug report, or partnership inquiry? We'd love to hear from you."
        >
            {!submitted ? (
                <div className="grid grid-cols-1 md:grid-cols-5 gap-10">
                    {/* Form */}
                    <form onSubmit={handleSubmit} className="md:col-span-3 space-y-4">
                        <div className="grid grid-cols-2 gap-4">
                            <div>
                                <label className="block text-xs font-semibold uppercase tracking-wider text-tertiary mb-2">Name</label>
                                <input
                                    type="text"
                                    className="input-modern"
                                    value={form.name}
                                    onChange={e => setForm({ ...form, name: e.target.value })}
                                    placeholder="Enes Efe"
                                    required
                                />
                            </div>
                            <div>
                                <label className="block text-xs font-semibold uppercase tracking-wider text-tertiary mb-2">Email</label>
                                <input
                                    type="email"
                                    className="input-modern"
                                    value={form.email}
                                    onChange={e => setForm({ ...form, email: e.target.value })}
                                    placeholder="you@company.com"
                                    required
                                />
                            </div>
                        </div>
                        <div>
                            <label className="block text-xs font-semibold uppercase tracking-wider text-tertiary mb-2">Subject</label>
                            <input
                                type="text"
                                className="input-modern"
                                value={form.subject}
                                onChange={e => setForm({ ...form, subject: e.target.value })}
                                placeholder="What's this about?"
                                required
                            />
                        </div>
                        <div>
                            <label className="block text-xs font-semibold uppercase tracking-wider text-tertiary mb-2">Message</label>
                            <textarea
                                className="input-modern resize-none"
                                rows={5}
                                value={form.message}
                                onChange={e => setForm({ ...form, message: e.target.value })}
                                placeholder="Tell us everything…"
                                required
                            />
                        </div>
                        <button type="submit" className="btn-primary py-2.5 px-7" disabled={loading}>
                            {loading ? <span className="spinner w-4 h-4" /> : "Send message →"}
                        </button>
                    </form>

                    {/* Contact info */}
                    <div className="md:col-span-2 space-y-5">
                        {[
                            { icon: "✉️", label: "Email", value: "support@fertile-notify.shop" },
                            { icon: "⚡", label: "Response time", value: "Within 24 hours (business days)" },
                            { icon: "🐛", label: "Bug reports", value: "Open an issue on GitHub" },
                        ].map(c => (
                            <div key={c.label} className="card p-4 flex gap-4 items-start">
                                <span className="text-xl shrink-0 mt-0.5">{c.icon}</span>
                                <div>
                                    <p className="text-xs font-bold uppercase tracking-wider text-tertiary mb-1">{c.label}</p>
                                    <p className="text-sm text-secondary">{c.value}</p>
                                </div>
                            </div>
                        ))}
                    </div>
                </div>
            ) : (
                <div className="text-center py-16 space-y-4">
                    <div className="w-14 h-14 bg-green-500/10 border border-green-500/20 rounded-2xl flex items-center justify-center mx-auto text-3xl">
                        ✓
                    </div>
                    <h2 className="font-bold text-xl">Message sent!</h2>
                    <p className="text-secondary">We'll get back to you within 24 hours. Thanks, {form.name.split(" ")[0]}!</p>
                </div>
            )}
        </InfoPageLayout>
    );
}
