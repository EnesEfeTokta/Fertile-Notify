import { useNavigate } from "react-router-dom";

export default function HomePage() {
    const navigate = useNavigate();

    return (
        <div className="min-h-screen bg-primary flex flex-col items-center justify-center px-4 animate-fade-in">
            <div className="max-w-5xl mx-auto text-center">
                {/* Hero Title */}
                <h1 className="text-5xl md:text-7xl font-display font-bold text-primary mb-4 tracking-tight">
                    FERTILE-NOTIFY
                </h1>

                {/* Subtitle */}
                <div className="inline-block mb-3 px-4 py-1.5 border border-primary rounded-md">
                    <p className="text-base md:text-lg text-secondary font-medium">
                        Modern Notification Service
                    </p>
                </div>

                <p className="text-base text-secondary mb-10 max-w-2xl mx-auto leading-relaxed">
                    Powerful and flexible notification management for your business.
                    Reach your users with Email, SMS, and more.
                </p>

                {/* CTA Buttons */}
                <div className="flex flex-col sm:flex-row gap-3 justify-center items-center mb-20">
                    <button
                        className="btn-primary text-sm min-w-[160px]"
                        onClick={() => navigate("/register")}
                    >
                        Get Started
                    </button>
                    <button
                        className="btn-secondary text-sm min-w-[160px]"
                        onClick={() => navigate("/pricing")}
                    >
                        View Pricing
                    </button>
                    <button
                        className="btn-secondary text-sm min-w-[160px]"
                        onClick={() => navigate("/login")}
                    >
                        Sign In
                    </button>
                </div>

                {/* Features */}
                <div className="grid grid-cols-1 md:grid-cols-3 gap-4 max-w-4xl mx-auto">
                    <div className="card p-6 text-left hover:border-secondary transition-smooth">
                        <div className="text-3xl mb-3">📧</div>
                        <h3 className="text-base font-semibold text-primary mb-2">Email Notifications</h3>
                        <p className="text-sm text-secondary leading-relaxed">
                            Stay in touch with your users through professional email notifications.
                        </p>
                    </div>
                    <div className="card p-6 text-left hover:border-secondary transition-smooth">
                        <div className="text-3xl mb-3">💬</div>
                        <h3 className="text-base font-semibold text-primary mb-2">SMS Support</h3>
                        <p className="text-sm text-secondary leading-relaxed">
                            Use SMS notification channel for instant reach.
                        </p>
                    </div>
                    <div className="card p-6 text-left hover:border-secondary transition-smooth">
                        <div className="text-3xl mb-3">⚡</div>
                        <h3 className="text-base font-semibold text-primary mb-2">Fast & Reliable</h3>
                        <p className="text-sm text-secondary leading-relaxed">
                            Fast and reliable notification delivery with modern infrastructure.
                        </p>
                    </div>
                </div>
            </div>
        </div>
    );
}