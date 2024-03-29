using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestingSoftwareAPI.Data;
using TestingSoftwareAPI.Models;

namespace TestingSoftwareAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationCodeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RegistrationCodeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/RegistrationCode
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RegistrationCode>>> GetRegistrationCode()
        {
            return await _context.RegistrationCode.ToListAsync();
        }

        // GET: api/RegistrationCode/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RegistrationCode>> GetRegistrationCode(Guid id)
        {
            var registrationCode = await _context.RegistrationCode.FindAsync(id);

            if (registrationCode == null)
            {
                return NotFound();
            }

            return registrationCode;
        }
        [HttpGet("count/{id}")]
        public async Task<ActionResult<StudentExam>> CountRegistrationCode(int id)
        {
            var countRegistrationCode = await _context.RegistrationCode.Where(m => m.ExamId == id).CountAsync();
            return Ok(countRegistrationCode);
        }

        // PUT: api/RegistrationCode/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRegistrationCode(Guid id, RegistrationCode registrationCode)
        {
            if (id != registrationCode.RegistrationCodeID)
            {
                return BadRequest();
            }

            _context.Entry(registrationCode).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RegistrationCodeExists(id))
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

        // POST: api/RegistrationCode
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RegistrationCode>> PostRegistrationCode(RegistrationCode registrationCode)
        {
            _context.RegistrationCode.Add(registrationCode);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRegistrationCode", new { id = registrationCode.RegistrationCodeID }, registrationCode);
        }

        // DELETE: api/RegistrationCode/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRegistrationCode(Guid id)
        {
            var registrationCode = await _context.RegistrationCode.FindAsync(id);
            if (registrationCode == null)
            {
                return NotFound();
            }

            _context.RegistrationCode.Remove(registrationCode);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RegistrationCodeExists(Guid id)
        {
            return _context.RegistrationCode.Any(e => e.RegistrationCodeID == id);
        }
    }
}
