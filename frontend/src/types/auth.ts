export interface LoginRequest {
    email: string;
    password: string;
}

export interface LoginResponse {
    token: string;
}

export interface RegisterRequest {
    companyName: string;
    password: string;
    email: string;
    phoneNumber?: string;
    plan: 'Free' | 'Pro' | 'Enterprise';
}