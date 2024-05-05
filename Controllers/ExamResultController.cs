using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestingSoftwareAPI.Data;
using TestingSoftwareAPI.Models;
using TestingSoftwareAPI.Models.Process;
using TestingSoftwareAPI.Models.ViewModel;

namespace TestingSoftwareAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamResultController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private ExcelProcess _excelProcess = new ExcelProcess();
        private StudentProcess _studentProcess = new StudentProcess();
        private SubjectProcess _subjectProcess = new SubjectProcess();
        private SubjectExamProcess _subjectExamProcess = new SubjectExamProcess();

        public ExamResultController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ExamResult
        [HttpGet("get-by-examID-examBag")]
        public async Task<ActionResult<IEnumerable<ExamResult>>> GetExamResult(int examID, int examBag)
        {
            return await _context.ExamResult.Where(m => m.ExamId == examID && m.ExamBag == examBag).ToListAsync();
        }

        // GET: api/ExamResult/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ExamResult>> GetExamResult(Guid id)
        {
            var examResult = await _context.ExamResult.FindAsync(id);

            if (examResult == null)
            {
                return NotFound();
            }

            return examResult;
        }

        // PUT: api/ExamResult/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutExamResult(Guid id, ExamResult examResult)
        {
            if (id != examResult.ExamResultID)
            {
                return BadRequest();
            }

            _context.Entry(examResult).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExamResultExists(id))
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

        // POST: api/ExamResult
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ExamResult>> PostExamResult(ExamResult examResult)
        {
            _context.ExamResult.Add(examResult);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetExamResult", new { id = examResult.ExamResultID }, examResult);
        }

        // DELETE: api/ExamResult/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExamResult(Guid id)
        {
            var examResult = await _context.ExamResult.FindAsync(id);
            if (examResult == null)
            {
                return NotFound();
            }

            _context.ExamResult.Remove(examResult);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("scan-registration-code-number")]
        public async Task<ActionResult<string>> LectureScanRegistrationCodeNumber([FromBody] List<LecturersScanCodeVM> lecturersScanCodes, int examId, int examBag)
        {
            string messageResult = "";
            messageResult = await CheckDataFromListVM(lecturersScanCodes);
            if(messageResult.Length == 0) {
                var listVT = await (from regisCode in _context.RegistrationCode
                            join stdExam in _context.StudentExam on regisCode.StudentExamID equals stdExam.StudentExamID
                            where stdExam.ExamId == examId && stdExam.ExamBag == examBag && stdExam.IsActive == false
                            select regisCode.RegistrationCodeNumber).ToListAsync();
                var listDT = await (from regisCode in _context.RegistrationCode
                            join stdExam in _context.StudentExam on regisCode.StudentExamID equals stdExam.StudentExamID
                            where stdExam.ExamId == examId && stdExam.ExamBag == examBag && stdExam.IsActive == true
                            select regisCode.RegistrationCodeNumber).ToListAsync();
                List<int> registrationCodeNumberList = new List<int>();
                for(int i  = 0; i < lecturersScanCodes.Count(); i++)
                {
                    registrationCodeNumberList.Add(lecturersScanCodes[i].RegistrationCodeNumber);
                }
                var listExcelVT = registrationCodeNumberList.Except(listDT);
                var listThieuBaiThi = listDT.Except(registrationCodeNumberList);
                if(listExcelVT.Count() == 0 && listThieuBaiThi.Count() == 0) {
                    var subjectCode = _context.SubjectExam.Where(m => m.ExamId == examId && m.ExamBag == examBag).FirstOrDefault().SubjectCode;
                    var importListVT = listVT.Select(registrationCodeNumber => new ExamResult
                                        {
                                            RegistrationCodeNumber = registrationCodeNumber,
                                            ExamResult1 = "V",
                                            ExamResult2 = "V",
                                            AverageScore = "V",
                                            ExamId = examId,
                                            ExamBag = examBag,
                                            IsActive = false,
                                            SubjectCode = subjectCode
                                        }).ToList();
                    await _context.ExamResult.AddRangeAsync(importListVT);
                    var examResultList = lecturersScanCodes.Select(m => new ExamResult {
                        RegistrationCodeNumber = m.RegistrationCodeNumber,
                        ExamResult1 = "" + m.ExamResult1,
                        ExamResult2 = "" + m.ExamResult2,
                        AverageScore = "" + m.AverageScore,
                        ExamId = examId,
                        ExamBag = examBag,
                        IsActive = true,
                        SubjectCode = subjectCode
                    }).ToList();
                    await _context.ExamResult.AddRangeAsync(examResultList);
                    //cap nhat thong tin bang SubjectExam
                    var subjectExamCode = await _context.SubjectExam.Where(m => m.ExamId == examId && m.ExamBag == examBag).Select(m => m.SubjectExamID).FirstOrDefaultAsync();
                    var updateSubjectExam = await _context.SubjectExam.FindAsync(subjectExamCode);
                    if (updateSubjectExam == null)
                    {
                        messageResult = "Yêu cầu thực hiện không thành công, không thể cập nhật trạng thái túi bài thi. Vui lòng thử lại sau";
                    } else {
                        updateSubjectExam.IsMatchingTestScore = true;
                        updateSubjectExam.UserMatchingTestScore = "Username Login";
                        _context.Entry(updateSubjectExam).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                        messageResult = "Nhập điểm thành công";
                    }
                } else if(listExcelVT.Count() > 0) {
                    var checkListVT = listExcelVT.Except(listVT);
                    if(checkListVT.Count() == 0) {
                        messageResult = "Các sinh viên: ";
                        foreach(var item in listExcelVT) {
                            messageResult += item + ", ";
                        }
                        messageResult += " đã tích vắng thi nhưng vẫn có điểm, vui lòng liên hệ quản trị viên để được hỗ trợ!";
                    }
                    else {
                        messageResult = "Các mã phách: ";
                        foreach(var item in checkListVT) {
                            messageResult += item + ", ";
                        }
                        messageResult += " không đúng vui lòng kiểm tra lại";
                    }
                } else if(listThieuBaiThi.Count() > 0) {
                    messageResult = "Còn thiếu " + listThieuBaiThi.Count() + " bài thi hoặc danh sách vắng thi bị sai, vui lòng liên hệ với quản trị viên để được hỗ trợ!";
                }
            }
            return messageResult;
        }

        [HttpPost("upload-file-result")]
        public async Task<IActionResult> Upload(IFormFile file, int examId, int examBag)
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
                string uniqueFileName = "File" + Guid.NewGuid().ToString() + "_" + file.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                Directory.CreateDirectory(uploadsFolder);
                
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                    var fileLocation = new FileInfo(filePath).ToString();
                    var result = await ImportDataFromExcel(fileLocation, examId, examBag);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }
        private async Task<string> ImportDataFromExcel(string fileLocation, int examId, int examBag)
        {
            string messageResult = "";
            messageResult = await CheckDataFromExcel(fileLocation);
            if(messageResult.Length == 0)
            {
                var listVT = await (from regisCode in _context.RegistrationCode
                                                    join stdExam in _context.StudentExam on regisCode.StudentExamID equals stdExam.StudentExamID
                                                    where stdExam.ExamId == examId && stdExam.ExamBag == examBag && stdExam.IsActive == false
                                                    select regisCode.RegistrationCodeNumber).ToListAsync();
                var listDT = await (from regisCode in _context.RegistrationCode
                                                    join stdExam in _context.StudentExam on regisCode.StudentExamID equals stdExam.StudentExamID
                                                    where stdExam.ExamId == examId && stdExam.ExamBag == examBag && stdExam.IsActive == true
                                                    select regisCode.RegistrationCodeNumber).ToListAsync();
                var dataFromExcel =  _excelProcess.ExcelToDataTable(fileLocation);
                List<int> excelListInt = new List<int>();
                for(int i = 0; i < dataFromExcel.Rows.Count; i++) {
                    excelListInt.Add(Convert.ToInt32(dataFromExcel.Rows[i][0]));
                }
                var listExcelVT = excelListInt.Except(listDT);
                var listThieuBaiThi = listDT.Except(excelListInt);
                if(listExcelVT.Count() == 0 && listThieuBaiThi.Count() == 0) {
                    var subjectCode = _context.SubjectExam.Where(m => m.ExamId == examId && m.ExamBag == examBag).FirstOrDefault().SubjectCode;
                    //import du lieu vao examResult
                    var importListVT = listVT.Select(registrationCodeNumber => new ExamResult
                                        {
                                            RegistrationCodeNumber = registrationCodeNumber,
                                            ExamResult1 = "V",
                                            ExamResult2 = "V",
                                            AverageScore = "V",
                                            ExamId = examId,
                                            ExamBag = examBag,
                                            IsActive = false,
                                            SubjectCode = subjectCode
                                        }).ToList();
                    await _context.ExamResult.AddRangeAsync(importListVT);
                    var examResultList = dataFromExcel.AsEnumerable()
                                        .Select(row => new ExamResult
                                        {
                                            RegistrationCodeNumber = Convert.ToInt32(row.Field<string>(0)),
                                            ExamResult1 = row.Field<string>(1),
                                            ExamResult2 = row.Field<string>(2),
                                            AverageScore = row.Field<string>(3),
                                            ExamId = examId,
                                            ExamBag = examBag,
                                            IsActive = true,
                                            SubjectCode = subjectCode
                                        })
                                        .ToList();
                    await _context.ExamResult.AddRangeAsync(examResultList);
                    //cap nhat thong tin bang SubjectExam
                    var subjectExamCode = await _context.SubjectExam.Where(m => m.ExamId == examId && m.ExamBag == examBag).Select(m => m.SubjectExamID).FirstOrDefaultAsync();
                    var updateSubjectExam = await _context.SubjectExam.FindAsync(subjectExamCode);
                    if (updateSubjectExam == null)
                    {
                        messageResult = "Yêu cầu thực hiện không thành công, không thể cập nhật trạng thái túi bài thi. Vui lòng thử lại sau";
                    }
                    else {
                        updateSubjectExam.IsMatchingTestScore = true;
                        updateSubjectExam.UserMatchingTestScore = "Username Login";
                        _context.Entry(updateSubjectExam).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                        messageResult = "Nhập điểm thành công";
                    } 
                }
                //neu thi sinh bi tich vang nhung van di thi hoac quet ma phach bi sai
                else if(listExcelVT.Count() > 0) {
                    //kiem tra danh sach cac ma phach khong di thi nhung van co diem co ton tai trong list vang thi khong
                    var checkListVT = listExcelVT.Except(listVT);
                    if(checkListVT.Count() == 0) {
                        messageResult = "Các sinh viên: ";
                        foreach(var item in listExcelVT) {
                            messageResult += item + ", ";
                        }
                        messageResult += " đã tích vắng thi nhưng vẫn có điểm, vui lòng liên hệ quản trị viên để được hỗ trợ!";
                    }
                    else {
                        messageResult = "Các mã phách: ";
                        foreach(var item in checkListVT) {
                            messageResult += item + ", ";
                        }
                        messageResult += " không đúng vui lòng kiểm tra lại";
                    }
                } else if(listThieuBaiThi.Count() > 0) {
                    messageResult = "Còn thiếu " + listThieuBaiThi.Count() + " bài thi hoặc danh sách vắng thi bị sai, vui lòng liên hệ với quản trị viên để được hỗ trợ!";
                }
            }
            return messageResult;
        }

        private async Task<string> CheckDataFromExcel(string fileLocation)
        {
            string messageResult = "";
            var dataFromExcel = _excelProcess.ExcelToDataTable(fileLocation);
            if (dataFromExcel.Columns.Count !=4) messageResult = "File không hợp lệ (Dữ liệu bao gồm Số phách, Điểm 1, Điểm 2 và Điểm Trung bình)";
            else if (dataFromExcel.Rows.Count == 0) messageResult = "File chưa có dữ liệu, vui lòng kiểm tra lại!";
            else {
                for(int i = 0; i < dataFromExcel.Rows.Count; i++)
                {
                    try {
                        int maPhach = Convert.ToInt32(dataFromExcel.Rows[i][0].ToString());
                    }
                    catch {
                        messageResult += (i+1) +"; ";
                    }
                }
                if(messageResult.Length > 0) messageResult = "Số phách: " + messageResult + " không hợp lệ";
            }
            
            return messageResult;
        }

        private async Task<string> CheckDataFromListVM(List<LecturersScanCodeVM> lecturersScanCode)
        {
            string messageResult = "";
            if (lecturersScanCode.Count() == 0) messageResult = "Dữ liệu chưa có vui lòng kiểm tra lại!";
            else {
                for(int i = 0; i < lecturersScanCode.Count(); i++)
                {
                    try {
                        Convert.ToDouble(lecturersScanCode[i].RegistrationCodeNumber.ToString());
                        Convert.ToDouble(lecturersScanCode[i].ExamResult1.ToString());
                        Convert.ToDouble(lecturersScanCode[i].ExamResult2.ToString());
                        Convert.ToDouble(lecturersScanCode[i].AverageScore.ToString());
                    }
                    catch {
                        messageResult += (i+1) +"; ";
                    }
                }
                if(messageResult.Length > 0) messageResult = "Bản ghi: " + messageResult + " không hợp lệ";
            }
            
            return messageResult;
        }
        private bool ExamResultExists(Guid id)
        {
            return _context.ExamResult.Any(e => e.ExamResultID == id);
        }
    }
}
