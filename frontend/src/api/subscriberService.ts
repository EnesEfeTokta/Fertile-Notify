import axiosClient  from "./axiosClient";
import type { SubscriberProfile } from "../types/subscriber";

export const subscriberService = {
    getProfile: async (): Promise<SubscriberProfile> => {
        const response = await axiosClient.get<SubscriberProfile>("/subscribers/me");
        return response.data;
    },
}