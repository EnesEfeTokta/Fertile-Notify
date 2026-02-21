import axiosClient from "./axiosClient";
import type { CreateOrUpdateCustom, TemplateQuery, Template, Notification } from "../types/template";

export const templateSevice = {
    getAllTemplates: async (): Promise<Template[]> => {
        const response = await axiosClient.get("templates");
        return response.data;
    },
    createOrUpdateTemplate: async (data: CreateOrUpdateCustom): Promise<void> => {
        const response = await axiosClient.post("templates/create-or-update-custom", data);
        return response.data;
    },
    queryTemplate: async (data: TemplateQuery): Promise<void> => {
        const response = await axiosClient.post("templates/query", data);
        return response.data;
    },
    getNotificationLogs: async (): Promise<Notification[]> => {
        const response = await axiosClient.get("templates/logs");
        return response.data;
    }
}