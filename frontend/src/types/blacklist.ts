export interface BlacklistEntry {
    id: string;
    recipientAddress: string;
    channels: string[];
    isActive: boolean;
    createdAt: string;
}

export interface BanRecipientRequest {
    recipientAddress: string;
    channels: string[];
}

export interface UpdateBanRequest {
    channels: string[];
    isActive: boolean;
}
