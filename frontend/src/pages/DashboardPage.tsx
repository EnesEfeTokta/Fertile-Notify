import React, { useState } from 'react';
import { subscriberService } from '../api/subscriberService';
import type { SubscriberProfile } from '../types/subscriber';

export default function DashboardPage() {
    const [profile, setProfile] = useState<SubscriberProfile | null>(null);
    const [error, setError] = useState<string | null>(null);
    const [loading, setLoading] = useState<boolean>(true);

    const fetchProfile = async () => {
        try {
            setLoading(true);
            setError(null);
            const data = await subscriberService.getProfile();
            console.log("Profile data received:", data);
            setProfile(data);
        } catch (err: unknown) {
            console.error("Error fetching profile:", err);
            setError("Internal server error while fetching profile.");
        } finally {
            setLoading(false);
        }
    };

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
                        <span>{profile.companyName}</span>
                    </div>
                    <div className="border-b pb-2">
                        <label className="font-bold mr-2">Firma E-Postası:</label>
                        <span>{profile.email}</span>
                    </div>
                    <div className="border-b pb-2">
                        <label className="font-bold mr-2">Firma Telefonu:</label>
                        <span>{profile.phoneNumber || "N/A"}</span>
                    </div>
                    <div className="border-b pb-2">
                        <label className="font-bold mr-2">Aktif Kanallar:</label>
                        <span>{profile.activeChannels?.join(', ') || 'Yok'}</span>
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
                </div>
            )}

            {!loading && !error && !profile && (
                <p className="text-gray-500">Profil bilgisi bulunamadı.</p>
            )}
        </div>
    );
}