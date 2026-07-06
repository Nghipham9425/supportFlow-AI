# SupportFlow AI - Agent Context

This file gives future Codex sessions enough project context to continue work without the original chat history.

## Project Summary

SupportFlow AI is a full-stack AI-assisted customer support platform.

The goal is not a generic chatbot. The product is a realistic support workflow:

1. A customer submits a support request.
2. The request becomes a ticket.
3. A support agent manages tickets in an admin workspace.
4. The knowledge base is prepared for AI retrieval.
5. AI will later summarize tickets, retrieve relevant knowledge, and draft replies.
6. Humans still review and approve AI output.

This is intended as a CV/internship project, so favor clear architecture, demoable workflows, and practical AI usage over overly broad scope.

## Tech Stack

Frontend:

- Next.js app router
- TypeScript
- Tailwind CSS v4
- shadcn/ui with Radix
- TanStack Query
- lucide-react

Backend:

- ASP.NET Core Web API
- C#
- Entity Framework Core
- Swagger
- Clean Architecture style projects

Database:

- PostgreSQL
- pgvector image via Docker Compose
- Local container name: `supportflow-postgres`
- Local DB: `supportflow_db`

AI:

- Current implementation uses `FakeEmbeddingProvider`.
- Ticket analysis and draft reply are implemented with fake AI providers.
- OpenAI config exists but `OpenAIEmbeddingProvider` is not implemented yet.
- Default planned embedding model: `text-embedding-3-small`.
- Do not require an OpenAI API key for normal local development yet.

## Repository Layout

Root:

- `backend/` - ASP.NET Core solution
- `frontend/` - Next.js app
- `docker-compose.yml` - local PostgreSQL + pgvector
- `docs/` - project docs

Backend projects:

- `SupportFlow.Domain`
  - Business entities and enums.
  - Examples: `Ticket`, `KnowledgeArticle`, `KnowledgeChunk`.
- `SupportFlow.Application`
  - DTOs and interfaces/contracts.
  - Defines what the app can do, not how it is implemented.
- `SupportFlow.Infrastructure`
  - EF Core persistence, service implementations, external providers.
  - Contains `AppDbContext`, migrations, fake AI provider.
- `SupportFlow.Api`
  - Controllers, DI, Swagger, CORS, appsettings.

Remember this rule:

```text
Domain = what the business objects are
Application = what the app can do
Infrastructure = how it is done technically
Api = how clients call it over HTTP
```

## Current Features

Tickets:

- Public support form creates tickets.
- Agent ticket list.
- Ticket detail page.
- Ticket search/filter UI.
- Delete confirmation.
- Manual ticket creation.
- Mock `Analyze with AI` flow updates summary, sentiment, priority, category, and status.
- Mock `Generate draft reply` flow creates and stores an AI draft reply.

Knowledge:

- Knowledge article CRUD.
- Knowledge chunks generated from article content.
- `Prepare for AI` button regenerates chunks.
- Article card shows AI readiness via `chunkCount` and `isAiReady`.
- `View chunks` dialog shows chunk content and embedding status.

Embedding workflow:

- `IEmbeddingProvider` exists.
- `FakeEmbeddingProvider` implements fake embeddings.
- `IKnowledgeEmbeddingService` exists.
- `KnowledgeEmbeddingService` calls the embedding provider for each chunk.
- For now it marks chunks as:
  - `IsEmbedded = true`
  - `EmbeddedAt = DateTime.UtcNow`
- It does not store vector values yet.
- Frontend `Generate embeddings` button calls:
  - `POST /api/knowledge-articles/{id}/chunks/embed`

Ticket AI workflow:

- `ITicketAnalyzer` exists.
- `FakeTicketAnalyzer` implements mock ticket triage.
- `ITicketAnalysisService` updates ticket AI fields.
- `ITicketDraftReplyGenerator` exists.
- `FakeTicketDraftReplyGenerator` creates a mock customer-facing reply.
- `ITicketDraftReplyService` stores `AiDraftReply` and sets status to `Drafted`.
- Frontend ticket detail page has:
  - `Analyze with AI`
  - `Generate draft reply`
  - AI summary display
  - draft reply textarea

Config:

- `AI:EmbeddingProvider` defaults to `Fake`.
- `OpenAI:ApiKey` and `OpenAI:EmbeddingModel` are configured, but the real provider is not active.
- API keys must never be committed. Use `dotnet user-secrets` later.

