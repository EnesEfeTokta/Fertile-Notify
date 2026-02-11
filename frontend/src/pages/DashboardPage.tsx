import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { subscriberService } from '../api/subscriberService';
import type { SubscriberProfile, ApiKey } from '../types/subscriber';

export default function DashboardPage() {
    const [profile, setProfile] = useState<SubscriberProfile | null>(null);
    const navigate = useNavigate();
    const [error, setError] = useState<string | null>(null);
    const [loading, setLoading] = useState<boolean>(true);
    const [updating, setUpdating] = useState<boolean>(false);
    const [apiKeysList, setApiKeysList] = useState<ApiKey[]>([]);
    const [newApiKey, setNewApiKey] = useState<string | null>(null);
    const [copied, setCopied] = useState<boolean>(false);

    const fetchProfile = React.useCallback(async () => {
        try {
            setLoading(true);
            setError(null);
            const data = await subscriberService.getProfile();
            console.log("Profile data received:", data);
            setProfile(data);
        } catch (err) {
            console.error("Error fetching profile:", err);
            setError("Error fetching profile data. Please try again later.");
        } finally {
            setLoading(false);
        }
    }, []);

    const updateCompanyName = async (newName: string) => {
        try {
            if (!newName.trim()) {
                alert("Company name cannot be empty.");
                return;
            }
            setUpdating(true);
            console.log("Updating company name to:", newName);
            await subscriberService.setCompanyName({ companyName: newName });
            await fetchProfile();
            alert("Company name updated successfully.");
        } catch (err: unknown) {
            console.error("Error updating company name:", err);
            const errorMessage = (err as { response?: { data?: { Error?: { Message?: string } } } })?.response?.data?.Error?.Message || "Company name update failed.";
            alert(errorMessage);
        } finally {
            setUpdating(false);
        }
    }

    const updateContactInfo = async (newEmail: string, newPhone?: string) => {
        try {
            if (!newEmail.trim()) {
                alert("Email address cannot be empty.");
                return;
            }
            setUpdating(true);
            const updates: { email?: string; phoneNumber?: string } = { email: newEmail };
            if (newPhone) {
                updates.phoneNumber = newPhone;
            }
            console.log("Updating contact info:", updates);
            await subscriberService.setContactInfo(updates);
            await fetchProfile();
            alert("Contact information updated successfully.");
        } catch (err: unknown) {
            console.error("Error updating contact information:", err);
            const errorMessage = (err as { response?: { data?: { Error?: { Message?: string } } } })?.response?.data?.Error?.Message || "Contact information update failed.";
            alert(errorMessage);
        } finally {
            setUpdating(false);
        }
    }

    const updateChannel = async (channel: string, enable: boolean) => {
        try {
            if (!channel.trim()) {
                alert("Channel cannot be empty.");
                return;
            }
            setUpdating(true);
            console.log(`Updating channel ${channel} to ${enable ? 'enable' : 'disable'}`);
            await subscriberService.setChannel({ channel, enable });
            await fetchProfile();
            alert("Channel updated successfully.");
        } catch (err: unknown) {
            console.error("Error updating channel:", err);
            const errorMessage = (err as { response?: { data?: { Error?: { Message?: string } } } })?.response?.data?.Error?.Message || "Channel update failed.";
            alert(errorMessage);
        } finally {
            setUpdating(false);
        }
    }

    const updatePassword = async (currentPassword: string, newPassword: string) => {
        try {
            if (!currentPassword || !newPassword) {
                alert("Both current and new passwords are required.");
                return;
            }
            setUpdating(true);
            console.log("Updating password");
            await subscriberService.setPassword({ currentPassword, newPassword });
            alert("Password updated successfully. Please log in again.");
            localStorage.removeItem('token');
            navigate('/login');
        } catch (err: unknown) {
            console.error("Error updating password:", err);
            const errorMessage = (err as { response?: { data?: { Error?: { Message?: string } } } })?.response?.data?.Error?.Message || "Password update failed.";
            alert(errorMessage);
        } finally {
            setUpdating(false);
        }
    }

    const handleLogout = () => {
        localStorage.removeItem('token');
        navigate('/login');
    };

    const createApiKey = async (name: string) => {
        try {
            if (!name.trim()) {
                alert("API key name cannot be empty.");
                return;
            }
            setUpdating(true);
            console.log("Creating API key with name:", name);
            const response = await subscriberService.setApikey({ name });
            setNewApiKey(response.apiKey);
            await loadApiKeys();
        } catch (err: unknown) {
            console.error("Error creating API key:", err);
            const errorMessage = (err as { response?: { data?: { Error?: { Message?: string } } } })?.response?.data?.Error?.Message || "API key creation failed.";
            alert(errorMessage);
        } finally {
            setUpdating(false);
        }
    }

    const apiKeys = React.useCallback(async () => {
        try {
            console.log("Fetching API keys");
            const keys = await subscriberService.getApiKeys();
            console.log("API keys fetched:", keys);
            return keys;
        } catch (err: unknown) {
            console.error("Error fetching API keys:", err);
            return [];
        }
    }, []);

    const deleteApiKey = async (key: string) => {
        try {
            if (!key.trim()) {
                alert("API key cannot be empty.");
                return;
            }
            setUpdating(true);
            console.log("Deleting API key:", key);
            await subscriberService.deleteApiKey(key);
            await fetchProfile();
            alert("API key deleted successfully.");
        } catch (err: unknown) {
            console.error("Error deleting API key:", err);
            const errorMessage = (err as { response?: { data?: { Error?: { Message?: string } } } })?.response?.data?.Error?.Message || "API key deletion failed.";
            alert(errorMessage);
        } finally {
            setUpdating(false);
        }
    }

    const loadApiKeys = React.useCallback(async () => {
        const keys = await apiKeys();
        setApiKeysList(keys);
    }, [apiKeys]);

    React.useEffect(() => {
        fetchProfile();
        loadApiKeys();
    }, [fetchProfile, loadApiKeys]);

    const handleCopyApiKey = () => {
        if (newApiKey) {
            navigator.clipboard.writeText(newApiKey);
            setCopied(true);
            setTimeout(() => setCopied(false), 2000);
        }
    };

    const closeApiKeyModal = () => {
        setNewApiKey(null);
        setCopied(false);
    };

    return (
        <div className="min-h-screen bg-primary">
            {/* API Key Modal */}
            {newApiKey && (
                <div className="fixed inset-0 bg-black/80 backdrop-blur-sm flex items-center justify-center z-50 px-4">
                    <div className="card-elevated max-w-2xl w-full p-8">
                        <div className="text-center mb-6">
                            <div className="text-5xl mb-4">🔑</div>
                            <h2 className="text-2xl font-display font-semibold text-primary mb-2">
                                API Key Created
                            </h2>
                            <div className="h-px w-24 bg-primary-500 mx-auto"></div>
                        </div>

                        {/* Warning Message */}
                        <div className="bg-red-500/10 border border-red-500/20 p-4 mb-6 rounded-md">
                            <p className="text-red-400 font-semibold text-sm mb-2">⚠️ Important</p>
                            <p className="text-red-300 text-sm">
                                This API key is shown only once. Save it in a secure location.
                                You will not be able to view it again.
                            </p>
                        </div>

                        {/* API Key Display */}
                        <div className="bg-secondary border border-primary p-4 rounded-md mb-6">
                            <label className="block text-secondary font-medium mb-2 text-sm">
                                Your API Key:
                            </label>
                            <div className="bg-primary p-3 border border-primary-500/30 rounded-md">
                                <p className="text-primary-400 font-mono text-sm break-all select-all">
                                    {newApiKey}
                                </p>
                            </div>
                        </div>

                        {/* Action Buttons */}
                        <div className="flex gap-3">
                            <button
                                onClick={handleCopyApiKey}
                                className="flex-1 btn-primary flex items-center justify-center gap-2"
                            >
                                {copied ? (
                                    <>
                                        <span>✓</span>
                                        <span>Copied!</span>
                                    </>
                                ) : (
                                    <>
                                        <span>📋</span>
                                        <span>Copy Key</span>
                                    </>
                                )}
                            </button>
                            <button
                                onClick={closeApiKeyModal}
                                className="flex-1 btn-secondary"
                            >
                                Close
                            </button>
                        </div>
                    </div>
                </div>
            )}

            {/* Header */}
            <header className="bg-secondary border-b border-primary">
                <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4 flex justify-between items-center">
                    <div>
                        <h1 className="text-2xl font-display font-semibold text-primary">Dashboard</h1>
                        <p className="text-sm text-secondary mt-1">Welcome back</p>
                    </div>
                    <button
                        onClick={handleLogout}
                        className="px-4 py-2 text-sm bg-red-500/10 text-red-400 border border-red-500/20 rounded-md hover:bg-red-500/20 transition-colors"
                    >
                        Logout
                    </button>
                </div>
            </header>

            <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
                {/* Loading State */}
                {loading && (
                    <div className="flex justify-center items-center py-12">
                        <div className="spinner"></div>
                        <span className="ml-3 text-secondary text-sm">Loading...</span>
                    </div>
                )}

                {/* Error State */}
                {error && (
                    <div className="card p-4 bg-red-500/10 border-red-500/20 mb-6">
                        <p className="font-semibold text-red-400 text-sm">Error:</p>
                        <p className="text-red-300 text-sm">{error}</p>
                    </div>
                )}

                {/* Profile Content */}
                {!loading && !error && profile && (
                    <div className="space-y-6">
                        {/* Profile Information Card */}
                        <div className="card p-6">
                            <h2 className="text-lg font-display font-semibold text-primary mb-6">
                                Profile Information
                            </h2>

                            <div className="space-y-4">
                                {/* Company Name */}
                                <div className="p-4 bg-tertiary rounded-md border-l-2 border-primary-500">
                                    <label className="block text-sm font-medium text-secondary mb-2">Company Name</label>
                                    <div className="flex gap-2">
                                        <input
                                            type="text"
                                            className="input-modern flex-1"
                                            onChange={(e) => setProfile({ ...profile, companyName: e.target.value })}
                                            value={profile.companyName}
                                        />
                                        <button
                                            className="btn-primary px-6 disabled:opacity-50"
                                            onClick={() => updateCompanyName(profile.companyName)}
                                            disabled={updating}
                                        >
                                            {updating ? '...' : 'Update'}
                                        </button>
                                    </div>
                                </div>

                                {/* Email */}
                                <div className="p-4 bg-tertiary rounded-md border-l-2 border-primary-500">
                                    <label className="block text-sm font-medium text-secondary mb-2">Email</label>
                                    <div className="flex gap-2">
                                        <input
                                            type="email"
                                            className="input-modern flex-1"
                                            onChange={(e) => setProfile({ ...profile, email: e.target.value })}
                                            value={profile.email}
                                        />
                                        <button
                                            className="btn-primary px-6 disabled:opacity-50"
                                            onClick={() => updateContactInfo(profile.email, profile.phoneNumber)}
                                            disabled={updating}
                                        >
                                            {updating ? '...' : 'Update'}
                                        </button>
                                    </div>
                                </div>

                                {/* Phone */}
                                <div className="p-4 bg-tertiary rounded-md border-l-2 border-primary-500">
                                    <label className="block text-sm font-medium text-secondary mb-2">Phone</label>
                                    <div className="flex gap-2">
                                        <input
                                            type="tel"
                                            className="input-modern flex-1"
                                            onChange={(e) => setProfile({ ...profile, phoneNumber: e.target.value })}
                                            value={profile.phoneNumber}
                                        />
                                        <button
                                            className="btn-primary px-6 disabled:opacity-50"
                                            onClick={() => updateContactInfo(profile.email, profile.phoneNumber)}
                                            disabled={updating}
                                        >
                                            {updating ? '...' : 'Update'}
                                        </button>
                                    </div>
                                </div>
                            </div>
                        </div>

                        {/* Notification Channels Card */}
                        <div className="card p-6">
                            <h2 className="text-lg font-display font-semibold text-primary mb-6">
                                Notification Channels
                            </h2>

                            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                                {['email', 'sms', 'console'].map((channel) => {
                                    const isActive = profile.activeChannels.includes(channel);
                                    return (
                                        <button
                                            key={channel}
                                            className={`p-4 rounded-md font-medium transition-colors text-sm border ${isActive
                                                ? 'bg-green-500/10 text-green-400 border-green-500/20'
                                                : 'bg-tertiary text-tertiary border-secondary hover:border-hover'
                                                } disabled:opacity-50`}
                                            onClick={() => updateChannel(channel, !isActive)}
                                            disabled={updating}
                                        >
                                            <div className="text-2xl mb-2">
                                                {channel === 'email' && '📧'}
                                                {channel === 'sms' && '💬'}
                                                {channel === 'console' && '🖥️'}
                                            </div>
                                            <div className="capitalize">{channel}</div>
                                            <div className="text-xs mt-1">
                                                {isActive ? 'Active' : 'Inactive'}
                                            </div>
                                        </button>
                                    );
                                })}
                            </div>
                        </div>

                        {/* Subscription Information Card */}
                        <div className="card p-6">
                            <h2 className="text-lg font-display font-semibold text-primary mb-6">
                                Subscription Information
                            </h2>

                            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
                                {/* Plan */}
                                <div className="p-4 bg-tertiary rounded-md border border-primary">
                                    <p className="text-xs text-secondary font-medium mb-1">Plan</p>
                                    <p className="text-xl font-semibold text-primary">{profile.subscription?.plan || 'N/A'}</p>
                                </div>

                                {/* Monthly Limit */}
                                <div className="p-4 bg-tertiary rounded-md border border-primary">
                                    <p className="text-xs text-secondary font-medium mb-1">Limit</p>
                                    <p className="text-xl font-semibold text-primary">{profile.subscription?.monthlyLimit ?? 'N/A'}</p>
                                </div>

                                {/* Used This Month */}
                                <div className="p-4 bg-tertiary rounded-md border border-primary">
                                    <p className="text-xs text-secondary font-medium mb-1">Usage</p>
                                    <p className="text-xl font-semibold text-primary">{profile.subscription?.usedThisMonth ?? 'N/A'}</p>
                                </div>

                                {/* Expiry Date */}
                                <div className="p-4 bg-tertiary rounded-md border border-primary">
                                    <p className="text-xs text-secondary font-medium mb-1">Expires</p>
                                    <p className="text-base font-semibold text-primary">
                                        {profile.subscription?.expiresAt
                                            ? new Date(profile.subscription.expiresAt).toLocaleDateString('en-US')
                                            : 'N/A'}
                                    </p>
                                </div>
                            </div>
                        </div>

                        {/* API Keys Card */}
                        <div className="card p-6">
                            <h2 className="text-lg font-display font-semibold text-primary mb-6">
                                API Keys
                            </h2>
                            <div className="max-w-md space-y-4">
                                <div>
                                    <label className="block text-sm font-medium text-secondary mb-2">API Key Name</label>
                                    <input
                                        type="text"
                                        placeholder="Key Name"
                                        id="apiKeyName"
                                        className="input-modern"
                                    />
                                </div>
                                <button
                                    className="btn-primary disabled:opacity-50"
                                    onClick={async () => {
                                        const apiKeyNameInput = document.getElementById('apiKeyName') as HTMLInputElement;
                                        if (apiKeyNameInput.value.trim()) {
                                            await createApiKey(apiKeyNameInput.value);
                                            apiKeyNameInput.value = '';
                                        }
                                    }}
                                    disabled={updating}
                                >
                                    {updating ? 'Creating...' : 'Create API Key'}
                                </button>
                            </div>
                            <div className="mt-6 space-y-2">
                                {apiKeysList.map((apiKey) => (
                                    <div key={apiKey.id} className="flex items-center justify-between p-3 bg-tertiary rounded-md border border-primary">
                                        <div className="flex-1">
                                            <p className="text-sm text-primary font-medium mb-1">{apiKey.name}</p>
                                            <p className="text-xs text-secondary font-mono">{apiKey.prefix}...</p>
                                            <p className="text-xs text-tertiary mt-1">Created: {new Date(apiKey.createdAt).toLocaleDateString('en-US')}</p>
                                        </div>
                                        <button
                                            onClick={async () => {
                                                if (confirm(`Delete "${apiKey.name}" key?`)) {
                                                    await deleteApiKey(apiKey.id);
                                                    await loadApiKeys();
                                                }
                                            }}
                                            className="px-3 py-1.5 bg-red-500/10 text-red-400 text-xs border border-red-500/20 rounded-md hover:bg-red-500/20 transition-colors disabled:opacity-50"
                                            disabled={updating || !apiKey.isActive}
                                        >
                                            {apiKey.isActive ? 'Delete' : 'Inactive'}
                                        </button>
                                    </div>
                                ))}
                            </div>
                        </div>

                        {/* Password Update Card */}
                        <div className="card p-6">
                            <h2 className="text-lg font-display font-semibold text-primary mb-6">
                                Password Update
                            </h2>

                            <div className="max-w-md space-y-4">
                                <div>
                                    <label className="block text-sm font-medium text-secondary mb-2">Current Password</label>
                                    <input
                                        type="password"
                                        placeholder="••••••••"
                                        id="currentPassword"
                                        className="input-modern"
                                    />
                                </div>
                                <div>
                                    <label className="block text-sm font-medium text-secondary mb-2">New Password</label>
                                    <input
                                        type="password"
                                        placeholder="••••••••"
                                        id="newPassword"
                                        className="input-modern"
                                    />
                                </div>
                                <button
                                    className="btn-primary disabled:opacity-50"
                                    onClick={() => {
                                        const currentPasswordInput = document.getElementById('currentPassword') as HTMLInputElement;
                                        const newPasswordInput = document.getElementById('newPassword') as HTMLInputElement;
                                        updatePassword(currentPasswordInput.value, newPasswordInput.value);
                                    }}
                                    disabled={updating}
                                >
                                    {updating ? 'Updating...' : 'Update Password'}
                                </button>
                            </div>
                        </div>
                    </div>
                )}

                {/* No Profile State */}
                {!loading && !error && !profile && (
                    <div className="card p-12 text-center">
                        <p className="text-base text-secondary">Profile not found.</p>
                    </div>
                )}
            </main>
        </div>
    );
}