export interface CreateExtraCreditPaymentIntentRequest {
    credits: number;
}

export interface ExtraCreditPaymentIntent {
    paymentIntentId: string;
    clientSecret: string;
    credits: number;
    amountInCents: number;
    currency: string;
}
