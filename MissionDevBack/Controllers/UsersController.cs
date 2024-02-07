using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MissionDevBack.Db;
using MissionDevBack.Models;
using MissionDevBack.Services;

namespace MissionDevBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly MissionDevContext _context;
        private readonly FileStorageService _storageService;

        public UsersController(MissionDevContext context, FileStorageService storageService)
        {
            _context = context;
            _storageService = storageService;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var User = await _context.Users.FindAsync(id);

            if (User == null)
            {
                return NotFound();
            }

            return User;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(string id, User User)
        {
            if (id != User.Id)
            {
                return BadRequest();
            }

            _context.Entry(User).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User User)
        {
            _context.Users.Add(User);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = User.Id }, User);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("adjime")]
        [Authorize]
        public async Task<IActionResult> TestUser()
        {
            return Ok(User.Identity.IsAuthenticated);
        }

        [HttpPost("yeah")]
        public async Task<IActionResult> TestUploadFile(List<IFormFile> files)
        {
            if (files.Count == 0)
            {
                return BadRequest();
            }
            foreach (var file in files) {
                var responseWriteFile = await _storageService.WriteUserFileToStorageAsync(file, User.Identity.Name);
                foreach (var errorFile in responseWriteFile.Errors)
                {
                    ModelState.AddModelError("File", errorFile);
                }
            }
            if (!ModelState.IsValid) {
                return BadRequest(ModelState.Values.SelectMany(value => value.Errors).ToList());
            }
            return Ok();
        }

        [HttpPost("oula2/{userFileId}")]
        public async Task<IActionResult> TestEncore(int userFileId)
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

        [HttpPost("oula3/{userFileId}")]
        public async Task<IActionResult> TestEncorze(int userFileId)
        {
            var userFile = await _context.UserFiles.FindAsync(userFileId);
            if (userFile == null)
            {
                return NotFound();
            }
            var file = _storageService.EraseUserFileFromStorage(userFile);
            if (!file.IsSuccess)
            {
                return NotFound();
            }
            
            return Ok();
        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
