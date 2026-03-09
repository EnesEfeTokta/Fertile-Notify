import axiosClient from "./axiosClient";
import type { StatisticsResponse } from "../types/statistic";

export const statisticService = {
    getStatistics: async (period: string): Promise<StatisticsResponse> => {
        const response = await axiosClient.get(`statistics?period=${period}`);
        return response.data;
    }
}