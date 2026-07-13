"use client"

import {
  PriorityBadge,
  StatusBadge,
} from "@/components/tickets/ticket-status-badge"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Skeleton } from "@/components/ui/skeleton"
import { Textarea } from "@/components/ui/textarea"
import { ticketsApi } from "@/lib/api"
import {
  ticketCategoryLabels,
  ticketChannelLabels,
  ticketPriorityLabels,
  ticketSentimentLabels,
  ticketStatusLabels,
} from "@/types/ticket"
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query"
import {
  ArrowLeft,
  BadgeCheck,
  Bot,
  BookOpen,
  Clock,
  Copy,
  FileText,
  Mail,
  MessageSquareText,
  Send,
  Sparkles,
  User,
} from "lucide-react"
import Link from "next/link"
import { useState } from "react"
import { toast } from "sonner"

export function TicketDetail({ ticketId }: { ticketId: string }) {
  const queryClient = useQueryClient()
  const [draftEdit, setDraftEdit] = useState<{
    ticketId: string
    value: string
  } | null>(null)

  const {
    data: ticket,
    isLoading,
    isError,
  } = useQuery({
    queryKey: ["tickets", ticketId],
    queryFn: () => ticketsApi.getById(ticketId),
  })

  const {
    data: relatedKnowledge = [],
    isLoading: isRelatedKnowledgeLoading,
    isError: isRelatedKnowledgeError,
  } = useQuery({
    queryKey: ["tickets", ticketId, "related-knowledge"],
    queryFn: () => ticketsApi.getRelatedKnowledge(ticketId),
  })

  const analyzeTicket = useMutation({
    mutationFn: () => ticketsApi.analyze(ticketId),
    onSuccess: (updatedTicket) => {
      queryClient.setQueryData(["tickets", ticketId], updatedTicket)
      queryClient.invalidateQueries({ queryKey: ["tickets"] })
      toast.success("Ticket analyzed")
    },
    onError: () => {
      toast.error("Could not analyze ticket")
    },
  })

  const generateDraftReply = useMutation({
    mutationFn: () => ticketsApi.generateDraftReply(ticketId),
    onSuccess: (updatedTicket) => {
      queryClient.setQueryData(["tickets", ticketId], updatedTicket)
      queryClient.invalidateQueries({ queryKey: ["tickets"] })
      setDraftEdit({
        ticketId,
        value: updatedTicket.aiDraftReply ?? "",
      })
      toast.success("Draft reply generated")
    },
    onError: () => {
      toast.error("Could not generate draft reply")
    },
  })

  if (isLoading) {
    return (
      <div className="space-y-6">
        <Skeleton className="h-10 w-48" />
        <Skeleton className="h-44 w-full" />
        <div className="grid gap-6 xl:grid-cols-[1fr_360px]">
          <Skeleton className="h-96 w-full" />
          <Skeleton className="h-96 w-full" />
        </div>
      </div>
    )
  }

  if (isError || !ticket) {
    return (
      <div className="rounded-xl border border-rose-200 bg-rose-50 p-8 text-sm text-rose-700">
        Could not load this ticket.
      </div>
    )
  }

  const draftReply =
    draftEdit?.ticketId === ticketId
      ? draftEdit.value
      : (ticket.aiDraftReply ?? "")

  const copyDraftReply = async () => {
    if (!draftReply.trim()) return

    await navigator.clipboard.writeText(draftReply)
    toast.success("Draft reply copied")
  }

  return (
    <div className="space-y-6">
      <div className="flex flex-col gap-4 md:flex-row md:items-start md:justify-between">
        <div className="space-y-3">
          <Button variant="ghost" size="sm" asChild className="px-0">
            <Link href="/tickets">
              <ArrowLeft className="size-4" />
              Back to tickets
            </Link>
          </Button>
          <div>
            <div className="mb-3 flex flex-wrap items-center gap-2">
              <StatusBadge status={ticket.status} />
              <PriorityBadge priority={ticket.priority} />
            </div>
            <h1 className="max-w-4xl text-2xl font-semibold tracking-tight">
              {ticket.subject}
            </h1>
            <p className="mt-2 text-sm text-slate-500">
              Created{" "}
              {new Intl.DateTimeFormat("en", {
                month: "long",
                day: "numeric",
                hour: "2-digit",
                minute: "2-digit",
              }).format(new Date(ticket.createdAt))}
            </p>
          </div>
        </div>
        <div className="flex flex-col items-stretch gap-2 sm:flex-row md:items-center">
          <Button
            className="bg-emerald-600 text-white hover:bg-emerald-700"
            disabled={analyzeTicket.isPending}
            onClick={() => analyzeTicket.mutate()}
          >
            <Sparkles className="size-4" />
            {analyzeTicket.isPending ? "Analyzing..." : "Analyze with AI"}
          </Button>
          <Button
            variant="outline"
            disabled={generateDraftReply.isPending || !ticket.aiSummary}
            onClick={() => generateDraftReply.mutate()}
          >
            <Bot className="size-4" />
            {!ticket.aiSummary
              ? "Analyze first"
              : generateDraftReply.isPending
                ? "Drafting..."
                : "Draft reply"}
          </Button>
        </div>
      </div>

      <div className="grid gap-6 xl:grid-cols-[1fr_380px]">
        <div className="space-y-6">
          <Card className="border border-slate-200 bg-white shadow-sm">
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <MessageSquareText className="size-4 text-slate-500" />
                Customer issue
              </CardTitle>
            </CardHeader>
            <CardContent>
              <p className="whitespace-pre-wrap text-sm leading-7 text-slate-700">
                {ticket.description}
              </p>
            </CardContent>
          </Card>

          <Card className="border border-slate-200 bg-white shadow-sm">
            <CardHeader>
              <div className="flex flex-col gap-2 sm:flex-row sm:items-start sm:justify-between">
                <div>
                  <CardTitle className="flex items-center gap-2">
                    <Bot className="size-4 text-emerald-600" />
                    AI workspace
                  </CardTitle>
                  <p className="mt-1 text-sm text-slate-500">
                    Triage the request, gather context, then review the reply
                    before sending.
                  </p>
                </div>
                <div className="flex items-center gap-2 rounded-md border border-emerald-200 bg-emerald-50 px-3 py-1.5 text-xs font-medium text-emerald-700">
                  <Sparkles className="size-3.5" />
                  Human review required
                </div>
              </div>
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="grid gap-4 md:grid-cols-2">
                <div className="rounded-lg border border-slate-200 bg-slate-50 p-4">
                  <div className="mb-3 flex items-start justify-between gap-3">
                    <div className="flex items-center gap-2 text-sm font-medium">
                      <Sparkles className="size-4 text-emerald-600" />
                      AI triage
                    </div>
                    <StepState done={Boolean(ticket.aiSummary)} />
                  </div>
                  {ticket.aiSummary ? (
                    <div className="space-y-3">
                      <p className="text-sm leading-6 text-slate-700">
                        {ticket.aiSummary}
                      </p>
                      <div className="grid gap-2 text-xs text-slate-600 sm:grid-cols-3">
                        <MiniMetric
                          label="Category"
                          value={ticketCategoryLabels[ticket.category]}
                        />
                        <MiniMetric
                          label="Sentiment"
                          value={ticketSentimentLabels[ticket.sentiment]}
                        />
                        <MiniMetric
                          label="Priority"
                          value={ticketPriorityLabels[ticket.priority]}
                        />
                      </div>
                    </div>
                  ) : (
                    <p className="text-sm text-slate-500">
                      Run analysis to classify the ticket and generate a concise
                      support summary.
                    </p>
                  )}
                </div>
                <div className="rounded-lg border border-slate-200 bg-slate-50 p-4">
                  <div className="mb-3 flex items-start justify-between gap-3">
                    <div className="flex items-center gap-2 text-sm font-medium">
                      <BookOpen className="size-4 text-sky-600" />
                      Knowledge context
                    </div>
                    <span className="rounded-md border border-slate-200 bg-white px-2 py-1 text-xs font-medium text-slate-500">
                      {relatedKnowledge.length} matches
                    </span>
                  </div>
                  {isRelatedKnowledgeLoading ? (
                    <div className="space-y-2">
                      <Skeleton className="h-12 w-full" />
                      <Skeleton className="h-12 w-full" />
                    </div>
                  ) : isRelatedKnowledgeError ? (
                    <p className="text-sm text-rose-600">
                      Could not load related knowledge.
                    </p>
                  ) : relatedKnowledge.length === 0 ? (
                    <p className="text-sm text-slate-500">
                      No related knowledge found yet. Prepare articles for AI or
                      add more knowledge base content.
                    </p>
                  ) : (
                    <div className="space-y-2">
                      {relatedKnowledge.map((item, index) => (
                        <div
                          key={item.chunkId}
                          className="rounded-md border border-slate-200 bg-white p-3"
                        >
                          <div className="mb-1 flex items-start justify-between gap-3">
                            <p className="text-sm font-medium text-slate-800">
                              {item.articleTitle}
                            </p>
                            <span className="shrink-0 rounded-md bg-sky-50 px-2 py-1 text-xs font-medium text-sky-700">
                              {index === 0 ? "Best match" : "Related"}
                            </span>
                          </div>
                          <p className="line-clamp-3 text-xs leading-5 text-slate-500">
                            {item.content}
                          </p>
                        </div>
                      ))}
                    </div>
                  )}
                </div>
              </div>
              <div className="rounded-lg border border-slate-200">
                <div className="flex flex-col gap-3 border-b border-slate-200 bg-slate-50 p-4 sm:flex-row sm:items-center sm:justify-between">
                  <div>
                    <div className="flex items-center gap-2 text-sm font-medium">
                      <FileText className="size-4 text-slate-600" />
                      Draft reply
                    </div>
                    <p className="mt-1 text-xs text-slate-500">
                      Edit the suggested response before copying or sending.
                    </p>
                  </div>
                  <div className="flex flex-wrap gap-2">
                    <Button
                      variant="outline"
                      size="sm"
                      onClick={copyDraftReply}
                      disabled={!draftReply.trim()}
                    >
                      <Copy className="size-4" />
                      Copy
                    </Button>
                    <Button size="sm" disabled>
                      <Send className="size-4" />
                      Send later
                    </Button>
                  </div>
                </div>

                <Textarea
                  className="min-h-52 resize-y border-0 bg-white p-4 shadow-none focus-visible:ring-0"
                  placeholder="Draft reply will appear here..."
                  value={draftReply}
                  onChange={(event) =>
                    setDraftEdit({ ticketId, value: event.target.value })
                  }
                />
              </div>
            </CardContent>
          </Card>
        </div>

        <aside className="space-y-6">
          <Card className="border border-slate-200 bg-white shadow-sm">
            <CardHeader>
              <CardTitle>Ticket properties</CardTitle>
            </CardHeader>
            <CardContent className="space-y-4 text-sm">
              <Property
                label="Status"
                value={ticketStatusLabels[ticket.status]}
              />
              <Property
                label="Priority"
                value={ticketPriorityLabels[ticket.priority]}
              />
              <Property
                label="Category"
                value={ticketCategoryLabels[ticket.category]}
              />
              <Property
                label="Sentiment"
                value={ticketSentimentLabels[ticket.sentiment]}
              />
              <Property
                label="Channel"
                value={ticketChannelLabels[ticket.channel]}
              />
            </CardContent>
          </Card>

          <Card className="border border-slate-200 bg-white shadow-sm">
            <CardHeader>
              <CardTitle>Customer</CardTitle>
            </CardHeader>
            <CardContent className="space-y-3 text-sm">
              <div className="flex items-center gap-3">
                <div className="flex size-10 items-center justify-center rounded-full bg-emerald-100 text-emerald-700">
                  <User className="size-4" />
                </div>
                <div>
                  <p className="font-medium">{ticket.customerName}</p>
                  <p className="text-slate-500">Customer profile</p>
                </div>
              </div>
              <div className="flex items-center gap-2 text-slate-600">
                <Mail className="size-4" />
                {ticket.customerEmail}
              </div>
            </CardContent>
          </Card>

          <Card className="border border-slate-200 bg-white shadow-sm">
            <CardHeader>
              <CardTitle>Activity</CardTitle>
            </CardHeader>
            <CardContent className="space-y-4 text-sm">
              <TimelineItem
                title="Ticket created"
                description="Initial customer issue was captured."
              />
              <TimelineItem
                title={
                  ticket.aiSummary
                    ? "AI analysis completed"
                    : "Waiting for AI analysis"
                }
                description={
                  ticket.aiSummary
                    ? "AI triage updated summary, sentiment, priority, and category."
                    : "Run AI analysis to triage this ticket."
                }
              />
              {ticket.aiDraftReply && (
                <TimelineItem
                  title="Draft reply generated"
                  description="A suggested customer response is ready for review."
                />
              )}
            </CardContent>
          </Card>
        </aside>
      </div>
    </div>
  )
}

