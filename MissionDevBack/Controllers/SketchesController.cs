using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MissionDevBack.Db;
using MissionDevBack.Models;

namespace MissionDevBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SketchesController : ControllerBase
    {
        private readonly MissionDevContext _context;

        public SketchesController(MissionDevContext context)
        {
            _context = context;
        }

        // GET: api/Sketches/Projects/1
        [HttpGet("Projects/{projectId}")]
        public async Task<ActionResult<IEnumerable<Sketch>>> GetSketchesByProject(int projectId)
        {
            return await _context.Sketches.Where(sk => sk.ProjectId == projectId).ToListAsync();
        }

        // GET: api/Sketches/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Sketch>> GetSketch(int id)
        {
            var Sketch = await _context.Sketches.FindAsync(id);

            if (Sketch == null)
            {
                return NotFound();
            }

            return Sketch;
        }

        // PUT: api/Sketches/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSketch(int id, Sketch Sketch)
        {
            if (id != Sketch.Id)
            {
                return BadRequest();
            }

            _context.Entry(Sketch).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SketchExists(id))
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

        // POST: api/Sketches
        [HttpPost]
        public async Task<ActionResult<Sketch>> PostSketch(Sketch Sketch)
        {
            _context.Sketches.Add(Sketch);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSketch", new { id = Sketch.Id }, Sketch);
        }

        // DELETE: api/Sketches/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSketch(int id)
        {
            var sketch = await _context.Sketches.FindAsync(id);
            if (sketch == null)
            {
                return NotFound();
            }

            _context.Sketches.Remove(sketch);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SketchExists(int id)
        {
            return _context.Sketches.Any(e => e.Id == id);
        }
    }
}
