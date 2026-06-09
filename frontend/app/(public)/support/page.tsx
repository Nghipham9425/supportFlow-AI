import { SupportTicketForm } from "@/components/support/support-ticket-form";
import { ArrowRight, Bot, Inbox, ShieldCheck } from "lucide-react";
import Link from "next/link";

export default function SupportPage() {
  return (
    <main className="min-h-screen bg-slate-50">
      <header className="border-b border-slate-200 bg-white">
        <div className="mx-auto flex h-16 max-w-6xl items-center justify-between px-4">
          <Link href="/support" className="flex items-center gap-3">
            <span className="flex size-9 items-center justify-center rounded-md bg-slate-950 text-white">
              <Inbox className="size-4" />
            </span>
            <span>
              <span className="block text-sm font-semibold">SupportFlow</span>
              <span className="block text-xs text-emerald-700">Customer support</span>
            </span>
          </Link>
          <Link
            href="/tickets"
            className="hidden items-center gap-2 text-sm font-medium text-slate-600 hover:text-slate-950 sm:flex"
          >
            Agent workspace
            <ArrowRight className="size-4" />
          </Link>
        </div>
      </header>

      <section className="mx-auto grid max-w-6xl gap-8 px-4 py-10 lg:grid-cols-[1fr_520px] lg:py-16">
        <div className="flex flex-col justify-center">
          <div className="mb-6 inline-flex w-fit items-center gap-2 rounded-full border border-emerald-200 bg-emerald-50 px-3 py-1 text-sm text-emerald-700">
            <Bot className="size-4" />
            AI-assisted support workflow
          </div>
          <h1 className="max-w-2xl text-4xl font-semibold tracking-tight text-slate-950 lg:text-5xl">
            Get help from the support team
          </h1>
          <p className="mt-4 max-w-xl text-base leading-7 text-slate-600">
            Send your issue to the support queue. Our team can review your
            request, use AI-assisted triage, and follow up with the right next
            steps.
          </p>
          <div className="mt-8 grid gap-3 text-sm text-slate-600 sm:grid-cols-3">
            <Feature icon={Inbox} title="Submit" text="Create a request in minutes." />
            <Feature icon={Bot} title="Triage" text="AI helps organize the queue." />
            <Feature icon={ShieldCheck} title="Review" text="Agents approve every reply." />
          </div>
        </div>

        <SupportTicketForm />
      </section>
    </main>
  );
}

function Feature({
  icon: Icon,
  title,
  text,
}: {
  icon: React.ComponentType<{ className?: string }>;
  title: string;
  text: string;
}) {
  return (
    <div className="rounded-xl border border-slate-200 bg-white p-4 shadow-sm">
      <Icon className="mb-3 size-5 text-emerald-600" />
      <p className="font-medium text-slate-950">{title}</p>
      <p className="mt-1 text-sm leading-5 text-slate-500">{text}</p>
    </div>
  );
}
