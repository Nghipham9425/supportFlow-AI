export type AuthUser = {
  id: string
  name: string
  email: string
  role: UserRole
  createdAt: string
}

export type LoginInput = {
  email: string
  password: string
}
export type AuthResponse = {
  accessToken: string
  expiresAt: string
  user: AuthUser
}

export const UserRole = {
  Customer: 0,
  Admin: 1,
} as const

export type UserRole = (typeof UserRole)[keyof typeof UserRole]
