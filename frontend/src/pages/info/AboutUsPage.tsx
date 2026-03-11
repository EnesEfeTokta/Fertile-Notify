import { InfoPageLayout, Section } from "./InfoPageLayout";

export default function AboutUsPage() {
    return (
        <InfoPageLayout
            badge="Our Story"
            title="About Fertile Notify"
            subtitle="We're building the notification infrastructure that developers actually want to use."
        >
            <Section title="Our Mission">
                <p>
                    Fertile Notify was born from frustration. Building notification systems is tedious, repetitive,
                    and pulls engineering time away from what matters — your core product. We set out to fix that.
                </p>
                <p>
                    Our goal is simple: give development teams a unified, reliable, and beautiful API to reach their
                    users across every channel — without the boilerplate, without the complexity, and without
                    rebuilding integrations from scratch every time.
                </p>
            </Section>

            <Section title="What We Believe">
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mt-2">
                    {[
                        { icon: "🎯", title: "Developer Experience First", desc: "Our API is designed for clarity and simplicity. If it takes more than 5 minutes to get started, we've failed." },
                        { icon: "🔒", title: "Reliability by Default", desc: "Automatic retries, delivery tracking, and a 99.9% uptime SLA aren't premium features — they're the baseline." },
                        { icon: "🌍", title: "Omnichannel as Standard", desc: "Email, SMS, Slack, Push, Webhooks — all from one endpoint. Fragmented integrations shouldn't be the default." },
                        { icon: "📖", title: "Transparent Pricing", desc: "No surprise bills. No line-item fees. Start free and upgrade only when you need to." },
                    ].map(v => (
                        <div key={v.title} className="card p-5">
                            <div className="text-2xl mb-3">{v.icon}</div>
                            <h3 className="text-sm font-bold text-primary mb-1.5">{v.title}</h3>
                            <p className="text-sm text-secondary leading-relaxed">{v.desc}</p>
                        </div>
                    ))}
                </div>
            </Section>

            <Section title="Built by Developers">
                <p>
                    Fertile Notify is built by a small team of developers who have spent years in the trenches
                    of notification hell. We've seen the fragmented libraries, the inconsistent APIs, and
                    the late nights debugging why an email didn't send.
                </p>
                <p>
                    We're building the tool we always wished existed. If you have suggestions, feedback,
                    or just want to say hi, we'd love to hear from you.
                </p>
                <div className="mt-4">
                    <a href="/contact" className="btn-secondary inline-flex">
                        Get in touch →
                    </a>
                </div>
            </Section>
        </InfoPageLayout>
    );
}
