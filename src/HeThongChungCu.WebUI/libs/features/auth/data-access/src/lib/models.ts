export interface LoginRequest {
  username: string;
  password: string;
}

export interface AuthTokens {
  accessToken: string;
  refreshToken?: string;
}

export interface AuthUserResult {
  userId: number;
  username: string;
  email?: string;
  anhDaiDienUrl?: string;
  role?: string;
  fullName?: string;
  accessToken?: string;
  refreshToken?: string;
}

export interface RegisterResidentRequest {
  username: string;
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  phoneNumber: string;
  idCard: string;
  dob: string;
  gioiTinhId: number;
  diaChi: string;
}

export interface ApiResponse<T = any> {
  result?: T;
  warningMessages?: string[];
  errors?: Array<{ code?: string; description?: string }> | { [key: string]: string[] };
  isOk?: boolean;
}

export interface ForgotPasswordRequest {
  username: string;
}

export interface ResetPasswordRequest {
  username: string;
  resetCode: string;
  newPassword: string;
  confirmPassword?: string;
}

export interface ChangePasswordRequest {
  oldPassword: string;
  newPassword: string;
  confirmPassword?: string;
}

export interface RefreshTokenRequest {
  refreshToken: string;
}
