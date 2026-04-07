import { useState, useEffect } from "react";
import { useSearchParams, useNavigate } from "react-router-dom";
import { publicService } from "../api/publicService";

const CHANNELS = [
    { id: "email",    label: "Email",     icon: "✉️" },
    { id: "sms",      label: "SMS",       icon: "💬" },
    { id: "whatsapp", label: "WhatsApp",  icon: "🟢" },
    { id: "telegram", label: "Telegram",  icon: "✈️" },
    { id: "slack",    label: "Slack",     icon: "🔔" },
    { id: "discord",  label: "Discord",   icon: "👾" },
    { id: "webpush",  label: "Web Push",  icon: "🌍" },
    { id: "msteams",  label: "MS Teams",  icon: "👥" },
];

const COMPLAINT_REASONS = [
    "Spam",
    "Inappropriate Content",
    "Scam / Phishing",
    "Too Frequent",
    "Did not sign up",
    "Other",
];

type Tab = "manage" | "report";
type SuccessType = "manage" | "report" | null;

export default function RecipientsManagerPage() {
    const [searchParams] = useSearchParams();
    const navigate = useNavigate();

    const recipient = searchParams.get("recipient");
    const subId     = searchParams.get("subId");
    const token     = searchParams.get("token");

    const [activeTab, setActiveTab]           = useState<Tab>("manage");
    const [selectedChannels, setSelectedChannels] = useState<string[]>([]);
    const [allChannels, setAllChannels]       = useState(false);
    const [reportReason, setReportReason]     = useState(COMPLAINT_REASONS[0]);
    const [reportDescription, setReportDescription] = useState("");

    const [loading, setLoading]         = useState(false);
    const [success, setSuccess]         = useState<SuccessType>(null);
    const [error, setError]             = useState<string | null>(null);
    const [paramError, setParamError]   = useState(false);

    useEffect(() => {
        if (!recipient || !subId || !token) {
            setParamError(true);
        }
    }, [recipient, subId, token]);

    /* ---- helpers ---- */
    const toggleChannel = (id: string) => {
        if (allChannels) return;
        setSelectedChannels(prev =>
            prev.includes(id) ? prev.filter(c => c !== id) : [...prev, id]
        );
    };

    const handleAllChannels = () => {
        setAllChannels(prev => {
            if (!prev) setSelectedChannels(CHANNELS.map(c => c.id));
            else setSelectedChannels([]);
            return !prev;
        });
    };

    const switchTab = (tab: Tab) => {
        setActiveTab(tab);
        setError(null);
    };

    /* ---- submit manage ---- */
    const handleManage = async () => {
        if (!recipient || !subId || !token) return;
        if (selectedChannels.length === 0) {
            setError("Please select at least one channel to block.");
            return;
        }
        setLoading(true);
        setError(null);
        try {
            await publicService.unsubscribe({
                recipient,
                subscriberId: subId,
                token,
                channels: selectedChannels,
            });
            setSuccess("manage");
        } catch (err: any) {
            setError(err?.message || "Something went wrong. Please try again.");
        } finally {
            setLoading(false);
        }
    };

    /* ---- submit report ---- */
    const handleReport = async () => {
        if (!recipient || !subId || !token) return;
        if (reportReason === "Other" && !reportDescription.trim()) {
            setError("Please provide a description when selecting 'Other'.");
            return;
        }
        setLoading(true);
        setError(null);
        try {
            await publicService.submitComplaint({
                subscriberId:        subId,
                reporterEmail:       recipient,
                reason:              reportReason,
                description:         reportDescription.trim() || "No additional details provided.",
                notificationSubject: "User complaint via management link",
                notificationBody:    "Content unavailable via public link.",
            });
            setSuccess("report");
        } catch (err: any) {
            setError(err?.message || "Something went wrong. Please try again.");
        } finally {
            setLoading(false);
        }
    };

    /* ---- invalid link ---- */
    if (paramError) {
        return (
            <div className="min-h-screen bg-primary flex items-center justify-center p-6">
                <div className="max-w-md w-full text-center animate-fade-in">
                    <div className="w-16 h-16 bg-red-500/10 rounded-full flex items-center justify-center text-3xl mx-auto mb-6">
                        ⚠️
                    </div>
                    <h1 className="text-2xl font-display font-bold text-white mb-3">
                        Invalid Link
                    </h1>
                    <p className="text-secondary mb-8 text-sm leading-relaxed">
                        This management link is missing required parameters. Please use
                        the link provided in your notification.
                    </p>
                    <button onClick={() => navigate("/home")} className="btn-secondary">
                        ← Back to fertile<span className="text-accent-primary font-bold">notify</span>
                    </button>
                </div>
            </div>
        );
    }

    /* ---- success screen ---- */
    if (success) {
        const isManage = success === "manage";
        return (
            <div className="min-h-screen bg-primary flex items-center justify-center p-6">
                <div className="max-w-md w-full text-center animate-fade-in">
                    <div className={`w-20 h-20 rounded-full flex items-center justify-center text-4xl mx-auto mb-6 ${isManage ? "bg-green-500/10 text-green-400" : "bg-blue-500/10 text-blue-400"}`}>
                        {isManage ? "✓" : "📋"}
                    </div>
                    <h1 className="text-3xl font-display font-bold text-white mb-4">
                        {isManage ? "Preferences Saved" : "Report Submitted"}
                    </h1>
                    <p className="text-secondary leading-relaxed mb-8 text-sm">
                        {isManage
                            ? "Your notification preferences have been updated. You will no longer receive the selected notification types from this sender."
                            : "Thank you for your report. Our trust and safety team will review this sender and take appropriate action."}
                    </p>
                    <button onClick={() => navigate("/home")} className="btn-secondary w-full">
                        ← Back to fertile<span className="text-accent-primary font-bold">notify</span>
                    </button>
                </div>
            </div>
        );
    }

    /* ---- main page ---- */
    return (
        <div className="min-h-screen bg-primary relative overflow-hidden flex flex-col items-center justify-center p-6 py-12">
            {/* background */}
            <div className="fixed inset-0 bg-grid-fine pointer-events-none opacity-20" />
            <div className="fixed inset-0 glow-overlay opacity-30" />

            <div className="max-w-xl w-full relative z-10 animate-fade-in-up">

                {/* header */}
                <div className="text-center mb-8">
                    <div className="flex justify-center mb-5">
                        <div className="w-10 h-10 bg-white rounded-md flex items-center justify-center shadow-lg">
                            <span className="text-black font-bold text-sm font-mono">FN</span>
                        </div>
                    </div>
                    <h1 className="text-3xl font-display font-bold text-white mb-2">
                        Notification Manager
                    </h1>
                    <p className="text-secondary text-sm">
                        Managing preferences for{" "}
                        <span className="text-white font-semibold bg-white/5 px-2 py-0.5 rounded border border-white/10">
                            {recipient}
                        </span>
                    </p>
                </div>

                {/* error banner */}
                {error && (
                    <div className="mb-5 p-4 bg-red-500/10 border border-red-500/20 rounded-xl text-red-400 text-sm flex items-start gap-3">
                        <svg className="w-4 h-4 shrink-0 mt-0.5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 9v2m0 4h.01M12 4a8 8 0 110 16A8 8 0 0112 4z" />
                        </svg>
                        {error}
                    </div>
                )}

                {/* card */}
                <div className="card p-0 shadow-2xl border-primary/60 overflow-hidden bg-secondary/20 backdrop-blur-xl">

                    {/* tab bar */}
                    <div className="flex border-b border-white/5">
                        <button
                            onClick={() => switchTab("manage")}
                            className={`flex-1 flex items-center justify-center gap-2 py-4 text-sm font-semibold transition-all duration-200
                                ${activeTab === "manage"
                                    ? "text-white border-b-2 border-accent-primary bg-accent-dim"
                                    : "text-secondary hover:text-white hover:bg-white/5 border-b-2 border-transparent"}`}
                        >
                            <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z" />
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                            </svg>
                            Manage Notifications
                        </button>
                        <button
                            onClick={() => switchTab("report")}
                            className={`flex-1 flex items-center justify-center gap-2 py-4 text-sm font-semibold transition-all duration-200
                                ${activeTab === "report"
                                    ? "text-red-400 border-b-2 border-red-500 bg-red-500/5"
                                    : "text-secondary hover:text-red-400 hover:bg-red-500/5 border-b-2 border-transparent"}`}
                        >
                            <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 21v-4m0 0V5a2 2 0 012-2h6.5l1 1H21l-3 6 3 6h-8.5l-1-1H5a2 2 0 00-2 2zm9-13.5V9" />
                            </svg>
                            Report Abuse
                        </button>
                    </div>

                    {/* tab content */}
                    <div className="p-7">

                        {/* ===================== MANAGE TAB ===================== */}
                        {activeTab === "manage" && (
                            <div className="animate-fade-in">
                                <div className="mb-5">
                                    <h2 className="text-white font-semibold text-base mb-1">
                                        Channel Preferences
                                    </h2>
                                    <p className="text-secondary text-xs leading-relaxed">
                                        Select which channels from this sender you want to block.
                                        Other senders are not affected.
                                    </p>
                                </div>

                                {/* "block all" row */}
                                <div
                                    onClick={handleAllChannels}
                                    className={`flex items-center gap-3 p-4 rounded-xl border cursor-pointer transition-all duration-200 mb-3
                                        ${allChannels
                                            ? "bg-accent-dim border-accent-primary ring-1 ring-accent-primary/30"
                                            : "bg-primary/40 border-primary hover:border-hover"}`}
                                >
                                    <div className={`w-5 h-5 rounded-md border-2 flex items-center justify-center text-[10px] transition-colors shrink-0
                                        ${allChannels ? "bg-accent-primary border-accent-primary text-white" : "border-tertiary text-transparent"}`}>
                                        ✓
                                    </div>
                                    <span className="text-sm font-semibold text-white">Block all channels</span>
                                    {allChannels && <span className="ml-auto text-xs text-accent-primary font-medium">All selected</span>}
                                </div>

                                {/* individual channels */}
                                <div className="grid grid-cols-2 gap-2.5 mb-6">
                                    {CHANNELS.map(ch => {
                                        const isSelected = selectedChannels.includes(ch.id);
                                        return (
                                            <div
                                                key={ch.id}
                                                onClick={() => toggleChannel(ch.id)}
                                                className={`flex items-center gap-3 p-3.5 rounded-xl border transition-all duration-200
                                                    ${allChannels ? "opacity-40 cursor-not-allowed" : "cursor-pointer"}
                                                    ${isSelected
                                                        ? "bg-accent-dim border-accent-primary ring-1 ring-accent-primary/20"
                                                        : "bg-primary/20 border-primary/50 hover:border-hover"}`}
                                            >
                                                <span className="text-base">{ch.icon}</span>
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

                                {/* selected summary */}
                                {(selectedChannels.length > 0) && (
                                    <div className="flex flex-wrap gap-1.5 mb-5">
                                        {(allChannels ? CHANNELS.map(c => c.id) : selectedChannels).map(id => {
                                            const ch = CHANNELS.find(c => c.id === id);
                                            return (
                                                <span key={id} className="badge text-xs">
                                                    {ch?.icon} {ch?.label}
                                                </span>
                                            );
                                        })}
                                    </div>
                                )}

                                <button
                                    onClick={handleManage}
                                    disabled={loading || selectedChannels.length === 0}
                                    className="btn-primary w-full py-3.5 text-sm font-bold flex items-center justify-center gap-2"
                                >
                                    {loading && <span className="spinner w-4 h-4" />}
                                    {loading ? "Saving…" : "Save Preferences"}
                                </button>
                                <p className="text-[11px] text-tertiary text-center mt-3">
                                    Only affects notifications from this specific sender.
                                </p>
                            </div>
                        )}

                        {/* ===================== REPORT TAB ===================== */}
                        {activeTab === "report" && (
                            <div className="animate-fade-in space-y-5">
                                <div className="mb-1">
                                    <h2 className="text-white font-semibold text-base mb-1">
                                        Report this Sender
                                    </h2>
                                    <p className="text-secondary text-xs leading-relaxed">
                                        Select the reason that best describes the issue. Our team
                                        will review your report within 24 hours.
                                    </p>
                                </div>

                                {/* reason select */}
                                <div>
                                    <label className="block text-xs font-semibold text-secondary uppercase tracking-wider mb-2">
                                        Reason
                                    </label>
                                    <div className="grid grid-cols-2 gap-2">
                                        {COMPLAINT_REASONS.map(r => (
                                            <button
                                                key={r}
                                                onClick={() => setReportReason(r)}
                                                className={`px-3 py-2.5 rounded-xl border text-sm font-medium text-left transition-all duration-200
                                                    ${reportReason === r
                                                        ? "bg-red-500/10 border-red-500/40 text-red-400 ring-1 ring-red-500/20"
                                                        : "bg-primary/30 border-primary/50 text-secondary hover:border-hover hover:text-white"}`}
                                            >
                                                {r}
                                            </button>
                                        ))}
                                    </div>
                                </div>

                                {/* description */}
                                <div>
                                    <label className="block text-xs font-semibold text-secondary uppercase tracking-wider mb-2">
                                        Additional Details{reportReason === "Other" && <span className="text-red-400 ml-1">*</span>}
                                    </label>
                                    <textarea
                                        className="input-modern w-full resize-none"
                                        rows={4}
                                        placeholder="Provide any extra context that may help our review…"
                                        value={reportDescription}
                                        onChange={e => setReportDescription(e.target.value)}
                                    />
                                </div>

                                {/* warning notice */}
                                <div className="flex gap-3 p-4 bg-red-500/5 border border-red-500/10 rounded-xl">
                                    <svg className="w-4 h-4 text-red-400 shrink-0 mt-0.5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
                                    </svg>
                                    <p className="text-xs text-secondary leading-relaxed">
                                        False or misleading reports may result in action against your account.
                                        Frequent and valid reports may trigger a review of the sender's account.
                                    </p>
                                </div>

                                <button
                                    onClick={handleReport}
                                    disabled={loading}
                                    className="w-full py-3.5 text-sm font-bold flex items-center justify-center gap-2 rounded-xl bg-red-600 hover:bg-red-500 text-white transition-colors disabled:opacity-60 disabled:cursor-not-allowed"
                                >
                                    {loading && <span className="spinner w-4 h-4 !border-white/30 !border-t-white" />}
                                    {loading ? "Submitting…" : "Submit Report"}
                                </button>
                            </div>
                        )}
                    </div>
                </div>

                {/* footer */}
                <div className="mt-8 text-center space-y-3">
                    <p className="text-xs text-tertiary">
                        Powered by fertile<span className="text-accent-primary font-semibold">notify</span>
                    </p>
                    <button
                        onClick={() => navigate("/home")}
                        className="text-xs text-tertiary hover:text-secondary transition-colors"
                    >
                        ← Back to home
                    </button>
                </div>
            </div>
        </div>
    );
}
