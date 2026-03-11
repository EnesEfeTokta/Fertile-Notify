import { useNavigate } from "react-router-dom";

const PricingPlanPage = () => {
    const navigate = useNavigate();

    return (
        <div className="min-h-screen bg-primary text-primary flex flex-col items-center py-20 px-4 animate-fade-in relative">
            {/* Back Button */}
            <button
                onClick={() => navigate("/")}
                className="absolute top-8 left-8 text-secondary hover:text-primary transition-smooth flex items-center gap-2 text-sm"
            >
                <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M10 19l-7-7m0 0l7-7m-7 7h18" />
                </svg>
                Back to Home
            </button>

            <div className="max-w-6xl w-full space-y-16">
                {/* Header */}
                <div className="text-center space-y-4">
                    <h1 className="text-4xl md:text-6xl font-display font-bold tracking-tight text-primary">
                        Pricing Plans
                    </h1>
                    <p className="text-lg text-secondary max-w-2xl mx-auto">
                        Simple, transparent pricing for teams of all sizes.
                        Choose the plan that fits your notification needs.
                    </p>
                </div>

                {/* Pricing Cards Container */}
                <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
                    {/* Free Plan */}
                    <div className="card p-8 flex flex-col hover:border-hover transition-smooth">
                        <div className="mb-8">
                            <h3 className="text-xl font-semibold mb-2 text-primary">Free</h3>
                            <p className="text-secondary text-sm h-10">For Simple / Test Projects / Students</p>
                            <div className="mt-6 flex items-baseline">
                                <span className="text-5xl font-bold tracking-tight text-primary">$0</span>
                                <span className="text-secondary ml-1">/mo</span>
                            </div>
                        </div>

                        <ul className="space-y-4 mb-10 flex-1 border-t border-primary pt-8">
                            <li className="flex items-start gap-3 text-sm text-secondary">
                                <svg className="w-5 h-5 text-primary-500 shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
                                </svg>
                                <span>2,000 console notifications / mo</span>
                            </li>
                            <li className="flex items-start gap-3 text-sm text-secondary">
                                <svg className="w-5 h-5 text-primary-500 shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
                                </svg>
                                <span>150 email notifications / mo</span>
                            </li>
                            <li className="flex items-start gap-3 text-sm text-secondary">
                                <svg className="w-5 h-5 text-primary-500 shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
                                </svg>
                                <span>Detailed statistics (Daily & Weekly)</span>
                            </li>
                            <li className="flex items-start gap-3 text-sm text-secondary">
                                <svg className="w-5 h-5 text-primary-500 shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
                                </svg>
                                <span>20 requests per minute limit</span>
                            </li>
                        </ul>

                        <button className="btn-secondary w-full text-sm">
                            Get Started
                        </button>
                    </div>

                    {/* Pro Plan */}
                    <div className="card p-8 flex flex-col border-primary-500 relative bg-secondary scale-105 shadow-xl z-10">
                        <div className="absolute top-0 right-0 bg-primary-500 text-white px-3 py-1 rounded-bl-md text-[10px] font-bold uppercase tracking-widest">
                            Most Productive
                        </div>
                        <div className="mb-8">
                            <h3 className="text-xl font-semibold mb-2 text-primary">Pro</h3>
                            <p className="text-secondary text-sm h-10">Businesses (Small / Medium)</p>
                            <div className="mt-6 flex items-baseline">
                                <span className="text-5xl font-bold tracking-tight text-primary">$9,99</span>
                                <span className="text-secondary ml-1">/mo</span>
                            </div>
                        </div>

                        <ul className="space-y-4 mb-10 flex-1 border-t border-primary pt-8">
                            <li className="flex items-start gap-3 text-sm text-secondary">
                                <svg className="w-5 h-5 text-primary-500 shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
                                </svg>
                                <span>10,000 console notifications / mo</span>
                            </li>
                            <li className="flex items-start gap-3 text-sm text-secondary">
                                <svg className="w-5 h-5 text-primary-500 shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
                                </svg>
                                <span>500 email notifications / mo</span>
                            </li>
                            <li className="flex items-start gap-3 text-sm text-secondary">
                                <svg className="w-5 h-5 text-primary-500 shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
                                </svg>
                                <span>250 SMS limit per month</span>
                            </li>
                            <li className="flex items-start gap-3 text-sm text-secondary">
                                <svg className="w-5 h-5 text-primary-500 shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
                                </svg>
                                <span>Stats (Daily, Weekly, Monthly, 3M)</span>
                            </li>
                            <li className="flex items-start gap-3 text-sm text-secondary">
                                <svg className="w-5 h-5 text-primary-500 shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
                                </svg>
                                <span>100 requests per minute limit</span>
                            </li>
                        </ul>

                        <button className="btn-primary w-full text-sm">
                            Upgrade to Pro
                        </button>
                    </div>

                    {/* Enterprise Plan */}
                    <div className="card p-8 flex flex-col hover:border-hover transition-smooth">
                        <div className="mb-8">
                            <h3 className="text-xl font-semibold mb-2 text-primary">Enterprise</h3>
                            <p className="text-secondary text-sm h-10">For Serious Institutions</p>
                            <div className="mt-6 flex items-baseline">
                                <span className="text-4xl font-bold tracking-tight text-primary">Custom</span>
                            </div>
                        </div>

                        <ul className="space-y-4 mb-10 flex-1 border-t border-primary pt-8">
                            <li className="flex items-start gap-3 text-sm text-secondary">
                                <svg className="w-5 h-5 text-primary-500 shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
                                </svg>
                                <span>Unlimited console notifications</span>
                            </li>
                            <li className="flex items-start gap-3 text-sm text-secondary">
                                <svg className="w-5 h-5 text-primary-500 shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
                                </svg>
                                <span>5,000 email notifications / mo</span>
                            </li>
                            <li className="flex items-start gap-3 text-sm text-secondary">
                                <svg className="w-5 h-5 text-primary-500 shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
                                </svg>
                                <span>1,000 SMS limit per month</span>
                            </li>
                            <li className="flex items-start gap-3 text-sm text-secondary">
                                <svg className="w-5 h-5 text-primary-500 shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
                                </svg>
                                <span>Stats (Up to 1Y & All-time)</span>
                            </li>
                            <li className="flex items-start gap-3 text-sm text-secondary">
                                <svg className="w-5 h-5 text-primary-500 shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
                                </svg>
                                <span>1,000 requests / minute</span>
                            </li>
                            <li className="flex items-start gap-3 text-sm text-secondary">
                                <svg className="w-5 h-5 text-primary-500 shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
                                </svg>
                                <span className="font-medium text-primary-400">Priority technical support</span>
                            </li>
                        </ul>

                        <button className="btn-secondary w-full text-sm">
                            Contact Us
                        </button>
                    </div>
                </div>

                {/* Comparison Table Section */}
                <div className="pt-20 space-y-12">
                    <div className="text-center">
                        <h2 className="text-3xl font-display font-semibold text-primary">Compare Features</h2>
                        <p className="text-secondary mt-2">Find the perfect balance for your growth.</p>
                    </div>

                    <div className="overflow-x-auto">
                        <table className="w-full text-left border-collapse border-t border-primary/20">
                            <thead>
                                <tr className="border-b border-primary/20">
                                    <th className="py-6 px-4 text-sm font-semibold text-tertiary uppercase tracking-wider w-1/4">Capability</th>
                                    <th className="py-6 px-4 text-lg font-bold text-primary w-1/4">Free</th>
                                    <th className="py-6 px-4 text-lg font-bold text-accent-light w-1/4">Pro</th>
                                    <th className="py-6 px-4 text-lg font-bold text-primary w-1/4">Enterprise</th>
                                </tr>
                            </thead>
                            <tbody className="divide-y divide-primary/10">
                                {/* Notifications */}
                                <tr>
                                    <td className="py-5 px-4 text-sm font-medium text-primary">Console Notifications</td>
                                    <td className="py-5 px-4 text-sm text-secondary">2,000 / mo</td>
                                    <td className="py-5 px-4 text-sm text-secondary">10,000 / mo</td>
                                    <td className="py-5 px-4 text-sm text-secondary font-semibold">Unlimited</td>
                                </tr>
                                <tr>
                                    <td className="py-5 px-4 text-sm font-medium text-primary">Email Notifications</td>
                                    <td className="py-5 px-4 text-sm text-secondary">150 / mo</td>
                                    <td className="py-5 px-4 text-sm text-secondary">500 / mo</td>
                                    <td className="py-5 px-4 text-sm text-secondary">5,000 / mo</td>
                                </tr>
                                <tr>
                                    <td className="py-5 px-4 text-sm font-medium text-primary">SMS Support</td>
                                    <td className="py-5 px-4 text-sm text-secondary">---</td>
                                    <td className="py-5 px-4 text-sm text-secondary">250 / mo</td>
                                    <td className="py-5 px-4 text-sm text-secondary">1,000 / mo</td>
                                </tr>
                                {/* Channels */}
                                <tr>
                                    <td className="py-5 px-4 text-sm font-medium text-primary">Slack & Discord</td>
                                    <td className="py-5 px-4 text-sm text-secondary">Limited (1 ea)</td>
                                    <td className="py-5 px-4 text-sm text-secondary">Unlimited</td>
                                    <td className="py-5 px-4 text-sm text-secondary font-semibold">Unlimited</td>
                                </tr>
                                <tr>
                                    <td className="py-5 px-4 text-sm font-medium text-primary">Push Notifications</td>
                                    <td className="py-5 px-4 text-sm text-secondary">---</td>
                                    <td className="py-5 px-4 text-sm text-secondary font-semibold text-accent-light">✓ Included</td>
                                    <td className="py-5 px-4 text-sm text-secondary font-semibold text-primary">✓ Advanced</td>
                                </tr>
                                {/* Platform */}
                                <tr>
                                    <td className="py-5 px-4 text-sm font-medium text-primary">Stat Retention</td>
                                    <td className="py-5 px-4 text-sm text-secondary">7 Days</td>
                                    <td className="py-5 px-4 text-sm text-secondary">3 Months</td>
                                    <td className="py-5 px-4 text-sm text-secondary font-semibold">History Export</td>
                                </tr>
                                <tr>
                                    <td className="py-5 px-4 text-sm font-medium text-primary">Rate Limits</td>
                                    <td className="py-5 px-4 text-sm text-secondary">20 req / min</td>
                                    <td className="py-5 px-4 text-sm text-secondary">100 req / min</td>
                                    <td className="py-5 px-4 text-sm text-secondary">1,000+ req / min</td>
                                </tr>
                                <tr>
                                    <td className="py-5 px-4 text-sm font-medium text-primary">Support</td>
                                    <td className="py-5 px-4 text-sm text-secondary">Documentation</td>
                                    <td className="py-5 px-4 text-sm text-secondary">Email Support</td>
                                    <td className="py-5 px-4 text-sm text-secondary font-semibold text-accent-primary">24/7 Priority</td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>

                {/* FAQ Section */}
                <div className="pt-20 space-y-10">
                    <h2 className="text-3xl font-display font-semibold text-center text-primary">Frequently Asked Questions</h2>
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-6 max-w-4xl mx-auto">
                        <div className="space-y-2">
                            <h4 className="font-semibold text-primary">What happens after my free trial ends?</h4>
                            <p className="text-secondary text-sm leading-relaxed">
                                After your 14-day free trial, your account will be automatically downgraded to the Free plan
                                unless you upgrade. You'll keep access to your history but limits will apply.
                            </p>
                        </div>
                        <div className="space-y-2">
                            <h4 className="font-semibold text-primary">Can I change my plan later?</h4>
                            <p className="text-secondary text-sm leading-relaxed">
                                Yes, you can upgrade or downgrade at any time. Billing changes take effect immediately
                                for upgrades and at the next cycle for downgrades.
                            </p>
                        </div>
                        <div className="space-y-2">
                            <h4 className="font-semibold text-primary">Do you offer refunds?</h4>
                            <p className="text-secondary text-sm leading-relaxed">
                                We offer a 30-day money-back guarantee for all paid plans. No questions asked.
                            </p>
                        </div>
                        <div className="space-y-2">
                            <h4 className="font-semibold text-primary">Is there a setup fee?</h4>
                            <p className="text-secondary text-sm leading-relaxed">
                                No, there are no setup fees or hidden charges. You only pay for your selected plan.
                            </p>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    )
}

export default PricingPlanPage;