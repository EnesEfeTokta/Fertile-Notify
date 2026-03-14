import axiosClient from "./axiosClient";

export interface UnsubscribeRequest {
    recipient: string;
    subscriberId: string;
    token: string;
    channels?: string[];
}

export const publicService = {
    unsubscribe: async (data: UnsubscribeRequest): Promise<void> => {
        await axiosClient.post("/notifications/unsubscribe", data);
    }
};
