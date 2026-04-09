import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { ApiResponse } from '../models/api-response.model';
import {
  AuthResponse,
  UserProfile,
  LoginRequest,
  RegisterRequest,
  RefreshTokenRequest,
  VerifyEmailRequest,
  SendOtpRequest,
  VerifyOtpRequest,
  ResetPasswordRequest,
} from '../models/auth.model';

@Injectable({ providedIn: 'root' })
export class AuthService {

    constructor(private api: ApiService) {}

    // POST /api/auth/login
    login(req: LoginRequest): Observable<ApiResponse<AuthResponse>> {
        return this.api.post<AuthResponse>('/api/auth/login', req);
    }

    // POST /api/auth/register
    register(req: RegisterRequest): Observable<ApiResponse<null>> {
        return this.api.post<null>('/api/auth/register', req);
    }

    // POST /api/auth/refresh
    refreshToken(refreshToken: string): Observable<ApiResponse<AuthResponse>> {
        return this.api.post<AuthResponse>('/api/auth/refresh', { refreshToken });
    }

    // POST /api/auth/verify-email
    verifyEmail(req: VerifyEmailRequest): Observable<ApiResponse<null>> {
        return this.api.post<null>('/api/auth/verify-email', req);
    }

    // POST /api/auth/send-otp
    sendOtp(req: SendOtpRequest): Observable<ApiResponse<null>> {
        return this.api.post<null>('/api/auth/send-otp', req);
    }

    // POST /api/auth/verify-otp
    verifyOtp(req: VerifyOtpRequest): Observable<ApiResponse<null>> {
        return this.api.post<null>('/api/auth/verify-otp', req);
    }

    // POST /api/auth/reset-password
    resetPassword(req: ResetPasswordRequest): Observable<ApiResponse<null>> {
        return this.api.post<null>('/api/auth/reset-password', req);
    }

    // GET /api/users/profile (requires JWT)
    getProfile(): Observable<ApiResponse<UserProfile>> {
        return this.api.get<UserProfile>('/api/users/profile');
    }
}