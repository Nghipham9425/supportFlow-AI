# SupportFlow AI - Project Overview

## English

### What Is This Website?

**SupportFlow AI** is a full-stack customer support platform with an AI-assisted helpdesk workflow.

The website has two main sides:

1. **Customer-facing support page**
   - Customers submit support requests through a public form.
   - The request becomes a ticket in the support queue.

2. **Support agent workspace**
   - Support agents view, search, filter, and manage tickets.
   - Agents open a ticket detail page to review the issue.
   - Future AI features will help classify tickets, retrieve related knowledge base articles, and generate draft replies.

The goal is not to build a simple chatbot. The goal is to build a realistic support workflow where AI assists human agents, while humans still review and approve the final response.

### Target Users

- Customers who need to submit support requests
- Support agents who handle incoming tickets
- Support managers who monitor ticket status and workload

### Current User Flow

```text
Customer
-> opens /support
-> submits a support request
-> ticket is created in the database
-> support agent opens /tickets
-> agent reviews the queue
-> agent opens /tickets/{id}
-> agent reviews ticket details
```

### Current Pages

```text
/support
- Public customer support form
- Creates a ticket
- Shows confirmation after submission

/tickets
- Support agent ticket queue
- Search tickets
- Filter by status/priority
- View ticket metrics
- Delete ticket with confirmation
- Create a manual ticket if needed

/tickets/[id]
- Ticket detail page
- Customer issue
- Ticket properties
- Customer information
- Activity timeline
- AI workspace placeholder
```

### Current Features

- Public support request form
- Ticket creation
- Ticket list
- Ticket search
- Ticket filters
- Ticket metrics
- Ticket detail page
- Delete confirmation dialog
- Manual ticket creation for agents
- PostgreSQL database integration
- ASP.NET Core backend API
- Next.js frontend

### Planned AI Workflow

```text
Ticket created
-> AI classifies category and priority
-> AI summarizes the issue
-> system searches knowledge base with RAG
-> AI drafts a support reply with citations
-> support agent reviews and edits the draft
-> support agent approves the reply
-> ticket is resolved
```

### Tech Stack

```text
Frontend:
- Next.js
- TypeScript
- Tailwind CSS
- shadcn/ui
- TanStack Query

Backend:
- ASP.NET Core Web API
- C#
- Entity Framework Core
- Swagger

Database:
- PostgreSQL
- pgvector for future vector search
- Docker Compose for local database

AI:
- OpenAI API
- Embeddings
- RAG with pgvector
```

### Why This Project Is Useful for a CV

This project shows more than basic CRUD. It demonstrates:

- Full-stack development
- Clean backend architecture
- REST API design
- Database integration
- Product workflow thinking
- Customer-facing and admin-facing UI
- AI-assisted system design
- Future-ready RAG architecture

### CV Description

```text
Built SupportFlow AI, a full-stack AI-assisted customer support platform using Next.js, TypeScript, ASP.NET Core, C#, PostgreSQL, and OpenAI APIs. The system includes a public support request form, an agent ticket workspace, ticket detail workflows, and planned RAG-based AI triage and draft response generation.
```

## Tiếng Việt

### Trang Web Này Là Gì?

**SupportFlow AI** là một nền tảng hỗ trợ khách hàng full-stack, có định hướng tích hợp AI để hỗ trợ quy trình xử lý ticket.

Website có hai phía chính:

1. **Trang dành cho khách hàng**
   - Khách hàng gửi yêu cầu hỗ trợ thông qua form public.
   - Yêu cầu đó được tạo thành ticket trong hệ thống.

2. **Workspace dành cho nhân viên support/admin**
   - Nhân viên support xem, tìm kiếm, lọc và quản lý ticket.
   - Nhân viên mở trang chi tiết ticket để xem vấn đề.
   - Các tính năng AI sau này sẽ hỗ trợ phân loại ticket, tìm tài liệu liên quan, và tạo bản nháp phản hồi.

Mục tiêu của dự án không phải là làm một chatbot đơn giản. Mục tiêu là xây một workflow hỗ trợ khách hàng thực tế, trong đó AI hỗ trợ con người, còn con người vẫn kiểm tra và duyệt phản hồi cuối cùng.

### Người Dùng Mục Tiêu

- Khách hàng cần gửi yêu cầu hỗ trợ
- Nhân viên support xử lý ticket
- Quản lý support theo dõi trạng thái và khối lượng công việc

### Quy Trình Hiện Tại

```text
Khách hàng
-> mở /support
-> gửi yêu cầu hỗ trợ
-> hệ thống tạo ticket trong database
-> nhân viên support mở /tickets
-> nhân viên xem danh sách ticket
-> nhân viên mở /tickets/{id}
-> nhân viên xem chi tiết ticket
```

### Các Trang Hiện Có

```text
/support
- Form public để khách hàng gửi yêu cầu hỗ trợ
- Tạo ticket mới
- Hiển thị xác nhận sau khi gửi thành công

/tickets
- Hàng đợi ticket dành cho nhân viên support
- Tìm kiếm ticket
- Lọc theo trạng thái/độ ưu tiên
- Xem các chỉ số ticket
- Xóa ticket có xác nhận
- Tạo ticket thủ công nếu cần

/tickets/[id]
- Trang chi tiết ticket
- Nội dung vấn đề của khách hàng
- Thuộc tính ticket
- Thông tin khách hàng
- Timeline hoạt động
- Khu vực AI placeholder
```

### Tính Năng Hiện Tại

- Form gửi yêu cầu hỗ trợ public
- Tạo ticket
- Danh sách ticket
- Tìm kiếm ticket
- Lọc ticket
- Chỉ số tổng quan ticket
- Trang chi tiết ticket
- Dialog xác nhận khi xóa
- Tạo ticket thủ công cho nhân viên support
- Kết nối PostgreSQL
- Backend ASP.NET Core
- Frontend Next.js

### Quy Trình AI Dự Kiến

```text
Ticket được tạo
-> AI phân loại category và priority
-> AI tóm tắt vấn đề
-> hệ thống tìm tài liệu liên quan bằng RAG
-> AI tạo bản nháp phản hồi có citation
-> nhân viên support xem và chỉnh sửa bản nháp
-> nhân viên support duyệt phản hồi
-> ticket được xử lý xong
```

### Tech Stack

```text
Frontend:
- Next.js
- TypeScript
- Tailwind CSS
- shadcn/ui
- TanStack Query

Backend:
- ASP.NET Core Web API
- C#
- Entity Framework Core
- Swagger

Database:
- PostgreSQL
- pgvector cho vector search sau này
- Docker Compose để chạy database local

AI:
- OpenAI API
- Embeddings
- RAG với pgvector
```

### Vì Sao Dự Án Này Tốt Cho CV?

Dự án này không chỉ là CRUD cơ bản. Nó thể hiện:

- Khả năng làm full-stack
- Kiến trúc backend rõ ràng
- Thiết kế REST API
- Kết nối database
- Tư duy xây dựng workflow sản phẩm
- UI cho cả khách hàng và admin
- Thiết kế hệ thống có AI hỗ trợ
- Kiến trúc sẵn sàng cho RAG/vector search

### Mô Tả Cho CV

```text
Xây dựng SupportFlow AI, một nền tảng hỗ trợ khách hàng full-stack sử dụng Next.js, TypeScript, ASP.NET Core, C#, PostgreSQL và OpenAI APIs. Hệ thống gồm form gửi yêu cầu hỗ trợ public, workspace quản lý ticket cho nhân viên support, trang chi tiết ticket, và định hướng tích hợp AI/RAG để phân loại ticket và tạo bản nháp phản hồi.
```
