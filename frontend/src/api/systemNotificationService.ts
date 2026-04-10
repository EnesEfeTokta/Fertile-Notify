import apiClient from "./axiosClient";

export interface SystemNotificationDto {
    id: string;
    title: string;
    message: string;
    isRead: boolean;
    createdAt: string;
}

export const getSystemNotifications = async (status: "all" | "read" | "unread" = "all"): Promise<SystemNotificationDto[]> => {
    const response = await apiClient.get<SystemNotificationDto[]>(`/system-notifications?status=${status}`);
    return response.data;
};

export const markSystemNotificationAsRead = async (notificationId: string): Promise<void> => {
    await apiClient.patch(`/system-notifications/${notificationId}/read`);
};
