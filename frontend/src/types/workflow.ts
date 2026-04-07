export interface ChannelRecipientGroup {
    channel: string;
    recipients: string[];
}

export interface Workflow {
    id: string;
    name: string;
    description: string;
    eventType: string;
    channel: string;
    eventTrigger: string;
    cronExpression: string;
    recipients: ChannelRecipientGroup[];
    isActive: boolean;
    createdAt: string;
    subject: string;
    body: string;
}

export interface CreateWorkflowRequest {
    name: string;
    description: string;
    eventType: string;
    channels: string;
    eventTrigger: string;
    cronExpression: string;
    subject: string;
    body: string;
    to: ChannelRecipientGroup[];
}

export interface UpdateWorkflowRequest {
    id: string;
    name?: string;
    description?: string;
    eventType?: string;
    channel?: string;
    eventTrigger?: string;
    cronExpression?: string;
    subject?: string;
    body?: string;
    to?: ChannelRecipientGroup[];
}
