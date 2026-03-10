import React, { useState, useCallback, useEffect } from 'react';
import { subscriberService } from '../api/subscriberService';
import type { SubscriberProfile } from '../types/subscriber';
import AppShell from '../components/AppShell';
import { useToast } from '../components/Toast';

const SectionCard = ({ title, subtitle, children }: { title: string; subtitle?: string; children: React.ReactNode }) => (
    <div className="card p-6 space-y-5 h-full">
        <div>
            <h2 className="text-base font-bold text-primary">{title}</h2>
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

    return (
        <AppShell title="Account Settings" companyName={profile?.companyName} plan={profile?.subscription?.plan}>
            <ToastContainer />
            {loading ? (
                <div className="flex items-center gap-3 py-16"><div className="spinner" /></div>
            ) : (
                <div className="grid grid-cols-1 xl:grid-cols-2 gap-6 max-w-[1400px]">
                    <div className="space-y-6">
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
                    </div>

                    <div className="space-y-6">
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
                    </div>
                </div>
            )}
        </AppShell>
    );
}
