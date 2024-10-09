using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestingSoftwareAPI.Models
{
    [Table("StudentExams")]
    public class StudentExam
    {
        [Key]
        public Guid StudentExamID { get; set; }
        [Required]
        public string? IdentificationNumber { get; set; }
        public string? ClassName { get; set; }
        public string? TestDay { get; set; }
        public string? TestRoom { get; set; }
        [Required]
        public string? LessonStart { get; set; }
        [Required]
        public string? LessonNumber { get; set; }

        public int ExamId { get; set; }
        public bool IsActive { get; set; } = true;
        [Required]
        public int ExamBag { get; set; }
        [Required]
        public string? StudentCode { get; set; }
        [ForeignKey("StudentCode")]
        public Student? Student { get; set; }
        [Required]
        public string? SubjectCode { get; set; }
        [ForeignKey("SubjectCode")]
        public Subject? Subject { get; set; }
    }
}