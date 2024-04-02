using System.Security.AccessControl;
using Microsoft.EntityFrameworkCore;
using TestingSoftwareAPI.Data;
using System.Linq;
using TestingSoftwareAPI.Models.Process;

namespace TestingSoftwareAPI.Models.Queries
{
    public class StudentExRegisCodeQuery
    {
        private RandomValue _randomValue = new RandomValue();
        private readonly ApplicationDbContext _context;
        public StudentExRegisCodeQuery(ApplicationDbContext context){
            _context = context;
        }
        //get list RegistrationCodeID from RegistrationCode by ExamID
        public async Task<List<Guid>> GetRegistrationCodeIDByExamID(int examID)
        {
            var result = await _context.RegistrationCode.Where(m => m.ExamId == examID).Select(m => m.RegistrationCodeID).ToListAsync();
            return result;
        }
        //get start RegistrationCode
        public int GetStartRegistrationCode(int examID)
        {
            int startRegistrationCode = _context.Exam.Where(m => m.ExamId == examID).First().StartRegistrationCode;
            int countRegistrationCodeByExamID = GetRegisteredCodeByExamID(examID).Result.Count;
            if(GetRegisteredCodeByExamID(examID).Result.Count != 0) {
                 startRegistrationCode += countRegistrationCodeByExamID;
            }
            return startRegistrationCode;
        }
        //get list StudentExamID from StudentExam by ExamID
        public async Task<List<Guid>> GetStudentExamByExamID(int examID)
        {
            var studentExamList = await _context.StudentExam.Where(m => m.ExamId == examID).Select(m => m.StudentExamID).ToListAsync();
            return studentExamList;
        }
        //get list StudentExamID from RegistrationCode by ExamID
        public async Task<List<Guid>> GetRegisteredCodeByExamID(int examID)
        {
            var result = await _context.RegistrationCode.Where(m => m.ExamId == examID).Select(m => m.StudentExamID).ToListAsync();
            return result;
        }
        //get list not have RegistrationCode from StudentExam by ExamID
        public List<Guid> GetUnregisteredCodeByExamID(List<Guid>studentExamList, List<Guid> registeredCodeList)
        {
            if(registeredCodeList.Count == 0) {
                return studentExamList;
            } else {
                List<Guid> result = studentExamList.Except(registeredCodeList).ToList();
                return result;
            }
        }
        public List<Guid> GetUnregisteredCodeByExamID(int examID)
        {
            var studentExamList = GetStudentExamByExamID(examID).Result;
            var registeredCodeList = GetRegisteredCodeByExamID(examID).Result;
            if(registeredCodeList.Count == 0) {
                return studentExamList;
            } else {
                List<Guid> result = studentExamList.Except(registeredCodeList).ToList();
                return result;
            }
        }
        public int[] GenerateExamCode(int examID)
        {
            //lay danh sach chua sinh phach
            var unregisteredCodeList = GetUnregisteredCodeByExamID(examID);
            //lay danh sach sinh phach ngau nhien
            int[] A = _randomValue.RandomDistinctArray(GetStartRegistrationCode(examID), unregisteredCodeList.Count);
            //tra ve viewmodel
            return A;
        }
        public List<RegistrationCode> getListUnregisteredCodeByExamID(int examID)
        {
            var result = new List<RegistrationCode>();
            var listStudentExamID = GetUnregisteredCodeByExamID(examID);
            var randomCode = GenerateExamCode(examID);
            for(int i=0; i < listStudentExamID.Count; i++)
            {
                var regisCode = new RegistrationCode();
                regisCode.RegistrationCodeNumber = randomCode[i];
                regisCode.StudentExamID = listStudentExamID[i];
                regisCode.ExamId = examID;
                result.Add(regisCode);
            }
            return result;
        }
    }
}