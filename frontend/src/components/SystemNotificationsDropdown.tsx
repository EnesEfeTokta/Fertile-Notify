import { useState, useEffect, useRef } from "react";
import { getSystemNotifications, markSystemNotificationAsRead } from "../api/systemNotificationService";
import type { SystemNotificationDto } from "../api/systemNotificationService";

export default function SystemNotificationsDropdown() {
    const [notifications, setNotifications] = useState<SystemNotificationDto[]>([]);
    const [isOpen, setIsOpen] = useState(false);
    const [loading, setLoading] = useState(false);
    const dropdownRef = useRef<HTMLDivElement>(null);

    const unreadCount = notifications.filter(n => !n.isRead).length;

    useEffect(() => {
        const fetchNotifications = async () => {
            setLoading(true);
            try {
                const data = await getSystemNotifications("all");
                if (data && Array.isArray(data)) {
                    // Sort by createdAt descending
                    data.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());
                    setNotifications(data);
                } else {
                    setNotifications([]);
                }
            } catch (error) {
                console.error("Failed to fetch system notifications", error);
            } finally {
                setLoading(false);
            }
        };

        fetchNotifications();
    }, []);

    useEffect(() => {
        const handleClickOutside = (event: MouseEvent) => {
            if (dropdownRef.current && !dropdownRef.current.contains(event.target as Node)) {
                setIsOpen(false);
            }
        };

        if (isOpen) {
            document.addEventListener("mousedown", handleClickOutside);
        } else {
            document.removeEventListener("mousedown", handleClickOutside);
        }

        return () => {
            document.removeEventListener("mousedown", handleClickOutside);
        };
    }, [isOpen]);

    const handleMarkAsRead = async (id: string, isRead: boolean) => {
        if (isRead) return;

        try {
            await markSystemNotificationAsRead(id);
            setNotifications(prev => prev.map(n => n.id === id ? { ...n, isRead: true } : n));
        } catch (error) {
            console.error("Failed to mark notification as read", error);
        }
    };

    return (
        <div className="relative" ref={dropdownRef}>
            <button
                onClick={() => setIsOpen(!isOpen)}
                className="relative p-2 rounded-full text-secondary hover:text-primary hover:bg-tertiary transition-colors"
                aria-label="Notifications"
            >
                <svg fill="none" stroke="currentColor" viewBox="0 0 24 24" className="w-5 h-5">
                    <path
                        strokeLinecap="round"
                        strokeLinejoin="round"
                        strokeWidth={1.75}
                        d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9"
                    />
                </svg>
                {unreadCount > 0 && (
                    <span className="absolute top-1 right-1.5 flex h-2.5 w-2.5">
                        <span className="animate-ping absolute inline-flex h-full w-full rounded-full bg-red-500 opacity-75"></span>
                        <span className="relative inline-flex rounded-full h-2.5 w-2.5 bg-red-500"></span>
                    </span>
                )}
            </button>

            {isOpen && (
                <div className="absolute right-0 mt-2 w-80 bg-elevated border border-primary rounded-xl shadow-2xl z-50 overflow-hidden animate-fade-in-down">
                    <div className="flex items-center justify-between px-4 py-3 border-b border-primary bg-secondary">
                        <h3 className="font-display font-semibold text-sm">Notifications</h3>
                        {unreadCount > 0 && (
                            <span className="text-xs bg-accent-dim text-accent-primary px-2 py-0.5 rounded-full font-medium">
                                {unreadCount} new
                            </span>
                        )}
                    </div>

                    <div className="max-h-96 overflow-y-auto">
                        {loading && notifications.length === 0 ? (
                            <div className="p-4 text-center text-sm text-tertiary">Loading...</div>
                        ) : notifications.length === 0 ? (
                            <div className="p-6 text-center text-sm text-tertiary">
                                No notifications found
                            </div>
                        ) : (
                            <div className="divide-y divide-primary">
                                {notifications.map(notif => (
                                    <div
                                        key={notif.id}
                                        onClick={() => handleMarkAsRead(notif.id, notif.isRead)}
                                        className={`p-4 transition-colors cursor-pointer ${notif.isRead ? "bg-transparent hover:bg-tertiary/50" : "bg-accent-dim hover:bg-accent-dim/80"
                                            }`}
                                    >
                                        <div className="flex justify-between items-start mb-1 gap-2">
                                            <h4 className={`text-sm font-medium ${notif.isRead ? "text-primary" : "text-accent-primary"}`}>
                                                {notif.title}
                                            </h4>
                                            {!notif.isRead && (
                                                <span className="shrink-0 h-2 w-2 mt-1.5 rounded-full bg-accent-primary"></span>
                                            )}
                                        </div>
                                        <p className="text-xs text-secondary mt-1 leading-relaxed line-clamp-3">
                                            {notif.message}
                                        </p>
                                        <p className="text-[10px] text-tertiary mt-2">
                                            {new Date(notif.createdAt).toLocaleString()}
                                        </p>
                                    </div>
                                ))}
                            </div>
                        )}
                    </div>
                </div>
            )}
        </div>
    );
}
