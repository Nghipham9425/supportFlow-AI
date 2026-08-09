import {
  CreateTicketInput,
  RelatedKnowledge,
  SendTicketReplyInput,
  Ticket,
  TicketReply,
} from "@/types/ticket"
import {
  CreateKnowledgeArticleInput,
  KnowledgeArticle,
  KnowledgeChunk,
  UpdateKnowledgeArticleInput,
} from "@/types/knowledge"
import { DashboardSummary } from "@/types/dashboard"
import { AuthResponse, LoginInput } from "@/types/auth"
import { getAccessToken } from "./auth-session"

const API_BASE_URL =
  process.env.NEXT_PUBLIC_API_BASE_URL ?? "http://localhost:5059/api"
async function request<T>(path: string, options?: RequestInit): Promise<T> {
  const accessToken = getAccessToken()
  const response = await fetch(`${API_BASE_URL}${path}`, {
    headers: {
      "Content-Type": "application/json",
      ...(accessToken ? { Authorization: `Bearer ${accessToken}` } : {}),
      ...options?.headers,
    },
    ...options,
  })

  if (!response.ok) {
    const errorBody = await response.json().catch(() => null)

    const message =
      errorBody && typeof errorBody.message === "string"
        ? errorBody.message
        : `API request failed: ${response.status}`

    throw new Error(message)
  }

  if (response.status === 204) {
    return undefined as T
  }

  return response.json() as Promise<T>
}

export const dashboardApi = {
  getSummary: () => request<DashboardSummary>("/dashboard/summary"),
}

export const ticketsApi = {
  list: () => request<Ticket[]>("/tickets"),
  getById: (id: string) => request<Ticket>(`/tickets/${id}`),
  create: (input: CreateTicketInput) =>
    request<Ticket>("/tickets", {
      method: "POST",
      body: JSON.stringify(input),
    }),
  analyze: (id: string) =>
    request<Ticket>(`/tickets/${id}/analyze`, {
      method: "POST",
    }),

  generateDraftReply: (id: string) =>
    request<Ticket>(`/tickets/${id}/draft-reply`, {
      method: "POST",
    }),
  assignToMe: (id: string) =>
    request<Ticket>(`/tickets/${id}/assign-to-me`, { method: "POST" }),
  sendReply: (id: string, input: SendTicketReplyInput) =>
    request<TicketReply>(`/tickets/${id}/replies`, {
      method: "POST",
      body: JSON.stringify(input),
    }),
  getReplies: (id: string) =>
    request<TicketReply[]>(`/tickets/${id}/replies`),
  remove: (id: string) =>
    request<void>(`/tickets/${id}`, {
      method: "DELETE",
    }),
  getRelatedKnowledge: (id: string) =>
    request<RelatedKnowledge[]>(`/tickets/${id}/related-knowledge`),
}

export const knowledgeApi = {
  list: () => request<KnowledgeArticle[]>("/knowledge-articles"),
  create: (input: CreateKnowledgeArticleInput) =>
    request<KnowledgeArticle>("/knowledge-articles", {
      method: "POST",
      body: JSON.stringify(input),
    }),
  remove: (id: string) =>
    request<void>(`/knowledge-articles/${id}`, {
      method: "DELETE",
    }),
  update: (id: string, input: UpdateKnowledgeArticleInput) =>
    request<KnowledgeArticle>(`/knowledge-articles/${id}`, {
      method: "PATCH",
      body: JSON.stringify(input),
    }),
  getChunks: (id: string) =>
    request<KnowledgeChunk[]>(`/knowledge-articles/${id}/chunks`),
  regenerateChunks: (id: string) =>
    request<KnowledgeChunk[]>(`/knowledge-articles/${id}/chunks/regenerate`, {
      method: "POST",
    }),

  generateEmbeddings: (id: string) =>
    request<KnowledgeChunk[]>(`/knowledge-articles/${id}/chunks/embed`, {
      method: "POST",
    }),
}

export const authApi = {
  login: (input: LoginInput) =>
    request<AuthResponse>("/auth/login", {
      method: "POST",
      body: JSON.stringify(input),
    }),
}
