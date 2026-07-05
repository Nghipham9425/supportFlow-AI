"use client";

import { PriorityBadge, StatusBadge } from "@/components/tickets/ticket-status-badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";
import { Textarea } from "@/components/ui/textarea";
import { ticketsApi } from "@/lib/api";
import {
  ticketCategoryLabels,
  ticketChannelLabels,
  ticketPriorityLabels,
  ticketSentimentLabels,
  ticketStatusLabels,
} from "@/types/ticket";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  ArrowLeft,
  Bot,
  BookOpen,
  Clock,
  Mail,
  MessageSquareText,
  Sparkles,
  User,
} from "lucide-react";
import Link from "next/link";
import { toast } from "sonner";

export function TicketDetail({ ticketId }: { ticketId: string }) {
  const queryClient = useQueryClient();

  const {
    data: ticket,
    isLoading,
    isError,
  } = useQuery({
    queryKey: ["tickets", ticketId],
    queryFn: () => ticketsApi.getById(ticketId),
  });

  const analyzeTicket = useMutation({
    mutationFn: () => ticketsApi.analyze(ticketId),
    onSuccess: (updatedTicket) => {
      queryClient.setQueryData(["tickets", ticketId], updatedTicket);
      queryClient.invalidateQueries({ queryKey: ["tickets"] });
      toast.success("Ticket analyzed with mock AI");
    },
    onError: () => {
      toast.error("Could not analyze ticket");
    },
  });

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
    );
  }

  if (isError || !ticket) {
    return (
      <div className="rounded-xl border border-rose-200 bg-rose-50 p-8 text-sm text-rose-700">
        Could not load this ticket.
      </div>
    );
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
        <Button
          className="bg-emerald-600 text-white hover:bg-emerald-700"
          disabled={analyzeTicket.isPending}
          onClick={() => analyzeTicket.mutate()}
        >
          <Sparkles className="size-4" />
          {analyzeTicket.isPending ? "Analyzing..." : "Analyze with AI"}
        </Button>
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
              <CardTitle className="flex items-center gap-2">
                <Bot className="size-4 text-emerald-600" />
                AI workspace
              </CardTitle>
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="grid gap-4 md:grid-cols-2">
                <div className="rounded-lg border border-dashed border-slate-200 bg-slate-50 p-4">
                  <div className="mb-2 flex items-center gap-2 text-sm font-medium">
                    <Sparkles className="size-4 text-emerald-600" />
                    AI summary
                  </div>
                  {ticket.aiSummary ? (
                    <p className="text-sm leading-6 text-slate-700">
                      {ticket.aiSummary}
                    </p>
                  ) : (
                    <p className="text-sm text-slate-500">
                      Run analysis to classify the ticket and generate a concise
                      support summary.
                    </p>
                  )}
                </div>
                <div className="rounded-lg border border-dashed border-slate-200 bg-slate-50 p-4">
                  <div className="mb-2 flex items-center gap-2 text-sm font-medium">
                    <BookOpen className="size-4 text-sky-600" />
                    Related knowledge
                  </div>
                  <p className="text-sm text-slate-500">
                    Retrieved articles and citations will appear here after RAG
                    is connected.
                  </p>
                </div>
              </div>
              <Textarea
                className="min-h-36 bg-white"
                placeholder="Draft reply will appear here..."
                disabled
              />
            </CardContent>
          </Card>
        </div>

        <aside className="space-y-6">
          <Card className="border border-slate-200 bg-white shadow-sm">
            <CardHeader>
              <CardTitle>Ticket properties</CardTitle>
            </CardHeader>
            <CardContent className="space-y-4 text-sm">
              <Property label="Status" value={ticketStatusLabels[ticket.status]} />
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
              <Property label="Channel" value={ticketChannelLabels[ticket.channel]} />
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
                title={ticket.aiSummary ? "AI analysis completed" : "Waiting for AI analysis"}
                description={
                  ticket.aiSummary
                    ? "Mock AI triage updated summary, sentiment, priority, and category."
                    : "Run AI analysis to triage this ticket."
                }
              />
            </CardContent>
          </Card>
        </aside>
      </div>
    </div>
  );
}

function Property({ label, value }: { label: string; value: string }) {
  return (
    <div className="flex items-center justify-between gap-4 border-b border-slate-100 pb-3 last:border-0 last:pb-0">
      <span className="text-slate-500">{label}</span>
      <span className="font-medium">{value}</span>
    </div>
  );
}

function TimelineItem({
  title,
  description,
}: {
  title: string;
  description: string;
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
  );
}
