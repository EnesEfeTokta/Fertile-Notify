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

const FieldRow = ({ label, children, optional }: { label: string; children: React.ReactNode; optional?: boolean }) => (
    <div className="space-y-1.5">
        <label className="block text-xs font-semibold uppercase tracking-wider text-tertiary">
            {label}
            {optional && <span className="text-muted normal-case font-normal tracking-normal ml-1">(optional)</span>}
        </label>
        {children}
    </div>
);

export default function AccountPage() {
    const [profile, setProfile] = useState<SubscriberProfile | null>(null);
    const [loading, setLoading] = useState(true);
    const [updating, setUpdating] = useState(false);
    const { showToast, ToastContainer } = useToast();

    /* ── Contact & Company fields ── */
    const [companyNameField, setCompanyNameField]             = useState('');
    const [emailField, setEmailField]                         = useState('');
    const [phoneField, setPhoneField]                         = useState('');

    /* ── Read-only info fields (no update endpoint yet) ── */
    const [logoUrlField, setLogoUrlField]                     = useState('');
    const [websiteUrlField, setWebsiteUrlField]               = useState('');
    const [locationField, setLocationField]                   = useState('');
    const [descriptionField, setDescriptionField]             = useState('');

    /* ── Security ── */
    const [currentPassword, setCurrentPassword] = useState('');
    const [newPassword, setNewPassword]         = useState('');
    const [showCurrentPw, setShowCurrentPw]     = useState(false);
    const [showNewPw, setShowNewPw]             = useState(false);

    /* ── Export ── */
    const [exporting, setExporting] = useState(false);

    /* ── Delete account ── */
    const [showDeleteConfirm, setShowDeleteConfirm] = useState(false);
    const [deleteConfirmText, setDeleteConfirmText] = useState('');
    const [deleting, setDeleting]                   = useState(false);

    const fetchData = useCallback(async () => {
        try {
            setLoading(true);
            const data = await subscriberService.getProfile();
            setProfile(data);
            setCompanyNameField(data.companyName || '');
            setEmailField(data.email || '');
            setPhoneField(data.phoneNumber || '');
            setLogoUrlField(data.logoUrl || '');
            setWebsiteUrlField(data.websiteUrl || '');
            setLocationField(data.location || '');
            setDescriptionField(data.companyDescription || '');
        } catch {
            showToast('Failed to load account data.', 'error');
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
            showToast('Company name updated.');
            fetchData();
        } catch { showToast('Error updating name.', 'error'); }
        finally { setUpdating(false); }
    };

    const updateContact = async () => {
        setUpdating(true);
        try {
            await subscriberService.setContactInfo({ email: emailField, phoneNumber: phoneField });
            showToast('Contact info updated.');
            fetchData();
        } catch { showToast('Error updating contact.', 'error'); }
        finally { setUpdating(false); }
    };

    const updatePassword = async () => {
        setUpdating(true);
        try {
            await subscriberService.setPassword({ currentPassword, newPassword });
            showToast('Password updated. Logging out...');
            setTimeout(() => {
                localStorage.clear();
                window.location.href = '/login';
            }, 2000);
        } catch { showToast('Invalid password.', 'error'); }
        finally { setUpdating(false); }
    };

    const handleExport = async () => {
        setExporting(true);
        try {
            await subscriberService.exportData();
            showToast('Your data has been downloaded.');
        } catch {
            showToast('Failed to export data.', 'error');
        } finally {
            setExporting(false);
        }
    };

    const handleDeleteAccount = async () => {
        if (deleteConfirmText !== 'DELETE') return;
        setDeleting(true);
        try {
            await subscriberService.deleteAccount();
            showToast('Account deleted. Redirecting...');
            setTimeout(() => {
                localStorage.clear();
                window.location.href = '/login';
            }, 1500);
        } catch {
            showToast('Failed to delete account.', 'error');
            setDeleting(false);
        }
    };

    return (
        <AppShell
            title="Account Settings"
            companyName={profile?.companyName}
            plan={profile?.subscription?.plan}
            logoUrl={profile?.logoUrl}
        >
            <ToastContainer />
            {loading ? (
                <div className="flex items-center gap-3 py-16"><div className="spinner" /></div>
            ) : (
                <div className="space-y-4 max-w-[1400px]">

                    {/* ── Row 1: Company Identity ── */}
                    <div className="grid grid-cols-1 lg:grid-cols-2 gap-4">
                        <SectionCard title="Company Identity" subtitle="Your company name displayed across the platform">
                            <FieldRow label="Company Name">
                                <div className="flex gap-2">
                                    <input
                                        className="input-modern"
                                        value={companyNameField}
                                        onChange={e => setCompanyNameField(e.target.value)}
                                    />
                                    <button
                                        className="btn-primary py-2 px-4 shrink-0"
                                        onClick={updateCompanyName}
                                        disabled={updating}
                                    >
                                        Save
                                    </button>
                                </div>
                            </FieldRow>

                            <FieldRow label="Company Description" optional>
                                <textarea
                                    className="input-modern resize-none"
                                    rows={3}
                                    value={descriptionField}
                                    onChange={e => setDescriptionField(e.target.value)}
                                    placeholder="A short description of your company…"
                                    disabled
                                />
                                <p className="text-[10px] text-muted mt-1">
                                    * Description can be set at registration. Contact support to update.
                                </p>
                            </FieldRow>
                        </SectionCard>

                        {/* ── Logo & Web Presence ── */}
                        <SectionCard title="Web Presence" subtitle="Logo and website information">
                            {/* Logo preview */}
                            <div className="flex items-center gap-4">
                                <div className="shrink-0">
                                    {logoUrlField ? (
                                        <img
                                            src={logoUrlField}
                                            alt="Company logo"
                                            className="w-16 h-16 rounded-xl object-cover border border-primary"
                                            onError={e => { (e.currentTarget as HTMLImageElement).style.display = 'none'; }}
                                        />
                                    ) : (
                                        <div className="w-16 h-16 rounded-xl bg-accent-dim border border-blue-500/20 flex items-center justify-center">
                                            <span className="text-lg font-bold text-accent-primary">
                                                {(profile?.companyName ?? 'FN').slice(0, 2).toUpperCase()}
                                            </span>
                                        </div>
                                    )}
                                </div>
                                <div className="flex-1 min-w-0">
                                    <p className="text-sm font-semibold text-primary truncate">{profile?.companyName}</p>
                                    {profile?.websiteUrl && (
                                        <a
                                            href={profile.websiteUrl}
                                            target="_blank"
                                            rel="noopener noreferrer"
                                            className="text-xs text-accent-primary hover:underline flex items-center gap-1 mt-1"
                                        >
                                            <svg className="w-3 h-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M10 6H6a2 2 0 00-2 2v10a2 2 0 002 2h10a2 2 0 002-2v-4M14 4h6m0 0v6m0-6L10 14" />
                                            </svg>
                                            {profile.websiteUrl}
                                        </a>
                                    )}
                                    {profile?.location && (
                                        <p className="text-xs text-tertiary flex items-center gap-1 mt-1">
                                            <svg className="w-3 h-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
                                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
                                            </svg>
                                            {profile.location}
                                        </p>
                                    )}
                                </div>
                            </div>

                            <FieldRow label="Logo URL" optional>
                                <input
                                    className="input-modern"
                                    type="url"
                                    value={logoUrlField}
                                    onChange={e => setLogoUrlField(e.target.value)}
                                    placeholder="https://…/logo.png"
                                    disabled
                                />
                            </FieldRow>

                            <FieldRow label="Website URL" optional>
                                <input
                                    className="input-modern"
                                    type="url"
                                    value={websiteUrlField}
                                    onChange={e => setWebsiteUrlField(e.target.value)}
                                    placeholder="https://yourcompany.com"
                                    disabled
                                />
                            </FieldRow>

                            <FieldRow label="Location" optional>
                                <input
                                    className="input-modern"
                                    type="text"
                                    value={locationField}
                                    onChange={e => setLocationField(e.target.value)}
                                    placeholder="San Francisco, CA"
                                    disabled
                                />
                            </FieldRow>

                            <div className="rounded-lg border border-blue-500/15 bg-blue-500/5 px-3 py-2 text-[11px] text-blue-400 flex items-start gap-2">
                                <svg className="w-3.5 h-3.5 mt-0.5 shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                                </svg>
                                Logo, website and location can be set during registration. Contact support to update these fields.
                            </div>
                        </SectionCard>
                    </div>

                    {/* ── Row 2: Contact & Security ── */}
                    <div className="grid grid-cols-1 lg:grid-cols-2 gap-4">
                        <SectionCard title="Contact Information" subtitle="Update your email and phone number">
                            <FieldRow label="Email Address">
                                <input
                                    className="input-modern"
                                    type="email"
                                    value={emailField}
                                    onChange={e => setEmailField(e.target.value)}
                                />
                            </FieldRow>
                            <FieldRow label="Phone Number" optional>
                                <input
                                    className="input-modern"
                                    type="tel"
                                    value={phoneField}
                                    onChange={e => setPhoneField(e.target.value)}
                                />
                            </FieldRow>
                            <button
                                className="btn-secondary w-full"
                                onClick={updateContact}
                                disabled={updating}
                            >
                                Update Contact Info
                            </button>
                        </SectionCard>

                        <SectionCard title="Security" subtitle="Manage your password and account protection">
                            <FieldRow label="Current Password">
                                <div className="relative">
                                    <input
                                        className="input-modern pr-10"
                                        type={showCurrentPw ? 'text' : 'password'}
                                        value={currentPassword}
                                        onChange={e => setCurrentPassword(e.target.value)}
                                    />
                                    <button
                                        className="absolute right-3 top-1/2 -translate-y-1/2 text-tertiary hover:text-primary transition-colors"
                                        onClick={() => setShowCurrentPw(!showCurrentPw)}
                                    >
                                        {showCurrentPw ? '🙈' : '👁️'}
                                    </button>
                                </div>
                            </FieldRow>
                            <FieldRow label="New Password">
                                <div className="relative">
                                    <input
                                        className="input-modern pr-10"
                                        type={showNewPw ? 'text' : 'password'}
                                        value={newPassword}
                                        onChange={e => setNewPassword(e.target.value)}
                                    />
                                    <button
                                        className="absolute right-3 top-1/2 -translate-y-1/2 text-tertiary hover:text-primary transition-colors"
                                        onClick={() => setShowNewPw(!showNewPw)}
                                    >
                                        {showNewPw ? '🙈' : '👁️'}
                                    </button>
                                </div>
                            </FieldRow>
                            <button
                                className="btn-primary w-full py-3"
                                onClick={updatePassword}
                                disabled={updating || !newPassword}
                            >
                                Update Password
                            </button>
                        </SectionCard>
                    </div>

                    {/* ── Row 3: Data & Danger Zone ── */}
                    <div className="grid grid-cols-1 lg:grid-cols-2 gap-4">
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
                                            disabled={deleteConfirmText !== 'DELETE' || deleting}
                                        >
                                            Delete
                                        </button>
                                        <button
                                            className="py-2 px-4 rounded-lg text-xs font-semibold btn-secondary"
                                            onClick={() => { setShowDeleteConfirm(false); setDeleteConfirmText(''); }}
                                        >
                                            Cancel
                                        </button>
                                    </div>
                                </div>
                            )}
                        </SectionCard>
                    </div>
                </div>
            )}
        </AppShell>
    );
}
