"use client";

import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
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
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { Plus } from "lucide-react";
import { FormEvent, useState } from "react";
import { toast } from "sonner";

const initialForm: CreateTicketInput = {
  customerName: "",
  customerEmail: "",
  subject: "",
  description: "",
  channel: 2,
};

export function CreateTicketDialog() {
  const [open, setOpen] = useState(false);
  const [form, setForm] = useState<CreateTicketInput>(initialForm);
  const queryClient = useQueryClient();

  const createTicket = useMutation({
    mutationFn: ticketsApi.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["tickets"] });
      setForm(initialForm);
      setOpen(false);
      toast.success("Ticket created");
    },
    onError: () => toast.error("Could not create ticket"),
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

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>
        <Button variant="outline">
          <Plus className="size-4" />
          Manual ticket
        </Button>
      </DialogTrigger>
      <DialogContent className="sm:max-w-xl">
        <DialogHeader>
          <DialogTitle>Create manual ticket</DialogTitle>
          <DialogDescription>
            Add a support ticket manually when the customer reaches your team
            through phone, chat, or another external channel.
          </DialogDescription>
        </DialogHeader>
        <form className="space-y-5" onSubmit={onSubmit}>
          <div className="grid gap-4 sm:grid-cols-2">
            <Input
              placeholder="Customer name"
              value={form.customerName}
              onChange={(event) => updateField("customerName", event.target.value)}
              required
            />
            <Input
              type="email"
              placeholder="Customer email"
              value={form.customerEmail}
              onChange={(event) =>
                updateField("customerEmail", event.target.value)
              }
              required
            />
          </div>
          <Input
            placeholder="Subject"
            value={form.subject}
            onChange={(event) => updateField("subject", event.target.value)}
            required
          />
          <Textarea
            placeholder="Describe the customer issue"
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
              <SelectValue placeholder="Channel" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="1">Email</SelectItem>
              <SelectItem value="2">Web</SelectItem>
              <SelectItem value="3">Chat</SelectItem>
              <SelectItem value="4">Phone</SelectItem>
              <SelectItem value="5">Social</SelectItem>
            </SelectContent>
          </Select>
          <DialogFooter>
            <Button
              type="submit"
              className="bg-emerald-600 text-white hover:bg-emerald-700"
              disabled={createTicket.isPending}
            >
              {createTicket.isPending ? "Creating..." : "Create ticket"}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
