import { useState, useEffect, useCallback } from "react";
import AppShell from "../components/AppShell";
import { blacklistService } from "../api/blacklistService";
import { NOTIFICATION_CHANNELS } from "../constants/channels";
import type { BlacklistEntry } from "../types/blacklist";
import { useToast } from "../components/Toast";

export default function BlacklistPage() {
    const [items, setItems] = useState<BlacklistEntry[]>([]);
    const [loading, setLoading] = useState(true);
    const [showModal, setShowModal] = useState(false);
    const [editingItem, setEditingItem] = useState<BlacklistEntry | null>(null);
    
    // Form state
    const [recipientAddress, setRecipientAddress] = useState("");
    const [selectedChannels, setSelectedChannels] = useState<string[]>([]);
    const [isActive, setIsActive] = useState(true);
    const [submitting, setSubmitting] = useState(false);

    const { showToast, ToastContainer } = useToast();

    const fetchBlacklist = useCallback(async () => {
        try {
            setLoading(true);
            const data = await blacklistService.getAll();
            setItems(data);
        } catch (err) {
            showToast("Failed to load blacklist.", "error");
        } finally {
            setLoading(false);
        }
    }, [showToast]);

    useEffect(() => {
        fetchBlacklist();
    }, [fetchBlacklist]);

    const handleOpenModal = (item?: BlacklistEntry) => {
        if (item) {
            setEditingItem(item);
            setRecipientAddress(item.recipientAddress);
            setSelectedChannels(item.channels);
            setIsActive(item.isActive);
        } else {
            setEditingItem(null);
            setRecipientAddress("");
            setSelectedChannels([]);
            setIsActive(true);
        }
        setShowModal(true);
    };

    const handleCloseModal = () => {
        setShowModal(false);
        setEditingItem(null);
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!recipientAddress) {
            showToast("Recipient address is required.", "error");
            return;
        }

        setSubmitting(true);
        try {
            if (editingItem) {
                await blacklistService.update(editingItem.id, {
                    channels: selectedChannels,
                    isActive: isActive
                });
                showToast("Entry updated successfully.");
            } else {
                await blacklistService.add({
                    recipientAddress,
                    channels: selectedChannels
                });
                showToast("Recipient added to blacklist.");
            }
            handleCloseModal();
            fetchBlacklist();
        } catch (err) {
            showToast("Operation failed.", "error");
        } finally {
            setSubmitting(false);
        }
    };

    const handleDelete = async (id: string) => {
        if (!confirm("Are you sure you want to remove this recipient from the blacklist?")) return;
        
        try {
            await blacklistService.delete(id);
            showToast("Recipient removed from blacklist.");
            fetchBlacklist();
        } catch (err) {
            showToast("Failed to delete entry.", "error");
        }
    };

    const toggleChannel = (channelId: string) => {
        if (selectedChannels.includes(channelId)) {
            setSelectedChannels(selectedChannels.filter(c => c !== channelId));
        } else {
            setSelectedChannels([...selectedChannels, channelId]);
        }
    };

    return (
        <AppShell title="Blacklist Management">
            <ToastContainer />

            <div className="mb-6 flex justify-between items-center">
                <div>
                    <h2 className="text-secondary text-sm">Manage recipients who should not receive certain notifications.</h2>
                </div>
                <button onClick={() => handleOpenModal()} className="btn-primary flex items-center gap-2">
                    <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v16m8-8H4" />
                    </svg>
                    Ban Recipient
                </button>
            </div>

            {loading ? (
                <div className="flex justify-center py-20">
                    <div className="spinner" />
                </div>
            ) : items.length === 0 ? (
                <div className="card p-20 text-center">
                    <div className="text-4xl mb-4 opacity-20">🛡️</div>
                    <h3 className="text-lg font-bold text-white mb-2">Blacklist is empty</h3>
                    <p className="text-secondary text-sm mb-6">You haven't added any recipients to the blacklist yet.</p>
                    <button onClick={() => handleOpenModal()} className="btn-secondary">Add First Recipient</button>
                </div>
            ) : (
                <div className="card overflow-hidden">
                    <table className="w-full text-left border-collapse">
                        <thead>
                            <tr className="bg-secondary/50 border-b border-primary">
                                <th className="px-6 py-4 text-[10px] font-bold uppercase tracking-widest text-muted">Recipient</th>
                                <th className="px-6 py-4 text-[10px] font-bold uppercase tracking-widest text-muted">Restricted Channels</th>
                                <th className="px-6 py-4 text-[10px] font-bold uppercase tracking-widest text-muted">Status</th>
                                <th className="px-6 py-4 text-[10px] font-bold uppercase tracking-widest text-muted text-right">Actions</th>
                            </tr>
                        </thead>
                        <tbody className="divide-y divide-primary">
                            {items.map((item) => (
                                <tr key={item.id} className="hover:bg-tertiary/20 transition-colors">
                                    <td className="px-6 py-4">
                                        <div className="font-medium text-white text-sm">{item.recipientAddress}</div>
                                        <div className="text-[10px] text-tertiary mt-1">Added {new Date(item.createdAt).toLocaleDateString()}</div>
                                    </td>
                                    <td className="px-6 py-4">
                                        <div className="flex flex-wrap gap-1.5">
                                            {item.channels.length === 0 ? (
                                                <span className="tag-accent text-red-400 bg-red-400/10 border-red-500/20">ALL CHANNELS</span>
                                            ) : (
                                                item.channels.map(ch => (
                                                    <span key={ch} className="tag">{ch}</span>
                                                ))
                                            )}
                                        </div>
                                    </td>
                                    <td className="px-6 py-4">
                                        <span className={`inline-flex items-center gap-1.5 px-2 py-0.5 rounded-full text-[10px] font-bold uppercase ${item.isActive ? "text-green-400 bg-green-500/10" : "text-tertiary bg-tertiary/10"}`}>
                                            <span className={`w-1 h-1 rounded-full ${item.isActive ? "bg-green-400" : "bg-tertiary"}`} />
                                            {item.isActive ? "Active" : "Paused"}
                                        </span>
                                    </td>
                                    <td className="px-6 py-4 text-right">
                                        <div className="flex justify-end gap-2">
                                            <button onClick={() => handleOpenModal(item)} className="p-2 hover:bg-tertiary rounded-lg text-secondary hover:text-primary transition-colors">
                                                <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15.232 5.232l3.536 3.536m-2.036-5.036a2.5 2.5 0 113.536 3.536L6.5 21.036H3v-3.572L16.732 3.732z" />
                                                </svg>
                                            </button>
                                            <button onClick={() => handleDelete(item.id)} className="p-2 hover:bg-red-500/10 rounded-lg text-secondary hover:text-red-400 transition-colors">
                                                <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                                                </svg>
                                            </button>
                                        </div>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            )}

            {/* Modal */}
            {showModal && (
                <div className="fixed inset-0 z-[100] flex items-center justify-center p-6">
                    <div className="absolute inset-0 bg-black/80 backdrop-blur-sm" onClick={handleCloseModal} />
                    <div className="card w-full max-w-lg relative z-10 shadow-2xl animate-fade-in-up">
                        <div className="p-6 border-b border-primary flex justify-between items-center">
                            <h3 className="font-display font-bold text-lg">{editingItem ? "Edit Restriction" : "Ban Recipient"}</h3>
                            <button onClick={handleCloseModal} className="text-tertiary hover:text-white transition-colors">
                                <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                                </svg>
                            </button>
                        </div>

                        <form onSubmit={handleSubmit} className="p-6 space-y-6">
                            <div>
                                <label className="block text-xs font-bold uppercase tracking-widest text-muted mb-2">Recipient Address</label>
                                <input
                                    disabled={!!editingItem}
                                    type="text"
                                    className="input-modern"
                                    placeholder="email@example.com or +905..."
                                    value={recipientAddress}
                                    onChange={(e) => setRecipientAddress(e.target.value)}
                                />
                                <p className="text-[10px] text-tertiary mt-2">Enter the exact address you want to block.</p>
                            </div>

                            <div>
                                <label className="block text-xs font-bold uppercase tracking-widest text-muted mb-3">Restricted Channels</label>
                                <div className="grid grid-cols-2 gap-2">
                                    {NOTIFICATION_CHANNELS.map(ch => (
                                        <button
                                            key={ch.id}
                                            type="button"
                                            onClick={() => toggleChannel(ch.id)}
                                            className={`flex items-center gap-2.5 p-3 rounded-lg border text-left transition-all ${selectedChannels.includes(ch.id)
                                                ? "bg-accent-dim border-accent-primary text-accent-primary"
                                                : "bg-primary/20 border-primary text-secondary hover:border-hover"
                                                }`}
                                        >
                                            <span className="text-lg">{ch.icon}</span>
                                            <span className="text-xs font-medium">{ch.name}</span>
                                        </button>
                                    ))}
                                </div>
                                <p className="text-[10px] text-tertiary mt-3">
                                    If no channels are selected, the recipient will be blocked on <strong>ALL</strong> channels.
                                </p>
                            </div>

                            {editingItem && (
                                <div className="flex items-center justify-between p-4 bg-tertiary/20 rounded-xl border border-primary">
                                    <div>
                                        <p className="text-sm font-semibold text-white">Active Status</p>
                                        <p className="text-[11px] text-secondary">Toggle to pause or resume this restriction.</p>
                                    </div>
                                    <button
                                        type="button"
                                        onClick={() => setIsActive(!isActive)}
                                        className={`w-12 h-6 rounded-full transition-all relative ${isActive ? "bg-green-500" : "bg-tertiary"}`}
                                    >
                                        <div className={`absolute top-1 w-4 h-4 bg-white rounded-full transition-all ${isActive ? "right-1" : "left-1"}`} />
                                    </button>
                                </div>
                            )}

                            <div className="flex gap-3 pt-4">
                                <button type="button" onClick={handleCloseModal} className="btn-secondary flex-1">Cancel</button>
                                <button type="submit" disabled={submitting} className="btn-primary flex-1">
                                    {submitting ? <span className="spinner w-4 h-4 mr-2" /> : null}
                                    {editingItem ? "Update Changes" : "Confirm Ban"}
                                </button>
                            </div>
                        </form>
                    </div>
                </div>
            )}
        </AppShell>
    );
}
