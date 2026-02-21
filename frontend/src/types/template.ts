export interface CreateOrUpdateCustom {
    name:string,
    description: string,
    eventType: string,
    channel: string,
    subjectTemplate: string,
    bodyTemplate: string
}

export interface TemplateQuery {
    isTemplateTypeCustom: boolean,
    queries: Query[]
}

export interface Query {
    eventType: string,
    channel: string
}

export interface Template {
    id: string,
    name: string,
    description: string,
    event: string,
    channel: string,
    subject: string,
    body: string,
    source: 'Default' | 'Custom'
}

export interface Notification {
    id: string,
    recipient: string,
    channel: string,
    event: string,
    subject: string,
    body: string,
    createdAt: string
}