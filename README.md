# 📄 EduPlay — AI-Powered Document Processor API

<div align="center">

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![EF Core](https://img.shields.io/badge/EF_Core-10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL_Server-2022-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)
![MediatR](https://img.shields.io/badge/MediatR-CQRS-FF6B6B?style=for-the-badge)
![JWT](https://img.shields.io/badge/JWT-Auth-000000?style=for-the-badge&logo=jsonwebtokens&logoColor=white)
![Gemini](https://img.shields.io/badge/Google-Gemini_AI-4285F4?style=for-the-badge&logo=google&logoColor=white)

**Upload documents → Extract text → Get AI summaries. Built for students.**

[API Endpoints](#-api-endpoints) · [Architecture](#-architecture) · [Setup](#-setup--installation) · [Auth Flow](#-authentication-flow)

</div>

---

## ✨ Features

| Feature | Details |
|---|---|
| 📁 **Document Upload** | PDF, DOCX, DOC, TXT — up to 20 MB |
| 🔍 **Text Extraction** | PDF via IronOCR (Arabic + English), DOCX via OpenXML, TXT native |
| 🤖 **AI Summarization** | Google Gemini 2.5 Flash summarizes documents for students |
| 🔐 **JWT Auth** | Access token + refresh token rotation |
| 👮 **Role-Based Access** | `User` and `Admin` roles |
| ⚙️ **Background Jobs** | `DocumentProcessingJob` for async processing |
| 🛡️ **Global Error Handling** | Centralized middleware with typed exceptions |
| 📖 **OpenAPI / Swagger** | JWT-secured interactive docs at `/swagger` |

---

## 🏗️ Architecture

The project follows **Clean Architecture** with **CQRS via MediatR**.

```
EduPlay/
├── 📦 API/                         → Controllers, Middleware, Program.cs
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   └── DocumentsController.cs
│   ├── Middlewares/
│   │   └── GlobalExceptionMiddleware.cs
│   └── Services/
│       └── CurrentUserService.cs   (stub — implemented in Infrastructure)
│
├── 📦 Application/                 → Use Cases, CQRS, DTOs, Validators
│   ├── Behaviors/
│   │   └── ValidationBehavior.cs   (MediatR pipeline)
│   ├── Features/
│   │   ├── Auth/
│   │   │   ├── Commands/           (Register, Login, RefreshToken, RevokeToken)
│   │   │   └── DTOs/               (UserDto, LoginResponseDto)
│   │   └── Documents/
│   │       ├── Commands/           (Upload, Analyze, Delete)
│   │       ├── Queries/            (GetById, GetMine, GetAll)
│   │       └── DTOs/               (DocumentDto, DocumentAnalysisDto)
│   └── Interfaces/
│       ├── Repositories/           (IUserRepository, IDocumentRepository, …)
│       └── Services/               (IJwtService, IAIService, ITextExtractionService, …)
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
        ├── CurrentUserService.cs
        ├── GeminiService.cs
        ├── LocalFileStorageService.cs
        └── TextExtractionService.cs
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
     │   POST /api/auth/revoke/{userId}  (logout)         │
     │ ──────────────────────────────────────────────────▶│
```

> **Token lifetime:** Access token = **60 minutes** · Refresh token = **60 days**

---

## 📡 API Endpoints

### 🔑 Auth — `/api/auth`

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| `POST` | `/register` | ❌ Public | Register a new user |
| `POST` | `/login` | ❌ Public | Login → returns access + refresh tokens |
| `POST` | `/refresh` | ❌ Public | Exchange refresh token for a new pair |
| `POST` | `/revoke/{userId}` | ✅ Bearer | Revoke refresh token (logout). Admin can revoke any user. |

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
| `404` | Document not found |
| `409` | Email already registered |
| `500` | Unexpected server error |

---

## 🧱 Domain Model

```
┌──────────────────────────────┐
│           User               │
├──────────────────────────────┤
│ + UserID : int (PK)          │
│ + UserName : string          │
│ + Email : string (unique)    │
│ + PasswordHash : string      │
│ + UserPermissions : enum     │ ──── User | Admin
│ + RefreshToken : string?     │
│ + RefreshTokenExpiryTime     │
└──────────┬───────────────────┘
           │ 1
           │ has many
           │ *
┌──────────▼───────────────────┐       ┌──────────────────────────────┐
│         Document             │       │       DocumentAnalysis        │
├──────────────────────────────┤       ├──────────────────────────────┤
│ + DocumentID : int (PK)      │  1:1  │ + DocumentAnalysisID : int   │
│ + FileName : string          │◀─────▶│ + ExtractedText : string     │
│ + FilePath : string          │       │ + AiSummary : string         │
│ + FileSizeInBytes : long     │       │ + AiResponseJson : string?   │
│ + ContentType : enum         │       │ + AnalyzedAt : DateTime      │
│ + ProcessingStatus : enum    │       └──────────────────────────────┘
│ + UploadedAt : DateTime      │
│ + UserId : int (FK)          │
└──────────────────────────────┘

ProcessingStatus: Pending → Processing → Completed
                                      ↘ Failed → (can retry)
```

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

---

## 🧪 Password Policy

All passwords must satisfy:

- ✅ Minimum **8 characters**
- ✅ At least one **uppercase** letter `[A-Z]`
- ✅ At least one **lowercase** letter `[a-z]`
- ✅ At least one **number** `[0-9]`
- ✅ At least one **special character** `[!@#$%...]`

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

### Base URL
```
http://localhost:5220
```

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

### CORS
> Not yet configured. Add your frontend origin in `Program.cs` before connecting.

---

## 📁 File Storage

Uploaded files are stored locally under:
```
API/uploads/<guid>.<ext>
```

> For production, replace `LocalFileStorageService` with an Azure Blob / S3 implementation of `IFileStorageService`.

---

## 🤝 Contributing

This backend is part of a competition project. The backend is maintained by **Omar**. Frontend integration is handled by a separate developer.

---

<div align="center">

Built with ❤️ using **.NET 10** · **Clean Architecture** · **CQRS** · **Gemini AI**

</div>
