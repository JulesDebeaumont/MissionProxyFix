using Microsoft.AspNetCore.Mvc;
using MissionDevBack.Db;
using MissionDevBack.Models;
using MissionDevBack.Services;

namespace MissionDevBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailBoxController : ControllerBase
    {
        private readonly MissionDevContext _context;
        private readonly ZimbraService _mailService;
        private readonly FileStorageService _storageService;

        public MailBoxController(MissionDevContext context, ZimbraService mailService, FileStorageService storageService)
        {
            _context = context;
            _mailService = mailService;
            _storageService = storageService;
        }

        // POST: api/MailBox/GetAuthToken
        [HttpPost("GetAuthToken")]
        public async Task<IActionResult> GetAuthToken(int id, [FromBody] ProjectGetZimbraAuthParams bodyParams)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            var responseService = await _mailService.GetUserZimbraAuthToken(bodyParams.email, bodyParams.password);
            if (!responseService.IsSuccess)
            {
                return BadRequest(responseService.Errors);
            }

            return Ok(responseService.Token);
        }


        // POST: api/MailBox/GetMailsByFolder
        [HttpPost("GetMailsByFolder")]
        public async Task<IActionResult> GetMailsByFolder(int id, [FromBody] ProjectGetZimbraLastMails bodyParams)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            var responseService = await _mailService.GetLastMailsByFolderName(bodyParams.FolderName, bodyParams.MailCount, bodyParams.AuthToken);
            if (!responseService.IsSuccess)
            {
                return BadRequest(responseService.Errors);
            }

            return Ok(responseService.MailPreviews);
        }

        // POST: api/MailBox/GetMailById
        [HttpPost("GetMailById")]
        public async Task<IActionResult> GetMailById(int id, [FromBody] ProjectGetZimbraMail bodyParams)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            var responseService = await _mailService.GetMailById(bodyParams.MailId, bodyParams.AuthToken);
            if (!responseService.IsSuccess)
            {
                return BadRequest(responseService.Errors);
            }

            return Ok(responseService.Mail);
        }

        // POST: api/MailBox/GetAttachment
        [HttpGet("GetAttachment")]
        public async Task<IActionResult> GetAttachment([FromQuery] ProjectGetZimbraMailAttachment queryParams)
        {
            var responseService = await _mailService.GetMailAttachmentBlob(queryParams.MailId, queryParams.Part, queryParams.AuthToken);
            if (!responseService.IsSuccess)
            {
                return BadRequest(responseService.Errors);
            }

            return File(responseService.Blob, "images/png"); // TODO content-type
        }

        // POST: api/MailBox/TransfertAttachmentToProject
        [HttpGet("TransfertAttachmentToProject")]
        public async Task<IActionResult> TransfertAttachmentToProject([FromQuery] ProjectGetZimbraMailAttachment queryParams)
        {
            var existingFile = _context.ProjectFiles.Where(pf => pf.ProjectId == queryParams.ProjectId).FirstOrDefault();
            if (existingFile is not null)
            {
                return BadRequest("Fichier déjà transféré");
            }
            var responseService = await _mailService.GetMailAttachmentBlob(queryParams.MailId, queryParams.Part, queryParams.AuthToken);
            if (!responseService.IsSuccess)
            {
                return BadRequest(responseService.Errors);
            }
            using (MemoryStream memoryStream = new MemoryStream(responseService.Blob))
            {
                var file = new FormFile(memoryStream, 0, responseService.Blob.Length, null, "TODO.png")
                {
                    ContentType = "TODO"
                };
                var responseWriteFile = await _storageService.WriteProjectFileToStorageAsync(file, queryParams.ProjectId);
                foreach (var errorFile in responseWriteFile.Errors)
                {
                    ModelState.AddModelError("File", errorFile);
                }
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.Values.SelectMany(value => value.Errors).ToList());
            }
            return Ok();
        }
    }
}
