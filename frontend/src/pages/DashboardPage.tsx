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

    React.useEffect(() => {
        fetchProfile();
    }, []);

    return (
        <div className="p-10">
            <h1 className="text-3xl mb-6">Hoş Geldiniz</h1>

            {loading && <p className="text-blue-500">Yükleniyor...</p>}
            {error && (
                <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">
                    <p className="font-bold">Hata:</p>
                    <p>{error}</p>
                </div>
            )}

            {!loading && !error && profile && (
                <div className="space-y-4">
                    <div className="border-b pb-2">
                        <label className="font-bold mr-2">Firma Adı:</label>
                        <div>
                            <input
                                type="text"
                                className="border p-1 mr-2"
                                onChange={(e) => setProfile({ ...profile, companyName: e.target.value })}
                                value={profile.companyName}
                            />
                            <button
                                className="bg-blue-500 text-white px-3 py-1 rounded hover:bg-blue-600 disabled:bg-gray-400 disabled:cursor-not-allowed"
                                onClick={() => updateCompanyName(profile.companyName)}
                                disabled={updating}>
                                {updating ? 'Güncelleniyor...' : 'Güncelle'}
                            </button>
                        </div>
                    </div>
                    <div className="border-b pb-2">
                        <label className="font-bold mr-2">Firma E-Postası:</label>
                        <div>
                            <input
                                type="text"
                                className="border p-1 mr-2"
                                onChange={(e) => setProfile({ ...profile, email: e.target.value })}
                                value={profile.email}
                            />
                            <button
                                className="bg-blue-500 text-white px-3 py-1 rounded hover:bg-blue-600 disabled:bg-gray-400 disabled:cursor-not-allowed"
                                onClick={() => updateContactInfo(profile.email, profile.phoneNumber)}
                                disabled={updating}>
                                {updating ? 'Güncelleniyor...' : 'Güncelle'}
                            </button>
                        </div>
                    </div>
                    <div className="border-b pb-2">
                        <label className="font-bold mr-2">Firma Telefonu:</label>
                        <div>
                            <input
                                type="text"
                                className="border p-1 mr-2"
                                onChange={(e) => setProfile({ ...profile, phoneNumber: e.target.value })}
                                value={profile.phoneNumber}
                            />
                            <button
                                className="bg-blue-500 text-white px-3 py-1 rounded hover:bg-blue-600 disabled:bg-gray-400 disabled:cursor-not-allowed"
                                onClick={() => updateContactInfo(profile.email, profile.phoneNumber)}
                                disabled={updating}>
                                {updating ? 'Güncelleniyor...' : 'Güncelle'}
                            </button>
                        </div>
                    </div>
                    <div className="border-b pb-2">
                        <label className="font-bold mr-2">Aktif Kanallar:</label>
                        <div className="space-x-2">
                            {['Email', 'SMS', 'Console'].map((channel) => (
                                <button
                                    key={channel}
                                    className={`px-3 py-1 rounded ${profile.activeChannels.includes(channel)
                                        ? 'bg-green-500 text-white hover:bg-green-600'
                                        : 'bg-gray-300 text-black hover:bg-gray-400'
                                        } disabled:opacity-50 disabled:cursor-not-allowed`}
                                    onClick={() => updateChannel(channel, !profile.activeChannels.includes(channel))}
                                    disabled={updating}>
                                    {profile.activeChannels.includes(channel) ? `Devre Dışı Bırak ${channel}` : `Etkinleştir ${channel}`}
                                </button>
                            ))}
                        </div>
                    </div>

                    <h2 className="text-2xl mt-6 mb-4 font-bold">Abonelik Bilgileri</h2>
                    <div className="border-b pb-2">
                        <label className="font-bold mr-2">Abonelik Planı:</label>
                        <span>{profile.subscription?.plan || 'N/A'}</span>
                    </div>
                    <div className="border-b pb-2">
                        <label className="font-bold mr-2">Aylık Limit:</label>
                        <span>{profile.subscription?.monthlyLimit ?? 'N/A'}</span>
                    </div>
                    <div className="border-b pb-2">
                        <label className="font-bold mr-2">Aylık Kullanım:</label>
                        <span>{profile.subscription?.usedThisMonth ?? 'N/A'}</span>
                    </div>
                    <div className="border-b pb-2">
                        <label className="font-bold mr-2">Bitiş:</label>
                        <span>
                            {profile.subscription?.expiresAt ? new Date(profile.subscription.expiresAt).toLocaleDateString('tr-TR') : 'N/A'}
                        </span>
                    </div>

                    <h2 className="text-2xl mt-6 mb-4 font-bold">Şifre Güncelleme</h2>
                    <div>
                        <input
                            type="password"
                            placeholder="Mevcut Şifre"
                            id="currentPassword"
                            className="border p-1 mr-2 mb-2"
                        />
                        <br />
                        <input
                            type="password"
                            placeholder="Yeni Şifre"
                            id="newPassword"
                            className="border p-1 mr-2 mb-2"
                        />
                        <br />
                        <button
                            className="bg-blue-500 text-white px-3 py-1 rounded hover:bg-blue-600 disabled:bg-gray-400 disabled:cursor-not-allowed"
                            onClick={() => {
                                const currentPasswordInput = document.getElementById('currentPassword') as HTMLInputElement;
                                const newPasswordInput = document.getElementById('newPassword') as HTMLInputElement;
                                updatePassword(currentPasswordInput.value, newPasswordInput.value);
                            }}
                            disabled={updating}>
                            {updating ? 'Güncelleniyor...' : 'Şifreyi Güncelle'}
                        </button>
                    </div>
                </div>
            )}

            {!loading && !error && !profile && (
                <p className="text-gray-500">Profil bilgisi bulunamadı.</p>
            )}
        </div>
    );
}