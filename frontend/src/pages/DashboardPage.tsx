import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { subscriberService } from '../api/subscriberService';
import type { SubscriberProfile } from '../types/subscriber';

export default function DashboardPage() {
    const [profile, setProfile] = useState<SubscriberProfile | null>(null);
    const navigate = useNavigate();
    const [error, setError] = useState<string | null>(null);
    const [loading, setLoading] = useState<boolean>(true);
    const [updating, setUpdating] = useState<boolean>(false);

    const fetchProfile = async () => {
        try {
            setLoading(true);
            setError(null);
            const data = await subscriberService.getProfile();
            console.log("Profile data received:", data);
            setProfile(data);
        } catch (err) {
            console.error("Error fetching profile:", err);
            setError("Internal server error while fetching profile.");
        } finally {
            setLoading(false);
        }
    };

    const updateCompanyName = async (newName: string) => {
        try {
            if (!newName.trim()) {
                alert("Firma adı boş olamaz.");
                return;
            }
            setUpdating(true);
            console.log("Updating company name to:", newName);
            await subscriberService.setCompanyName({ companyName: newName });
            await fetchProfile();
            alert("Company name updated successfully.");
        } catch (err) {
            console.error("Error updating company name:", err);
            alert("Company name update failed.");
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
        } catch (err) {
            console.error("Error updating contact information:", err);
            alert("Contact information update failed.");
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
        } catch (err) {
            console.error("Error updating channel:", err);
            alert("Channel update failed.");
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
        } catch (err) {
            console.error("Error updating password:", err);
            alert("Password update failed.");
        } finally {
            setUpdating(false);
        }
    }

    const handleLogout = () => {
        localStorage.removeItem('token');
        navigate('/login');
    };

    React.useEffect(() => {
        fetchProfile();
    }, []);

    return (
        <div className="min-h-screen bg-gradient-to-br from-gray-900 via-purple-900 to-gray-900">
            {/* Header */}
            <header className="bg-gray-900/80 backdrop-blur-md border-b-2 border-purple-500/50">
                <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4 flex justify-between items-center">
                    <div>
                        <h1 className="text-3xl font-display font-bold gradient-text uppercase tracking-wider">Dashboard</h1>
                        <p className="text-gray-400 mt-1 uppercase text-sm tracking-wide">Hoş Geldiniz</p>
                    </div>
                    <button
                        onClick={handleLogout}
                        className="px-6 py-2 bg-red-600 text-white font-semibold hover:bg-red-700 transition-all duration-300 clip-sharp-sm border-2 border-red-500 uppercase text-sm tracking-wide"
                    >
                        Çıkış Yap
                    </button>
                </div>
            </header>

            <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
                {/* Loading State */}
                {loading && (
                    <div className="flex justify-center items-center py-12">
                        <div className="spinner"></div>
                        <span className="ml-3 text-purple-400 font-semibold uppercase tracking-wide">Yükleniyor...</span>
                    </div>
                )}

                {/* Error State */}
                {error && (
                    <div className="card p-6 bg-red-900/30 border-l-4 border-red-500 mb-6 animate-slide-up">
                        <p className="font-bold text-red-300 uppercase">Hata:</p>
                        <p className="text-red-400">{error}</p>
                    </div>
                )}

                {/* Profile Content */}
                {!loading && !error && profile && (
                    <div className="space-y-6 animate-fade-in">
                        {/* Profile Information Card */}
                        <div className="card p-6">
                            <h2 className="text-2xl font-display font-bold text-purple-300 mb-6 flex items-center uppercase tracking-wide">
                                Profil Bilgileri
                            </h2>

                            <div className="space-y-4">
                                {/* Company Name */}
                                <div className="p-4 bg-gray-900/40 clip-sharp-sm border-l-2 border-purple-500">
                                    <label className="block text-sm font-semibold text-purple-300 mb-2 uppercase tracking-wide">Firma Adı</label>
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
                                            {updating ? '...' : 'Güncelle'}
                                        </button>
                                    </div>
                                </div>

                                {/* Email */}
                                <div className="p-4 bg-gray-900/40 clip-sharp-sm border-l-2 border-cyan-500">
                                    <label className="block text-sm font-semibold text-cyan-300 mb-2 uppercase tracking-wide">Firma E-Postası</label>
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
                                            {updating ? '...' : 'Güncelle'}
                                        </button>
                                    </div>
                                </div>

                                {/* Phone */}
                                <div className="p-4 bg-gray-900/40 clip-sharp-sm border-l-2 border-pink-500">
                                    <label className="block text-sm font-semibold text-pink-300 mb-2 uppercase tracking-wide">Firma Telefonu</label>
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
                                            {updating ? '...' : 'Güncelle'}
                                        </button>
                                    </div>
                                </div>
                            </div>
                        </div>

                        {/* Notification Channels Card */}
                        <div className="card p-6">
                            <h2 className="text-2xl font-display font-bold text-purple-300 mb-6 flex items-center uppercase tracking-wide">
                                Bildirim Kanalları
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
                                                {channel === 'email' }
                                                {channel === 'sms' }
                                                {channel === 'console' }
                                            </div>
                                            <div>{channel}</div>
                                            <div className="text-sm mt-1">
                                                {isActive ? 'Kullanımda' : 'Kullanımda değil'}
                                            </div>
                                        </button>
                                    );
                                })}
                            </div>
                        </div>

                        {/* Subscription Information Card */}
                        <div className="card p-6">
                            <h2 className="text-2xl font-display font-bold text-purple-300 mb-6 flex items-center uppercase tracking-wide">
                                Abonelik Bilgileri
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
                                    <p className="text-sm text-cyan-300 font-semibold mb-1 uppercase tracking-wide">Kullanım</p>
                                    <p className="text-2xl font-bold text-white">{profile.subscription?.usedThisMonth ?? 'N/A'}</p>
                                </div>

                                {/* Expiry Date */}
                                <div className="p-4 bg-gradient-to-br from-pink-900/50 to-pink-700/30 clip-sharp border-2 border-pink-500">
                                    <p className="text-sm text-pink-300 font-semibold mb-1 uppercase tracking-wide">Bitiş</p>
                                    <p className="text-lg font-bold text-white">
                                        {profile.subscription?.expiresAt
                                            ? new Date(profile.subscription.expiresAt).toLocaleDateString('tr-TR')
                                            : 'N/A'}
                                    </p>
                                </div>
                            </div>
                        </div>

                        {/* Password Update Card */}
                        <div className="card p-6">
                            <h2 className="text-2xl font-display font-bold text-purple-300 mb-6 flex items-center uppercase tracking-wide">
                                Şifre Güncelleme
                            </h2>

                            <div className="max-w-md space-y-4">
                                <div>
                                    <label className="block text-sm font-semibold text-purple-300 mb-2 uppercase tracking-wide">Mevcut Şifre</label>
                                    <input
                                        type="password"
                                        placeholder="••••••••"
                                        id="currentPassword"
                                        className="input-modern"
                                    />
                                </div>
                                <div>
                                    <label className="block text-sm font-semibold text-purple-300 mb-2 uppercase tracking-wide">Yeni Şifre</label>
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
                                    {updating ? 'Güncelleniyor...' : 'Şifreyi Güncelle'}
                                </button>
                            </div>
                        </div>
                    </div>
                )}

                {/* No Profile State */}
                {!loading && !error && !profile && (
                    <div className="card p-12 text-center">
                        <p className="text-xl text-gray-400 uppercase tracking-wide">Profil bilgisi bulunamadı.</p>
                    </div>
                )}
            </main>
        </div>
    );
}