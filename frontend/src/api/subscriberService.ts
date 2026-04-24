import axiosClient from "./axiosClient";
import type { SubscriberProfile, UpdateSubscriberProfile, UpdateChannel, UpdatePassword, CreateApiKey, ApiKeyResponse, ApiKey, ChannelSetting, UpdateApiKeyScopes, UpdateApiKeyStatus } from "../types/subscriber";

export const subscriberService = {
    getProfile: async (): Promise<SubscriberProfile> => {
        const response = await axiosClient.get<SubscriberProfile>("/subscribers/me");
        return response.data;
    },
    // Update any subset of profile fields (company + contact)
    updateProfile: async (data: UpdateSubscriberProfile): Promise<void> => {
        await axiosClient.put("/subscribers/profile", data);
    },
    setChannel: async (data: UpdateChannel): Promise<void> => {
        await axiosClient.post("/subscribers/channels", data);
    },
    setPassword: async (data: UpdatePassword): Promise<void> => {
        await axiosClient.put("/subscribers/password", data);
    },
    setApikey: async (data: CreateApiKey): Promise<ApiKeyResponse> => {
        const response = await axiosClient.post<ApiKeyResponse>("/subscribers/api-keys", data);
        return response.data;
    },
    getApiKeys: async (): Promise<ApiKey[]> => {
        const response = await axiosClient.get<ApiKey[]>("/subscribers/api-keys");
        return response.data;
    },
    deleteApiKey: async (key: string): Promise<void> => {
        await axiosClient.delete(`/subscribers/api-keys/${key}`);
    },
    updateApiKeyScopes: async (key: string, data: UpdateApiKeyScopes): Promise<void> => {
        await axiosClient.patch(`/subscribers/api-keys/${key}/scopes`, {
            scopes: data.scopes.join(","),
        });
    },
    updateApiKeyStatus: async (key: string, data: UpdateApiKeyStatus): Promise<void> => {
        await axiosClient.patch(`/subscribers/api-keys/${key}/status`, data);
    },
    setChannelSetting: async (data: ChannelSetting): Promise<void> => {
        await axiosClient.post("/subscribers/settings/channel-setting", data);
    },
    getChannelSetting: async (channel: string): Promise<ChannelSetting> => {
        const response = await axiosClient.get<{ data: ChannelSetting }>("/subscribers/settings/channel-setting", {
            params: { channel }
        });
        return response.data.data;
    },
    buyCredits: async (count: number): Promise<void> => {
        await axiosClient.patch("/subscribers/add-extra-credits", { count });
    },
    deleteAccount: async (): Promise<void> => {
        await axiosClient.delete("/subscribers/delete-account");
    },
    exportData: async (): Promise<void> => {
        const response = await axiosClient.get("/subscribers/export-data", {
            responseType: "blob",
        });
        const blob = new Blob([response.data], { type: "application/json" });
        const url = URL.createObjectURL(blob);
        const fileName = `fertilenotify-export-${new Date().toISOString().slice(0, 19).replace(/[:T]/g, "-")}.json`;
        const a = document.createElement("a");
        a.href = url;
        a.download = fileName;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        URL.revokeObjectURL(url);
    },
};