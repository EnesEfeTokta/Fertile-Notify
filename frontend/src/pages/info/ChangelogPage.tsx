import { InfoPageLayout } from "./InfoPageLayout";

export default function ChangelogPage() {
    const releases = [
        {
            version: "v1.3.0", date: "March 2025", tag: "Latest",
            changes: [
                "Added Channel Settings modal for per-channel API key configuration",
                "Added live password strength indicator on registration page",
                "Added \"Forgot Password\" link to login page",
                "Improved pricing page with detailed feature comparison table",
                "Redesigned homepage with animated notification demo and how-it-works section",
                "Footer pages (About, Contact, Privacy, Docs, Changelog, API Ref) now live",
            ]
        },
        {
            version: "v1.2.0", date: "February 2025", tag: null,
            changes: [
                "Added Statistics page with plan-based access control (Free/Pro/Enterprise)",
                "Introduced Console Notifications channel with live log viewer modal",
                "Added Templates page for managing notification templates",
                "SMS and email delivery now fully integrated with backend",
            ]
        },
        {
            version: "v1.1.0", date: "January 2025", tag: null,
            changes: [
                "Dashboard redesign with subscription info, API key management, and channel toggles",
                "OTP-based two-factor authentication on login",
                "Introduced Free, Pro, and Enterprise subscription tiers",
                "CORS fix for cross-origin API access on production",
            ]
        },
        {
            version: "v1.0.0", date: "December 2024", tag: "Initial release",
            changes: [
                "Initial public release of Fertile Notify",
                "Email (MJML + HTML), SMS, Slack, Discord, Push, and Webhook channels",
                "REST API with API key authentication",
                "Vercel-style dark theme frontend with React + Vite",
            ]
        },
    ];

    return (
        <InfoPageLayout
            badge="Changelog"
            title="What's new"
            subtitle="Every update, fix, and improvement to Fertile Notify — in one place."
        >
            <div className="space-y-12">
                {releases.map(r => (
                    <div key={r.version} className="relative pl-8 border-l border-primary">
                        {/* Dot */}
                        <div className="absolute -left-[5px] top-0 w-2.5 h-2.5 rounded-full bg-accent-primary border-2 border-primary" />

                        <div className="flex items-center gap-3 mb-4">
                            <span className="font-mono font-bold text-primary">{r.version}</span>
                            {r.tag && <span className="tag-accent">{r.tag}</span>}
                            <span className="text-xs text-tertiary">{r.date}</span>
                        </div>
                        <ul className="space-y-2">
                            {r.changes.map(c => (
                                <li key={c} className="flex items-start gap-2 text-sm text-secondary">
                                    <span className="text-accent-primary mt-0.5 shrink-0">+</span>
                                    {c}
                                </li>
                            ))}
                        </ul>
                    </div>
                ))}
            </div>
        </InfoPageLayout>
    );
}
