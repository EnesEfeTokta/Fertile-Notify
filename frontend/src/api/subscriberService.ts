import axiosClient  from "./axiosClient";
import type { SubscriberProfile, UpdateCompanyName, UpdateContactInfo, UpdateChannel, UpdatePassword, CreateApiKey, ApiKeyResponse, ApiKey } from "../types/subscriber";

export const subscriberService = {
    getProfile: async (): Promise<SubscriberProfile> => {
        const response = await axiosClient.get<SubscriberProfile>("/subscribers/me");
        return response.data;
    },
    setCompanyName: async (data: UpdateCompanyName): Promise<void> => {
        await axiosClient.put("/subscribers/company-name", data);
    },
    setContactInfo: async (data: UpdateContactInfo): Promise<void> => {
        await axiosClient.put("/subscribers/contact", data);
    },
    setChannel: async (data: UpdateChannel): Promise<void> => {
        await axiosClient.post("/subscribers/channels", data);
    },
    setPassword: async (data: UpdatePassword): Promise<void> => {
        await axiosClient.put("/subscribers/password", data);
    },
    setApikey: async (data: CreateApiKey): Promise<ApiKeyResponse> => {
        const response = await axiosClient.post<ApiKeyResponse>("/subscribers/create-api-key", data);
        return response.data;
    },
    getApiKeys: async (): Promise<ApiKey[]> => {
        const response = await axiosClient.get<ApiKey[]>("/subscribers/api-keys");
        return response.data;
    },
    deleteApiKey: async (key: string): Promise<void> => {
        await axiosClient.delete(`/subscribers/api-keys/${key}`);
    }
};