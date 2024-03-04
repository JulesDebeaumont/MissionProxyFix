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
        public async Task<ActionResult<IEnumerable<GetProjectsDTOOut>>> GetProjects([FromQuery] ProjectIndexEndpointParams projectIndexParams)
        {
            if (projectIndexParams.limit > 20)
            {
                projectIndexParams.limit = 20;
            }
            var rowCount = await _context.Projects.CountAsync();
            var projects = await _context.Projects
                .Select(p => new GetProjectsDTOOut
                {
                    Id = p.Id,
                    Title = p.Title,
                    Deadline = p.Deadline,
                    State = p.State,
                    Users = new List<GetProjectsDTOOut.UserDTO>(p.ProjectUsers.Select(pu => new GetProjectsDTOOut.UserDTO
                    {
                        Id = pu.User.Id,
                        Fullname = pu.User.Fullname
                    }))
                })
                .Skip(projectIndexParams.offset)
                .Take(projectIndexParams.limit)
                .ToListAsync();
            return Ok(new { rowCount, projects });
        }

        // GET: api/Projects/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GetProjectDTOOut>> GetProject(int id)
        {
            var project = await _context.Projects
            .Where(p => p.Id == id)
            .Select(p => new GetProjectDTOOut
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                State = p.State,
                Deadline = p.Deadline,
                ProjectUsers = new List<GetProjectDTOOut.ProjectUserDTO>(p.ProjectUsers.Select(pu => new GetProjectDTOOut.ProjectUserDTO
                {
                    Id = pu.User.Id,
                    Fullname = pu.User.Fullname
                }))
            })
            .FirstOrDefaultAsync();

            if (project == null)
            {
                return NotFound();
            }

            return project;
        }

        // PUT: api/Projects/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProject(int id, PutProjectDTOIn projectParams)
        {
            if (id != projectParams.Id)
            {
                return BadRequest();
            }

            var updateProject = new Project
            {
                Id = projectParams.Id,
                Title = projectParams.Title,
                Description = projectParams.Description,
                State = projectParams.State,
                Deadline = projectParams.Deadline
            };

            _context.Entry(updateProject).State = EntityState.Modified;

            var existProjectUserIds = await _context.ProjectUsers
            .Where(pu => pu.ProjectId == updateProject.Id)
            .Select(pu => pu.UserId)
            .ToListAsync();

            var newProjectUsers = new List<ProjectUser>(projectParams.ProjectUsers
            .Where(pu => !existProjectUserIds.Contains(pu.UserId))
            .Select(pu => new ProjectUser
            {
                UserId = pu.UserId,
                ProjectId = updateProject.Id
            }
            ));
            _context.ProjectUsers.AddRange(newProjectUsers);

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
        public async Task<ActionResult<PostProjectDTOOut>> PostProject(PostProjectDTOIn projectParams)
        {
            var newProject = new Project
            {
                Title = projectParams.Title,
                Description = projectParams.Description,
                State = projectParams.State,
                Deadline = projectParams.Deadline,
            };
            _context.Projects.Add(newProject);
            await _context.SaveChangesAsync();

            var newProjectUsers = new List<ProjectUser>(projectParams.ProjectUsers.Select(pu => new ProjectUser
            {
                UserId = pu.UserId,
                ProjectId = newProject.Id
            }
            ));
            _context.ProjectUsers.AddRange(newProjectUsers);
            await _context.SaveChangesAsync();

            var projectDTO = new PostProjectDTOOut
            {
                Id = newProject.Id,
                Title = newProject.Title,
                Description = newProject.Description,
                State = newProject.State,
                Deadline = newProject.Deadline,
                ProjectUsers = new List<PostProjectDTOOut.ProjectUserDTO>(newProjectUsers.Select(pu => new PostProjectDTOOut.ProjectUserDTO
                {
                    UserId = pu.UserId,
                    ProjectId = pu.ProjectId,
                }))
            };

            return CreatedAtAction("GetProject", new { id = newProject.Id }, projectDTO);
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
