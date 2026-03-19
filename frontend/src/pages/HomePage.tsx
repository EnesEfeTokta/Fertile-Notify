import { useNavigate } from "react-router-dom";
import { useEffect, useState, useRef } from "react";

// ---- Sub-components ----

const NotifCard = ({
    icon, color, title, subtitle, time, delay = 0
}: {
    icon: string; color: string; title: string; subtitle: string; time: string; delay?: number;
}) => (
    <div
        className="notif-card animate-notif-rise"
        style={{ animationDelay: `${delay}s`, opacity: 0 }}
    >
        <div className="flex items-center gap-3">
            <div className={`w-9 h-9 rounded-full flex items-center justify-center text-lg shrink-0 ${color}`}>
                {icon}
            </div>
            <div className="flex-1 min-w-0">
                <p className="text-sm font-semibold text-white truncate">{title}</p>
                <p className="text-xs text-[#555] truncate mt-0.5">{subtitle}</p>
            </div>
            <span className="text-[10px] text-[#444] shrink-0">{time}</span>
        </div>
    </div>
);

const ChannelTag = ({ label, icon }: { label: string; icon: string }) => (
    <span className="inline-flex items-center gap-1.5 px-3 py-1 bg-elevated rounded-full text-xs font-medium text-secondary border border-primary">
        <span>{icon}</span>{label}
    </span>
);

const StepCard = ({ n, title, desc }: { n: string; title: string; desc: string }) => (
    <div className="how-step">
        <div className="how-step-number">{n}</div>
        <h3 className="text-base font-bold text-primary mb-1.5">{title}</h3>
        <p className="text-sm text-secondary leading-relaxed">{desc}</p>
    </div>
);

const FeatureCard = ({
    icon, title, description, tags
}: {
    icon: string; title: string; description: string; tags: string[];
}) => (
    <div className="card p-7 group hover-reveal-border transition-smooth relative overflow-hidden h-full flex flex-col">
        <div className="text-3xl mb-5">{icon}</div>
        <h3 className="text-base font-bold text-primary mb-2 group-hover:text-accent-light transition-colors">{title}</h3>
        <p className="text-sm text-secondary leading-relaxed mb-4 flex-1">{description}</p>
        <div className="flex flex-wrap gap-1.5 mt-auto">
            {tags.map(tag => (
                <span key={tag} className="tag">{tag}</span>
            ))}
        </div>
    </div>
);



