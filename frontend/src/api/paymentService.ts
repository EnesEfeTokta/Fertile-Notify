import axiosClient from "./axiosClient";
import type { CreateExtraCreditPaymentIntentRequest, ExtraCreditPaymentIntent } from "../types/payment";

export const paymentService = {
    createExtraCreditPaymentIntent: async (data: CreateExtraCreditPaymentIntentRequest): Promise<ExtraCreditPaymentIntent> => {
        const response = await axiosClient.post<ExtraCreditPaymentIntent>("/payments/extra-credits/intent", data);
        return response.data;
    },
};
