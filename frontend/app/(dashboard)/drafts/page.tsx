"use client"
import { ticketsApi } from "@/lib/api"
import { useQuery } from "@tanstack/react-query"
import { Skeleton } from "@/components/ui/skeleton"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { ticketPriorityLabels, ticketStatusLabels } from "@/types/ticket"
import { FileText } from "lucide-react"
import Link from "next/link"

export default function DraftsPage() {
  const {
    data: tickets = [],
    isLoading,
    isError,
  } = useQuery({
    queryKey: ["tickets"],
    queryFn: ticketsApi.list,
  })

  const draftedTickets = tickets.filter(
    (ticket) => ticket.status === 3 && ticket.aiDraftReply,
  )

  if (isLoading) {
    return (
      <div className="space-y-7">
        <Skeleton className="h-10 w-64" />
        <div className="space-y-3">
          <Skeleton className="h-28 w-full" />
          <Skeleton className="h-28 w-full" />
          <Skeleton className="h-28 w-full" />
        </div>
      </div>
    )
  }

  if (isError) {
    return (
      <div className="rounded-md border bg-background p-8 text-sm text-destructive">
        Could not load AI drafts. Check that the ASP.NET API is running on port
        5059.
      </div>
    )
  }

  return (
    <div className="space-y-7">
      <div>
        <h1 className="text-2xl font-semibold tracking-tight">AI Drafts</h1>
        <p className="mt-1 text-sm text-muted-foreground">
          Review generated replies before sending them to customers.
        </p>
      </div>
      {draftedTickets.length === 0 ? (
        <div className="rounded-md border border-slate-200 bg-white p-8 text-sm text-muted-foreground shadow-sm">
          No AI drafts yet. Generate a draft reply from a ticket detail page
          first.
        </div>
      ) : (
        <div className="grid gap-4">
          {draftedTickets.map((ticket) => (
            <Card
              key={ticket.id}
              className="border border-slate-200 bg-white shadow-sm"
            >
              <CardHeader className="flex flex-row items-start justify-between gap-4 space-y-0">
                <div>
                  <CardTitle className="text-base">{ticket.subject}</CardTitle>
                  <p className="mt-1 text-sm text-muted-foreground">
                    {ticket.customerName} · {ticket.customerEmail}
                  </p>
                </div>
                <div className="flex shrink-0 items-center gap-2 text-xs">
                  <span className="rounded-md bg-sky-50 px-2 py-1 font-medium text-sky-700">
                    {ticketStatusLabels[ticket.status]}
                  </span>
                  <span className="rounded-md bg-amber-50 px-2 py-1 font-medium text-amber-700">
                    {ticketPriorityLabels[ticket.priority]}
                  </span>
                </div>
              </CardHeader>
              <CardContent className="space-y-4">
                <div className="rounded-md border border-slate-200 bg-slate-50 p-4">
                  <div className="mb-2 flex items-center gap-2 text-xs font-medium uppercase tracking-wide text-slate-500">
                    <FileText className="size-3.5" />
                    Draft preview
                  </div>
                  <p className="line-clamp-4 whitespace-pre-line text-sm leading-6 text-slate-700">
                    {ticket.aiDraftReply}
                  </p>
                </div>

                <div className="flex justify-end">
                  <Button asChild size="sm">
                    <Link href={`/tickets/${ticket.id}`}>Review</Link>
                  </Button>
                </div>
              </CardContent>
            </Card>
          ))}
        </div>
      )}
    </div>
  )
}
