"use client"

import { useAuth } from "@/components/auth/auth-provider"
import { Skeleton } from "@/components/ui/skeleton"
import { UserRole } from "@/types/auth"
import { useRouter } from "next/navigation"
import { ReactNode, useEffect } from "react"

export function AdminRouteGuard({ children }: { children: ReactNode }) {
  const router = useRouter()
  const { user, isLoading } = useAuth()

  useEffect(() => {
    if (isLoading) {
      return
    }

    if (!user) {
      router.replace("/login")
      return
    }

    if (user.role !== UserRole.Admin) {
      router.replace("/support")
    }
  }, [isLoading, router, user])

  if (isLoading || !user || user.role !== UserRole.Admin) {
    return (
      <div className="space-y-4 p-4">
        <Skeleton className="h-8 w-48" />
        <Skeleton className="h-64 w-full" />
      </div>
    )
  }

  return children
}
