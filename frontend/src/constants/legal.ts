export const PRIVACY_POLICY = {
    title: "Privacy Policy",
    lastUpdated: "April 11, 2026",
    introduction: "FertileNotify ('we', 'our', 'us') is committed to protecting your privacy and handling your data responsibly.",
    sections: [
        {
            id: 1,
            title: "Data We Collect",
            content: "We may collect the following categories of data:",
            list: [
                "Notification Data: message content, recipient information, and delivery outcomes (success, failure, rejection).",
                "Technical Data: API key usage logs, request metadata, and error logs.",
                "Account Data: company name, email address, and phone number.",
                "Billing Data: payment and invoicing information required for subscription and accounting operations."
            ]
        },
        {
            id: 2,
            title: "How We Use Data",
            content: "We process data to:",
            list: [
                "deliver notifications through configured channels,",
                "provide delivery reports, analytics, and service insights,",
                "detect abuse, prevent fraud, and protect service integrity,",
                "maintain and improve platform reliability and performance,",
                "comply with legal and regulatory obligations."
            ]
        },
        {
            id: 3,
            title: "Legal Bases (GDPR)",
            content: "Depending on the context, we rely on one or more of the following legal bases:",
            list: [
                "performance of a contract,",
                "compliance with legal obligations,",
                "legitimate interests (service security, reliability, and abuse prevention),",
                "consent, where required by applicable law."
            ]
        },
        {
            id: 4,
            title: "Data Sharing and Third Parties",
            list: [
                "We do not sell your personal data.",
                "Data is shared only with service providers required to deliver notifications or operate the platform (for example SMTP providers, SMS operators, push providers, and infrastructure vendors).",
                "You control your own channel integrations and destination providers.",
                "AI-assisted template generation may be available. Text sent to AI services is intended to be masked/non-personal where possible."
            ]
        },
        {
            id: 5,
            title: "Data Retention and Deletion",
            content: "Unless a longer retention period is required by law:",
            list: [
                "Operational logs are retained for 30 days and then deleted.",
                "Notification data older than 7 days may be anonymized for statistical and operational analysis.",
                "You may request deletion of personal data by using the unsubscribe endpoint at api/recipients/unsubscribe or by contacting us directly."
            ]
        },
        {
            id: 6,
            title: "Security Measures",
            content: "We implement technical and organizational safeguards, including:",
            list: [
                "access control and authentication mechanisms,",
                "JWT-based API security,",
                "hashing of sensitive stored secrets (such as passwords and API keys),",
                "monitoring and incident response procedures."
            ],
            footer: "No system is 100% secure, but we continuously improve our controls."
        },
        {
            id: 7,
            title: "International Transfers",
            content: "If data is transferred across borders, we apply appropriate safeguards required by applicable laws (for example, contractual or technical protections)."
        },
        {
            id: 8,
            title: "Your Rights",
            content: "Subject to applicable law, you may have rights to:",
            list: [
                "access your data,",
                "correct inaccurate data,",
                "request deletion,",
                "restrict or object to processing,",
                "receive your data in portable format,",
                "lodge a complaint with a supervisory authority."
            ],
            footer: "To exercise these rights, contact us through official support channels."
        },
        {
            id: 9,
            title: "Changes to This Policy",
            content: "We may update this Privacy Policy from time to time. Material changes will be reflected by updating the 'Last Updated' date and, where required, by additional notice."
        }
    ]
};

export const TERMS_OF_USE = {
    title: "Terms of Use",
    lastUpdated: "April 11, 2026",
    introduction: "Welcome to FertileNotify. By accessing or using the service, you agree to these Terms of Use.",
    sections: [
        {
            id: 1,
            title: "Service Scope",
            content: "FertileNotify provides notification automation across multiple channels (including email, SMS, WebPush, Webhook, and other supported integrations).\n\nService availability and delivery performance may depend on third-party providers (for example email, telecom, messaging, and cloud providers)."
        },
        {
            id: 2,
            title: "Account and User Responsibilities",
            content: "You are responsible for:",
            list: [
                "providing accurate account and recipient information,",
                "maintaining the confidentiality of API keys and credentials,",
                "obtaining valid legal consent and permissions for recipients under applicable laws (including GDPR/KVKK and local anti-spam regulations),",
                "ensuring your content and usage comply with applicable law."
            ]
        },
        {
            id: 3,
            title: "Notification Delivery Disclaimer",
            content: "FertileNotify does not guarantee that every message will reach an end recipient inbox/device.\n\nDelivery failures may occur due to reasons outside our control, including:",
            list: [
                "invalid recipient addresses or phone numbers,",
                "recipient spam/security filters,",
                "third-party provider outages, throttling, or delays,",
                "network or infrastructure disruptions."
            ]
        },
        {
            id: 4,
            title: "Acceptable Use Policy",
            content: "You may not use the service for illegal, abusive, or harmful activities. Prohibited use includes, but is not limited to:",
            list: [
                "fraud, phishing, deception, or impersonation,",
                "unauthorized marketing or spam campaigns,",
                "gambling-related illegal promotions,",
                "distribution of sexually explicit unlawful content,",
                "hate, harassment, threats, or targeted discrimination,",
                "malware distribution or security attacks,",
                "promoting illegal drug trade or related criminal activity."
            ],
            footer: "We may suspend or terminate accounts involved in prohibited use."
        },
        {
            id: 5,
            title: "Data Protection and Privacy",
            content: "Your use of the service is also governed by our Privacy Policy."
        },
        {
            id: 6,
            title: "Intellectual Property",
            content: "All rights, title, and interest in the platform, software, branding, and documentation remain with FertileNotify and its licensors, except where explicitly stated otherwise."
        },
        {
            id: 7,
            title: "Limitation of Liability",
            content: "To the maximum extent permitted by law:",
            list: [
                "the service is provided on an 'as is' and 'as available' basis,",
                "we disclaim implied warranties of merchantability, fitness for a particular purpose, and non-infringement,",
                "we are not liable for indirect, incidental, special, consequential, or punitive damages,",
                "total liability for direct claims is limited to the amount paid by you for the service in the applicable billing period, where legally permitted."
            ]
        },
        {
            id: 8,
            title: "Service Changes and Termination",
            content: "We may modify, suspend, or discontinue parts of the service at any time, with reasonable notice where feasible.\n\nWe may suspend or terminate access for violations of these Terms, legal requirements, or security risks."
        },
        {
            id: 9,
            title: "Governing Law and Disputes",
            content: "These Terms are governed by applicable local laws and regulations. Disputes shall be resolved in the competent courts/jurisdictions as required by law and contract."
        },
        {
            id: 10,
            title: "Changes to These Terms",
            content: "We may update these Terms from time to time. Continued use of the service after updates becomes effective means you accept the revised Terms."
        }
    ]
};
