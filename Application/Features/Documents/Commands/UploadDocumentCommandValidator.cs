using FluentValidation;

namespace Application.Features.Documents.Commands
{
    public class UploadDocumentCommandValidator : AbstractValidator<UploadDocumentCommand>
    {
        private static readonly string[] AllowedExtensions = { ".pdf", ".doc", ".docx", ".txt" };

        public UploadDocumentCommandValidator()
        {
            RuleFor(p => p.CurrentUserId)
                .GreaterThan(0).WithMessage("User ID must be valid.");

            RuleFor(p => p.File)
                .NotNull().WithMessage("A file is required.");

            When(p => p.File != null, () =>
            {
                RuleFor(p => p.File.Length)
                    .GreaterThan(0).WithMessage("File cannot be empty.")
                    .LessThanOrEqualTo(20 * 1024 * 1024).WithMessage("File size must not exceed 20 MB.");

                RuleFor(p => p.File.FileName)
                    .Must(name => AllowedExtensions.Contains(System.IO.Path.GetExtension(name).ToLowerInvariant()))
                    .WithMessage("Only .pdf, .doc, .docx and .txt files are allowed.");
            });
        }
    }
}