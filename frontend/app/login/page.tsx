"use client"

import { useAuth } from "@/components/auth/auth-provider"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { authApi } from "@/lib/api"
import { useMutation } from "@tanstack/react-query"
import { LockKeyhole, ShieldCheck, UserRoundCheck } from "lucide-react"
import { useRouter } from "next/navigation"
import { FormEvent, useState } from "react"
import { toast } from "sonner"

const DEMO_EMAIL = "supportflow.demo@example.com"
const DEMO_PASSWORD = "SupportFlowDemo123!"

export default function LoginPage() {
  const router = useRouter()
  const { signIn } = useAuth()
  const [email, setEmail] = useState("")
  const [password, setPassword] = useState("")

  const loginMutation = useMutation({
    mutationFn: authApi.login,
    onSuccess: (response) => {
      signIn(response)
      toast.success("Signed in successfully")
      router.replace("/dashboard")
    },
    onError: (error) => {
      toast.error(error instanceof Error ? error.message : "Could not sign in")
    },
  })

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()

    loginMutation.mutate({
      email,
      password,
    })
  }

  function fillDemoAccount() {
    setEmail(DEMO_EMAIL)
    setPassword(DEMO_PASSWORD)
  }

  return (
    <main className="flex min-h-screen items-center justify-center bg-slate-50 px-4 py-8">
      <Card className="w-full max-w-md border border-slate-200 bg-white shadow-sm">
        <CardHeader className="space-y-3">
          <div className="flex size-10 items-center justify-center rounded-md bg-slate-950 text-white">
            <ShieldCheck className="size-5" />
          </div>
          <div>
            <CardTitle className="text-xl">Agent sign in</CardTitle>
            <p className="mt-1 text-sm text-muted-foreground">
              Access the SupportFlow agent workspace.
            </p>
          </div>
        </CardHeader>

        <CardContent>
          <div className="mb-5 border-y border-slate-200 bg-slate-50 px-3 py-3">
            <div className="flex items-center justify-between gap-3">
              <div>
                <p className="text-sm font-medium text-slate-900">Demo account</p>
                <p className="mt-0.5 text-xs text-slate-500">Admin workspace access</p>
              </div>
              <Button
                type="button"
                variant="outline"
                size="sm"
                onClick={fillDemoAccount}
              >
                <UserRoundCheck className="size-4" />
                Use demo
              </Button>
            </div>
            <dl className="mt-3 grid gap-1 text-xs">
              <div className="flex min-w-0 gap-2">
                <dt className="w-16 shrink-0 text-slate-500">Email</dt>
                <dd className="min-w-0 break-all font-mono text-slate-700">
                  {DEMO_EMAIL}
                </dd>
              </div>
              <div className="flex min-w-0 gap-2">
                <dt className="w-16 shrink-0 text-slate-500">Password</dt>
                <dd className="min-w-0 break-all font-mono text-slate-700">
                  {DEMO_PASSWORD}
                </dd>
              </div>
            </dl>
          </div>

          <form className="space-y-5" onSubmit={handleSubmit}>
            <div className="space-y-2">
              <label className="text-sm font-medium" htmlFor="email">
                Email
              </label>
              <Input
                id="email"
                type="email"
                autoComplete="email"
                placeholder="agent@example.com"
                value={email}
                onChange={(event) => setEmail(event.target.value)}
                required
              />
            </div>

            <div className="space-y-2">
              <label className="text-sm font-medium" htmlFor="password">
                Password
              </label>
              <Input
                id="password"
                type="password"
                autoComplete="current-password"
                placeholder="Enter your password"
                value={password}
                onChange={(event) => setPassword(event.target.value)}
                required
              />
            </div>

            <Button
              type="submit"
              className="w-full bg-slate-950 text-white hover:bg-slate-800"
              disabled={loginMutation.isPending}
            >
              <LockKeyhole className="size-4" />
              {loginMutation.isPending ? "Signing in..." : "Sign in"}
            </Button>
          </form>
        </CardContent>
      </Card>
    </main>
  )
}
