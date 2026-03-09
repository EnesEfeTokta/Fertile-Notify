import { useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";

const FeatureCard = ({ icon, title, description }: { icon: string, title: string, description: string }) => (
    <div className="card p-8 group hover-reveal-border transition-smooth relative overflow-hidden h-full">
        <div className="text-4xl mb-6 relative z-10">{icon}</div>
        <h3 className="text-xl font-display font-bold text-primary mb-3 relative z-10 group-hover:text-accent-light transition-colors">{title}</h3>
        <p className="text-secondary leading-relaxed relative z-10">{description}</p>
        <div className="absolute top-0 right-0 p-4 opacity-5 group-hover:opacity-10 transition-opacity">
            <div className="text-6xl">{icon}</div>
        </div>
    </div>
);

const NavLink = ({ label, href, onClick }: { label: string, href?: string, onClick?: () => void }) => (
    <a
        href={href || "#"}
        onClick={(e) => {
            if (onClick) {
                e.preventDefault();
                onClick();
            }
        }}
        className="text-sm font-medium text-secondary hover:text-primary transition-colors py-1 cursor-pointer"
    >
        {label}
    </a>
);

export default function HomePage() {
    const navigate = useNavigate();
    const [scrolled, setScrolled] = useState(false);

    useEffect(() => {
        const handleScroll = () => setScrolled(window.scrollY > 20);
        window.addEventListener('scroll', handleScroll);
        return () => window.removeEventListener('scroll', handleScroll);
    }, []);

    const channels = [
        { icon: "📧", title: "Email Service", description: "Deliver professional emails using our visual MJML editor or raw HTML API." },
        { icon: "💬", title: "SMS Messaging", description: "Reach users instantly with lightning-fast SMS delivery across global networks." },
        { icon: "🔵", title: "Slack & Discord", description: "Native integrations for modern team collaboration and real-time alerts." },
        { icon: "📱", title: "Push Notifications", description: "Keep engagement high with Web and Mobile push notifications (FCM supported)." },
        { icon: "📬", title: "Telegram & WhatsApp", description: "Connect with your users on their favorite messaging platforms seamlessly." },
        { icon: "⚡", title: "Webhooks", description: "The ultimate flexibility. Send event data to any endpoint your system needs." }
    ];

    return (
        <div className="min-h-screen bg-primary text-primary selection:bg-accent-primary selection:text-white">
            <div className="fixed inset-0 bg-grid z-0 opacity-40 pointer-events-none"></div>
            <div className="fixed inset-0 glow-overlay z-0"></div>

            {/* Navigation */}
            <nav className={`fixed top-0 left-0 right-0 z-50 transition-all duration-300 ${scrolled ? 'bg-primary/80 backdrop-blur-md border-b border-primary py-3' : 'bg-transparent py-6'}`}>
                <div className="max-w-7xl mx-auto px-6 flex justify-between items-center">
                    <div className="flex items-center space-x-12">
                        <div className="flex items-center space-x-2 cursor-pointer" onClick={() => window.scrollTo({ top: 0, behavior: 'smooth' })}>
                            <div className="w-8 h-8 bg-white rounded-lg flex items-center justify-center font-bold text-black border-2 border-primary">F</div>
                            <span className="font-display font-bold tracking-tight text-xl">FERTILE <span className="text-accent-primary">NOTIFY</span></span>
                        </div>
                        <div className="hidden md:flex space-x-6">
                            <NavLink label="Features" onClick={() => document.getElementById('features')?.scrollIntoView({ behavior: 'smooth' })} />
                            <NavLink label="Pricing" onClick={() => navigate("/pricing")} />
                            <NavLink label="Templates" onClick={() => navigate("/templates")} />
                            <NavLink label="Statistics" onClick={() => navigate("/statistics")} />
                        </div>
                    </div>
                    <div className="flex items-center space-x-4">
                        <NavLink label="Sign In" onClick={() => navigate("/login")} />
                        <button className="btn-primary text-xs font-semibold py-2 px-6" onClick={() => navigate("/register")}>
                            Get Started
                        </button>
                    </div>
                </div>
            </nav>

            {/* Hero Section */}
            <section className="relative pt-40 pb-32 px-6">
                <div className="max-w-5xl mx-auto text-center relative z-10">
                    <div className="section-label animate-fade-in mx-auto">
                        <span className="w-2 h-2 rounded-full bg-accent-primary animate-pulse"></span>
                        <span>Now in Public Beta</span>
                    </div>

                    <h1 className="hero-text mb-8 animate-fade-in-up">
                        NOTIFY YOUR <br />
                        <span className="gradient-text">USERS ANYWHERE.</span>
                    </h1>

                    <p className="text-xl text-secondary mb-12 max-w-2xl mx-auto leading-relaxed animate-fade-in-up delay-200">
                        Fertile-Notify is a unified notification engine for modern businesses.
                        One API to reach them all: Email, SMS, Slack, Discord, and more.
                    </p>

                    <div className="flex flex-col sm:flex-row gap-4 justify-center items-center animate-fade-in-up delay-300">
                        <button
                            className="btn-primary py-4 px-10 text-base font-bold shadow-lg shadow-accent-primary/20"
                            onClick={() => navigate("/register")}
                        >
                            Start Building Now
                        </button>
                        <button
                            className="btn-secondary py-4 px-10 text-base font-bold"
                            onClick={() => navigate("/dashboard")}
                        >
                            Explore Dashboard
                        </button>
                    </div>
                </div>
            </section>

            {/* Features Grid */}
            <section id="features" className="relative py-32 px-6 bg-secondary/30">
                <div className="max-w-7xl mx-auto">
                    <div className="text-center mb-20">
                        <h2 className="text-4xl md:text-5xl font-display font-bold mb-6">Omnichannel is the default.</h2>
                        <p className="text-secondary text-lg max-w-2xl mx-auto">
                            Don't waste time building separate integrations for every platform.
                            Our engine handles the complexity so you can focus on the message.
                        </p>
                    </div>

                    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
                        {channels.map((ch, idx) => (
                            <div key={idx} className={`animate-fade-in-up transition-delay-${idx * 100}`}>
                                <FeatureCard {...ch} />
                            </div>
                        ))}
                    </div>
                </div>
            </section>

            {/* Visual Editor Highlight */}
            <section className="relative py-32 px-6">
                <div className="max-w-7xl mx-auto flex flex-col lg:flex-row items-center gap-20">
                    <div className="lg:w-1/2">
                        <div className="section-label">Developer First</div>
                        <h2 className="text-4xl md:text-5xl font-display font-bold mb-6 leading-tight">
                            Design visually. <br />
                            <span className="text-secondary">Send programmatically.</span>
                        </h2>
                        <p className="text-secondary text-lg mb-8 leading-relaxed">
                            Use our visual MJML editor to build responsive emails that look great on every device.
                            Ready to send? Just one API call with your template ID and variables.
                        </p>
                        <div className="space-y-4">
                            {[
                                "MJML-based responsive engine",
                                "Variable interpolation (Handlebars)",
                                "Preview on Mobile & Desktop",
                                "One-click deployment"
                            ].map((item, i) => (
                                <div key={i} className="flex items-center space-x-3 text-sm font-medium">
                                    <div className="w-5 h-5 rounded-full bg-accent-primary/10 flex items-center justify-center text-accent-primary">✓</div>
                                    <span>{item}</span>
                                </div>
                            ))}
                        </div>
                    </div>
                    <div className="lg:w-1/2 relative">
                        <div className="card p-4 bg-tertiary border-secondary shadow-2xl skew-y-3 hover:skew-y-0 transition-all duration-500">
                            <div className="flex items-center space-x-2 border-b border-primary pb-3 mb-4">
                                <div className="flex space-x-1.5">
                                    <div className="w-3 h-3 rounded-full bg-red-500/50"></div>
                                    <div className="w-3 h-3 rounded-full bg-yellow-500/50"></div>
                                    <div className="w-3 h-3 rounded-full bg-green-500/50"></div>
                                </div>
                                <span className="text-xs text-tertiary font-mono">send_test_notification.js</span>
                            </div>
                            <pre className="text-sm font-mono text-accent-light leading-loose overflow-x-auto">
                                {`await fertile.notify({
    subscriber: "user_789",
    template: "welcome_email",
    data: {
        name: "John Doe",
        link: "https://fertile.shop"
    },
    channels: ["email", "push"]
});`}
                            </pre>
                        </div>
                        <div className="absolute -z-10 -bottom-10 -right-10 w-64 h-64 bg-accent-primary blur-[100px] opacity-20"></div>
                    </div>
                </div>
            </section>

            {/* CTA Final */}
            <section className="relative py-32 px-6">
                <div className="max-w-4xl mx-auto card p-12 md:p-20 text-center relative overflow-hidden bg-gradient-to-br from-tertiary via-secondary to-bg-primary">
                    <div className="relative z-10">
                        <h2 className="text-4xl md:text-5xl font-display font-bold mb-8">Ready to grow your engagement?</h2>
                        <button
                            className="btn-primary py-5 px-12 text-lg font-bold"
                            onClick={() => navigate("/register")}
                        >
                            Build Your First Notification
                        </button>
                    </div>
                    <div className="absolute top-0 right-0 w-96 h-96 bg-accent-primary blur-[120px] opacity-10 -mr-48 -mt-48"></div>
                </div>
            </section>

            {/* Footer */}
            <footer className="relative py-20 px-6 border-t border-primary">
                <div className="max-w-7xl mx-auto flex flex-col md:flex-row justify-between items-start gap-12">
                    <div className="md:w-1/3">
                        <div className="flex items-center space-x-2 mb-6">
                            <div className="w-8 h-8 bg-white rounded-lg flex items-center justify-center font-bold text-black">F</div>
                            <span className="font-display font-bold tracking-tight text-xl">FERTILE <span className="text-accent-primary">NOTIFY</span></span>
                        </div>
                        <p className="text-sm text-tertiary leading-relaxed">
                            Building the infrastructure for the next generation of customer communication.
                            Built for developers, loved by businesses.
                        </p>
                    </div>
                    <div className="grid grid-cols-2 lg:grid-cols-3 gap-12 md:w-2/3 lg:w-1/2">
                        <div className="space-y-4">
                            <h4 className="text-xs font-bold uppercase tracking-widest text-secondary">Product</h4>
                            <div className="flex flex-col space-y-2">
                                <NavLink label="Features" />
                                <NavLink label="API Reference" />
                                <NavLink label="Changelog" />
                                <NavLink label="Integrations" />
                            </div>
                        </div>
                        <div className="space-y-4">
                            <h4 className="text-xs font-bold uppercase tracking-widest text-secondary">Company</h4>
                            <div className="flex flex-col space-y-2">
                                <NavLink label="About Us" />
                                <NavLink label="Careers" />
                                <NavLink label="Contact" />
                                <NavLink label="Privacy" />
                            </div>
                        </div>
                        <div className="space-y-4">
                            <h4 className="text-xs font-bold uppercase tracking-widest text-secondary">Support</h4>
                            <div className="flex flex-col space-y-2">
                                <NavLink label="Documentation" />
                                <NavLink label="Guides" />
                                <NavLink label="Help Center" />
                                <NavLink label="API Status" />
                            </div>
                        </div>
                    </div>
                </div>
                <div className="max-w-7xl mx-auto mt-20 pt-8 border-t border-primary/30 flex flex-col md:flex-row justify-between items-center gap-4">
                    <p className="text-xs text-tertiary">
                        © {new Date().getFullYear()} Fertile Notify. All rights reserved.
                    </p>
                    <div className="flex space-x-6">
                        {["Twitter", "GitHub", "LinkedIn"].map((social) => (
                            <a key={social} href="#" className="text-xs text-tertiary hover:text-primary transition-colors">{social}</a>
                        ))}
                    </div>
                </div>
            </footer>
        </div>
    );
}
