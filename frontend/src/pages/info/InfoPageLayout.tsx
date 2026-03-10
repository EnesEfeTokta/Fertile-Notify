import { useNavigate } from "react-router-dom";
import type { ReactNode } from "react";

// ---- Shared Info Page Layout ----
export const InfoPageLayout = ({
    children, badge, title, subtitle
}: {
    children: ReactNode;
    badge: string;
    title: string;
    subtitle: string;
}) => {
    const navigate = useNavigate();
    return (
        <div className="min-h-screen bg-primary text-primary">
            <div className="fixed inset-0 bg-grid-fine opacity-30 pointer-events-none z-0" />

            {/* Top nav */}
            <nav className="relative z-10 border-b border-primary bg-primary/80 backdrop-blur-md py-4 px-6">
                <div className="max-w-7xl mx-auto flex justify-between items-center">
                    <button onClick={() => navigate("/")} className="flex items-center gap-2 group">
                        <div className="w-6 h-6 bg-white rounded flex items-center justify-center">
                            <span className="text-black font-bold text-[10px] font-mono">FN</span>
                        </div>
                        <span className="font-display font-bold text-sm">
                            fertile<span className="text-accent-primary">notify</span>
                        </span>
                    </button>
                    <button onClick={() => navigate(-1)} className="flex items-center gap-1.5 text-xs text-tertiary hover:text-primary transition-colors">
                        <svg className="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
                        </svg>
                        Back
                    </button>
                </div>
            </nav>

            {/* Hero */}
            <div className="relative z-10 pt-20 pb-16 px-6 text-center border-b border-primary">
                <div className="absolute inset-0 pointer-events-none"
                    style={{ background: "radial-gradient(ellipse 70% 50% at 50% 0%, rgba(59,130,246,0.1) 0%, transparent 70%)" }}
                />
                <div className="relative max-w-3xl mx-auto">
                    <div className="section-label mx-auto mb-4">{badge}</div>
                    <h1 className="font-display font-bold text-4xl md:text-5xl tracking-tight mb-4">{title}</h1>
                    <p className="text-secondary text-lg max-w-xl mx-auto leading-relaxed">{subtitle}</p>
                </div>
            </div>

            {/* Content */}
            <div className="relative z-10 max-w-3xl mx-auto px-6 py-16">
                {children}
            </div>

            {/* Footer  */}
            <footer className="border-t border-primary py-8 px-6 relative z-10">
                <div className="max-w-7xl mx-auto flex justify-between items-center">
                    <p className="text-[11px] text-muted">© {new Date().getFullYear()} Fertile Notify</p>
                    <div className="flex items-center gap-4">
                        {[
                            { l: "Privacy", r: "/privacy" },
                            { l: "Contact", r: "/contact" },
                            { l: "API", r: "/api-reference" },
                        ].map(({ l, r }) => (
                            <button key={l} onClick={() => navigate(r)} className="text-xs text-tertiary hover:text-primary transition-colors">
                                {l}
                            </button>
                        ))}
                    </div>
                </div>
            </footer>
        </div>
    );
};

// ---- Section component ----
export const Section = ({ title, children }: { title: string; children: ReactNode }) => (
    <div className="mb-12">
        <h2 className="text-xl font-bold mb-4 text-primary">{title}</h2>
        <div className="text-secondary leading-relaxed space-y-3">{children}</div>
    </div>
);
