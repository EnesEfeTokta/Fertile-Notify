import axios, { type AxiosResponse } from "axios";
import type { ApiResponse } from "../types/api";

const axiosClient = axios.create({
    baseURL: import.meta.env.VITE_API_URL,
    headers: {
        "Content-Type": "application/json",
    },
});

axiosClient.interceptors.request.use(config => {
    const token = localStorage.getItem("accessToken");
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});

axiosClient.interceptors.response.use(
    (response: AxiosResponse<ApiResponse<any>>) => {
        if (response.data && typeof response.data === 'object' && 'success' in response.data) {
            if (response.data.success) {
                return { ...response, data: response.data.data };
            } else {
                return Promise.reject(response.data);
            }
        }
        return response;
    },
    async (error) => {
        const originalRequest = error.config;

        if (error.response?.status === 401 && !originalRequest._retry) {
            originalRequest._retry = true;

            try {
                const refreshToken = localStorage.getItem("refreshToken");
                if (!refreshToken) {
                    throw new Error("No refresh token available");
                }
                const response = await axios.post<ApiResponse<any>>(`${import.meta.env.VITE_API_URL}/auth/refresh-token`, {
                    refreshToken: refreshToken
                });

                const { accessToken, refreshToken: newRefreshToken } = response.data.data;

                localStorage.setItem("accessToken", accessToken);
                localStorage.setItem("refreshToken", newRefreshToken.token);

                originalRequest.headers.Authorization = `Bearer ${accessToken}`;
                return axiosClient(originalRequest);
            } catch (err) {
                console.error('Session expired:', err);

                localStorage.removeItem('accessToken');
                localStorage.removeItem('refreshToken');
                window.location.href = '/login';

                return Promise.reject(err);
            }
        }

        if (error.success === false) {
            return Promise.reject(error);
        }

        return Promise.reject(error);
    }
);

export default axiosClient;