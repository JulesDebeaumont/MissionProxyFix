using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MissionDevBack.Db;
using MissionDevBack.Models;

namespace MissionDevBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly MissionDevContext _context;

        public ProjectsController(MissionDevContext context)
        {
            _context = context;
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
            var projects = await _context.Projects
                .Skip(projectIndexParams.offset)
                .Take(projectIndexParams.limit)
                .Select(p => new 
                {
                    Id = p.Id,
                    Title = p.Title,
                    State = p.State,
                    Deadline = p.Deadline,
                    ProjectUsers = p.ProjectUsers.Select(pu => new 
                    {
                        Id = pu.Id,
                        ProjectId = pu.ProjectId,
                        UserId = pu.UserId,
                        User = new 
                        {
                            Id = pu.UserId,
                            Fullname = pu.User.Fullname
                        }
                    })
                })
                .ToListAsync();
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

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }
    }
}
