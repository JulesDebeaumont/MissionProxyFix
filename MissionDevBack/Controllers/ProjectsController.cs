using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MissionDevBack.Db;
using MissionDevBack.Models;
using MissionDevBack.Services;

namespace MissionDevBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly MissionDevContext _context;
        private readonly ZimbraService _mailService;

        public ProjectsController(MissionDevContext context, ZimbraService mailService)
        {
            _context = context;
            _mailService = mailService;
        }

        // GET: api/Projects
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects([FromQuery] ProjectIndexEndpointParams projectIndexParams)
        {
            if (projectIndexParams.limit > 20)
            {
                projectIndexParams.limit = 20;
            }
            var rowCount = _context.Projects.Count();
            var projects = await _context.Projects.Skip(projectIndexParams.offset).Take(projectIndexParams.limit).ToListAsync();
            return Ok(new { rowCount, projects });
        }

        // GET: api/Projects/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);

            if (project == null)
            {
                return NotFound();
            }

            return project;
        }

        // PUT: api/Projects/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProject(int id, Project project)
        {
            if (id != project.Id)
            {
                return BadRequest();
            }

            _context.Entry(project).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Projects
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Project>> PostProject(Project project)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProject", new { id = project.Id }, project);
        }

        // DELETE: api/Projects/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        // POST: api/Projects/5/GetZimbraAuthToken
        [HttpPost("{id}/GetZimbraAuthToken")]
        public async Task<IActionResult> GetZimbraAuthToken(int id, [FromBody] ProjectGetZimbraAuthParams bodyParams)
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


        // POST: api/Projects/5/GetZimbraMailsByFolder
        [HttpPost("{id}/GetZimbraMailsByFolder")]
        public async Task<IActionResult> GetZimbraLastMailByFolderName(int id, [FromBody] ProjectGetZimbraLastMails bodyParams)
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

        // POST: api/Projects/5/GetZimbraMailById
        [HttpPost("{id}/GetZimbraMailById")]
        public async Task<IActionResult> GetZimbraMailById(int id, [FromBody] ProjectGetZimbraMail bodyParams)
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

        // POST: api/GetZimbraMailAttachment
        [HttpGet("GetZimbraMailAttachment")]
        public async Task<IActionResult> GetZimbraMailAttachment([FromQuery] ProjectGetZimbraMailAttachment queryParams)
        {
            var responseService = await _mailService.GetMailAttachmentBlob(queryParams.MailId, queryParams.Part, queryParams.AuthToken);
            if (!responseService.IsSuccess)
            {
                return BadRequest(responseService.Errors);
            }

            return File(responseService.Blob, "images/png");
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }
    }
}
