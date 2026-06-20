using Application.Features.Documents.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Documents.Commands
{
    public class UploadDocumentCommand : IRequest<DocumentDto>
    {
        public IFormFile File { get; set; } = null!;
        public int CurrentUserId { get; set; }
    }
}