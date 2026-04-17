import axiosClient from "./axiosClient";
import type { CreateExtraCreditPaymentIntentRequest, ExtraCreditPaymentIntent, PaymentLog } from "../types/payment";

export const paymentService = {
    createExtraCreditPaymentIntent: async (data: CreateExtraCreditPaymentIntentRequest): Promise<ExtraCreditPaymentIntent> => {
        const response = await axiosClient.post<ExtraCreditPaymentIntent>("/payments/extra-credits/intent", data);
        return response.data;
    },
    getPaymentHistory: async (): Promise<PaymentLog[]> => {
        const response = await axiosClient.get<{ data: PaymentLog[] }>("/payments/history");
        return (response.data as any).data ?? response.data;
    },
};

