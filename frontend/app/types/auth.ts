export interface ApiError {
  message: string
}

export interface AuthLoginRequest {
  email: string
  password: string
}

export interface AuthLoginResponse {
  success: boolean
  message: string
  data?: unknown
}
