import { useState, useEffect } from "react";
import { useSearchParams, useNavigate } from "react-router-dom";
import { publicService } from "../api/publicService";

const CHANNELS = [
    { id: "email", label: "Email", icon: "✉️" },
    { id: "sms", label: "SMS", icon: "💬" },
    { id: "whatsapp", label: "WhatsApp", icon: "🟢" },
    { id: "telegram", label: "Telegram", icon: "✈️" },
    { id: "slack", label: "Slack", icon: "🔔" },
    { id: "discord", label: "Discord", icon: "👾" },
    { id: "webpush", label: "Web Push", icon: "🌍" },
    { id: "msteams", label: "MS Teams", icon: "👥" },
];

export default function UnsubscribePage() {
    const [searchParams] = useSearchParams();
    const navigate = useNavigate();

    const recipient = searchParams.get("recipient");
    const subId = searchParams.get("subId");
    const token = searchParams.get("token");

    const [selectedChannels, setSelectedChannels] = useState<string[]>([]);
    const [allChannels, setAllChannels] = useState(false);
    const [loading, setLoading] = useState(false);
    const [success, setSuccess] = useState(false);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        if (!recipient || !subId || !token) {
            setError("Invalid unsubscribe link. Please check your notification.");
        }
    }, [recipient, subId, token]);

    const toggleChannel = (id: string) => {
        if (selectedChannels.includes(id)) {
            setSelectedChannels(selectedChannels.filter(c => c !== id));
            setAllChannels(false);
        } else {
            setSelectedChannels([...selectedChannels, id]);
        }
    };

    const handleAllChannels = () => {
        if (!allChannels) {
            setSelectedChannels(CHANNELS.map(c => c.id));
            setAllChannels(true);
        } else {
            setSelectedChannels([]);
            setAllChannels(false);
        }
    };

    const handleUnsubscribe = async () => {
        if (!recipient || !subId || !token) return;
        if (selectedChannels.length === 0) {
            setError("Please select at least one channel to unsubscribe.");
            return;
        }

        setLoading(true);
        setError(null);
        try {
            await publicService.unsubscribe({
                recipient: recipient,
                subscriberId: subId,
                token: token,
                channels: selectedChannels
            });
            setSuccess(true);
        } catch (err: any) {
            setError(err.message || "An error occurred while unsubscribing.");
        } finally {
            setLoading(false);
        }
    };

    if (success) {
        return (
            <div className="min-h-screen bg-primary flex items-center justify-center p-6 text-center">
                <div className="max-w-md w-full animate-fade-in">
                    <div className="w-20 h-20 bg-green-500/10 rounded-full flex items-center justify-center text-4xl mx-auto mb-6 text-green-500">
                        ✓
                    </div>
                    <h1 className="text-3xl font-display font-bold text-white mb-4">Unsubscribed successfully</h1>
                    <p className="text-secondary leading-relaxed mb-8">
                        Your preferences have been updated. You will no longer receive selected notifications from this sender.
                    </p>
                    <button onClick={() => navigate("/home")} className="btn-secondary w-full">
                        Back to Home
                    </button>
                </div>
            </div>
        );
    }

    return (
        <div className="min-h-screen bg-primary relative overflow-hidden flex flex-col items-center justify-center p-6">
            <div className="fixed inset-0 bg-grid-fine pointer-events-none opacity-20" />
            <div className="fixed inset-0 glow-overlay opacity-30" />

            <div className="max-w-xl w-full relative z-10 animate-fade-in-up">
                <div className="text-center mb-10">
                    <div className="flex justify-center mb-6">
                        <div className="w-10 h-10 bg-white rounded-md flex items-center justify-center shadow-lg">
                            <span className="text-black font-bold text-sm font-mono">FN</span>
                        </div>
                    </div>
                    <h1 className="text-3xl font-display font-bold text-white mb-3">Notification Preferences</h1>
                    <p className="text-secondary text-base">
                        Update your notification settings for <span className="text-primary font-medium">{recipient}</span>
                    </p>
                </div>

                {error && (
                    <div className="mb-6 p-4 bg-red-500/10 border border-red-500/20 rounded-xl text-red-500 text-sm animate-shake">
                        {error}
                    </div>
                )}

                <div className="card p-8 shadow-2xl border-primary/50 bg-secondary/30 backdrop-blur-xl">
                    <h3 className="text-sm font-bold text-white uppercase tracking-widest mb-6 opacity-70">
                        Select channels to block:
                    </h3>

                    <div className="grid grid-cols-2 gap-3 mb-8">
                        <div 
                            onClick={handleAllChannels}
                            className={`
                                flex items-center gap-3 p-4 rounded-xl border cursor-pointer transition-all duration-200 col-span-2
                                ${allChannels ? "bg-accent-dim border-accent-primary ring-1 ring-accent-primary/50" : "bg-primary/40 border-primary hover:border-hover"}
                            `}
                        >
                            <div className={`w-5 h-5 rounded-md border flex items-center justify-center text-[10px] transition-colors ${allChannels ? "bg-accent-primary border-accent-primary text-white" : "border-tertiary text-transparent"}`}>
                                ✓
                            </div>
                            <span className="text-sm font-semibold text-white">Block all channels</span>
                        </div>

                        {CHANNELS.map(ch => {
                            const isSelected = selectedChannels.includes(ch.id);
                            return (
                                <div 
                                    key={ch.id} 
                                    onClick={() => !allChannels && toggleChannel(ch.id)}
                                    className={`
                                        flex items-center gap-3 p-4 rounded-xl border transition-all duration-200
                                        ${allChannels ? "opacity-40 cursor-not-allowed" : "cursor-pointer"}
                                        ${isSelected ? "bg-accent-dim border-accent-primary ring-1 ring-accent-primary/30" : "bg-primary/20 border-primary/50 hover:border-hover"}
                                    `}
                                >
                                    <span className="text-lg">{ch.icon}</span>
                                    <span className={`text-sm font-medium transition-colors ${isSelected ? "text-white" : "text-secondary"}`}>
                                        {ch.label}
                                    </span>
                                    {isSelected && !allChannels && (
                                        <span className="ml-auto text-accent-primary text-xs">✓</span>
                                    )}
                                </div>
                            );
                        })}
                    </div>

                    <div className="space-y-3">
                        <button
                            onClick={handleUnsubscribe}
                            disabled={loading || !!error}
                            className={`btn-primary w-full py-4 text-base font-bold flex items-center justify-center gap-3 ${loading ? "opacity-70" : ""}`}
                        >
                            {loading ? <span className="spinner w-5 h-5" /> : null}
                            {loading ? "Updating..." : "Save Preferences"}
                        </button>
                        <p className="text-[11px] text-tertiary text-center">
                            Note: This will only affect notifications from this specific sender.
                        </p>
                    </div>
                </div>

                <div className="mt-10 text-center">
                    <button onClick={() => navigate("/home")} className="text-sm text-tertiary hover:text-secondary transition-colors">
                        ← Back to fertile<span className="text-accent-primary font-bold">notify</span>
                    </button>
                </div>
            </div>
        </div>
    );
}
