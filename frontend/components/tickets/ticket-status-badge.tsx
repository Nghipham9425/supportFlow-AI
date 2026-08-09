import { Badge } from "@/components/ui/badge";
import { cn } from "@/lib/utils";
import {
  TicketPriority,
  TicketStatus,
  ticketPriorityLabels,
  ticketStatusLabels,
} from "@/types/ticket";

export function StatusBadge({ status }: { status: TicketStatus }) {
  const colorByStatus: Record<TicketStatus, string> = {
    1: "border-sky-200 bg-sky-50 text-sky-700",
    2: "border-violet-200 bg-violet-50 text-violet-700",
    3: "border-cyan-200 bg-cyan-50 text-cyan-700",
    4: "border-emerald-200 bg-emerald-50 text-emerald-700",
    5: "border-rose-200 bg-rose-50 text-rose-700",
    6: "border-amber-200 bg-amber-50 text-amber-700",
    7: "border-emerald-200 bg-emerald-50 text-emerald-700",
    8: "border-slate-200 bg-slate-100 text-slate-700",
  };

  return (
    <Badge className={cn("rounded-full border px-2.5 py-1", colorByStatus[status])}>
      {ticketStatusLabels[status]}
    </Badge>
  );
}

export function PriorityBadge({ priority }: { priority: TicketPriority }) {
  const colorByPriority: Record<TicketPriority, string> = {
    1: "border-slate-200 bg-slate-100 text-slate-700",
    2: "border-sky-200 bg-sky-50 text-sky-700",
    3: "border-amber-200 bg-amber-50 text-amber-700",
    4: "border-rose-200 bg-rose-50 text-rose-700",
  };

  return (
    <Badge
      className={cn(
        "rounded-full border px-2.5 py-1",
        colorByPriority[priority],
      )}
    >
      {ticketPriorityLabels[priority]}
    </Badge>
  );
}
