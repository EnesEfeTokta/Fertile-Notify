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

export interface UpdateCompanyName {
    companyName: string;
}

export interface UpdateContactInfo {
    email?: string;
    phoneNumber?: string;
}

export interface UpdateChannel {
    channel: string;
    enable: boolean;
}

export interface UpdatePassword {
    currentPassword: string;
    newPassword: string;
}

export interface CreateApiKey {
    name: string;
}

export interface ApiKeyResponse {
    apiKey: string;
    message: string;
}

export interface ApiKey {
    id: string;
    prefix: string;
    name: string;
    isActive: boolean;
    createdAt: string;
}