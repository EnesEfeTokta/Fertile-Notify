import React, { useState, useCallback, useEffect } from 'react';
import { subscriberService } from '../api/subscriberService';
import type { SubscriberProfile } from '../types/subscriber';
import AppShell from '../components/AppShell';
import { useToast } from '../components/Toast';

const SectionCard = ({ title, subtitle, children, danger }: { title: string; subtitle?: string; children: React.ReactNode; danger?: boolean }) => (
    <div className={`card p-6 space-y-5 h-full${danger ? ' border border-red-500/30' : ''}`}>
        <div>
            <h2 className={`text-base font-bold ${danger ? 'text-red-400' : 'text-primary'}`}>{title}</h2>
            {subtitle && <p className="text-xs text-tertiary mt-0.5">{subtitle}</p>}
        </div>
        {children}
    </div>
);

const FieldRow = ({ label, children }: { label: string; children: React.ReactNode }) => (
    <div className="space-y-1.5">
        <label className="block text-xs font-semibold uppercase tracking-wider text-tertiary">{label}</label>
        {children}
    </div>
);

export default function AccountPage() {
    const [profile, setProfile] = useState<SubscriberProfile | null>(null);
    const [loading, setLoading] = useState(true);
    const [updating, setUpdating] = useState(false);
    const { showToast, ToastContainer } = useToast();

    // Fields
    const [companyNameField, setCompanyNameField] = useState("");
    const [emailField, setEmailField] = useState("");
    const [phoneField, setPhoneField] = useState("");
    const [currentPassword, setCurrentPassword] = useState("");
    const [newPassword, setNewPassword] = useState("");
    const [showCurrentPw, setShowCurrentPw] = useState(false);
    const [showNewPw, setShowNewPw] = useState(false);

    // Export
    const [exporting, setExporting] = useState(false);

    // Delete account
    const [showDeleteConfirm, setShowDeleteConfirm] = useState(false);
    const [deleteConfirmText, setDeleteConfirmText] = useState("");
    const [deleting, setDeleting] = useState(false);

    const fetchData = useCallback(async () => {
        try {
            setLoading(true);
            const data = await subscriberService.getProfile();
            setProfile(data);
            setCompanyNameField(data.companyName || "");
            setEmailField(data.email || "");
            setPhoneField(data.phoneNumber || "");
        } catch {
            showToast("Failed to load account data.", "error");
        } finally {
            setLoading(false);
        }
    }, [showToast]);

    useEffect(() => { fetchData(); }, [fetchData]);

    const updateCompanyName = async () => {
        if (!companyNameField.trim()) return;
        setUpdating(true);
        try {
            await subscriberService.setCompanyName({ companyName: companyNameField });
            showToast("Company name updated.");
            fetchData();
        } catch { showToast("Error updating name.", "error"); }
        finally { setUpdating(false); }
    };

    const updateContact = async () => {
        setUpdating(true);
        try {
            await subscriberService.setContactInfo({ email: emailField, phoneNumber: phoneField });
            showToast("Contact info updated.");
            fetchData();
        } catch { showToast("Error updating contact.", "error"); }
        finally { setUpdating(false); }
    };

    const updatePassword = async () => {
        setUpdating(true);
        try {
            await subscriberService.setPassword({ currentPassword, newPassword });
            showToast("Password updated. Logging out...");
            setTimeout(() => {
                localStorage.clear();
                window.location.href = "/login";
            }, 2000);
        } catch { showToast("Invalid password.", "error"); }
        finally { setUpdating(false); }
    };

    const handleExport = async () => {
        setExporting(true);
        try {
            await subscriberService.exportData();
            showToast("Your data has been downloaded.");
        } catch {
            showToast("Failed to export data.", "error");
        } finally {
            setExporting(false);
        }
    };

    const handleDeleteAccount = async () => {
        if (deleteConfirmText !== "DELETE") return;
        setDeleting(true);
        try {
            await subscriberService.deleteAccount();
            showToast("Account deleted. Redirecting...");
            setTimeout(() => {
                localStorage.clear();
                window.location.href = "/login";
            }, 1500);
        } catch {
            showToast("Failed to delete account.", "error");
            setDeleting(false);
        }
    };

    return (
        <AppShell title="Account Settings" companyName={profile?.companyName} plan={profile?.subscription?.plan}>
            <ToastContainer />
            {loading ? (
                <div className="flex items-center gap-3 py-16"><div className="spinner" /></div>
            ) : (
                <div className="grid grid-cols-1 lg:grid-cols-2 gap-4 max-w-[1400px]">
                    {/* Top Row: Profile & Security */}
                    <SectionCard title="Profile Information" subtitle="Update your company and personal details">
                        <FieldRow label="Company Name">
                            <div className="flex gap-2">
                                <input className="input-modern" value={companyNameField} onChange={e => setCompanyNameField(e.target.value)} />
                                <button className="btn-primary py-2 px-4" onClick={updateCompanyName}>Save</button>
                            </div>
                        </FieldRow>
                        <FieldRow label="Email Address">
                            <input className="input-modern" type="email" value={emailField} onChange={e => setEmailField(e.target.value)} />
                        </FieldRow>
                        <FieldRow label="Phone Number (optional)">
                            <input className="input-modern" type="tel" value={phoneField} onChange={e => setPhoneField(e.target.value)} />
                        </FieldRow>
                        <button className="btn-secondary w-full" onClick={updateContact} disabled={updating}>Update Contact Info</button>
                    </SectionCard>

                    <SectionCard title="Security" subtitle="Manage your password and account protection">
                        <FieldRow label="Current Password">
                            <div className="relative">
                                <input className="input-modern pr-10" type={showCurrentPw ? "text" : "password"} value={currentPassword} onChange={e => setCurrentPassword(e.target.value)} />
                                <button className="absolute right-3 top-1/2 -translate-y-1/2 text-tertiary" onClick={() => setShowCurrentPw(!showCurrentPw)}>
                                    {showCurrentPw ? "🙈" : "👁️"}
                                </button>
                            </div>
                        </FieldRow>
                        <FieldRow label="New Password">
                            <div className="relative">
                                <input className="input-modern pr-10" type={showNewPw ? "text" : "password"} value={newPassword} onChange={e => setNewPassword(e.target.value)} />
                                <button className="absolute right-3 top-1/2 -translate-y-1/2 text-tertiary" onClick={() => setShowNewPw(!showNewPw)}>
                                    {showNewPw ? "🙈" : "👁️"}
                                </button>
                            </div>
                        </FieldRow>
                        <button className="btn-primary w-full py-3" onClick={updatePassword} disabled={updating || !newPassword}>Update Password</button>
                    </SectionCard>

                    {/* Bottom Row: Data & Danger Zone */}
                    <SectionCard title="Data & Privacy" subtitle="Download a copy of all your account data">
                        <p className="text-xs text-tertiary leading-relaxed">
                            Export all data associated with your account — profile, subscriptions, API keys,
                            and notification history — as a single JSON file.
                        </p>
                        <button
                            className="btn-secondary w-full py-2.5 flex items-center justify-center gap-2 text-xs"
                            onClick={handleExport}
                            disabled={exporting}
                        >
                            {exporting ? (
                                <><div className="spinner" style={{ width: 14, height: 14, borderWidth: 2 }} /> Preparing…</>
                            ) : (
                                <>
                                    <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                                        <path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4" />
                                        <polyline points="7 10 12 15 17 10" />
                                        <line x1="12" y1="15" x2="12" y2="3" />
                                    </svg>
                                    Download My Data (JSON)
                                </>
                            )}
                        </button>
                    </SectionCard>

                    <SectionCard title="⚠ Danger Zone" subtitle="Irreversible and destructive actions" danger>
                        <p className="text-xs text-tertiary leading-relaxed">
                            Deleting your account will permanently remove all of your data from our servers.
                            This action <span className="text-red-400 font-semibold">cannot be undone</span>.
                        </p>

                        {!showDeleteConfirm ? (
                            <button
                                className="w-full py-2.5 rounded-lg font-semibold text-xs border border-red-500/40 text-red-400 bg-red-500/10 hover:bg-red-500/20 transition-colors"
                                onClick={() => setShowDeleteConfirm(true)}
                            >
                                Delete My Account
                            </button>
                        ) : (
                            <div className="space-y-2">
                                <p className="text-[10px] font-semibold text-red-400 uppercase tracking-wider">
                                    Type <span className="font-mono bg-red-500/20 px-1 py-0.5 rounded">DELETE</span> to confirm
                                </p>
                                <div className="flex gap-2">
                                    <input
                                        className="input-modern py-2 border-red-500/30 focus:border-red-400 w-full text-xs"
                                        placeholder="DELETE"
                                        value={deleteConfirmText}
                                        onChange={e => setDeleteConfirmText(e.target.value)}
                                        autoFocus
                                    />
                                    <button
                                        className="py-2 px-4 rounded-lg text-xs font-semibold border border-red-500 text-red-400 bg-red-500/15 hover:bg-red-500/25 transition-all disabled:opacity-30"
                                        onClick={handleDeleteAccount}
                                        disabled={deleteConfirmText !== "DELETE" || deleting}
                                    >
                                        Delete
                                    </button>
                                    <button
                                        className="py-2 px-4 rounded-lg text-xs font-semibold btn-secondary"
                                        onClick={() => { setShowDeleteConfirm(false); setDeleteConfirmText(""); }}
                                    >
                                        Cancel
                                    </button>
                                </div>
                            </div>
                        )}
                    </SectionCard>
                </div>
            )}
        </AppShell>
    );
}
