using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeTypes;
using MissionDevBack.Db;
using MissionDevBack.Models;
using MissionDevBack.Services;

namespace MissionDevBack.Controllers
{
    [Route("api/MailBox")]
    [ApiController]
    public class MailBoxController : ControllerBase
    {
        private readonly MissionDevContext _context;
        private readonly MailBoxService _mailService;
        private readonly FileStorageService _storageService;

        public MailBoxController(MissionDevContext context, MailBoxService mailService, FileStorageService storageService)
        {
            _context = context;
            _mailService = mailService;
            _storageService = storageService;
        }

        // POST: api/MailBox/GetAuthToken
        [HttpPost("GetAuthToken")]
        public async Task<IActionResult> GetAuthToken([FromBody] ProjectGetZimbraAuthParams bodyParams)
        {
            var userMail = await _context.Users
            .Where(u => u.Id == User.Identity.Name)
            .Select(u => u.Email)
            .FirstOrDefaultAsync();

            if (userMail is null)
            {
                return BadRequest();
            }

            var responseService = await _mailService.GetMailBoxAuthToken(userMail, bodyParams.Password);
            if (!responseService.IsSuccess)
            {
                return BadRequest(responseService.Errors);
            }

            return Ok(responseService.Token);
        }


        // POST: api/MailBox/GetMails
        [HttpPost("GetMails")]
        public async Task<IActionResult> GetMails([FromBody] ProjectGetZimbraLastMails bodyParams)
        {
            var projectUser = await _context.ProjectUsers
            .Where(pu => pu.ProjectId == bodyParams.ProjectId && pu.UserId == User.Identity.Name)
            .FirstOrDefaultAsync();

            if (projectUser.MailFolderName is null)
            {
                return BadRequest();
            }

            var responseService = await _mailService.GetLastMailsByFolderName(projectUser.MailFolderName, bodyParams.MailCount, bodyParams.AuthToken);
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

            return File(responseService.Blob, MimeTypeMap.GetMimeType(queryParams.Filename));
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
            
            using (var memoryStream = new MemoryStream(responseService.Blob))
            {
                var file = new FormFile(memoryStream, 0, responseService.Blob.Length, null, queryParams.Filename);
                var responseWriteFile = await _storageService.WriteProjectFileToStorageAsync(file, queryParams.ProjectId, User.Identity.Name);
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
