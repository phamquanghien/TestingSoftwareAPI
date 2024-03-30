using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestingSoftwareAPI.Models
{
    [Table("Subjects")]
    public class Subject
    {
        [Key]
        public required string SubjectCode { get; set; }
        public required string SubjectName { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDelete { get; set; } = false;
    }
}