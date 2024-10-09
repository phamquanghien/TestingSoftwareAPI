using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestingSoftwareAPI.Models
{
    [Table("Students")]
    public class Student
    {
        [Key]
        [MaxLength(50)]
        [Required]
        public string? StudentCode { get; set; }
        [MaxLength(15)]
        public string? LastName { get; set; }

        public string? FirstName { get; set; }

        public string? BirthDay { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDelete { get; set; } = false;
    }
}