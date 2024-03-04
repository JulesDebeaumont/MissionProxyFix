using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MissionDevBack.Db;
using MissionDevBack.Models;
using MissionDevBack.Services;

namespace MissionDevBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectFilesController : ControllerBase
    {
        private readonly MissionDevContext _context;
        private readonly FileStorageService _storageService;

        public ProjectFilesController(MissionDevContext context, FileStorageService storageService)
        {
            _context = context;
            _storageService = storageService;
        }

        [HttpGet("Projects/{id}")]
        public async Task<IActionResult> GetFiles(int id)
        {
            var filesMeta = await _context.ProjectFiles
            .Where(pf => pf.ProjectId == id)
            .Select(pf => new GetProjectFilesDTOOut
            {
                Id = pf.Id,
                Filename = pf.Filename,
                MimeType = pf.MimeType,
                CreatedAt = pf.CreatedAt,
                UserId = pf.UserId,
                FromMailId = pf.FromMailId,
                IsShared = pf.IsShared,
                ProjectId = pf.ProjectId,
                User = new GetProjectFilesDTOOut.UserDTO
                {
                    Id = pf.User.Id,
                    Fullname = pf.User.Fullname
                }
            })
            .ToListAsync();
            return Ok(filesMeta);
        }


        [HttpPost("Upload")]
        public async Task<IActionResult> UploadFiles(ProjectFilesUploadFilesParams uploadFilesParams)
        {
            if (uploadFilesParams.Files.Count == 0)
            {
                return BadRequest();
            }
            foreach (var file in uploadFilesParams.Files)
            {
                var responseWriteFile = await _storageService.WriteProjectFileToStorageAsync(file, uploadFilesParams.ProjectId, User.Identity.Name);
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


        [HttpGet("Download/{projectFileId}")]
        public async Task<IActionResult> DownloadFile(int projectFileId)
        {
            var projectFile = await _context.ProjectFiles.FindAsync(projectFileId);
            if (projectFile == null)
            {
                return NotFound();
            }
            var file = await _storageService.GetProjectFileFromStorageAsync(projectFile);
            if (!file.IsSuccess)
            {
                return NotFound();
            }

            return File(file.FileBytes, projectFile.MimeType);
        }


        [HttpDelete("Delete/{projectFileId}")]
        public async Task<IActionResult> DeleteFile(int projectFileId)
        {
            var projectFile = await _context.ProjectFiles.FindAsync(projectFileId);
            if (projectFile == null)
            {
                return NotFound();
            }
            _context.ProjectFiles.Remove(projectFile);
            await _context.SaveChangesAsync();

            return Ok();
        }


        [HttpPut("ToggleIsShared/{projectFileId}")]
        public async Task<IActionResult> ToggleIsShared(int projectFileId)
        {
            var projectFile = await _context.ProjectFiles.FindAsync(projectFileId);
            if (projectFile == null)
            {
                return NotFound();
            }
            projectFile.IsShared = !projectFile.IsShared;
            await _context.SaveChangesAsync();

            var dtoProject = new ToggleIsSharedDTOOut
            {
                Id = projectFile.Id,
                IsShared = projectFile.IsShared
            };
            return Ok(dtoProject);
        }

    }
}
