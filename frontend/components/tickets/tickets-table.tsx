"use client";

import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogMedia,
  AlertDialogTitle,
  AlertDialogTrigger,
} from "@/components/ui/alert-dialog";
import { Button } from "@/components/ui/button";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { ticketsApi } from "@/lib/api";
import { Ticket, ticketChannelLabels } from "@/types/ticket";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { AlertTriangle, Inbox, Trash2 } from "lucide-react";
import { useRouter } from "next/navigation";
import { toast } from "sonner";
import { PriorityBadge, StatusBadge } from "./ticket-status-badge";

export function TicketsTable({ tickets }: { tickets: Ticket[] }) {
  const router = useRouter();
  const queryClient = useQueryClient();
  const deleteTicket = useMutation({
    mutationFn: ticketsApi.remove,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["tickets"] });
      toast.success("Ticket deleted");
    },
    onError: () => toast.error("Could not delete ticket"),
  });

  if (tickets.length === 0) {
    return (
      <div className="flex min-h-64 items-center justify-center rounded-xl border border-slate-200 bg-white">
        <div className="text-center">
          <div className="mx-auto mb-3 flex size-10 items-center justify-center rounded-full bg-muted">
            <Inbox className="size-5 text-muted-foreground" />
          </div>
          <p className="text-sm font-medium">No tickets found</p>
          <p className="mt-1 text-sm text-muted-foreground">
            Create a ticket or adjust your filters to continue.
          </p>
        </div>
      </div>
    );
  }

  return (
    <div className="overflow-hidden rounded-xl border border-slate-200 bg-white shadow-sm">
      <div className="divide-y divide-slate-200 md:hidden">
        {tickets.map((ticket) => (
          <div
            key={ticket.id}
            className="cursor-pointer space-y-3 p-4 active:bg-slate-50"
            onClick={() => router.push(`/tickets/${ticket.id}`)}
          >
            <div className="flex items-start justify-between gap-3">
              <div className="min-w-0">
                <p className="truncate font-medium">{ticket.customerName}</p>
                <p className="truncate text-xs text-muted-foreground">
                  {ticket.customerEmail}
                </p>
              </div>
              <TicketDeleteAction
                ticket={ticket}
                isPending={deleteTicket.isPending}
                onDelete={() => deleteTicket.mutate(ticket.id)}
              />
            </div>
            <div>
              <p className="line-clamp-2 text-sm font-medium">{ticket.subject}</p>
              <p className="mt-1 line-clamp-2 text-xs text-muted-foreground">
                {ticket.description}
              </p>
            </div>
            <div className="flex flex-wrap items-center gap-2">
              <PriorityBadge priority={ticket.priority} />
              <StatusBadge status={ticket.status} />
              <span className="text-xs text-muted-foreground">
                {ticketChannelLabels[ticket.channel]}
              </span>
            </div>
          </div>
        ))}
      </div>

      <div className="hidden overflow-x-auto md:block">
      <Table>
        <TableHeader>
          <TableRow className="bg-slate-50">
            <TableHead>Customer</TableHead>
            <TableHead>Subject</TableHead>
            <TableHead>Channel</TableHead>
            <TableHead>Priority</TableHead>
            <TableHead>Status</TableHead>
            <TableHead>Created</TableHead>
            <TableHead className="w-12" />
          </TableRow>
        </TableHeader>
        <TableBody>
          {tickets.map((ticket) => (
            <TableRow
              key={ticket.id}
              className="group cursor-pointer hover:bg-slate-50/80"
              onClick={() => router.push(`/tickets/${ticket.id}`)}
            >
              <TableCell>
                <div>
                  <p className="font-medium">{ticket.customerName}</p>
                  <p className="text-xs text-muted-foreground">
                    {ticket.customerEmail}
                  </p>
                </div>
              </TableCell>
              <TableCell>
                <div className="max-w-lg">
                  <p className="truncate font-medium">{ticket.subject}</p>
                  <p className="truncate text-xs text-muted-foreground">
                    {ticket.description}
                  </p>
                </div>
              </TableCell>
              <TableCell>{ticketChannelLabels[ticket.channel]}</TableCell>
              <TableCell>
                <PriorityBadge priority={ticket.priority} />
              </TableCell>
              <TableCell>
                <StatusBadge status={ticket.status} />
              </TableCell>
              <TableCell className="text-muted-foreground">
                {new Intl.DateTimeFormat("en", {
                  month: "short",
                  day: "numeric",
                  hour: "2-digit",
                  minute: "2-digit",
                }).format(new Date(ticket.createdAt))}
              </TableCell>
              <TableCell>
                <div className="flex items-center justify-end opacity-70 transition-opacity group-hover:opacity-100">
                  <TicketDeleteAction
                    ticket={ticket}
                    isPending={deleteTicket.isPending}
                    onDelete={() => deleteTicket.mutate(ticket.id)}
                  />
                </div>
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
      </div>
    </div>
  );
}

function TicketDeleteAction({
  ticket,
  isPending,
  onDelete,
}: {
  ticket: Ticket;
  isPending: boolean;
  onDelete: () => void;
}) {
  return (
    <AlertDialog>
      <AlertDialogTrigger asChild>
        <Button
          variant="ghost"
          size="icon-sm"
          onClick={(event) => event.stopPropagation()}
          disabled={isPending}
        >
          <Trash2 className="size-4" />
          <span className="sr-only">Delete ticket</span>
        </Button>
      </AlertDialogTrigger>
      <AlertDialogContent onClick={(event) => event.stopPropagation()}>
        <AlertDialogHeader>
          <AlertDialogMedia className="bg-rose-50 text-rose-700">
            <AlertTriangle className="size-5" />
          </AlertDialogMedia>
          <AlertDialogTitle>Delete ticket?</AlertDialogTitle>
          <AlertDialogDescription>
            This will permanently delete the ticket from{" "}
            <span className="font-medium text-foreground">
              {ticket.customerName}
            </span>
            . This action cannot be undone.
          </AlertDialogDescription>
        </AlertDialogHeader>
        <AlertDialogFooter>
          <AlertDialogCancel>Cancel</AlertDialogCancel>
          <AlertDialogAction variant="destructive" onClick={onDelete}>
            Delete ticket
          </AlertDialogAction>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  );
}
