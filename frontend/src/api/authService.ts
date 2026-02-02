import axiosClient from "./axiosClient";
import type { LoginRequest, LoginResponse, RegisterRequest } from "../types/auth";

export const authService = {
    login: async (data: LoginRequest): Promise<LoginResponse> => {
        const response = await axiosClient.post<LoginResponse>("/auth/login", data);
        return response.data;
    },
    register: async (data: RegisterRequest): Promise<void> => {
        await axiosClient.post("/subscribers/register", data);
    },
};