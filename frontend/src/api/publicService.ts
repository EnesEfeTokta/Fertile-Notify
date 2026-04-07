import axiosClient from "./axiosClient";

export interface UnsubscribeRequest {
    recipient: string;
    subscriberId: string;
    token: string;
    channels?: string[];
}

export interface ComplaintRequest {
    subscriberId: string;
    reporterEmail: string;
    reason: string;
    description: string;
    notificationSubject: string;
    notificationBody: string;
}

export const publicService = {
    unsubscribe: async (data: UnsubscribeRequest): Promise<void> => {
        await axiosClient.post("/recipients/unsubscribe", data);
    },
    submitComplaint: async (data: ComplaintRequest): Promise<void> => {
        await axiosClient.post("/recipients/complaint", data);
    }
};
