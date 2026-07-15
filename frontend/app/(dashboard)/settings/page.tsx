import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Bot, Database, KeyRound } from "lucide-react"

export default function SettingsPage() {
  return (
    <div className="space-y-7">
      <div>
        <h1 className="text-2xl font-semibold tracking-tight">Settings</h1>
        <p className="mt-1 text-sm text-muted-foreground">
          Review demo configuration and AI provider readiness.
        </p>
      </div>

      <div className="grid gap-4 lg:grid-cols-2">
        <Card className="border border-slate-200 bg-white shadow-sm">
          <CardHeader>
            <CardTitle className="flex items-center gap-2 text-base">
              <Bot className="size-4" />
              AI providers
            </CardTitle>
          </CardHeader>
          <CardContent className="space-y-3 text-sm text-slate-600">
            <div className="flex items-center justify-between">
              <span>Embedding provider</span>
              <span className="rounded-md bg-slate-100 px-2 py-1 text-xs font-medium text-slate-700">
                Fake / OpenAI
              </span>
            </div>
            <div className="flex items-center justify-between">
              <span>Draft reply provider</span>
              <span className="rounded-md bg-slate-100 px-2 py-1 text-xs font-medium text-slate-700">
                Fake / OpenAI
              </span>
            </div>
          </CardContent>
        </Card>

        <Card className="border border-slate-200 bg-white shadow-sm">
          <CardHeader>
            <CardTitle className="flex items-center gap-2 text-base">
              <Database className="size-4" />
              Demo environment
            </CardTitle>
          </CardHeader>
          <CardContent className="space-y-3 text-sm text-slate-600">
            <div className="flex items-center justify-between">
              <span>Database</span>
              <span className="rounded-md bg-emerald-50 px-2 py-1 text-xs font-medium text-emerald-700">
                PostgreSQL + pgvector
              </span>
            </div>
            <div className="flex items-center justify-between">
              <span>API</span>
              <span className="rounded-md bg-sky-50 px-2 py-1 text-xs font-medium text-sky-700">
                ASP.NET Core
              </span>
            </div>
          </CardContent>
        </Card>
      </div>

      <Card className="border border-slate-200 bg-white shadow-sm">
        <CardHeader>
          <CardTitle className="flex items-center gap-2 text-base">
            <KeyRound className="size-4" />
            OpenAI readiness checklist
          </CardTitle>
        </CardHeader>
        <CardContent className="grid gap-3 text-sm text-slate-600 md:grid-cols-3">
          <ChecklistItem
            title="API key"
            description="Stored with .NET user-secrets, not committed to source control."
          />
          <ChecklistItem
            title="Billing quota"
            description="Required before real OpenAI draft replies and embeddings can run."
          />
          <ChecklistItem
            title="Provider switch"
            description="Set AI providers from Fake to OpenAI through configuration."
          />
        </CardContent>
      </Card>
    </div>
  )
}

function ChecklistItem({
  title,
  description,
}: {
  title: string
  description: string
}) {
  return (
    <div className="rounded-md border border-slate-200 p-3">
      <p className="font-medium text-slate-900">{title}</p>
      <p className="mt-1 leading-5 text-muted-foreground">{description}</p>
    </div>
  )
}
