import { useState, useCallback, useEffect } from "react";
import AppShell from "../../components/AppShell";
import { subscriberService } from "../../api/subscriberService";
import type { SubscriberProfile } from "../../types/subscriber";

type EndpointProps = {
    method: "GET" | "POST" | "PUT" | "DELETE" | "PATCH";
    path: string;
    desc: string;
    body?: string;
    response?: string;
};

const methodColors: Record<string, string> = {
    GET: "bg-green-500/10 text-green-400 border-green-500/20",
    POST: "bg-blue-500/10 text-blue-400 border-blue-500/20",
    PUT: "bg-yellow-500/10 text-yellow-400 border-yellow-500/20",
    DELETE: "bg-red-500/10 text-red-400 border-red-500/20",
    PATCH: "bg-orange-500/10 text-orange-400 border-orange-500/20",
};

const Endpoint = ({ method, path, desc, body, response }: EndpointProps) => (
    <div className="card p-5 space-y-4">
        <div className="flex items-center gap-3 flex-wrap">
            <span className={`text-[10px] font-bold px-2.5 py-1 rounded border font-mono ${methodColors[method]}`}>{method}</span>
            <code className="font-mono text-sm text-primary">{path}</code>
        </div>
        <p className="text-sm text-secondary">{desc}</p>
        {body && (
            <div>
                <p className="text-[10px] uppercase tracking-widest text-tertiary font-bold mb-2">Request Body</p>
                <div className="code-block p-4 text-xs font-mono whitespace-pre">{body}</div>
            </div>
        )}
        {response && (
            <div>
                <p className="text-[10px] uppercase tracking-widest text-tertiary font-bold mb-2">Response</p>
                <div className="code-block p-4 text-xs font-mono whitespace-pre">{response}</div>
            </div>
        )}
    </div>
);

export default function ApiReferencePage() {
    const [profile, setProfile] = useState<SubscriberProfile | null>(null);

    const loadProfile = useCallback(async () => {
        try {
            if (localStorage.getItem("accessToken")) {
                const data = await subscriberService.getProfile();
                setProfile(data);
            }
        } catch {
            // Ignore if not logged in or failed
        }
    }, []);

    useEffect(() => { loadProfile(); }, [loadProfile]);

    return (
        <AppShell 
            title="API Reference" 
            companyName={profile?.companyName} 
            plan={profile?.subscription?.plan}
        >
            <div className="max-w-4xl space-y-10">
                <div className="mb-8">
                    <p className="text-secondary text-lg leading-relaxed">
                        Complete reference for the Fertile Notify REST API. Base URL: <code className="font-mono text-accent-light bg-elevated px-1.5 py-0.5 rounded text-sm">https://api.fertile-notify.shop/v1</code>
                    </p>
                </div>

                {/* Auth note */}
                <div className="card p-5 mb-10 flex gap-4 items-start border-l-2 border-accent-primary">
                    <div className="text-accent-primary text-xl shrink-0">🔑</div>
                    <div>
                        <p className="font-bold text-sm text-primary mb-1">Authentication</p>
                        <p className="text-sm text-secondary">
                            All requests require <code className="font-mono text-accent-light bg-elevated px-1 rounded">Authorization: Bearer YOUR_API_KEY</code> header.
                            Create API keys in the <a href="/api-keys" className="text-accent-primary hover:underline">API Keys</a> section.
                        </p>
                    </div>
                </div>

                <div className="space-y-10">
                    {/* Auth endpoints */}
                    <div>
                        <h2 className="font-bold text-lg mb-5 flex items-center gap-3">
                            Authentication
                            <span className="text-[10px] font-mono uppercase tracking-widest text-tertiary font-normal">no API key required</span>
                        </h2>
                        <div className="space-y-4">
                            <Endpoint
                                method="POST" path="/auth/login"
                                desc="Authenticate a user and begin the OTP verification flow."
                                body={`{\n  "email": "string",\n  "password": "string"\n}`}
                                response={`{ "message": "OTP sent to email" }`}
                            />
                            <Endpoint
                                method="POST" path="/auth/verify-otp"
                                desc="Complete login by submitting the OTP code received via email."
                                body={`{\n  "email": "string",\n  "otpCode": "string"\n}`}
                                response={`{ "accessToken": "string", "expiresIn": 3600 }`}
                            />
                            <Endpoint
                                method="POST" path="/auth/register"
                                desc="Create a new subscriber account."
                                body={`{\n  "companyName": "string",\n  "email": "string",\n  "password": "string",\n  "phoneNumber": "string?",\n  "plan": "Free | Pro | Enterprise"\n}`}
                            />
                        </div>
                    </div>

                    {/* Notify endpoint */}
                    <div>
                        <h2 className="font-bold text-lg mb-5">Notifications</h2>
                        <div className="space-y-4">
                            <Endpoint
                                method="POST" path="/notifications"
                                desc="Send a notification to one or more channels. Requires an active API key."
                                body={`{\n  "eventType": "ORDER_SHIPPED",\n  "parameters": {\n    "customer_name": "Enes",\n    "order_id": "#FL-2024"\n  },\n  "to": [\n    {\n      "channel": "email",\n      "recipients": ["user@example.com"]\n    }\n  ]\n}`}
                                response={`{\n  "success": true,\n  "message": "Notifications queued successfully",\n  "trackingId": "notif_8k2l9p"\n}`}
                            />
                        </div>
                    </div>

                    {/* Profile */}
                    <div>
                        <h2 className="font-bold text-lg mb-5">Profile</h2>
                        <div className="space-y-4">
                            <Endpoint method="GET" path="/subscriber/profile" desc="Retrieve the authenticated subscriber's profile including subscription and active channels." />
                            <Endpoint method="PUT" path="/subscriber/company-name" desc="Update the company name." body={`{ "companyName": "string" }`} />
                            <Endpoint method="PUT" path="/subscriber/contact-info" desc="Update email and phone number." body={`{ "email": "string", "phoneNumber": "string?" }`} />
                            <Endpoint method="PUT" path="/subscriber/channel" desc="Enable or disable a notification channel." body={`{ "channel": "string", "enable": true }`} />
                            <Endpoint method="PUT" path="/subscriber/password" desc="Change the authenticated user's password." body={`{ "currentPassword": "string", "newPassword": "string" }`} />
                        </div>
                    </div>

                    {/* API Keys */}
                    <div>
                        <h2 className="font-bold text-lg mb-5">API Keys</h2>
                        <div className="space-y-4">
                            <Endpoint method="GET" path="/subscriber/api-keys" desc="List all API keys for the authenticated subscriber. Keys are shown with prefix only." />
                            <Endpoint method="POST" path="/subscriber/api-keys" desc="Create a new API key. The full key is returned ONLY on creation." body={`{ "name": "string" }`} />
                            <Endpoint method="DELETE" path="/subscriber/api-keys/:id" desc="Permanently delete an API key. This action cannot be undone." />
                        </div>
                    </div>
                </div>
            </div>
        </AppShell>
    );
}
