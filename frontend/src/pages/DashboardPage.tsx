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
        <div className="min-h-screen bg-gradient-to-br from-gray-900 via-purple-900 to-gray-900">
            {/* API Key Modal */}
            {newApiKey && (
                <div className="fixed inset-0 bg-black/80 backdrop-blur-sm flex items-center justify-center z-50 px-4 animate-fade-in">
                    <div className="bg-gradient-to-br from-gray-900 to-purple-900 border-4 border-purple-500 clip-sharp max-w-2xl w-full p-8 animate-slide-up shadow-neon">
                        <div className="text-center mb-6">
                            <div className="text-6xl mb-4">🔑</div>
                            <h2 className="text-3xl font-display font-bold gradient-text uppercase tracking-wider mb-2">
                                Your API Key Created!
                            </h2>
                            <div className="h-1 w-32 bg-gradient-to-r from-purple-500 to-pink-500 mx-auto"></div>
                        </div>

                        {/* Warning Message */}
                        <div className="bg-red-900/40 border-l-4 border-red-500 p-4 mb-6 clip-sharp-sm">
                            <p className="text-red-300 font-bold uppercase text-sm mb-2">⚠️ IMPORTANT WARNING</p>
                            <p className="text-red-200 text-sm">
                                This API key is shown ONLY ONCE. 
                                Please save the key in a secure location. You will not be able to view 
                                the key again after closing this window!
                            </p>
                        </div>

                        {/* API Key Display */}
                        <div className="bg-gray-900/60 border-2 border-purple-500/50 p-4 clip-sharp-sm mb-6">
                            <label className="block text-purple-300 font-semibold mb-2 uppercase text-sm tracking-wide">
                                Your API Key:
                            </label>
                            <div className="bg-black/40 p-4 border border-cyan-500/30 clip-sharp-sm">
                                <p className="text-cyan-300 font-mono text-sm break-all select-all">
                                    {newApiKey}
                                </p>
                            </div>
                        </div>

                        {/* Action Buttons */}
                        <div className="flex gap-4">
                            <button
                                onClick={handleCopyApiKey}
                                className="flex-1 btn-gradient uppercase tracking-wider flex items-center justify-center gap-2"
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
                                className="flex-1 px-8 py-3 font-semibold text-white bg-gray-700 hover:bg-gray-600 transition-all duration-300 clip-sharp-sm border-2 border-gray-600 uppercase tracking-wider"
                            >
                                Close
                            </button>
                        </div>

                        {/* Additional Info */}
                        <p className="text-center text-gray-400 text-xs mt-4 uppercase tracking-wide">
                            Store the key in a secure password manager
                        </p>
                    </div>
                </div>
            )}

            {/* Header */}
            <header className="bg-gray-900/80 backdrop-blur-md border-b-2 border-purple-500/50">
                <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4 flex justify-between items-center">
                    <div>
                        <h1 className="text-3xl font-display font-bold gradient-text uppercase tracking-wider">Dashboard</h1>
                        <p className="text-gray-400 mt-1 uppercase text-sm tracking-wide">Welcome</p>
                    </div>
                    <button
                        onClick={handleLogout}
                        className="px-6 py-2 bg-red-600 text-white font-semibold hover:bg-red-700 transition-all duration-300 clip-sharp-sm border-2 border-red-500 uppercase text-sm tracking-wide"
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
                        <span className="ml-3 text-purple-400 font-semibold uppercase tracking-wide">Loading...</span>
                    </div>
                )}

                {/* Error State */}
                {error && (
                    <div className="card p-6 bg-red-900/30 border-l-4 border-red-500 mb-6 animate-slide-up">
                        <p className="font-bold text-red-300 uppercase">Error:</p>
                        <p className="text-red-400">{error}</p>
                    </div>
                )}

                {/* Profile Content */}
                {!loading && !error && profile && (
                    <div className="space-y-6 animate-fade-in">
                        {/* Profile Information Card */}
                        <div className="card p-6">
                            <h2 className="text-2xl font-display font-bold text-purple-300 mb-6 flex items-center uppercase tracking-wide">
                                Profile Information
                            </h2>

                            <div className="space-y-4">
                                {/* Company Name */}
                                <div className="p-4 bg-gray-900/40 clip-sharp-sm border-l-2 border-purple-500">
                                    <label className="block text-sm font-semibold text-purple-300 mb-2 uppercase tracking-wide">Company Name</label>
                                    <div className="flex gap-2">
                                        <input
                                            type="text"
                                            className="input-modern flex-1"
                                            onChange={(e) => setProfile({ ...profile, companyName: e.target.value })}
                                            value={profile.companyName}
                                        />
                                        <button
                                            className="btn-gradient disabled:opacity-50 disabled:cursor-not-allowed px-6"
                                            onClick={() => updateCompanyName(profile.companyName)}
                                            disabled={updating}
                                        >
                                            {updating ? '...' : 'Update'}
                                        </button>
                                    </div>
                                </div>

                                {/* Email */}
                                <div className="p-4 bg-gray-900/40 clip-sharp-sm border-l-2 border-cyan-500">
                                    <label className="block text-sm font-semibold text-cyan-300 mb-2 uppercase tracking-wide">Company Email</label>
                                    <div className="flex gap-2">
                                        <input
                                            type="email"
                                            className="input-modern flex-1"
                                            onChange={(e) => setProfile({ ...profile, email: e.target.value })}
                                            value={profile.email}
                                        />
                                        <button
                                            className="btn-gradient disabled:opacity-50 disabled:cursor-not-allowed px-6"
                                            onClick={() => updateContactInfo(profile.email, profile.phoneNumber)}
                                            disabled={updating}
                                        >
                                            {updating ? '...' : 'Update'}
                                        </button>
                                    </div>
                                </div>

                                {/* Phone */}
                                <div className="p-4 bg-gray-900/40 clip-sharp-sm border-l-2 border-pink-500">
                                    <label className="block text-sm font-semibold text-pink-300 mb-2 uppercase tracking-wide">Company Phone</label>
                                    <div className="flex gap-2">
                                        <input
                                            type="tel"
                                            className="input-modern flex-1"
                                            onChange={(e) => setProfile({ ...profile, phoneNumber: e.target.value })}
                                            value={profile.phoneNumber}
                                        />
                                        <button
                                            className="btn-gradient disabled:opacity-50 disabled:cursor-not-allowed px-6"
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
                            <h2 className="text-2xl font-display font-bold text-purple-300 mb-6 flex items-center uppercase tracking-wide">
                                Notification Channels
                            </h2>

                            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                                {['email', 'sms', 'console'].map((channel) => {
                                    const isActive = profile.activeChannels.includes(channel);
                                    return (
                                        <button
                                            key={channel}
                                            className={`p-4 font-semibold transition-all duration-300 clip-sharp border-2 uppercase tracking-wide ${isActive
                                                ? 'bg-gradient-to-r from-green-600 to-emerald-600 text-white border-green-500'
                                                : 'bg-gray-800 text-gray-400 hover:bg-gray-700 border-gray-700'
                                                } disabled:opacity-50 disabled:cursor-not-allowed`}
                                            onClick={() => updateChannel(channel, !isActive)}
                                            disabled={updating}
                                        >
                                            <div className="text-2xl mb-2">
                                                {channel === 'email'}
                                                {channel === 'sms'}
                                                {channel === 'console'}
                                            </div>
                                            <div>{channel}</div>
                                            <div className="text-sm mt-1">
                                                {isActive ? 'Active' : 'Inactive'}
                                            </div>
                                        </button>
                                    );
                                })}
                            </div>
                        </div>

                        {/* Subscription Information Card */}
                        <div className="card p-6">
                            <h2 className="text-2xl font-display font-bold text-purple-300 mb-6 flex items-center uppercase tracking-wide">
                                Subscription Information
                            </h2>

                            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
                                {/* Plan */}
                                <div className="p-4 bg-gradient-to-br from-purple-900/50 to-purple-700/30 clip-sharp border-2 border-purple-500">
                                    <p className="text-sm text-purple-300 font-semibold mb-1 uppercase tracking-wide">Plan</p>
                                    <p className="text-2xl font-bold text-white">{profile.subscription?.plan || 'N/A'}</p>
                                </div>

                                {/* Monthly Limit */}
                                <div className="p-4 bg-gradient-to-br from-blue-900/50 to-blue-700/30 clip-sharp border-2 border-blue-500">
                                    <p className="text-sm text-blue-300 font-semibold mb-1 uppercase tracking-wide">Limit</p>
                                    <p className="text-2xl font-bold text-white">{profile.subscription?.monthlyLimit ?? 'N/A'}</p>
                                </div>

                                {/* Used This Month */}
                                <div className="p-4 bg-gradient-to-br from-cyan-900/50 to-cyan-700/30 clip-sharp border-2 border-cyan-500">
                                    <p className="text-sm text-cyan-300 font-semibold mb-1 uppercase tracking-wide">Usage</p>
                                    <p className="text-2xl font-bold text-white">{profile.subscription?.usedThisMonth ?? 'N/A'}</p>
                                </div>

                                {/* Expiry Date */}
                                <div className="p-4 bg-gradient-to-br from-pink-900/50 to-pink-700/30 clip-sharp border-2 border-pink-500">
                                    <p className="text-sm text-pink-300 font-semibold mb-1 uppercase tracking-wide">Expires</p>
                                    <p className="text-lg font-bold text-white">
                                        {profile.subscription?.expiresAt
                                            ? new Date(profile.subscription.expiresAt).toLocaleDateString('en-US')
                                            : 'N/A'}
                                    </p>
                                </div>
                            </div>
                        </div>

                        {/* API Keys Card */}
                        <div className="card p-6">
                            <h2 className="text-2xl font-display font-bold text-purple-300 mb-6 flex items-center uppercase tracking-wide">
                                API Keys
                            </h2>
                            <div className="max-w-md space-y-4">
                                <div>
                                    <label className="block text-sm font-semibold text-purple-300 mb-2 uppercase tracking-wide">API Key Name</label>
                                    <input
                                        type="text"
                                        placeholder="Key Name"
                                        id="apiKeyName"
                                        className="input-modern"
                                    />
                                </div>
                                <button
                                    className="btn-gradient disabled:opacity-50 disabled:cursor-not-allowed uppercase tracking-wider"
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
                                    <div key={apiKey.id} className="flex items-center justify-between p-3 bg-gray-800/50 clip-sharp-sm border border-purple-500/30">
                                        <div className="flex-1">
                                            <p className="text-sm text-purple-300 font-semibold mb-1">{apiKey.name}</p>
                                            <p className="text-xs text-gray-400 font-mono">{apiKey.prefix}...</p>
                                            <p className="text-xs text-gray-500 mt-1">Created: {new Date(apiKey.createdAt).toLocaleDateString('en-US')}</p>
                                        </div>
                                        <button
                                            onClick={async () => {
                                                if (confirm(`Are you sure you want to delete the "${apiKey.name}" key?`)) {
                                                    await deleteApiKey(apiKey.id);
                                                    await loadApiKeys();
                                                }
                                            }}
                                            className="px-4 py-2 bg-red-600 text-white text-sm hover:bg-red-700 transition-all clip-sharp-sm border border-red-500 uppercase"
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
                            <h2 className="text-2xl font-display font-bold text-purple-300 mb-6 flex items-center uppercase tracking-wide">
                                Password Update
                            </h2>

                            <div className="max-w-md space-y-4">
                                <div>
                                    <label className="block text-sm font-semibold text-purple-300 mb-2 uppercase tracking-wide">Current Password</label>
                                    <input
                                        type="password"
                                        placeholder="••••••••"
                                        id="currentPassword"
                                        className="input-modern"
                                    />
                                </div>
                                <div>
                                    <label className="block text-sm font-semibold text-purple-300 mb-2 uppercase tracking-wide">New Password</label>
                                    <input
                                        type="password"
                                        placeholder="••••••••"
                                        id="newPassword"
                                        className="input-modern"
                                    />
                                </div>
                                <button
                                    className="btn-gradient disabled:opacity-50 disabled:cursor-not-allowed uppercase tracking-wider"
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
                        <p className="text-xl text-gray-400 uppercase tracking-wide">Profile not found.</p>
                    </div>
                )}
            </main>
        </div>
    );
}