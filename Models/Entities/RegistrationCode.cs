using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestingSoftwareAPI.Models
{
    [Table("RegistrationCodes")]
    public partial class RegistrationCode
    {
        [Key]
        public Guid RegistrationCodeID { get; set; }
        [Required]
        public int RegistrationCodeNumber { get; set; }
        public Guid StudentExamID { get; set; }
        public int ExamId { get; set; }
        public StudentExam? StudentExam { get; set; }
        public Exam? Exam { get; set; }
    }
}