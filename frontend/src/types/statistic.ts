export interface UsageStats {
    totalUsage: number;
    successCount: number;
    failedCount: number;
    statsByChannel: Record<string, number>;
    statsByEventType: Record<string, number>;
}

export interface SubscriptionSummary {
    plan: string;
    limit: number;
    used: number;
    remaining: number;
    expiresAt: string;
}

// Full statistics response from the API
export interface StatisticsResponse {
    usageStats: UsageStats;
    subscriptionSummary: SubscriptionSummary;
}

// Keep for backward compatibility
export type Statistic = UsageStats;