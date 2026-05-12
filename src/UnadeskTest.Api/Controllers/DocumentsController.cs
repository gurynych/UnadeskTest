using Microsoft.AspNetCore.Mvc;
using UnadeskTest.Api.Infrastructure.Mapping;
using UnadeskTest.Api.Services;
using UnadeskTest.Shared.Models;

namespace UnadeskTest.Api.Controllers
{
    [ApiController]
    [Route("api/documents")]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentService documentService;

        public DocumentsController(IDocumentService documentService)
        {
            this.documentService = documentService;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile? file, CancellationToken cancellationToken)
        {
            if (file is null)
            {
                return BadRequest("Требуется файл.");
            }

            PdfDocument document = await documentService.UploadAsync(file, cancellationToken);
            return AcceptedAtAction(nameof(GetTextById), new { id = document.Id }, DocumentMapper.ToDto(document));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            IReadOnlyCollection<PdfDocument> documents = await documentService.GetAllAsync(cancellationToken);
            return Ok(documents.Select(DocumentMapper.ToDto));
        }

        [HttpGet("{id:guid}/text")]
        public async Task<IActionResult> GetTextById(Guid id, CancellationToken cancellationToken)
        {
            PdfDocument? document = await documentService.GetByIdAsync(id, cancellationToken);

            if (document is null)
            {
                return NotFound();
            }

            return document.Status switch
            {
                DocumentStatus.Queued or DocumentStatus.Processing => StatusCode(StatusCodes.Status202Accepted, DocumentMapper.ToDto(document)),
                DocumentStatus.Failed => Conflict(DocumentMapper.ToDto(document)),
                _ => Ok(DocumentMapper.ToTextDto(document))
            };
        }
    }
}
