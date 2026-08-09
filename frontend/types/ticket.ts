export type TicketChannel = 1 | 2 | 3 | 4 | 5
export type TicketCategory = 1 | 2 | 3 | 4 | 5 | 6
export type TicketPriority = 1 | 2 | 3 | 4
export type TicketStatus = 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8
export type TicketSentiment = 1 | 2 | 3 | 4 | 5

export type Ticket = {
  id: string
  customerName: string
  customerEmail: string
  subject: string
  description: string
  channel: TicketChannel
  category: TicketCategory
  priority: TicketPriority
  status: TicketStatus
  assignedToUserId: string | null
  assignedToUserName: string | null
  assignedAt: string | null
  aiSummary: string | null
  aiDraftReply: string | null
  sentiment: TicketSentiment
  createdAt: string
  updatedAt: string
}

export type CreateTicketInput = {
  customerName: string
  customerEmail: string
  subject: string
  description: string
  channel: TicketChannel
}

export type SendTicketReplyInput = {
  content: string
}

export type TicketReply = {
  id: string
  ticketId: string
  sentByUserName: string
  recipientEmail: string
  subject: string
  content: string
  sentAt: string
}

export const ticketChannelLabels: Record<TicketChannel, string> = {
  1: "Email",
  2: "Web",
  3: "Chat",
  4: "Phone",
  5: "Social",
}

export const ticketPriorityLabels: Record<TicketPriority, string> = {
  1: "Low",
  2: "Medium",
  3: "High",
  4: "Critical",
}

export const ticketStatusLabels: Record<TicketStatus, string> = {
  1: "Open",
  2: "Analyzed",
  3: "Drafted",
  4: "Approved",
  5: "Rejected",
  6: "Pending customer",
  7: "Resolved",
  8: "Closed",
}

export const ticketCategoryLabels: Record<TicketCategory, string> = {
  1: "Other",
  2: "Billing",
  3: "Technical issue",
  4: "Account access",
  5: "Product question",
  6: "Bug report",
}

export const ticketSentimentLabels: Record<TicketSentiment, string> = {
  1: "Unknown",
  2: "Neutral",
  3: "Confused",
  4: "Frustrated",
  5: "Angry",
}

export type RelatedKnowledge = {
  articleId: string
  articleTitle: string
  chunkId: string
  content: string
  score: number
}
