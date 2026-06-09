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
                  <AlertDialog>
                    <AlertDialogTrigger asChild>
                      <Button
                        variant="ghost"
                        size="icon-sm"
                        onClick={(event) => event.stopPropagation()}
                        disabled={deleteTicket.isPending}
                      >
                        <Trash2 className="size-4" />
                        <span className="sr-only">Delete ticket</span>
                      </Button>
                    </AlertDialogTrigger>
                    <AlertDialogContent
                      onClick={(event) => event.stopPropagation()}
                    >
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
                        <AlertDialogAction
                          variant="destructive"
                          onClick={() => deleteTicket.mutate(ticket.id)}
                        >
                          Delete ticket
                        </AlertDialogAction>
                      </AlertDialogFooter>
                    </AlertDialogContent>
                  </AlertDialog>
                </div>
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </div>
  );
}
