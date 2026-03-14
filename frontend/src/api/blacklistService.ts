import axiosClient from "./axiosClient";
import type { BlacklistEntry, BanRecipientRequest, UpdateBanRequest } from "../types/blacklist";

export const blacklistService = {
    getAll: async (): Promise<BlacklistEntry[]> => {
        const response = await axiosClient.get<BlacklistEntry[]>("/blacklist");
        return response.data;
    },
    add: async (request: BanRecipientRequest): Promise<void> => {
        await axiosClient.post("/blacklist", request);
    },
    update: async (id: string, request: UpdateBanRequest): Promise<void> => {
        await axiosClient.put(`/blacklist/${id}`, request);
    },
    delete: async (id: string): Promise<void> => {
        await axiosClient.delete(`/blacklist/${id}`);
    }
};
