import { InfoPageLayout, Section } from "./InfoPageLayout";

export default function DocumentationPage() {
    return (
        <InfoPageLayout
            badge="Docs"
            title="Documentation"
            subtitle="Everything you need to integrate Fertile Notify into your application in minutes."
        >
            <Section title="Quick Start">
                <p>
                    Get started with Fertile Notify in 3 steps. No complicated setup, no configuration files.
                </p>
                <div className="mt-4 space-y-4">
                    {[
                        {
                            step: "1", title: "Create an account & get your API key",
                            desc: 'Sign up at fertile-notify.shop, then go to Dashboard → API Keys and create your first key.'
                        },
                        {
                            step: "2", title: "Configure your notification channels",
                            desc: "In the Dashboard, enable which channels you want to use (Email, SMS, Slack, etc.) and enter your credentials via the settings icon on each channel card."
                        },
                        {
                            step: "3", title: "Make your first API call",
                            desc: "Call POST /api/notify with your API key in the Authorization header. See the API Reference for the full request body."
                        }
                    ].map(s => (
                        <div key={s.step} className="card p-5 flex gap-4">
                            <div className="how-step-number shrink-0">{s.step}</div>
                            <div>
                                <h3 className="text-sm font-bold text-primary mb-1">{s.title}</h3>
                                <p className="text-sm text-secondary leading-relaxed">{s.desc}</p>
                            </div>
                        </div>
                    ))}
                </div>
            </Section>

            <Section title="Authentication">
                <p>
                    All API requests require a Bearer token in the <code className="font-mono text-accent-light bg-elevated px-1.5 py-0.5 rounded text-sm">Authorization</code> header.
                    You can create and manage API keys from the Dashboard.
                </p>
                <div className="code-block mt-4 p-5 text-sm font-mono">
                    Authorization: Bearer fn_live_xxxxxxxxxxxxxxxxxxxx
                </div>
            </Section>

            <Section title="Sending Notifications">
                <p>
                    Send a notification by POSTing to <code className="font-mono text-accent-light bg-elevated px-1.5 py-0.5 rounded text-sm">/api/notify</code>.
                    You can target one or multiple channels in the same request.
                </p>
                <div className="code-block mt-4 p-5 text-sm font-mono whitespace-pre">
                    {`POST /api/notify
{
  "subscriber_id": "user_123",
  "template_id": "welcome-email",
  "channels": ["email", "sms"],
  "data": {
    "name": "Enes",
    "company": "Acme Corp"
  }
}`}
                </div>
            </Section>

            <Section title="Notification Channels">
                <div className="divide-y divide-primary">
                    {[
                        { id: "console", name: "Console", setup: "No setup required. Logs appear in the Dashboard Console modal." },
                        { id: "email", name: "Email", setup: "Requires SMTP credentials or API key (SendGrid, Mailgun, etc.)." },
                        { id: "sms", name: "SMS", setup: "Requires Twilio or compatible SMS provider credentials." },
                        { id: "slack", name: "Slack", setup: "Requires a Slack Incoming Webhook URL from your workspace settings." },
                        { id: "discord", name: "Discord", setup: "Requires a Discord webhook URL from your server's channel settings." },
                        { id: "push", name: "Push (FCM)", setup: "Requires a Firebase Server Key and the user device token." },
                    ].map(ch => (
                        <div key={ch.id} className="py-4 flex gap-4">
                            <code className="font-mono text-[11px] text-accent-primary bg-accent-dim px-2 py-0.5 rounded self-start shrink-0">{ch.id}</code>
                            <div>
                                <p className="text-sm font-bold text-primary">{ch.name}</p>
                                <p className="text-sm text-secondary mt-0.5">{ch.setup}</p>
                            </div>
                        </div>
                    ))}
                </div>
            </Section>
        </InfoPageLayout>
    );
}
