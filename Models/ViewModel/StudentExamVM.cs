using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestingSoftwareAPI.Models.ViewModel
{
    public class StudentExamVM
    {
        public string StudentCode { get; set; }
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string IdentificationNumber { get; set; }
        
        public string ClassName { get; set; }
        public string SubjectName { get; set; }
        public string? TestDay { get; set; }
        public string? TestRoom { get; set; }
        public required string LessonStart { get; set; }

        public required string LessonNumber { get; set; }
        public bool IsActive { get; set; }
        public int ExamBag { get; set; }
        public int? OrdinalNumber { get; set; }
        
        public int ExamId { get; set; }
        [MaxLength(15)]
        
        public string? BirthDay { get; set; }
        [Display(Name = "Mã kỳ thi")]
        public required string ExamCode { get; set; }
        [Display(Name = "Tên kỳ thi")]
        public string SubjectCode { get; set; }
        [ForeignKey("SubjectCode")]
        public required string ExamName { get; set; }
    }
}