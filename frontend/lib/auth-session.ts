import { AuthResponse } from "@/types/auth"

export const AUTH_STORAGE_KEY = "supportflow-auth"

export function getStoredSession(): AuthResponse | null {
  if (typeof window === "undefined") {
    return null
  }

  const storedSession = localStorage.getItem(AUTH_STORAGE_KEY)

  if (!storedSession) {
    return null
  }

  try {
    const parsedSession = JSON.parse(storedSession) as AuthResponse
    const expiresAt = new Date(parsedSession.expiresAt)

    if (expiresAt <= new Date()) {
      localStorage.removeItem(AUTH_STORAGE_KEY)
      return null
    }

    return parsedSession
  } catch {
    localStorage.removeItem(AUTH_STORAGE_KEY)
    return null
  }
}

export function getAccessToken(): string | null {
  return getStoredSession()?.accessToken ?? null
}

export function saveSession(response: AuthResponse) {
  localStorage.setItem(AUTH_STORAGE_KEY, JSON.stringify(response))
}

export function clearSession() {
  localStorage.removeItem(AUTH_STORAGE_KEY)
}
