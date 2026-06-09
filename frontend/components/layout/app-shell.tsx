"use client";

import {
  Bot,
  BookOpen,
  Inbox,
  LayoutDashboard,
  Settings,
  Ticket,
} from "lucide-react";
import Link from "next/link";
import { usePathname } from "next/navigation";
import { ReactNode } from "react";
import { cn } from "@/lib/utils";

const navItems = [
  { href: "/dashboard", label: "Dashboard", icon: LayoutDashboard },
  { href: "/tickets", label: "Tickets", icon: Ticket },
  { href: "/knowledge", label: "Knowledge", icon: BookOpen },
  { href: "/drafts", label: "AI Drafts", icon: Bot },
  { href: "/settings", label: "Settings", icon: Settings },
];

export function AppShell({ children }: { children: ReactNode }) {
  const pathname = usePathname();

  return (
    <div className="min-h-screen bg-slate-50 text-foreground">
      <aside className="fixed inset-y-0 left-0 hidden w-64 border-r border-slate-200 bg-white lg:block">
        <div className="flex h-16 items-center border-b px-6">
          <Link href="/tickets" className="flex items-center gap-3">
            <span className="flex size-9 items-center justify-center rounded-md bg-slate-950 text-white shadow-sm shadow-emerald-900/10">
              <Inbox className="size-4" />
            </span>
            <span>
              <span className="block text-sm font-semibold">SupportFlow</span>
              <span className="block text-xs text-emerald-700">AI Helpdesk</span>
            </span>
          </Link>
        </div>
        <nav className="space-y-1 p-3">
          {navItems.map((item) => (
            <Link
              key={item.href}
              href={item.href}
              className={cn(
                "flex h-10 items-center gap-3 rounded-md px-3 text-sm transition-colors",
                pathname.startsWith(item.href)
                  ? "bg-slate-950 text-white shadow-sm"
                  : "text-slate-500 hover:bg-slate-100 hover:text-slate-950",
              )}
            >
              <item.icon className="size-4" />
              {item.label}
            </Link>
          ))}
        </nav>
      </aside>

      <div className="lg:pl-64">
        <header className="sticky top-0 z-30 flex h-16 items-center justify-between border-b border-slate-200 bg-white/90 px-4 backdrop-blur lg:px-8">
          <div className="flex items-center gap-3">
            <div className="flex size-9 items-center justify-center rounded-md border bg-background lg:hidden">
              <Inbox className="size-4" />
            </div>
            <div>
              <p className="text-sm font-medium">Support workspace</p>
              <p className="text-xs text-slate-500">Tickets, knowledge, and AI drafts</p>
            </div>
          </div>
          <div className="hidden items-center gap-3 md:flex">
            <div className="text-right">
              <p className="text-sm font-medium">Support Agent</p>
              <p className="text-xs text-slate-500">Demo workspace</p>
            </div>
            <div className="flex size-9 items-center justify-center rounded-full bg-emerald-100 text-sm font-semibold text-emerald-700">
              SA
            </div>
          </div>
        </header>
        <main className="px-4 py-7 lg:px-8">{children}</main>
      </div>
    </div>
  );
}
