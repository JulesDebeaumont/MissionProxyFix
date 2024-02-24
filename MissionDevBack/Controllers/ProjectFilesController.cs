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

        [HttpPost("Upload")]
        public async Task<IActionResult> UploadFiles(ProjectFilesUploadFilesParams uploadFilesParams)
        {
            if (uploadFilesParams.Files.Count == 0)
            {
                return BadRequest();
            }
            foreach (var file in uploadFilesParams.Files)
            {
                var responseWriteFile = await _storageService.WriteProjectFileToStorageAsync(file, uploadFilesParams.ProjectId);
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
        public async Task<IActionResult> DownloadFile(string projectFileId)
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
        public async Task<IActionResult> DeleteFile(string projectFileId)
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


        [HttpPatch("ToggleIsShared/{projectFileId}")]
        public async Task<IActionResult> ToggleIsShared(string projectFileId)
        {
            var projectFile = await _context.ProjectFiles.FindAsync(projectFileId);
            if (projectFile == null)
            {
                return NotFound();
            }
            projectFile.IsShared = !projectFile.IsShared;
            await _context.SaveChangesAsync();

            return Ok();
        }

    }
}
