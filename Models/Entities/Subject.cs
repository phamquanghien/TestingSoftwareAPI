using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestingSoftwareAPI.Models
{
    [Table("Subjects")]
    public class Subject
    {
        [Key]
        [Required]
        public string SubjectCode { get; set; }
        [Required]
        public string SubjectName { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDelete { get; set; } = false;
    }
}