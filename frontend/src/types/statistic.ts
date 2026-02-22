export interface Statistic {
    totalUsage: number,
    successCount: number,
    failedCount: number,
    statsByChannel: Record<string, number>,
    statsByEventType: Record<string, number>,
    successByChannel?: Record<string, number>,
    failedByChannel?: Record<string, number>,
    successByEventType?: Record<string, number>,
    failedByEventType?: Record<string, number>
}