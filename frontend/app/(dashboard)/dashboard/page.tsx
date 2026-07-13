"use client"

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Skeleton } from "@/components/ui/skeleton"
import { dashboardApi } from "@/lib/api"
import { useQuery } from "@tanstack/react-query"
import {
  AlertTriangle,
  BookOpen,
  CheckCircle2,
  Clock,
  FileText,
  Sparkles,
  Ticket,
} from "lucide-react"

export default function DashboardPage() {
  const {
    data: summary,
    isLoading,
    isError,
  } = useQuery({
    queryKey: ["dashboard-summary"],
    queryFn: dashboardApi.getSummary,
  })

  if (isLoading) {
    return (
      <div className="space-y-7">
        <Skeleton className="h-10 w-64" />
        <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
          {Array.from({ length: 8 }).map((_, index) => (
            <Skeleton key={index} className="h-32 w-full" />
          ))}
        </div>
      </div>
    )
  }

  if (isError || !summary) {
    return (
      <div className="rounded-md border bg-background p-8 text-sm text-destructive">
        Could not load dashboard summary. Check that the ASP.NET API is running
        on port 5059.
      </div>
    )
  }

  const knowledgeReadyPercent =
    summary.totalKnowledgeArticles === 0
      ? 0
      : Math.round(
          (summary.aiReadyKnowledgeArticles / summary.totalKnowledgeArticles) *
            100,
        )

  const embeddedChunksPercent =
    summary.totalKnowledgeChunks === 0
      ? 0
      : Math.round(
          (summary.embeddedKnowledgeChunks / summary.totalKnowledgeChunks) *
            100,
        )

  return (
    <div className="space-y-7">
      <div>
        <h1 className="text-2xl font-semibold tracking-tight">Dashboard</h1>
        <p className="mt-1 text-sm text-muted-foreground">
          Monitor support workload, AI drafting progress, and knowledge
          readiness.
        </p>
      </div>

      <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
        <MetricCard
          icon={Ticket}
          label="Total tickets"
          value={summary.totalTickets}
          accent="bg-slate-100 text-slate-700"
        />
        <MetricCard
          icon={Clock}
          label="Open tickets"
          value={summary.openTickets}
          accent="bg-sky-50 text-sky-700"
        />
        <MetricCard
          icon={Sparkles}
          label="Drafted tickets"
          value={summary.draftedTickets}
          accent="bg-violet-50 text-violet-700"
        />
        <MetricCard
          icon={CheckCircle2}
          label="Resolved tickets"
          value={summary.resolvedTickets}
          accent="bg-emerald-50 text-emerald-700"
        />
        <MetricCard
          icon={AlertTriangle}
          label="High priority"
          value={summary.highPriorityTickets}
          accent="bg-amber-50 text-amber-700"
        />
        <MetricCard
          icon={BookOpen}
          label="Knowledge articles"
          value={summary.totalKnowledgeArticles}
          accent="bg-indigo-50 text-indigo-700"
        />
        <MetricCard
          icon={FileText}
          label="Embedded chunks"
          value={`${summary.embeddedKnowledgeChunks}/${summary.totalKnowledgeChunks}`}
          accent="bg-teal-50 text-teal-700"
        />
        <MetricCard
          icon={Sparkles}
          label="AI-ready knowledge"
          value={`${knowledgeReadyPercent}%`}
          accent="bg-rose-50 text-rose-700"
        />
      </div>

      <div className="grid gap-4 lg:grid-cols-2">
        <ReadinessPanel
          title="Knowledge readiness"
          description="Articles with embedded chunks available for related knowledge search."
          current={summary.aiReadyKnowledgeArticles}
          total={summary.totalKnowledgeArticles}
          percent={knowledgeReadyPercent}
        />
        <ReadinessPanel
          title="Embedding coverage"
          description="Knowledge chunks that already have vector embeddings."
          current={summary.embeddedKnowledgeChunks}
          total={summary.totalKnowledgeChunks}
          percent={embeddedChunksPercent}
        />
      </div>
    </div>
  )
}

function MetricCard({
  icon: Icon,
  label,
  value,
  accent,
}: {
  icon: React.ComponentType<{ className?: string }>
  label: string
  value: number | string
  accent: string
}) {
  return (
    <Card className="border border-slate-200 bg-white shadow-sm">
      <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
        <CardTitle className="text-sm font-medium text-muted-foreground">
          {label}
        </CardTitle>
        <span
          className={`flex size-8 items-center justify-center rounded-md ${accent}`}
        >
          <Icon className="size-4" />
        </span>
      </CardHeader>
      <CardContent>
        <p className="text-2xl font-semibold">{value}</p>
      </CardContent>
    </Card>
  )
}

function ReadinessPanel({
  title,
  description,
  current,
  total,
  percent,
}: {
  title: string
  description: string
  current: number
  total: number
  percent: number
}) {
  return (
    <Card className="border border-slate-200 bg-white shadow-sm">
      <CardHeader>
        <CardTitle className="text-base">{title}</CardTitle>
        <p className="text-sm text-muted-foreground">{description}</p>
      </CardHeader>
      <CardContent className="space-y-3">
        <div className="flex items-center justify-between text-sm">
          <span className="text-muted-foreground">
            {current} of {total}
          </span>
          <span className="font-medium">{percent}%</span>
        </div>
        <div className="h-2 rounded-full bg-slate-100">
          <div
            className="h-2 rounded-full bg-slate-950"
            style={{ width: `${percent}%` }}
          />
        </div>
      </CardContent>
    </Card>
  )
}
