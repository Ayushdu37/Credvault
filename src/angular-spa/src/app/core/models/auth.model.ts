import { UserRole, UserStatus } from './enums.model';

// Mirrors: CredVault.Shared.Contracts.Identity.Responses.AuthResponse
export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: string; // DateTime → ISO string
}

// Mirrors: CredVault.Shared.Contracts.Identity.Responses.UserProfileResponse
export interface UserProfile {
  id: string;           // Guid → string
  email: string;
  fullName: string;
  phoneNumber: string;
  role: UserRole;
  status: UserStatus;
  isEmailVerified: boolean;
  createdAt: string;
}

// Mirrors: CredVault.Shared.Contracts.Identity.Requests.LoginUserRequest
export interface LoginRequest {
  email: string;
  password: string;
}

// Mirrors: CredVault.Shared.Contracts.Identity.Requests.RegisterUserRequest
export interface RegisterRequest {
  email: string;
  password: string;
  fullName: string;
  phoneNumber: string;
}

// Mirrors: CredVault.Shared.Contracts.Identity.Requests.RefreshTokenRequest
export interface RefreshTokenRequest {
  refreshToken: string;
}

// Mirrors: CredVault.Shared.Contracts.Identity.Requests.VerifyEmailRequest
export interface VerifyEmailRequest {
  email: string;
  otpCode: string;
}

// Mirrors: CredVault.Shared.Contracts.Identity.Requests.SendOTPRequest
export interface SendOtpRequest {
  email: string;
  purpose: string;  // 'Login' | 'Payment' | 'PasswordReset'
}

// Mirrors: CredVault.Shared.Contracts.Identity.Requests.VerifyOTPRequest
export interface VerifyOtpRequest {
  email: string;
  otpCode: string;
  purpose: string;
}

// Mirrors: CredVault.Shared.Contracts.Identity.Requests.ResetPasswordRequest
export interface ResetPasswordRequest {
  email: string;
  otpCode: string;
  newPassword: string;
}

// Re-export enums for convenience
export { UserRole, UserStatus } from './enums.model';