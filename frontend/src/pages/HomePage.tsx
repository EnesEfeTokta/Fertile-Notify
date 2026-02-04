import { useNavigate } from "react-router-dom";

export default function HomePage() {
    const navigate = useNavigate();

    return (
        <div className="min-h-screen bg-animated-gradient flex flex-col items-center justify-center px-4 animate-fade-in">
            <div className="max-w-4xl mx-auto text-center">
                {/* Hero Title */}
                <h1 className="text-6xl md:text-8xl font-display font-bold text-white mb-6 animate-slide-up text-shadow-lg tracking-tight">
                    FERTILE-NOTIFY
                </h1>

                {/* Subtitle */}
                <div className="inline-block mb-4 px-6 py-2 border-neon clip-sharp-sm animate-slide-up" style={{ animationDelay: '0.1s' }}>
                    <p className="text-xl md:text-2xl text-purple-300 font-semibold">
                        Modern Notification Service
                    </p>
                </div>

                <p className="text-lg text-gray-300 mb-12 max-w-2xl mx-auto animate-slide-up" style={{ animationDelay: '0.2s' }}>
                    Powerful and flexible notification management for your business. Reach your users with Email, SMS, and more.
                </p>

                {/* CTA Buttons */}
                <div className="flex flex-col sm:flex-row gap-4 justify-center items-center animate-slide-up mb-20" style={{ animationDelay: '0.3s' }}>
                    <button
                        className="btn-gradient text-lg min-w-[200px] uppercase tracking-wider"
                        onClick={() => navigate("/register")}
                    >
                        Get Started
                    </button>
                    <button
                        className="px-8 py-3 font-semibold text-white border-2 border-purple-500 hover:border-pink-500 transition-all duration-300 min-w-[200px] clip-sharp-sm hover:shadow-neon uppercase tracking-wider"
                        onClick={() => navigate("/login")}
                    >
                        Login
                    </button>
                </div>

                {/* Features */}
                <div className="grid grid-cols-1 md:grid-cols-3 gap-6 animate-slide-up" style={{ animationDelay: '0.4s' }}>
                    <div className="glass p-6 clip-sharp border-l-4 border-purple-500 hover:border-pink-500 transition-all">
                        <div className="text-4xl mb-3">📧</div>
                        <h3 className="text-xl font-bold text-purple-300 mb-2 uppercase tracking-wide">Email Notifications</h3>
                        <p className="text-gray-400">Stay in touch with your users through professional email notifications.</p>
                    </div>
                    <div className="glass p-6 clip-sharp border-l-4 border-cyan-500 hover:border-pink-500 transition-all">
                        <div className="text-4xl mb-3">💬</div>
                        <h3 className="text-xl font-bold text-cyan-300 mb-2 uppercase tracking-wide">SMS Support</h3>
                        <p className="text-gray-400">Use SMS notification channel for instant reach.</p>
                    </div>
                    <div className="glass p-6 clip-sharp border-l-4 border-pink-500 hover:border-purple-500 transition-all">
                        <div className="text-4xl mb-3">⚡</div>
                        <h3 className="text-xl font-bold text-pink-300 mb-2 uppercase tracking-wide">Fast & Reliable</h3>
                        <p className="text-gray-400">Fast and reliable notification delivery with modern infrastructure.</p>
                    </div>
                </div>
            </div>
        </div>
    );
}