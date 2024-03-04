using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MissionDevBack.Db;
using MissionDevBack.Models;
using MissionDevBack.Services;

namespace MissionDevBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserFilesController : ControllerBase
    {
        private readonly MissionDevContext _context;
        private readonly FileStorageService _storageService;

        public UserFilesController(MissionDevContext context, FileStorageService storageService)
        {
            _context = context;
            _storageService = storageService;
        }

        [HttpGet]
        public async Task<IActionResult> GetFiles()
        {
            if (User.Identity.IsAuthenticated == false)
            {
                return Unauthorized();
            }
            var filesMeta = await _context.UserFiles
            .Where(uf => uf.UserId == User.Identity.Name)
            .Select(uf => new GetUserFilesDTOOut
            {
                Id = uf.Id,
                Filename = uf.Filename,
                MimeType = uf.MimeType,
                CreatedAt = uf.CreatedAt
            })
            .ToListAsync();
            return Ok(filesMeta);
        }


        [HttpPost("Upload")]
        public async Task<IActionResult> UploadFiles(UserFilesUploadFilesParams uploadFilesParams)
        {
            if (User.Identity.IsAuthenticated == false)
            {
                return Unauthorized();
            }
            if (uploadFilesParams.Files.Count == 0)
            {
                return BadRequest();
            }
            foreach (var file in uploadFilesParams.Files)
            {
                var responseWriteFile = await _storageService.WriteUserFileToStorageAsync(file, User.Identity.Name);
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

        [HttpGet("Download/{userFileId}")]
        public async Task<IActionResult> DownloadFile(int userFileId)
        {
            var userFile = await _context.UserFiles.FindAsync(userFileId);
            if (userFile == null)
            {
                return NotFound();
            }
            var file = await _storageService.GetUserFileFromStorageAsync(userFile);
            if (!file.IsSuccess)
            {
                return NotFound();
            }

            return File(file.FileBytes, userFile.MimeType);
        }

        [HttpDelete("Delete/{userFileId}")]
        public async Task<IActionResult> DeleteFile(int userFileId)
        {
            var userFile = await _context.UserFiles.FindAsync(userFileId);
            if (userFile == null)
            {
                return NotFound();
            }
            _context.UserFiles.Remove(userFile);
            await _context.SaveChangesAsync();

            return Ok();
        }

    }
}
