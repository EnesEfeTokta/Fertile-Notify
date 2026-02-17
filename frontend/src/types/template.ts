export interface CreateOrUpdateCustom {
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
    id: string;
    event: string;
    channel: string;
    subject: string;
    body: string;
    source: 'Default' | 'Custom';
}