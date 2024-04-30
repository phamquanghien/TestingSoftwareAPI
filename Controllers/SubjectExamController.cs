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
    public class SubjectExamController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SubjectExamController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/SubjectExam
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubjectExam>>> GetSubjectExams()
        {
            return await _context.SubjectExam.ToListAsync();
        }
        [HttpGet("admin-get-list-by-examId")]
        public async Task<ActionResult<IEnumerable<SubjectExam>>> AminGetSubjectExamByExamID(int examID)
        {
            if (examID == 0 || examID == null)
            {
                return BadRequest("BadRequest");
            }
            var query = await _context.SubjectExam
                    .Include(m => m.Subject) // Bao gồm thông tin của Subject
                    .Where(m => m.ExamId == examID)
                    .ToListAsync();
            if(query == null) {
                return NotFound();
            }
            return query;
        }
        [HttpGet("employee-get-subject-exam")]
        public async Task<ActionResult<SubjectExam>> EmployeeGetSubjectExamByExamID(int examID, int examBag)
        {
            if (examID == 0 || examID == null || examBag == null)
            {
                return BadRequest("BadRequest");
            }
            
            var subjectExam = await _context.SubjectExam
                    .Include(m => m.Subject) // Bao gồm thông tin của Subject
                    .Where(m => m.ExamId == examID && m.ExamBag == examBag)
                    .FirstOrDefaultAsync();
            if(subjectExam == null) {
                return NotFound();
            }
            return subjectExam;
        }

        // GET: api/SubjectExam/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SubjectExam>> GetSubjectExams(Guid id)
        {
            var subjectExams = await _context.SubjectExam.FindAsync(id);

            if (subjectExams == null)
            {
                return NotFound();
            }

            return subjectExams;
        }

        // PUT: api/SubjectExam/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSubjectExams(Guid id, SubjectExam subjectExams)
        {
            if (id != subjectExams.SubjectExamID)
            {
                return BadRequest();
            }

            _context.Entry(subjectExams).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubjectExamsExists(id))
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

        // POST: api/SubjectExam
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SubjectExam>> PostSubjectExams(SubjectExam subjectExams)
        {
            _context.SubjectExam.Add(subjectExams);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSubjectExams", new { id = subjectExams.SubjectExamID }, subjectExams);
        }

        // DELETE: api/SubjectExam/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubjectExams(Guid id)
        {
            var subjectExams = await _context.SubjectExam.FindAsync(id);
            if (subjectExams == null)
            {
                return NotFound();
            }

            _context.SubjectExam.Remove(subjectExams);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SubjectExamsExists(Guid id)
        {
            return _context.SubjectExam.Any(e => e.SubjectExamID == id);
        }
    }
}
