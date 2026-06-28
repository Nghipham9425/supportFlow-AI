import { CreateTicketInput, Ticket } from "@/types/ticket"
import {
  CreateKnowledgeArticleInput,
  KnowledgeArticle,
  KnowledgeChunk,
  UpdateKnowledgeArticleInput,
} from "@/types/knowledge"

const API_BASE_URL =
  process.env.NEXT_PUBLIC_API_BASE_URL ?? "http://localhost:5059/api"

async function request<T>(path: string, options?: RequestInit): Promise<T> {
  const response = await fetch(`${API_BASE_URL}${path}`, {
    headers: {
      "Content-Type": "application/json",
      ...options?.headers,
    },
    ...options,
  })

  if (!response.ok) {
    throw new Error(`API request failed: ${response.status}`)
  }

  if (response.status === 204) {
    return undefined as T
  }

  return response.json() as Promise<T>
}

export const ticketsApi = {
  list: () => request<Ticket[]>("/tickets"),
  getById: (id: string) => request<Ticket>(`/tickets/${id}`),
  create: (input: CreateTicketInput) =>
    request<Ticket>("/tickets", {
      method: "POST",
      body: JSON.stringify(input),
    }),
  remove: (id: string) =>
    request<void>(`/tickets/${id}`, {
      method: "DELETE",
    }),
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
