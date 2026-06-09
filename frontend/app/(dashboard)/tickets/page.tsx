"use client";

import { CreateTicketDialog } from "@/components/tickets/create-ticket-dialog";
import { TicketsTable } from "@/components/tickets/tickets-table";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Skeleton } from "@/components/ui/skeleton";
import { Tabs, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { ticketsApi } from "@/lib/api";
import { useQuery } from "@tanstack/react-query";
import { AlertTriangle, CheckCircle2, Clock, Search, Ticket } from "lucide-react";
import { useMemo, useState } from "react";

type TicketFilter = "all" | "open" | "high" | "resolved";

export default function TicketsPage() {
  const [search, setSearch] = useState("");
  const [filter, setFilter] = useState<TicketFilter>("all");

  const { data: tickets = [], isLoading, isError } = useQuery({
    queryKey: ["tickets"],
    queryFn: ticketsApi.list,
  });

  const openTickets = tickets.filter((ticket) => ticket.status === 1).length;
  const highPriority = tickets.filter((ticket) => ticket.priority >= 3).length;
  const resolvedTickets = tickets.filter((ticket) => ticket.status === 6).length;
  const filteredTickets = useMemo(() => {
    const normalizedSearch = search.trim().toLowerCase();

    return tickets.filter((ticket) => {
      const matchesFilter =
        filter === "all" ||
        (filter === "open" && ticket.status === 1) ||
        (filter === "high" && ticket.priority >= 3) ||
        (filter === "resolved" && ticket.status === 6);

      const matchesSearch =
        normalizedSearch.length === 0 ||
        ticket.customerName.toLowerCase().includes(normalizedSearch) ||
        ticket.customerEmail.toLowerCase().includes(normalizedSearch) ||
        ticket.subject.toLowerCase().includes(normalizedSearch) ||
        ticket.description.toLowerCase().includes(normalizedSearch);

      return matchesFilter && matchesSearch;
    });
  }, [filter, search, tickets]);

  return (
    <div className="space-y-7">
      <div className="flex flex-col gap-4 md:flex-row md:items-start md:justify-between">
        <div>
          <h1 className="text-2xl font-semibold tracking-tight">Tickets</h1>
          <p className="mt-1 text-sm text-muted-foreground">
            Track incoming support requests and prepare them for AI triage.
          </p>
        </div>
        <CreateTicketDialog />
      </div>

      <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
        <MetricCard
          icon={Ticket}
          label="Total tickets"
          value={tickets.length}
          accent="text-slate-700 bg-slate-100"
        />
        <MetricCard
          icon={Clock}
          label="Open"
          value={openTickets}
          accent="text-sky-700 bg-sky-50"
        />
        <MetricCard
          icon={AlertTriangle}
          label="High priority"
          value={highPriority}
          accent="text-amber-700 bg-amber-50"
        />
        <MetricCard
          icon={CheckCircle2}
          label="Resolved"
          value={resolvedTickets}
          accent="text-emerald-700 bg-emerald-50"
        />
      </div>

      <div className="rounded-xl border border-slate-200 bg-white p-4 shadow-sm">
        <div className="flex flex-col gap-4 lg:flex-row lg:items-center lg:justify-between">
          <Tabs value={filter} onValueChange={(value) => setFilter(value as TicketFilter)}>
            <TabsList>
              <TabsTrigger value="all">All</TabsTrigger>
              <TabsTrigger value="open">Open</TabsTrigger>
              <TabsTrigger value="high">High priority</TabsTrigger>
              <TabsTrigger value="resolved">Resolved</TabsTrigger>
            </TabsList>
          </Tabs>
          <div className="relative w-full lg:w-80">
            <Search className="pointer-events-none absolute left-3 top-1/2 size-4 -translate-y-1/2 text-slate-400" />
            <Input
              className="h-10 rounded-md border border-slate-200 bg-white pl-10 pr-3 shadow-sm focus-visible:border-emerald-500"
              placeholder="Search customer, subject, issue..."
              value={search}
              onChange={(event) => setSearch(event.target.value)}
            />
          </div>
        </div>
      </div>

      {isLoading ? (
        <div className="space-y-3">
          <Skeleton className="h-12 w-full" />
          <Skeleton className="h-72 w-full" />
        </div>
      ) : isError ? (
        <div className="rounded-md border bg-background p-8 text-sm text-destructive">
          Could not load tickets. Check that the ASP.NET API is running on port 5059.
        </div>
      ) : (
        <TicketsTable tickets={filteredTickets} />
      )}
    </div>
  );
}

function MetricCard({
  icon: Icon,
  label,
  value,
  accent,
}: {
  icon: React.ComponentType<{ className?: string }>;
  label: string;
  value: number;
  accent: string;
}) {
  return (
    <Card className="border border-slate-200 bg-white shadow-sm">
      <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
        <CardTitle className="text-sm font-medium text-muted-foreground">
          {label}
        </CardTitle>
        <span className={`flex size-8 items-center justify-center rounded-md ${accent}`}>
          <Icon className="size-4" />
        </span>
      </CardHeader>
      <CardContent>
        <p className="text-2xl font-semibold">{value}</p>
      </CardContent>
    </Card>
  );
}
