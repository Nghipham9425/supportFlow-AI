"use client";

import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Textarea } from "@/components/ui/textarea";
import { ticketsApi } from "@/lib/api";
import { CreateTicketInput, TicketChannel } from "@/types/ticket";
import { useMutation } from "@tanstack/react-query";
import { CheckCircle2, Send } from "lucide-react";
import { FormEvent, useState } from "react";

const initialForm: CreateTicketInput = {
  customerName: "",
  customerEmail: "",
  subject: "",
  description: "",
  channel: 2,
};

export function SupportTicketForm() {
  const [form, setForm] = useState<CreateTicketInput>(initialForm);
  const [submittedTicketId, setSubmittedTicketId] = useState<string | null>(null);

  const createTicket = useMutation({
    mutationFn: ticketsApi.create,
    onSuccess: (ticket) => {
      setSubmittedTicketId(ticket.id);
      setForm(initialForm);
    },
  });

  function updateField<K extends keyof CreateTicketInput>(
    key: K,
    value: CreateTicketInput[K],
  ) {
    setForm((current) => ({ ...current, [key]: value }));
  }

  function onSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    createTicket.mutate(form);
  }

  if (submittedTicketId) {
    return (
      <Card className="border border-emerald-200 bg-white shadow-sm">
        <CardContent className="flex min-h-80 flex-col items-center justify-center text-center">
          <div className="mb-4 flex size-12 items-center justify-center rounded-full bg-emerald-100 text-emerald-700">
            <CheckCircle2 className="size-6" />
          </div>
          <h2 className="text-xl font-semibold">Request submitted</h2>
          <p className="mt-2 max-w-md text-sm leading-6 text-slate-500">
            Your support request has been received. A support agent will review
            the ticket and follow up by email.
          </p>
          <p className="mt-4 rounded-md bg-slate-50 px-3 py-2 text-xs text-slate-500">
            Ticket ID: {submittedTicketId}
          </p>
          <Button
            className="mt-6 bg-emerald-600 text-white hover:bg-emerald-700"
            onClick={() => setSubmittedTicketId(null)}
          >
            Submit another request
          </Button>
        </CardContent>
      </Card>
    );
  }

  return (
    <Card className="border border-slate-200 bg-white shadow-sm">
      <CardHeader>
        <CardTitle>Submit a support request</CardTitle>
      </CardHeader>
      <CardContent>
        <form className="space-y-5" onSubmit={onSubmit}>
          <div className="grid gap-4 sm:grid-cols-2">
            <Input
              placeholder="Your name"
              value={form.customerName}
              onChange={(event) => updateField("customerName", event.target.value)}
              required
            />
            <Input
              type="email"
              placeholder="Email address"
              value={form.customerEmail}
              onChange={(event) =>
                updateField("customerEmail", event.target.value)
              }
              required
            />
          </div>
          <Input
            placeholder="What do you need help with?"
            value={form.subject}
            onChange={(event) => updateField("subject", event.target.value)}
            required
          />
          <Textarea
            className="min-h-36"
            placeholder="Describe the issue, what you expected, and anything you already tried."
            value={form.description}
            onChange={(event) => updateField("description", event.target.value)}
            required
          />
          <Select
            value={String(form.channel)}
            onValueChange={(value) =>
              updateField("channel", Number(value) as TicketChannel)
            }
          >
            <SelectTrigger className="w-full">
              <SelectValue placeholder="Contact channel" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="2">Web form</SelectItem>
              <SelectItem value="1">Email</SelectItem>
              <SelectItem value="3">Chat</SelectItem>
              <SelectItem value="4">Phone</SelectItem>
              <SelectItem value="5">Social</SelectItem>
            </SelectContent>
          </Select>
          {createTicket.isError && (
            <p className="rounded-md bg-rose-50 px-3 py-2 text-sm text-rose-700">
              Could not submit your request. Please try again.
            </p>
          )}
          <Button
            type="submit"
            className="w-full bg-emerald-600 text-white hover:bg-emerald-700"
            disabled={createTicket.isPending}
          >
            <Send className="size-4" />
            {createTicket.isPending ? "Submitting..." : "Submit request"}
          </Button>
        </form>
      </CardContent>
    </Card>
  );
}
