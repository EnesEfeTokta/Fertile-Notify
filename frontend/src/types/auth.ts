export interface LoginRequest {
    email: string;
    password: string;
}

export interface LoginResponse {
    message: string;
}

export interface VerifyOtpRequest {
    email: string;
    otpCode: string;
}

export interface VerifyOtpResponse {
    accessToken: string;
    refreshToken:
    {
        token: string,
        expiresAt: string,
        isRevoked: boolean
    };
}

export interface RegisterRequest {
    companyName: string;
    password: string;
    email: string;
    phoneNumber?: string;
    plan: 'Free' | 'Pro' | 'Enterprise';
}