## Important Backend Endpoints

Knowledge articles:

- `GET /api/knowledge-articles`
- `GET /api/knowledge-articles/{id}`
- `POST /api/knowledge-articles`
- `PATCH /api/knowledge-articles/{id}`
- `DELETE /api/knowledge-articles/{id}`

Knowledge chunks:

- `GET /api/knowledge-articles/{id}/chunks`
- `POST /api/knowledge-articles/{id}/chunks/regenerate`
- `POST /api/knowledge-articles/{id}/chunks/embed`

Tickets:

- `POST /api/tickets/{id}/analyze`
- `POST /api/tickets/{id}/draft-reply`

Expected chunk flow:

```text
Create knowledge article
-> POST /chunks/regenerate
-> chunks are created
-> POST /chunks/embed
-> fake provider runs
-> chunks become embedded
-> UI shows Embedded
```

Expected mock ticket AI flow:

```text
Open ticket detail
-> click Analyze with AI
-> fake analyzer updates AiSummary, Sentiment, Priority, Category, Status = Analyzed
-> click Generate draft reply
-> fake draft generator stores AiDraftReply, Status = Drafted
-> UI shows the generated draft in a textarea
```

## Local Development

Start DB:

```powershell
cd D:\supportflow-ai
docker compose up -d
```

Run backend:

```powershell
cd D:\supportflow-ai\backend\SupportFlow.Api
dotnet run
```

Backend URL:

```text
http://localhost:5059
http://localhost:5059/swagger
```

Run frontend:

```powershell
cd D:\supportflow-ai\frontend
npm run dev
```

Frontend URL:

```text
http://localhost:3000
```

## Verification Commands

Backend:

```powershell
cd D:\supportflow-ai\backend
dotnet build
```

Frontend:

```powershell
cd D:\supportflow-ai\frontend
npm run lint
npm run build
```

Use these before committing meaningful changes.

## Coding Guidelines

General:

- Keep changes small and focused.
- Prefer the existing architecture and naming.
- Do not rewrite unrelated code.
- Do not commit secrets.
- Do not switch API client style unless explicitly requested.

Frontend:

- Current API client uses a custom `request<T>()` wrapper in `frontend/lib/api.ts`.
- Do not mix axios-style `api.post` calls unless the whole file is intentionally migrated.
- Client-side data fetching uses TanStack Query.
- UI should feel like a real support admin workspace, not a marketing landing page.
- Use lucide-react icons where suitable.

Backend:

- Controllers should stay thin.
- Put contracts in Application.
- Put implementations in Infrastructure.
- Keep Domain free of EF/OpenAI/HTTP details.
- Use DTOs for API responses.
- If adding AI provider code, use `IEmbeddingProvider`.

## Current AI Status

Implemented:

- Chunk generation.
- Fake embedding workflow.
- Embedding status in backend and frontend.
- Provider selection config.
- Mock ticket AI analysis.
- Mock ticket draft reply generation.

Not implemented yet:

- `OpenAIEmbeddingProvider`.
- Storing actual vector embeddings in PostgreSQL/pgvector.
- Vector similarity search.
- Real OpenAI-backed ticket analysis.
- Real OpenAI-backed draft reply generation.

## Suggested Next Steps

Best next task:

1. Polish the ticket detail AI workspace UI.
2. Add explicit "mock AI" copy only where helpful for development, but avoid making the production UI feel fake.
3. Create `OpenAIEmbeddingProvider` skeleton in `SupportFlow.Infrastructure/AI/OpenAI`.
4. Register `OpenAIOptions` with Options pattern.
5. Keep `AI:EmbeddingProvider` set to `Fake` by default.
6. Do not call OpenAI until the user is ready with API key/billing.

After that:

1. Add real OpenAI embedding calls.
2. Add vector storage with pgvector.
3. Implement similarity search.
4. Replace fake ticket analysis/draft providers with real OpenAI-backed providers.
5. Improve README and deployment docs.
6. Dockerize frontend/backend for VPS deployment.

## Collaboration Preference

The user wants to be the main coder and learn the project deeply.

Default behavior:

- Explain the purpose and flow before code.
- Guide in small steps.
- Do not dump a large full implementation unless the user explicitly asks "lam giup toi" or similar.
- When the user asks to implement, make the change and verify it.
- Use Vietnamese in conversation, casual but technically clear.