export default function HomePage() {
    const navigate = useNavigate();
    const [scrolled, setScrolled] = useState(false);
    const isAuthenticated = !!localStorage.getItem("accessToken");
    const heroRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        const handleScroll = () => setScrolled(window.scrollY > 40);
        window.addEventListener("scroll", handleScroll);
        return () => window.removeEventListener("scroll", handleScroll);
    }, []);

    const features = [
        {
            icon: "✉️", title: "Email Service", tags: ["MJML", "HTML", "Templates"],
            description: "Deliver pixel-perfect, responsive emails using our visual MJML editor or raw HTML API. Supports variables, layouts, and live preview."
        },
        {
            icon: "💬", title: "SMS & Messaging", tags: ["SMS", "Two-way", "Global"],
            description: "Reach your users globally with lightning-fast SMS delivery. Mobile OTPs, marketing blasts, and transactional messages in one place."
        },
        {
            icon: "🔔", title: "Slack & Discord", tags: ["Slack", "Discord", "Webhooks"],
            description: "Native integrations for team collaboration tools. Send rich blocks to Slack or messages to Discord channels without writing any boilerplate."
        },
        {
            icon: "📲", title: "Push Notifications", tags: ["FCM", "Web", "Mobile"],
            description: "Keep engagement high with Web Push and Mobile push powered by FCM. Supports background notifications and custom payloads."
        },
        {
            icon: "📩", title: "Telegram & WhatsApp", tags: ["Telegram Bot", "WhatsApp API"],
            description: "Connect with your audience on the platforms they already use—no onboarding friction. Works with Telegram Bots and WhatsApp Business API."
        },
        {
            icon: "⚡", title: "Webhooks", tags: ["Any URL", "Retries", "Secure"],
            description: "Maximum flexibility. Forward event data to any HTTP endpoint in your stack with automatic retries and signature verification."
        }
    ];

    const notifications = [
        { icon: "✉️", color: "bg-blue-500/15 text-blue-400", title: "Welcome to Fertile!", subtitle: "Your account is ready to go.", time: "just now", delay: 0 },
        { icon: "💳", color: "bg-green-500/15 text-green-400", title: "Payment confirmed", subtitle: "$29 / mo · Pro Plan", time: "2s ago", delay: 3.5 },
        { icon: "🔔", color: "bg-purple-500/15 text-purple-400", title: "New message in #dev", subtitle: "Sarah: PR is ready for review", time: "5s ago", delay: 7 },
        { icon: "🔐", color: "bg-orange-500/15 text-orange-400", title: "2FA Code: 847291", subtitle: "Expires in 5 minutes", time: "8s ago", delay: 10.5 },
    ];

    return (
        <div className="min-h-screen bg-primary text-primary overflow-x-hidden">
            {/* Background grid + glow */}
            <div className="fixed inset-0 bg-grid-fine pointer-events-none z-0" />
            <div className="fixed inset-0 glow-overlay z-0" />

            {/* ======= NAV ======= */}
            <nav
                className={`fixed top-0 left-0 right-0 z-50 transition-all duration-300 ${scrolled
                    ? "bg-primary/85 backdrop-blur-md border-b border-primary py-3.5"
                    : "bg-transparent py-5"
                    }`}
            >
                <div className="max-w-7xl mx-auto px-6 flex justify-between items-center">
                    {/* Brand */}
                    <div className="flex items-center gap-10">
                        <a
                            href="/"
                            className="flex items-center gap-2.5 group"
                            onClick={e => { e.preventDefault(); window.scrollTo({ top: 0, behavior: "smooth" }); }}
                        >
                            <div className="w-7 h-7 bg-white rounded-md flex items-center justify-center">
                                <span className="text-black font-bold text-xs font-mono">FN</span>
                            </div>
                            <span className="font-display font-bold text-[15px] tracking-tight">
                                fertile<span className="text-accent-primary">notify</span>
                            </span>
                        </a>
                        {/* Nav Links */}
                        <div className="hidden md:flex items-center gap-1">
                            {[
                                { label: "Features", action: () => document.getElementById("features")?.scrollIntoView({ behavior: "smooth" }) },
                                { label: "Pricing", action: () => navigate("/pricing") },
                                { label: "Changelog", action: () => navigate("/changelog") },
                            ].map(l => (
                                <button
                                    key={l.label}
                                    onClick={l.action}
                                    className="btn-ghost text-[13px] py-1.5"
                                >
                                    {l.label}
                                </button>
                            ))}
                            {isAuthenticated && (
                                <>
                                    <button onClick={() => navigate("/templates")} className="btn-ghost text-[13px] py-1.5">Templates</button>
                                    <button onClick={() => navigate("/statistics")} className="btn-ghost text-[13px] py-1.5">Statistics</button>
                                </>
                            )}
                        </div>
                    </div>
                    {/* Auth CTA */}
                    <div className="flex items-center gap-2.5">
                        {!isAuthenticated ? (
                            <>
                                <button
                                    onClick={() => navigate("/login")}
                                    className="btn-ghost text-[13px]"
                                >
                                    Sign in
                                </button>
                                <button
                                    onClick={() => navigate("/register")}
                                    className="btn-primary text-[13px]"
                                >
                                    Get Started
                                    <svg className="ml-1.5 w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 7l5 5m0 0l-5 5m5-5H6" />
                                    </svg>
                                </button>
                            </>
                        ) : (
                            <button
                                onClick={() => navigate("/dashboard")}
                                className="btn-primary text-[13px]"
                            >
                                Dashboard
                                <svg className="ml-1.5 w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 7l5 5m0 0l-5 5m5-5H6" />
                                </svg>
                            </button>
                        )}
                    </div>
                </div>
            </nav>

            {/* ======= HERO ======= */}
            <section ref={heroRef} className="relative min-h-screen flex items-center pt-20 pb-24 px-6 overflow-hidden">
                <div className="max-w-7xl mx-auto w-full grid grid-cols-1 lg:grid-cols-2 items-center gap-16">
                    {/* Left: Text */}
                    <div className="relative z-10">
                        <div className="section-label animate-fade-in">
                            <span className="w-1.5 h-1.5 rounded-full bg-green-400 animate-pulse" />
                            <span>Open Beta — Free to start</span>
                        </div>

                        <h1 className="hero-text-xl mt-4 mb-6 animate-fade-in-up">
                            One API.<br />
                            <span className="gradient-text">Every channel.</span>
                        </h1>

                        <p className="text-[17px] text-secondary leading-relaxed mb-3 max-w-lg animate-fade-in-up delay-100">
                            Fertile Notify is the unified notification infrastructure for modern development teams.
                            Ship faster. Reach users everywhere.
                        </p>

                        <div className="flex flex-wrap gap-2 mb-10 animate-fade-in-up delay-200">
                            <ChannelTag label="Email" icon="✉️" />
                            <ChannelTag label="SMS" icon="💬" />
                            <ChannelTag label="Slack" icon="🔔" />
                            <ChannelTag label="Push" icon="📲" />
                            <ChannelTag label="Webhooks" icon="⚡" />
                        </div>

                        <div className="flex flex-col sm:flex-row items-start gap-3 animate-fade-in-up delay-300">
                            {!isAuthenticated ? (
                                <button
                                    onClick={() => navigate("/register")}
                                    className="btn-primary px-7 py-3 text-[15px] shadow-accent"
                                >
                                    Start for free
                                    <svg className="ml-2 w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 7l5 5m0 0l-5 5m5-5H6" />
                                    </svg>
                                </button>
                            ) : (
                                <button
                                    onClick={() => navigate("/dashboard")}
                                    className="btn-primary px-7 py-3 text-[15px] shadow-accent"
                                >
                                    Go to Dashboard
                                    <svg className="ml-2 w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 7l5 5m0 0l-5 5m5-5H6" />
                                    </svg>
                                </button>
                            )}
                            <button
                                onClick={() => navigate("/api-reference")}
                                className="btn-secondary px-7 py-3 text-[15px]"
                            >
                                <span className="font-mono text-accent-light mr-2">{"<"}/{">"}</span>
                                View API Docs
                            </button>
                        </div>

                        <div className="flex items-center gap-5 mt-10 pt-8 border-t border-primary animate-fade-in-up delay-400">
                            <div className="flex items-center gap-1.5">
                                <span className="text-green-400 text-sm">✓</span>
                                <span className="text-xs text-tertiary">No credit card required</span>
                            </div>
                            <div className="flex items-center gap-1.5">
                                <span className="text-green-400 text-sm">✓</span>
                                <span className="text-xs text-tertiary">2,000 notifications free</span>
                            </div>
                        </div>
                    </div>

                    {/* Right: Live Notification Demo */}
                    <div className="relative hidden lg:flex flex-col items-center justify-center min-h-[480px]">
                        {/* Glow behind notifications */}
                        <div className="absolute w-72 h-72 rounded-full"
                            style={{ background: "radial-gradient(circle, rgba(59,130,246,0.12) 0%, transparent 70%)" }}
                        />

                        {/* Stacked notification cards */}
                        <div className="relative w-full max-w-[320px] space-y-3">
                            {/* Terminal-like header */}
                            <div className="card-glass px-4 py-3 mb-6 text-center">
                                <span className="text-[10px] font-mono uppercase tracking-widest text-tertiary">
                                    Live Notification Stream
                                </span>
                                <div className="flex justify-center gap-1 mt-1.5">
                                    <span className="w-1.5 h-1.5 rounded-full bg-green-500 animate-pulse" />
                                    <span className="text-[10px] text-tertiary">active</span>
                                </div>
                            </div>

                            {notifications.map((n, i) => (
                                <NotifCard key={i} {...n} />
                            ))}
                        </div>

                        {/* Decorative dots */}
                        <div className="absolute top-8 right-0 w-24 h-24 opacity-20">
                            <div className="grid grid-cols-4 gap-2">
                                {Array.from({ length: 16 }).map((_, i) => (
                                    <div key={i} className="w-1.5 h-1.5 rounded-full bg-primary/50" />
                                ))}
                            </div>
                        </div>
                    </div>
                </div>

                {/* Bottom gradient blend */}
                <div className="absolute bottom-0 left-0 right-0 h-32 pointer-events-none"
                    style={{ background: "linear-gradient(to top, var(--bg-primary), transparent)" }}
                />
            </section>

            {/* ======= HOW IT WORKS ======= */}
            <section className="py-28 px-6 relative">
                <div className="max-w-6xl mx-auto">
                    <div className="text-center mb-16">
                        <div className="section-label mx-auto">How it works</div>
                        <h2 className="hero-text text-4xl md:text-5xl mt-2">
                            Integrate in minutes,<br />
                            <span className="text-secondary font-light">not weeks.</span>
                        </h2>
                    </div>
                    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-5">
                        <StepCard n="01" title="Connect your channels" desc="Add your SMTP, Twilio, Slack webhook, or FCM credentials in the dashboard settings." />
                        <StepCard n="02" title="Design a template" desc="Use our visual MJML editor or write plain text. Set variables with Handlebars syntax." />
                        <StepCard n="03" title="Call our single API" desc="POST to /api/notify with your API key, template ID, and dynamic data. That's it." />
                        <StepCard n="04" title="Track delivery in real-time" desc="Monitor every delivery, open, and failure from our unified statistics dashboard." />
                    </div>
                </div>
            </section>

            {/* ======= FEATURES ======= */}
            <section id="features" className="py-28 px-6 bg-secondary/20 border-y border-primary">
                <div className="max-w-7xl mx-auto">
                    <div className="text-center mb-16">
                        <div className="section-label mx-auto">Channels</div>
                        <h2 className="hero-text text-4xl md:text-5xl mt-2">
                            Omnichannel is<br />
                            <span className="gradient-text-blue">the default.</span>
                        </h2>
                        <p className="text-secondary text-lg max-w-xl mx-auto mt-4">
                            Stop rebuilding integrations from scratch. Our engine handles all the complexity.
                        </p>
                    </div>
                    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-5">
                        {features.map((f, i) => (
                            <FeatureCard key={i} {...f} />
                        ))}
                    </div>
                </div>
            </section>

            {/* ======= CODE SHOWCASE ======= */}
            <section className="py-28 px-6 relative overflow-hidden">
                <div className="max-w-7xl mx-auto flex flex-col lg:flex-row items-center gap-20">
                    {/* Left: Text */}
                    <div className="lg:w-1/2 space-y-6">
                        <div className="section-label">Developer First</div>
                        <h2 className="font-display font-bold text-4xl md:text-5xl leading-tight tracking-tight">
                            One endpoint.<br />
                            <span className="text-secondary font-light">All channels.</span>
                        </h2>
                        <p className="text-secondary text-lg leading-relaxed">
                            Our REST API is designed for clarity. Authenticate once, then send to Email, SMS, Slack,
                            and more with a single request body. Rate limits, retries, and delivery reports are included.
                        </p>
                        <ul className="space-y-3.5">
                            {[
                                "MJML-based responsive email engine",
                                "Handlebars variable interpolation",
                                "Automatic retries on failure",
                                "Signed webhook verification",
                                "Delivery status webhooks"
                            ].map(item => (
                                <li key={item} className="flex items-center gap-3 text-sm">
                                    <div className="w-5 h-5 rounded-full bg-accent-dim border border-blue-500/20 flex items-center justify-center text-accent-primary text-[10px]">✓</div>
                                    <span className="text-secondary">{item}</span>
                                </li>
                            ))}
                        </ul>
                        <button onClick={() => navigate("/api-reference")} className="btn-secondary mt-2">
                            View full API reference →
                        </button>
                    </div>
                    {/* Right: Code block */}
                    <div className="lg:w-1/2 w-full">
                        <div className="code-block p-0 overflow-hidden shadow-accent-lg">
                            {/* Window chrome */}
                            <div className="flex items-center gap-2 px-4 py-3 border-b border-primary bg-elevated">
                                <div className="flex gap-1.5">
                                    <div className="w-3 h-3 rounded-full bg-red-500/50" />
                                    <div className="w-3 h-3 rounded-full bg-yellow-500/50" />
                                    <div className="w-3 h-3 rounded-full bg-green-500/50" />
                                </div>
                                <span className="text-[11px] text-tertiary font-mono ml-2">api_integration.js</span>
                            </div>
                            <pre className="p-6 text-[13px] font-mono leading-loose overflow-x-auto">
                                {`<span class="code-token-comment">// Send a multi-channel notification</span>
<span class="code-token-key">const</span> response = <span class="code-token-key">await</span> <span class="code-token-func">fetch</span>(<span class="code-token-string">'https://api.fertile.shop/v1/notifications'</span>, {
  method: <span class="code-token-string">'POST'</span>,
  headers: {
    <span class="code-token-string">'Authorization'</span>: <span class="code-token-string">\`Bearer \${API_KEY}\`</span>,
    <span class="code-token-string">'Content-Type'</span>: <span class="code-token-string">'application/json'</span>,
  },
  body: <span class="code-token-func">JSON.stringify</span>({
    eventType: <span class="code-token-string">'ORDER_SHIPPED'</span>,
    parameters: {
      customer_name: <span class="code-token-string">'Enes'</span>,
      order_id: <span class="code-token-string">'#FL-2024'</span>,
      tracking_url: <span class="code-token-string">'https://track.it/FL-2024'</span>
    },
    to: [
      {
        channel: <span class="code-token-string">'email'</span>,
        recipients: [<span class="code-token-string">'enes@example.com'</span>]
      },
      {
        channel: <span class="code-token-string">'sms'</span>,
        recipients: [<span class="code-token-string">'+905550000000'</span>]
      }
    ]
  }),
});`}
                            </pre>
                        </div>
                        {/* Response badge */}
                        <div className="flex items-center gap-2 mt-3 px-1">
                            <span className="w-2 h-2 rounded-full bg-green-500 animate-pulse" />
                            <span className="text-xs text-green-400 font-mono">200 OK · 3 channels delivered · 94ms</span>
                        </div>
                    </div>
                </div>
            </section>

            {/* ======= PRICING TEASER ======= */}
            <section className="py-28 px-6 border-y border-primary bg-secondary/10">
                <div className="max-w-5xl mx-auto text-center">
                    <div className="section-label mx-auto">Pricing</div>
                    <h2 className="hero-text text-4xl md:text-5xl mt-2 mb-4">
                        Simple, predictable<br />
                        <span className="text-secondary font-light">pricing.</span>
                    </h2>
                    <p className="text-secondary text-lg max-w-lg mx-auto mb-14">
                        Start free. Scale as you grow. No surprise bills.
                    </p>
                    <div className="grid grid-cols-1 md:grid-cols-3 gap-5 text-left">
                        {[
                            {
                                plan: "Free", price: "$0", period: "/ month",
                                desc: "For individuals and side projects.",
                                features: ["2,000 console notifications", "150 emails / month", "1 Slack & Discord channel"],
                                cta: "Start for free", primary: false
                            },
                            {
                                plan: "Pro", price: "$29", period: "/ month",
                                desc: "For growing teams with real users.",
                                features: ["10,000 console notifications", "500 emails / month", "250 SMS / month", "Unlimited Slack & Discord"],
                                cta: "Get Pro", primary: true
                            },
                            {
                                plan: "Enterprise", price: "Custom", period: "",
                                desc: "For large-scale deployments.",
                                features: ["Unlimited notifications", "Custom email limits", "1,000+ SMS / month", "24/7 priority support"],
                                cta: "Contact sales", primary: false
                            }
                        ].map(tier => (
                            <div key={tier.plan} className={`card p-7 flex flex-col gap-5 ${tier.primary ? "border-accent-primary/30 shadow-accent" : ""}`}>
                                <div>
                                    <p className="text-xs font-bold uppercase tracking-widest text-tertiary mb-2">{tier.plan}</p>
                                    <div className="flex items-end gap-1">
                                        <span className="font-display font-bold text-4xl text-primary">{tier.price}</span>
                                        {tier.period && <span className="text-secondary text-sm mb-1.5">{tier.period}</span>}
                                    </div>
                                    <p className="text-sm text-secondary mt-1.5">{tier.desc}</p>
                                </div>
                                <ul className="space-y-2.5 flex-1">
                                    {tier.features.map(f => (
                                        <li key={f} className="flex items-center gap-2.5 text-sm text-secondary">
                                            <svg className="w-4 h-4 text-green-500 shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
                                            </svg>
                                            {f}
                                        </li>
                                    ))}
                                </ul>
                                <button
                                    onClick={() => navigate(tier.plan === "Enterprise" ? "/contact" : "/register")}
                                    className={tier.primary ? "btn-primary w-full py-3" : "btn-secondary w-full py-3"}
                                >
                                    {tier.cta}
                                </button>
                            </div>
                        ))}
                    </div>
                    <p className="text-xs text-tertiary mt-6">
                        All plans include API access, real-time stats, and delivery logs.{" "}
                        <button onClick={() => navigate("/pricing")} className="text-accent-primary hover:underline">
                            See full comparison →
                        </button>
                    </p>
                </div>
            </section>

            {/* ======= CTA BANNER ======= */}
            <section className="py-32 px-6 relative overflow-hidden">
                <div className="absolute inset-0 glow-overlay-center" />
                <div className="max-w-3xl mx-auto text-center relative z-10">
                    <h2 className="hero-text text-4xl md:text-6xl mb-5">
                        Start sending<br />
                        <span className="gradient-text">in under 5 minutes.</span>
                    </h2>
                    <p className="text-secondary text-lg mb-10 max-w-lg mx-auto">
                        Join hundreds of developers who trust Fertile Notify to power their customer communication.
                        No complex setup. No hidden fees.
                    </p>
                    <div className="flex flex-col sm:flex-row gap-3.5 justify-center">
                        <button
                            onClick={() => navigate(isAuthenticated ? "/dashboard" : "/register")}
                            className="btn-primary px-9 py-3.5 text-[15px] shadow-accent-lg"
                        >
                            {isAuthenticated ? "Open Dashboard" : "Create free account"}
                        </button>
                    </div>
                </div>
            </section>

            {/* ======= FOOTER ======= */}
            <footer className="border-t border-primary py-16 px-6">
                <div className="max-w-7xl mx-auto">
                    <div className="flex flex-col md:flex-row justify-between gap-12">
                        {/* Brand */}
                        <div className="md:w-56">
                            <div className="flex items-center gap-2 mb-4">
                                <div className="w-6 h-6 bg-white rounded flex items-center justify-center">
                                    <span className="text-black font-bold text-[10px] font-mono">FN</span>
                                </div>
                                <span className="font-display font-bold text-sm">
                                    fertile<span className="text-accent-primary">notify</span>
                                </span>
                            </div>
                            <p className="text-xs text-tertiary leading-relaxed">
                                Unified notification infrastructure for modern development teams.
                            </p>
                        </div>

                        {/* Links */}
                        <div className="grid grid-cols-2 md:grid-cols-2 gap-10">
                            <div>
                                <p className="text-[10px] font-bold uppercase tracking-widest text-muted mb-4">Product</p>
                                <div className="flex flex-col gap-2.5">
                                    {[
                                        { label: "Features", action: () => document.getElementById("features")?.scrollIntoView({ behavior: "smooth" }) },
                                        { label: "API Reference", action: () => navigate("/api-reference") },
                                        { label: "Changelog", action: () => navigate("/changelog") },
                                        { label: "Documentation", action: () => navigate("/documentation") },
                                    ].map(l => (
                                        <button key={l.label} onClick={l.action} className="text-left text-sm text-tertiary hover:text-primary transition-colors">
                                            {l.label}
                                        </button>
                                    ))}
                                </div>
                            </div>
                            <div>
                                <p className="text-[10px] font-bold uppercase tracking-widest text-muted mb-4">Company</p>
                                <div className="flex flex-col gap-2.5">
                                    {[
                                        { label: "About Us", action: () => navigate("/about") },
                                        { label: "Contact", action: () => navigate("/contact") },
                                        { label: "Privacy", action: () => navigate("/privacy") },
                                    ].map(l => (
                                        <button key={l.label} onClick={l.action} className="text-left text-sm text-tertiary hover:text-primary transition-colors">
                                            {l.label}
                                        </button>
                                    ))}
                                </div>
                            </div>
                        </div>
                    </div>

                    {/* Bottom bar */}
                    <div className="mt-12 pt-8 border-t border-primary flex flex-col md:flex-row justify-between items-center gap-4">
                        <p className="text-[11px] text-muted">
                            © {new Date().getFullYear()} Fertile Notify. All rights reserved.
                        </p>
                        <div className="flex items-center gap-1">
                            <span className="w-1.5 h-1.5 rounded-full bg-green-500 animate-pulse" />
                            <span className="text-[11px] text-tertiary">All systems operational</span>
                        </div>
                    </div>
                </div>
            </footer>
        </div>
    );
}
