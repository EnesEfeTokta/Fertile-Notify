import axiosClient from "./axiosClient";
import type { Statistic } from "../types/statistic";

export const statisticService = {
    getStatistics: async (period: string): Promise<Statistic> => {
        const response = await axiosClient.get(`statistics?period=${period}`);
        return response.data;
    }
}