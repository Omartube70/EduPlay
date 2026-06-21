# 📄 EduPlay — AI-Powered Document Processor API

<div align="center">

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![EF Core](https://img.shields.io/badge/EF_Core-10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL_Server-2022-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)
![MediatR](https://img.shields.io/badge/MediatR-CQRS-FF6B6B?style=for-the-badge)
![JWT](https://img.shields.io/badge/JWT-Auth-000000?style=for-the-badge&logo=jsonwebtokens&logoColor=white)
![Gemini](https://img.shields.io/badge/Google-Gemini_AI-4285F4?style=for-the-badge&logo=google&logoColor=white)
![MonsterASP](https://img.shields.io/badge/Hosted_on-MonsterASP.NET-00C853?style=for-the-badge&logo=windows&logoColor=white)

**Upload documents → Extract text → Get AI summaries. Built for students.**

[🚀 Live Demo](#-live-demo) · [📡 API Endpoints](#-api-endpoints) · [🏗️ Architecture](#-architecture) · [⚙️ Setup](#-setup--installation) · [🔐 Auth Flow](#-authentication-flow)

</div>

---

## 🚀 Live Demo

> The API is **live and running** on MonsterASP.NET with a real SQL Server database.

| Resource | URL |
|----------|-----|
| 🌐 **Live API** | [`https://eduplayapi.runasp.net`](https://eduplayapi.runasp.net) |
| 📖 **Swagger UI** | [`https://eduplayapi.runasp.net/swagger/index.html`](https://eduplayapi.runasp.net/swagger/index.html) |
| 🗺️ **ERD (Lucidchart)** | [`View Database Diagram`](https://lucid.app/lucidchart/5af8fe89-a34b-4efc-9c6c-23d00dc13fc1/edit?viewport_loc=613%2C-224%2C2175%2C1212%2C0_0&invitationId=inv_54ab381d-0324-4858-bd94-639a0614e374) |

> **Tip for frontend devs:** Use the Swagger UI to explore and test all endpoints interactively before writing a single line of code.

---

## ✨ Features

| Feature | Details |
|---|---|
| 📁 **Document Upload** | PDF, DOCX, DOC, TXT — up to 20 MB |
| 🔍 **Text Extraction** | PDF via IronOCR (Arabic + English), DOCX via OpenXML, TXT native |
| 🤖 **AI Summarization** | Google Gemini 2.5 Flash summarizes documents for students |
| 🔐 **JWT Auth** | Access token + refresh token rotation |
| 👤 **Self-Service Profile** | Get/update own profile, change password |
| 👮 **Role-Based Access** | `User` and `Admin` roles |
| ⚙️ **Background Jobs** | `DocumentProcessingJob` for async processing |
| 🛡️ **Global Error Handling** | Centralized middleware with typed exceptions |
| 📖 **OpenAPI / Swagger** | JWT-secured interactive docs at `/swagger` |
| ☁️ **Live Hosting** | Deployed on MonsterASP.NET with SQL Server |

---

## 🏗️ Architecture

The project follows **Clean Architecture** with **CQRS via MediatR**.

```
EduPlay/
├── 📦 API/                         → Controllers, Middleware, Program.cs
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   ├── UsersController.cs
│   │   └── DocumentsController.cs
│   └── Middlewares/
│       └── GlobalExceptionMiddleware.cs
│
├── 📦 Application/                 → Use Cases, CQRS, DTOs, Validators
│   ├── Behaviors/
│   │   └── ValidationBehavior.cs   (MediatR pipeline)
│   ├── Features/
│   │   ├── Auth/
│   │   │   ├── Commands/           (Register, Login, RefreshToken, RevokeToken)
│   │   │   └── DTOs/               (UserDto, LoginResponseDto)
│   │   ├── Users/
│   │   │   ├── Commands/           (UpdateUser, ChangePassword, DeleteUser, PromoteToAdmin)
│   │   │   ├── Queries/            (GetAllUsers, GetUserById)
│   │   │   └── DTOs/               (UserDto, UpdateUserDto, ChangePasswordDto)
│   │   └── Documents/
│   │       ├── Commands/           (Upload, Analyze, Delete)
│   │       ├── Queries/            (GetById, GetMine, GetAll)
│   │       └── DTOs/               (DocumentDto, DocumentAnalysisDto)
│   └── Interfaces/
│       ├── Repositories/           (IUserRepository, IDocumentRepository, …)
│       └── Services/               (IJwtService, IAIService, ICurrentUserService, …)
│
├── 📦 Domain/                      → Entities, Enums, Business Rules
│   ├── Entities/
│   │   ├── User.cs
│   │   ├── Document.cs
│   │   └── DocumentAnalysis.cs
│   └── Enums/
│       ├── Permissions.cs          (User, Admin)
│       ├── ContentType.cs          (Pdf, Word, Txt)
│       └── ProcessingStatus.cs     (Pending, Processing, Completed, Failed)
│
└── 📦 Infrastructure/              → EF Core, Repos, Services, Security
    ├── BackgroundJobs/
    │   └── DocumentProcessingJob.cs
    ├── Persistence/
    │   ├── ApplicationDbContext.cs
    │   ├── Configurations/         (Fluent API configs for all entities)
    │   ├── Repositories/           (UserRepository, DocumentRepository, …)
    │   └── Migrations/
    ├── Security/
    │   ├── JwtService.cs
    │   ├── PasswordHasher.cs       (BCrypt)
    │   └── RefreshTokenGenerator.cs
    └── Services/
        ├── CurrentUserService.cs   (reads claims from HttpContext)
        ├── GeminiService.cs
        ├── LocalFileStorageService.cs
        └── TextExtractionService.cs
```

---

## 🗄️ Entity Relationship Diagram

> 📌 [View full interactive ERD on Lucidchart](https://lucid.app/lucidchart/5af8fe89-a34b-4efc-9c6c-23d00dc13fc1/edit?viewport_loc=613%2C-224%2C2175%2C1212%2C0_0&invitationId=inv_54ab381d-0324-4858-bd94-639a0614e374)

```
┌──────────────────────────────────┐
│              Users               │
├──────────────────────────────────┤
│ PK  UserID          INT          │
│     UserName        NVARCHAR(100)│ UNIQUE
│     Email           NVARCHAR(200)│ UNIQUE
│     PasswordHash    NVARCHAR(MAX)│
│     UserPermissions NVARCHAR(50) │ 'User' | 'Admin'
│     RefreshToken    NVARCHAR(MAX)│ nullable
│     RefreshTokenExpiryTime       │ nullable
└────────────────┬─────────────────┘
                 │ 1
                 │ CASCADE DELETE
                 │ *
┌────────────────▼─────────────────┐         ┌──────────────────────────────────┐
│            Documents             │         │         DocumentAnalyses          │
├──────────────────────────────────┤         ├──────────────────────────────────┤
│ PK  DocumentID     INT           │   1:1   │ PK  DocumentAnalysisID   INT     │
│     FileName       NVARCHAR(200) │◀──────▶ │     ExtractedText        TEXT    │
│     FilePath       NVARCHAR(MAX) │ CASCADE │     AiSummary            TEXT    │
│     FileSizeInBytes BIGINT       │ DELETE  │     AiResponseJson       TEXT?   │
│     ContentType    NVARCHAR(50)  │         │     AnalyzedAt           DATETIME│
│     ProcessingStatus NVARCHAR(50)│         │ FK  DocumentId           INT     │
│     UploadedAt     DATETIME2     │         └──────────────────────────────────┘
│ FK  UserId         INT           │
└──────────────────────────────────┘

ContentType:      Pdf | Word | Txt
ProcessingStatus: Pending → Processing → Completed
                                     └→ Failed  (retryable)
```

---

## 🔐 Authentication Flow

```
┌─────────┐         POST /api/auth/register          ┌─────────┐
│ Client  │ ──────────────────────────────────────── │   API   │
└─────────┘                                          └─────────┘
     │                                                    │
     │         POST /api/auth/login                       │
     │ ──────────────────────────────────────────────────▶│
     │◀────────────── { accessToken, refreshToken } ──────│
     │                                                    │
     │   GET /api/documents  [Authorization: Bearer ...]  │
     │ ──────────────────────────────────────────────────▶│
     │                                                    │
     │   (access token expires after 60 min)              │
     │                                                    │
     │         POST /api/auth/refresh                     │
     │ ──────────────────────────────────────────────────▶│
     │◀────────────── { new accessToken, new refreshToken}│
     │                                                    │
     │   POST /api/auth/logout/{userId}                   │
     │ ──────────────────────────────────────────────────▶│
```

> **Token lifetime:** Access token = **60 minutes** · Refresh token = **60 days**

---

## 📡 API Endpoints

> **Base URL (Production):** `https://eduplayapi.runasp.net`
> **Base URL (Local):** `http://localhost:5220`

### 🔑 Auth — `/api/auth`

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| `POST` | `/register` | ❌ Public | Register a new user |
| `POST` | `/login` | ❌ Public | Login → returns access + refresh tokens |
| `POST` | `/refresh` | ❌ Public | Exchange refresh token for a new pair |
| `POST` | `/logout/{targetUserId}` | ✅ Bearer | Revoke refresh token (logout). Admin can revoke any user. |

<details>
<summary><b>📋 Register — Request & Response</b></summary>

**Request**
```json
POST /api/auth/register
Content-Type: application/json

{
  "userName": "ahmed_ali",
  "email": "ahmed@example.com",
  "password": "SecurePass123!"
}
```

**Response `201 Created`**
```json
{
  "userId": 1,
  "userName": "ahmed_ali",
  "email": "ahmed@example.com",
  "userRole": "User"
}
```
</details>

<details>
<summary><b>📋 Login — Request & Response</b></summary>

**Request**
```json
POST /api/auth/login
Content-Type: application/json

{
  "email": "ahmed@example.com",
  "password": "SecurePass123!"
}
```

**Response `200 OK`**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "v8K2mN9xQr...",
  "user": {
    "userId": 1,
    "userName": "ahmed_ali",
    "email": "ahmed@example.com",
    "userRole": "User"
  }
}
```
</details>

<details>
<summary><b>📋 Refresh Token — Request & Response</b></summary>

**Request**
```json
POST /api/auth/refresh
Content-Type: application/json

{
  "refreshToken": "v8K2mN9xQr..."
}
```

**Response `200 OK`**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...(new)",
  "refreshToken": "newRefreshTokenValue...",
  "user": { ... }
}
```
</details>

---

### 👤 Users — `/api/users`

> All endpoints require `Authorization: Bearer <accessToken>`

| Method | Endpoint | Role | Description |
|--------|----------|------|-------------|
| `GET` | `/` | **Admin** | Get all users in the system |
| `GET` | `/me` | User | Get the currently authenticated user's profile |
| `GET` | `/{id}` | Owner / Admin | Get a single user by ID |
| `PUT` | `/{id}` | Owner / Admin | Update username/email |
| `POST` | `/{id}/change-password` | Owner / Admin | Change a user's password |
| `DELETE` | `/{id}` | Owner / Admin | Delete a user account |
| `POST` | `/{id}/promote` | **Admin** | Promote a user to Admin role |

<details>
<summary><b>📋 Get My Profile — Response</b></summary>

**Request**
```http
GET /api/users/me
Authorization: Bearer <token>
```

**Response `200 OK`**
```json
{
  "userId": 1,
  "userName": "ahmed_ali",
  "email": "ahmed@example.com",
  "userRole": "User"
}
```
</details>

<details>
<summary><b>📋 Update User — Request & Response</b></summary>

**Request**
```json
PUT /api/users/1
Authorization: Bearer <token>
Content-Type: application/json

{
  "userName": "ahmed_updated",
  "email": "ahmed.new@example.com"
}
```

**Response `200 OK`**
```json
{
  "userId": 1,
  "userName": "ahmed_updated",
  "email": "ahmed.new@example.com",
  "userRole": "User"
}
```
</details>

<details>
<summary><b>📋 Change Password — Request & Response</b></summary>

**Request**
```json
POST /api/users/1/change-password
Authorization: Bearer <token>
Content-Type: application/json

{
  "newPassword": "NewSecurePass456!"
}
```

**Response**
```
204 No Content
```

> ⚠️ Note: a non-admin user changing **their own** password should also be required to confirm
> their current password. If your handler doesn't already enforce this, add a `currentPassword`
> field to the request and verify it before issuing a new hash.
</details>

<details>
<summary><b>📋 Promote to Admin — Response</b></summary>

**Request**
```http
POST /api/users/5/promote
Authorization: Bearer <admin-token>
```

**Response `200 OK`**
```json
{
  "userId": 5,
  "userName": "sara_dev",
  "email": "sara@example.com",
  "userRole": "Admin"
}
```
</details>

---

### 📄 Documents — `/api/documents`

> All endpoints require `Authorization: Bearer <accessToken>`

| Method | Endpoint | Role | Description |
|--------|----------|------|-------------|
| `POST` | `/` | User | Upload a document (multipart/form-data, max 20MB) |
| `GET` | `/mine` | User | Get all documents belonging to the current user |
| `GET` | `/` | **Admin** | Get all documents in the system |
| `GET` | `/{id}` | Owner / Admin | Get a document by ID (with analysis if available) |
| `POST` | `/{id}/analyze` | Owner / Admin | Trigger text extraction + AI summarization |
| `DELETE` | `/{id}` | Owner / Admin | Delete a document and its file |

<details>
<summary><b>📋 Upload Document — Request & Response</b></summary>

**Request**
```http
POST /api/documents
Authorization: Bearer <token>
Content-Type: multipart/form-data

file: [binary file data]
```

**Response `201 Created`**
```json
{
  "documentId": 42,
  "fileName": "chapter1.pdf",
  "fileSizeInBytes": 524288,
  "contentType": "Pdf",
  "processingStatus": "Pending",
  "uploadedAt": "2026-06-20T15:00:00Z",
  "userId": 1,
  "analysis": null
}
```
</details>

<details>
<summary><b>📋 Analyze Document — Response</b></summary>

**Response `200 OK`**
```json
{
  "documentId": 42,
  "fileName": "chapter1.pdf",
  "fileSizeInBytes": 524288,
  "contentType": "Pdf",
  "processingStatus": "Completed",
  "uploadedAt": "2026-06-20T15:00:00Z",
  "userId": 1,
  "analysis": {
    "documentAnalysisId": 7,
    "extractedText": "Chapter 1: Introduction to...",
    "aiSummary": "This chapter introduces the fundamental concepts of...",
    "aiResponseJson": "{ ... raw Gemini response ... }",
    "analyzedAt": "2026-06-20T15:02:30Z"
  }
}
```
</details>

---

## ❌ Error Responses

All errors return a consistent JSON shape:

```json
{
  "status": 400,
  "title": "One or more validation errors occurred.",
  "errors": [
    { "field": "Email", "message": "A valid email address is required." },
    { "field": "Password", "message": "Password must contain at least one uppercase letter." }
  ]
}
```

| Status | Scenario |
|--------|----------|
| `400` | Validation errors, bad arguments |
| `401` | Invalid credentials, expired/invalid token |
| `403` | Accessing another user's resource |
| `404` | Document or user not found |
| `409` | Email already registered |
| `500` | Unexpected server error |

---

## ⚙️ Setup & Installation

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server (local or Docker)
- Google Gemini API Key ([Get one free](https://aistudio.google.com/))

### 1. Clone & Restore

```bash
git clone https://github.com/your-username/EduPlay.git
cd EduPlay
dotnet restore
```

### 2. Configure Secrets

> ⚠️ Never commit secrets to source control. Use `dotnet user-secrets`:

```bash
cd API
dotnet user-secrets set "Jwt:Key" "your-super-secret-key-at-least-32-chars"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=.;Database=EduPlayDB;Trusted_Connection=True;"
dotnet user-secrets set "AI:ApiKey" "your-gemini-api-key"
```

### 3. Run Migrations

```bash
dotnet ef database update --project Infrastructure --startup-project API
```

### 4. Run the API

```bash
cd API
dotnet run
```

| Environment | URL |
|-------------|-----|
| HTTP | `http://localhost:5220` |
| HTTPS | `https://localhost:7078` |
| Swagger UI | `https://localhost:7078/swagger` |
| **Production** | `https://eduplayapi.runasp.net` |

---

## 🧪 Password Policy

All passwords must satisfy:

- ✅ Minimum **8 characters**
- ✅ At least one **uppercase** letter `[A-Z]`
- ✅ At least one **lowercase** letter `[a-z]`
- ✅ At least one **number** `[0-9]`
- ✅ At least one **special character** `[!@#$%...]`

This applies to registration **and** to changing a password via `/api/users/{id}/change-password`.

---

## 📦 Key NuGet Packages

| Package | Layer | Purpose |
|---------|-------|---------|
| `MediatR` | Application | CQRS pipeline |
| `FluentValidation` | Application | Request validation |
| `AutoMapper` | Application | Entity ↔ DTO mapping |
| `IronOcr` | Infrastructure | PDF text extraction (Arabic support) |
| `DocumentFormat.OpenXml` | Infrastructure | DOCX text extraction |
| `PdfPig` | Infrastructure | PDF processing |
| `BCrypt.Net-Next` | Infrastructure | Password hashing |
| `System.IdentityModel.Tokens.Jwt` | Infrastructure | JWT generation & validation |
| `Microsoft.EntityFrameworkCore.SqlServer` | Infrastructure | ORM |
| `Scalar.AspNetCore` | API | OpenAPI docs |

---

## 🌐 Frontend Integration Guide

> For the frontend developer connecting to this API:

### Base URLs

| Environment | URL |
|-------------|-----|
| 🏭 **Production** | `https://eduplayapi.runasp.net` |
| 💻 **Local Dev** | `http://localhost:5220` |
| 📖 **Swagger (Prod)** | `https://eduplayapi.runasp.net/swagger/index.html` |

### Auth Headers
```http
Authorization: Bearer <accessToken>
```

### Supported File Types for Upload
```
.pdf  .doc  .docx  .txt
```
Max size: **20 MB**

### Processing Status Flow
```
Pending ──► Processing ──► Completed
                      └──► Failed (can be retried via POST /{id}/analyze)
```

### Recommended Frontend Flow
```
1. POST /api/auth/register              → create account
2. POST /api/auth/login                 → get { accessToken, refreshToken }
3. GET  /api/users/me                   → fetch profile to populate the UI
4. POST /api/documents                  → upload file (multipart)
5. POST /api/documents/{id}/analyze     → trigger AI analysis
6. GET  /api/documents/{id}             → poll until processingStatus === "Completed"
7. Display analysis.aiSummary           → show the AI summary to the user
8. PUT  /api/users/{id}                 → let the user edit username/email
9. POST /api/users/{id}/change-password → let the user change their password
10. POST /api/auth/refresh              → when accessToken expires (60 min)
11. POST /api/auth/logout/{id}          → logout
```

### CORS
> Not yet configured for production. If you get CORS errors, contact the backend developer to add your frontend origin.

---

## 📁 File Storage

Uploaded files are stored on the server under:
```
uploads/<guid>.<ext>
```

> For a future production upgrade, `LocalFileStorageService` can be swapped with an Azure Blob / S3 implementation of `IFileStorageService` without touching any other layer.

---

## 👨‍💻 Built By

<div align="center">

**Omar** — Backend Developer

Clean Architecture · CQRS · .NET 10 · SQL Server · Gemini AI

*Competition Project — Backend by Omar · Frontend by a separate developer*

</div>

---

<div align="center">

[![Live API](https://img.shields.io/badge/🌐_Live_API-eduplayapi.runasp.net-00C853?style=for-the-badge)](https://eduplayapi.runasp.net)
[![Swagger](https://img.shields.io/badge/📖_Swagger_UI-Try_it_Live-85EA2D?style=for-the-badge&logo=swagger&logoColor=black)](https://eduplayapi.runasp.net/swagger/index.html)
[![ERD](https://img.shields.io/badge/🗺️_ERD-View_on_Lucidchart-FF6B35?style=for-the-badge)](https://lucid.app/lucidchart/5af8fe89-a34b-4efc-9c6c-23d00dc13fc1/edit?viewport_loc=613%2C-224%2C2175%2C1212%2C0_0&invitationId=inv_54ab381d-0324-4858-bd94-639a0614e374)

Built with ❤️ by **Omar** using **.NET 10** · **Clean Architecture** · **CQRS** · **Gemini AI**

</div>
