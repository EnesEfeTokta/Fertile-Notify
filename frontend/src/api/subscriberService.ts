import axiosClient  from "./axiosClient";
import type { SubscriberProfile, UpdateCompanyName, UpdateContactInfo, UpdateChannel, UpdatePassword } from "../types/subscriber";

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
    }
};