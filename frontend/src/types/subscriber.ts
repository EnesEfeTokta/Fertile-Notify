export interface SubscriberProfile {
    id: string;
    companyName: string;
    email: string;
    phoneNumber?: string;
    activeChannels: string[];
    subscription?: Subscription;
}

export interface Subscription {
    plan: string;
    monthlyLimit: number;
    usedThisMonth: number;
    expiresAt: string;
}