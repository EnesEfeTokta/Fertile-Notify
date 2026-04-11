import React, { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { PRIVACY_POLICY, TERMS_OF_USE } from '../constants/legal';

interface LegalPageProps {
    type: 'privacy' | 'terms';
}

const LegalPage: React.FC<LegalPageProps> = ({ type }) => {
    const navigate = useNavigate();
    const data = type === 'privacy' ? PRIVACY_POLICY : TERMS_OF_USE;

    useEffect(() => {
        window.scrollTo(0, 0);
    }, [type]);

    return (
        <div className="min-h-screen bg-primary text-secondary selection:bg-accent-dim selection:text-accent-primary">
            {/* Background Decor */}
            <div className="fixed inset-0 overflow-hidden pointer-events-none">
                <div className="absolute top-0 left-1/2 -translate-x-1/2 w-full max-w-7xl h-full opacity-40">
                    <div className="absolute top-[-10%] right-[-10%] w-[60%] h-[60%] bg-blue-500/10 blur-[120px] rounded-full" />
                    <div className="absolute bottom-[-10%] left-[-10%] w-[60%] h-[60%] bg-purple-500/5 blur-[120px] rounded-full" />
                </div>
                <div className="absolute inset-0 bg-grid opacity-[0.03]" />
            </div>

            <div className="relative max-w-3xl mx-auto px-6 py-20">
                {/* Back Button */}
                <button 
                    onClick={() => navigate(-1)}
                    className="group mb-12 flex items-center gap-2 text-sm font-medium text-tertiary hover:text-primary transition-colors"
                >
                    <svg className="w-4 h-4 transition-transform group-hover:-translate-x-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
                    </svg>
                    Back to previous page
                </button>

                <header className="mb-16">
                    <div className="section-label">Legal Documentation</div>
                    <h1 className="hero-text text-5xl md:text-6xl mb-6">{data.title}</h1>
                    <p className="text-tertiary">Last Updated: {data.lastUpdated}</p>
                </header>

                <div className="space-y-12">
                    <section>
                        <p className="text-lg leading-relaxed text-secondary italic">
                            {data.introduction}
                        </p>
                    </section>

                    {data.sections.map((section) => (
                        <section key={section.id} className="space-y-4">
                            <h2 className="text-xl font-bold text-primary flex items-center gap-3">
                                <span className="text-accent-primary font-mono text-sm opacity-50">{section.id.toString().padStart(2, '0')}</span>
                                {section.title}
                            </h2>
                            {section.content && (
                                <p className="text-secondary leading-relaxed whitespace-pre-wrap">
                                    {section.content}
                                </p>
                            )}
                            {section.list && (
                                <ul className="space-y-3 pl-5">
                                    {section.list.map((item, i) => (
                                        <li key={i} className="flex gap-3 text-secondary leading-relaxed">
                                            <span className="shrink-0 w-1.5 h-1.5 rounded-full bg-accent-primary/40 mt-2.5" />
                                            {item}
                                        </li>
                                    ))}
                                </ul>
                            )}
                            {section.footer && (
                                <p className="text-tertiary text-sm italic pt-2">
                                    {section.footer}
                                </p>
                            )}
                        </section>
                    ))}
                </div>

                <footer className="mt-24 pt-12 border-t border-primary text-center">
                    <p className="text-sm text-tertiary">
                        © 2026 FertileNotify. All rights reserved.
                    </p>
                    <div className="flex justify-center gap-6 mt-4">
                        <button 
                            onClick={() => navigate(type === 'privacy' ? '/terms' : '/privacy')}
                            className="text-xs font-semibold uppercase tracking-wider text-accent-primary hover:text-accent-light transition-colors"
                        >
                            View {type === 'privacy' ? 'Terms of Use' : 'Privacy Policy'}
                        </button>
                    </div>
                </footer>
            </div>
        </div>
    );
};

export default LegalPage;