function StepState({ done }: { done: boolean }) {
  return (
    <span className="inline-flex items-center gap-1.5 rounded-md border border-slate-200 bg-white px-2 py-1 text-xs font-medium text-slate-500">
      {done ? (
        <>
          <BadgeCheck className="size-3.5 text-emerald-600" />
          Ready
        </>
      ) : (
        <>
          <Clock className="size-3.5" />
          Pending
        </>
      )}
    </span>
  )
}

function MiniMetric({ label, value }: { label: string; value: string }) {
  return (
    <div className="rounded-md border border-slate-200 bg-white px-3 py-2">
      <p className="text-[11px] font-medium uppercase text-slate-400">
        {label}
      </p>
      <p className="mt-1 font-medium text-slate-700">{value}</p>
    </div>
  )
}

function Property({ label, value }: { label: string; value: string }) {
  return (
    <div className="flex items-center justify-between gap-4 border-b border-slate-100 pb-3 last:border-0 last:pb-0">
      <span className="text-slate-500">{label}</span>
      <span className="font-medium">{value}</span>
    </div>
  )
}

function TimelineItem({
  title,
  description,
}: {
  title: string
  description: string
}) {
  return (
    <div className="flex gap-3">
      <div className="mt-0.5 flex size-7 shrink-0 items-center justify-center rounded-full bg-slate-100 text-slate-500">
        <Clock className="size-3.5" />
      </div>
      <div>
        <p className="font-medium">{title}</p>
        <p className="mt-1 text-slate-500">{description}</p>
      </div>
    </div>
  )
}
