using Application.Features.Documents.Commands;
using Application.Features.Documents.Queries;
using Application.Interfaces.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/documents")]
    [Authorize]
    public class DocumentsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public DocumentsController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        /// <summary>Upload a new document (.pdf, .doc, .docx, .txt — max 20 MB).</summary>
        [HttpPost]
        [RequestSizeLimit(20 * 1024 * 1024)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var result = await _mediator.Send(new UploadDocumentCommand
            {
                File = file,
                CurrentUserId = _currentUserService.UserId
            });

            return CreatedAtAction(nameof(GetById), new { id = result.DocumentId }, result);
        }

        /// <summary>Get all documents belonging to the current user.</summary>
        [HttpGet("mine")]
        public async Task<IActionResult> GetMine()
        {
            var result = await _mediator.Send(new GetMyDocumentsQuery
            {
                CurrentUserId = _currentUserService.UserId
            });

            return Ok(result);
        }

        /// <summary>Get all documents in the system. Admin only.</summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllDocumentsQuery());
            return Ok(result);
        }

        /// <summary>Get a single document by ID. Owner or admin only.</summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetDocumentByIdQuery
            {
                DocumentId = id,
                CurrentUserId = _currentUserService.UserId,
                IsAdmin = _currentUserService.IsAdmin
            });

            return Ok(result);
        }

        /// <summary>Trigger text extraction + AI summarization for a document. Owner or admin only.</summary>
        [HttpPost("{id:int}/analyze")]
        public async Task<IActionResult> Analyze(int id)
        {
            var result = await _mediator.Send(new AnalyzeDocumentCommand
            {
                DocumentId = id,
                CurrentUserId = _currentUserService.UserId,
                IsAdmin = _currentUserService.IsAdmin
            });

            return Ok(result);
        }

        /// <summary>Delete a document. Owner or admin only.</summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _mediator.Send(new DeleteDocumentCommand
            {
                DocumentId = id,
                CurrentUserId = _currentUserService.UserId,
                IsAdmin = _currentUserService.IsAdmin
            });

            return NoContent();
        }
    }
}