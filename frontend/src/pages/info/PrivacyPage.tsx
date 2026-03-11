import { InfoPageLayout, Section } from "./InfoPageLayout";

export default function PrivacyPage() {
    return (
        <InfoPageLayout
            badge="Legal"
            title="Privacy Policy"
            subtitle="We take privacy seriously. Here's exactly how we collect, use, and protect your data."
        >
            <div className="text-[11px] text-tertiary mb-10 font-mono">Last updated: March 2025</div>

            <Section title="Information We Collect">
                <p>
                    We collect information you provide directly when you sign up, including your company name, email address,
                    and phone number. We also collect usage data such as API call logs, delivery events, and session metadata
                    to help you monitor your notifications and improve our service.
                </p>
                <p>
                    We do <strong className="text-primary">not</strong> sell your personal data to third parties.
                    We do not use your customer data to train machine learning models.
                </p>
            </Section>

            <Section title="How We Use Your Data">
                <ul className="space-y-2">
                    {[
                        "To authenticate your account and protect your sessions",
                        "To process and deliver your notifications across channels",
                        "To provide usage statistics and delivery analytics",
                        "To send transactional service emails (billing receipts, alerts)",
                        "To improve reliability and diagnose errors in our infrastructure",
                    ].map(item => (
                        <li key={item} className="flex items-start gap-2 text-sm">
                            <span className="text-accent-primary mt-0.5">→</span>
                            {item}
                        </li>
                    ))}
                </ul>
            </Section>

            <Section title="Data Storage & Security">
                <p>
                    Your data is encrypted at rest using AES-256 and in transit using TLS 1.3. API keys are
                    one-way hashed and never stored in plaintext. We use industry-standard security practices
                    throughout our infrastructure.
                </p>
            </Section>

            <Section title="Data Retention">
                <p>
                    Notification logs are retained based on your plan (Free: 7 days, Pro: 90 days, Enterprise:
                    custom). Account data is retained for the lifetime of your account and deleted within 30 days
                    of account closure upon request.
                </p>
            </Section>

            <Section title="Your Rights">
                <p>
                    You have the right to access, correct, export, or delete your personal data at any time.
                    To exercise these rights, contact us at{" "}
                    <a href="mailto:privacy@fertile-notify.shop" className="text-accent-primary hover:underline">
                        privacy@fertile-notify.shop
                    </a>.
                </p>
            </Section>

            <Section title="Contact">
                <p>
                    For privacy questions or data requests, reach us at{" "}
                    <a href="mailto:privacy@fertile-notify.shop" className="text-accent-primary hover:underline">
                        privacy@fertile-notify.shop
                    </a>{" "}
                    or through our{" "}
                    <a href="/contact" className="text-accent-primary hover:underline">contact page</a>.
                </p>
            </Section>
        </InfoPageLayout>
    );
}
