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
    public class ExamController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ExamController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Exam
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Exam>>> GetExam()
        {
            return await _context.Exam.Where(m => m.IsDelete == false).OrderByDescending(m => m.ExamId).ToListAsync();
        }
        [HttpGet("get-exam-by-isActive")]
        public async Task<ActionResult<IEnumerable<Exam>>> GetExamByIsActive()
        {
            return await _context.Exam.Where(m => m.Status == false).OrderByDescending(m => m.ExamId).ToListAsync();
        }
        [HttpGet("check-exam-code/{examCode}")]
        public async Task<IActionResult> CheckExamCodeExist(string examCode)
        {
            if (string.IsNullOrEmpty(examCode))
                return BadRequest("Exam Code is required.");
            var isExist = await _context.Exam.AnyAsync(e => e.ExamCode == examCode);
            return Ok(isExist);
        }
        [HttpGet("check-exam-code/{examId}/{examCode}")]
        public async Task<IActionResult> CheckExamCodeExist(int examId, string examCode)
        {
            if (string.IsNullOrEmpty(examCode))
                return BadRequest("Exam Code is required.");
            var isExist = await _context.Exam.AnyAsync(e => e.ExamCode == examCode && e.ExamId != examId);
            return Ok(isExist);
        }

        // GET: api/Exam/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Exam>> GetExam(int id)
        {
            var exam = await _context.Exam.FindAsync(id);

            if (exam == null)
            {
                return NotFound();
            }

            return exam;
        }

        // PUT: api/Exam/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutExam(int id, Exam exam)
        {
            if (id != exam.ExamId)
            {
                return BadRequest();
            }

            _context.Entry(exam).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExamExists(id))
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

        // POST: api/Exam
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Exam>> PostExam(Exam exam)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (ExamExists(exam.ExamCode))
            {
                return BadRequest(new { message = "Exam Code đã tồn tại. Vui lòng nhập giá trị khác." });
            }
            _context.Exam.Add(exam);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetExam", new { id = exam.ExamId }, exam);
        }

        // DELETE: api/Exam/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExam(int id)
        {
            var exam = await _context.Exam.FindAsync(id);
            if (exam == null)
            {
                return NotFound();
            }
            var countStudentExam = _context.StudentExam.Where(x => x.ExamId == id).Select(m => m.StudentExamID).Count();
            if (countStudentExam < 1) _context.Exam.Remove(exam);
            else
            {
                exam.IsDelete = true;
                _context.Entry(exam).State = EntityState.Modified;
            }
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ExamExists(int id)
        {
            return _context.Exam.Any(e => e.ExamId == id);
        }
        private bool ExamExists(string examCode)
        {
            return _context.Exam.Any(e => e.ExamCode == examCode);
        }
    }
}
