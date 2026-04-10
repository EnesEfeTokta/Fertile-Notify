import type { ReactNode } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import SystemNotificationsDropdown from "./SystemNotificationsDropdown";

interface AppShellProps {
    children: ReactNode;
    title: string;
    actions?: ReactNode;
    companyName?: string;
    plan?: string;
}

const NavItem = ({
    icon, label, path, currentPath, onClick
}: {
    icon: ReactNode; label: string; path?: string; currentPath?: string; onClick?: () => void;
}) => {
    const navigate = useNavigate();
    const isActive = path && currentPath === path;
    return (
        <button
            onClick={onClick ?? (() => path && navigate(path))}
            className={`w-full flex items-center gap-3 px-3 py-2 rounded-lg text-sm font-medium transition-all duration-150 text-left ${isActive
                ? "bg-accent-dim text-accent-primary border border-blue-500/15"
                : "text-secondary hover:text-primary hover:bg-tertiary"
                }`}
        >
            <span className="w-4 h-4 shrink-0">{icon}</span>
            {label}
        </button>
    );
};

export default function AppShell({ children, title, actions, companyName, plan }: AppShellProps) {
    const navigate = useNavigate();
    const location = useLocation();

    const handleLogout = () => {
        localStorage.removeItem("accessToken");
        localStorage.removeItem("refreshToken");
        localStorage.removeItem("token");
        navigate("/login");
    };

    const planColors: Record<string, string> = {
        free: "bg-tertiary text-tertiary border-secondary",
        pro: "bg-blue-500/10 text-blue-400 border-blue-500/20",
        enterprise: "bg-purple-500/10 text-purple-400 border-purple-500/20",
    };
    const planColor = planColors[(plan ?? "free").toLowerCase()] ?? planColors["free"];

    return (
        <div className="min-h-screen bg-primary flex">
            {/* ---- Sidebar ---- */}
            <aside className="w-[220px] shrink-0 border-r border-primary flex flex-col bg-secondary sticky top-0 h-screen overflow-y-auto z-60">
                {/* Brand */}
                <div className="px-4 pt-5 pb-4 border-b border-primary">
                    <button onClick={() => navigate("/")} className="flex items-center gap-2 group">
                        <div className="w-6 h-6 bg-white rounded flex items-center justify-center shrink-0">
                            <span className="text-black font-bold text-[10px] font-mono">FN</span>
                        </div>
                        <span className="font-display font-bold text-sm">
                            fertile<span className="text-accent-primary">notify</span>
                        </span>
                    </button>
                </div>

                {/* Nav */}
                <nav className="flex-1 px-3 py-4 space-y-4 overflow-y-auto">
                    <div>
                        <p className="text-[10px] font-bold uppercase tracking-widest text-muted px-3 mb-2">App</p>
                        <div className="space-y-1">
                            <NavItem
                                currentPath={location.pathname}
                                path="/dashboard"
                                label="Dashboard"
                                icon={
                                    <svg fill="none" stroke="currentColor" viewBox="0 0 24 24" className="w-4 h-4">
                                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.75} d="M4 6h16M4 10h16M4 14h16M4 18h16" />
                                    </svg>
                                }
                            />
                            <NavItem
                                currentPath={location.pathname}
                                path="/statistics"
                                label="Statistics"
                                icon={
                                    <svg fill="none" stroke="currentColor" viewBox="0 0 24 24" className="w-4 h-4">
                                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.75} d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
                                    </svg>
                                }
                            />
                            <NavItem
                                currentPath={location.pathname}
                                path="/templates"
                                label="Templates"
                                icon={
                                    <svg fill="none" stroke="currentColor" viewBox="0 0 24 24" className="w-4 h-4">
                                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.75} d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                                    </svg>
                                }
                            />
                            <NavItem
                                currentPath={location.pathname}
                                path="/logs"
                                label="Delivery Logs"
                                icon={
                                    <svg fill="none" stroke="currentColor" viewBox="0 0 24 24" className="w-4 h-4">
                                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.75} d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
                                    </svg>
                                }
                            />
                            <NavItem
                                currentPath={location.pathname}
                                path="/workflows"
                                label="Workflows"
                                icon={
                                    <svg fill="none" stroke="currentColor" viewBox="0 0 24 24" className="w-4 h-4">
                                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.75} d="M13 10V3L4 14h7v7l9-11h-7z" />
                                    </svg>
                                }
                            />
                        </div>
                    </div>

                    <div>
                        <p className="text-[10px] font-bold uppercase tracking-widest text-muted px-3 mb-2">Developer</p>
                        <div className="space-y-1">
                            <NavItem
                                currentPath={location.pathname}
                                path="/api-keys"
                                label="Developers"
                                icon={
                                    <svg fill="none" stroke="currentColor" viewBox="0 0 24 24" className="w-4 h-4">
                                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.75} d="M15 7a2 2 0 012 2m4 0a6 6 0 01-7.743 5.743L11 17H9v2H7v2H4a1 1 0 01-1-1v-2.586a1 1 0 01.293-.707l5.964-5.964A6 6 0 1121 9z" />
                                    </svg>
                                }
                            />
                            <NavItem
                                currentPath={location.pathname}
                                path="/blacklist"
                                label="Blacklist"
                                icon={
                                    <svg fill="none" stroke="currentColor" viewBox="0 0 24 24" className="w-4 h-4">
                                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.75} d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z" />
                                    </svg>
                                }
                            />
                        </div>
                    </div>

                    <div>
                        <p className="text-[10px] font-bold uppercase tracking-widest text-muted px-3 mb-2">Management</p>
                        <div className="space-y-1">
                            <NavItem
                                currentPath={location.pathname}
                                path="/account"
                                label="Account Settings"
                                icon={
                                    <svg fill="none" stroke="currentColor" viewBox="0 0 24 24" className="w-4 h-4">
                                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.75} d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
                                    </svg>
                                }
                            />
                            <NavItem
                                currentPath={location.pathname}
                                path="/pricing"
                                label="Upgrade Plan"
                                icon={
                                    <svg fill="none" stroke="currentColor" viewBox="0 0 24 24" className="w-4 h-4">
                                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.75} d="M5 3v4M3 5h4M6 17v4m-2-2h4m5-16l2.286 6.857L21 12l-5.714 2.143L13 21l-2.286-6.857L5 12l5.714-2.143L13 3z" />
                                    </svg>
                                }
                            />
                            <NavItem
                                currentPath={location.pathname}
                                path="/buy-credits"
                                label="Buy Credits"
                                icon={
                                    <svg fill="none" stroke="currentColor" viewBox="0 0 24 24" className="w-4 h-4">
                                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.75} d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                                    </svg>
                                }
                            />
                        </div>
                    </div>
                </nav>

                {/* User info */}
                <div className="px-3 pb-4 border-t border-primary pt-4 space-y-3">
                    {companyName && (
                        <div className="px-3 py-2 rounded-lg bg-tertiary border border-primary">
                            <p className="text-xs font-semibold text-primary truncate">{companyName}</p>
                            {plan && (
                                <span className={`inline-flex items-center mt-1 px-1.5 py-0.5 text-[10px] font-bold uppercase tracking-wider rounded border ${planColor}`}>
                                    {plan}
                                </span>
                            )}
                        </div>
                    )}
                    <NavItem
                        label="Sign out"
                        onClick={handleLogout}
                        icon={
                            <svg fill="none" stroke="currentColor" viewBox="0 0 24 24" className="w-4 h-4">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.75} d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
                            </svg>
                        }
                    />
                </div>
            </aside>

            {/* ---- Main content ---- */}
            <div className="flex-1 flex flex-col min-w-0">
                {/* Sticky top bar */}
                <header className="sticky top-0 z-50 border-b border-primary bg-primary/85 backdrop-blur-md px-8 py-4 flex justify-between items-center">
                    <h1 className="font-display font-bold text-xl tracking-tight">{title}</h1>
                    <div className="flex items-center gap-4">
                        {actions && <div className="flex items-center gap-2">{actions}</div>}
                        <SystemNotificationsDropdown />
                    </div>
                </header>

                {/* Page content */}
                <main className="flex-1 px-8 py-8">
                    {children}
                </main>
            </div>
        </div>
    );
}
