"use client"

import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Skeleton } from "@/components/ui/skeleton"
import { Textarea } from "@/components/ui/textarea"
import { ticketsApi } from "@/lib/api"
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query"
import { Clock, MessageSquareText } from "lucide-react"
import { FormEvent, useState } from "react"
import { toast } from "sonner"

export function TicketActivityPanel({ ticketId }: { ticketId: string }) {
  const queryClient = useQueryClient()
  const [noteContent, setNoteContent] = useState("")

  const {
    data: activities = [],
    isLoading,
    isError,
  } = useQuery({
    queryKey: ["tickets", ticketId, "activities"],
    queryFn: () => ticketsApi.getActivities(ticketId),
  })

  const addNote = useMutation({
    mutationFn: (content: string) =>
      ticketsApi.addNote(ticketId, { content }),
    onSuccess: () => {
      setNoteContent("")
      queryClient.invalidateQueries({
        queryKey: ["tickets", ticketId, "activities"],
      })
      toast.success("Internal note added")
    },
    onError: (error) => {
      toast.error(
        error instanceof Error ? error.message : "Could not add internal note",
      )
    },
  })

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()

    const content = noteContent.trim()

    if (!content) {
      return
    }

    addNote.mutate(content)
  }

  return (
    <Card className="border border-slate-200 bg-white shadow-sm">
      <CardHeader>
        <CardTitle>Activity</CardTitle>
      </CardHeader>
      <CardContent className="space-y-4 text-sm">
        <form
          className="space-y-3 border-b border-slate-200 pb-4"
          onSubmit={handleSubmit}
        >
          <div>
            <label
              className="text-sm font-medium text-slate-900"
              htmlFor={`internal-note-${ticketId}`}
            >
              Internal note
            </label>
            <p className="mt-1 text-xs text-slate-500">
              Visible to support agents only.
            </p>
          </div>

          <Textarea
            id={`internal-note-${ticketId}`}
            value={noteContent}
            onChange={(event) => setNoteContent(event.target.value)}
            placeholder="Add context for other agents..."
            maxLength={2000}
            rows={4}
          />

          <div className="flex items-center justify-between gap-3">
            <span className="text-xs text-slate-400">
              {noteContent.length}/2000
            </span>
            <Button
              type="submit"
              size="sm"
              disabled={!noteContent.trim() || addNote.isPending}
            >
              <MessageSquareText className="size-4" />
              {addNote.isPending ? "Adding..." : "Add note"}
            </Button>
          </div>
        </form>

        {isLoading ? (
          <Skeleton className="h-24 w-full" />
        ) : isError ? (
          <p className="text-sm text-destructive">
            Could not load ticket activity.
          </p>
        ) : activities.length === 0 ? (
          <p className="text-sm text-slate-500">
            No activity has been recorded.
          </p>
        ) : (
          activities.map((activity) => (
            <TimelineItem
              key={activity.id}
              title={activity.message}
              description={`${activity.actorUserName ?? "System"} · ${new Date(
                activity.createdAt,
              ).toLocaleString()}`}
            />
          ))
        )}
      </CardContent>
    </Card>
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
      <div className="min-w-0">
        <p className="break-words font-medium">{title}</p>
        <p className="mt-1 break-words text-slate-500">{description}</p>
      </div>
    </div>
  )
}
