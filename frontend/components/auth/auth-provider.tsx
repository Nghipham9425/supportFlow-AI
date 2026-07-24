"use client"
import { AuthResponse, AuthUser } from "@/types/auth"
import { createContext, ReactNode, useContext, useState } from "react"
import { clearSession, getStoredSession, saveSession } from "@/lib/auth-session"

type AuthContextValue = {
  user: AuthUser | null
  accessToken: string | null
  isLoading: boolean
  signIn: (response: AuthResponse) => void
  signOut: () => void
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined)

export function AuthProvider({ children }: { children: ReactNode }) {
  const [session, setSession] = useState<AuthResponse | null>(getStoredSession)
  const isLoading = false

  function signIn(response: AuthResponse) {
    saveSession(response)
    setSession(response)
  }
  function signOut() {
    clearSession()
    setSession(null)
  }

  return (
    <AuthContext.Provider
      value={{
        user: session?.user ?? null,
        accessToken: session?.accessToken ?? null,
        isLoading,
        signIn,
        signOut,
      }}
    >
      {children}
    </AuthContext.Provider>
  )
}

export function useAuth() {
  const context = useContext(AuthContext)
  if (!context) {
    throw new Error("useAuth must be used inside AuthProvider.")
  }
  return context
}
