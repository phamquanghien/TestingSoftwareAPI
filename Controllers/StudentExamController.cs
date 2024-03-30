using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestingSoftwareAPI.Data;
using TestingSoftwareAPI.Models;
using TestingSoftwareAPI.Models.Process;

namespace TestingSoftwareAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentExamController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private ExcelProcess _excelProcess = new ExcelProcess();
        public StudentExamController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: api/StudentExam
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentExam>>> GetStudentExam()
        {
            return await _context.StudentExam.ToListAsync();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<StudentExam>> GetStudentExam(int id)
        {
            var studentExam = await _context.StudentExam.FindAsync(id);

            if (studentExam == null)
            {
                return NotFound();
            }

            return studentExam;
        }
        
        // GET: api/StudentExam/5
        [HttpGet("count/{id}")]
        public async Task<ActionResult<StudentExam>> CountStudentExam(int id)
        {
            var countStudentExam = await _context.StudentExam.Where(m => m.ExamId == id).CountAsync();
            return Ok(countStudentExam);
        }

        // PUT: api/StudentExam/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudentExam(Guid id, StudentExam studentExam)
        {
            if (id != studentExam.StudentExamID)
            {
                return BadRequest();
            }

            _context.Entry(studentExam).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExamExists(id))
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

        // POST: api/StudentExam
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<StudentExam>> PostStudentExam(StudentExam studentExam)
        {
            _context.StudentExam.Add(studentExam);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStudentExam", new { id = studentExam.StudentExamID }, studentExam);
        }

        // DELETE: api/StudentExam/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudentExam(Guid id)
        {
            var studentExam = await _context.StudentExam.FindAsync(id);
            if (studentExam == null)
            {
                return NotFound();
            }

            _context.StudentExam.Remove(studentExam);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file, int examId, bool isOverWrite)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is required.");

            // Kiểm tra loại file nếu cần thiết
            if (Path.GetExtension(file.FileName).ToLower() != ".xlsx" && Path.GetExtension(file.FileName).ToLower() != ".xls")
            {
                return BadRequest("Only Excel files are allowed.");
            }
            try
            {
                string uploadsFolder = Path.Combine("Uploads","Excels");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                Directory.CreateDirectory(uploadsFolder);
                
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                    var fileLocation = new FileInfo(filePath).ToString();
                    var result = await ImportDataFromExcel(examId,fileLocation, isOverWrite);
                    return Ok("File uploaded successfully: " + result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }
        private async Task<int> ImportDataFromExcel(int examId, string fileLocation, bool isOverWrite)
        {
            var dataFromExcel =  _excelProcess.ExcelToDataTable(fileLocation);
            var studentDataTable =  _excelProcess.GetStudentTableDistinct(dataFromExcel);
            if(studentDataTable.Rows.Count > 0) {
                if(isOverWrite){
                    await _context.BulkDeleteAsync(await _context.StudentExam.Where(m => m.ExamId == examId).ToListAsync());
                }
                //get list Student code
                var existingStudentCodes = await _context.Student.Select(m => m.StudentCode).ToListAsync();
                //get list student do not exist in database at studentDataTable
                try {
                    var newStudents = studentDataTable.AsEnumerable()
                                    .Where(row => !existingStudentCodes.Contains(row.Field<string>(0)))
                                    .Select(row => new Student
                                    {
                                        StudentCode = row.Field<string>(0),
                                        FirstName = row.Field<string>(1),
                                        LastName = row.Field<string>(2),
                                        BirthDay = row.Field<string>(3)
                                    });
                    await _context.Student.AddRangeAsync(newStudents);
                    var newStudentExams = dataFromExcel.AsEnumerable()
                                        .Select( row => new StudentExam
                                        {
                                            StudentCode = row.Field<string>(0),
                                            SubjectCode = row.Field<string>(1),
                                            IdentificationNumber = row.Field<string>(5),
                                            ClassName = row.Field<string>(6),
                                            SubjectName = row.Field<string>(7),
                                            TestDay = row.Field<string>(8),
                                            TestRoom = row.Field<string>(9),
                                            LessonStart = row.Field<string>(10),
                                            LessonNumber = row.Field<string>(11),
                                            ExamBag = Convert.ToInt32(row.Field<string>(12)),
                                            ExamId = examId
                                        });
                    await _context.StudentExam.AddRangeAsync(newStudentExams);
                    // //save data to database with bulk copy
                    await _context.BulkSaveChangesAsync();
                    return dataFromExcel.Rows.Count;
                } catch {
                    
                }
            }
            return 0;
        }
        private bool StudentExamExists(Guid id)
        {
            return _context.StudentExam.Any(e => e.StudentExamID == id);
        }
    }
}
