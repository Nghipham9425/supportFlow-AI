import { AppShell } from "@/components/layout/app-shell"
import { ReactNode } from "react"
import { AdminRouteGuard } from "@/components/auth/admin-route-guard"
export default function DashboardLayout({ children }: { children: ReactNode }) {
  return (
    <AdminRouteGuard>
      <AppShell>{children}</AppShell>
    </AdminRouteGuard>
  )
}
