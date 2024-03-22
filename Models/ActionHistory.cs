using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TestingSoftwareAPI.Models
{
    [Table("ActionHistories")]
    public class ActionHistory
    {
        [Key]
        public Guid ActionHistoryID { get; set; }
        [StringLength(50)]
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Detail { get; set; }
        public DateTime CreateTime { get; set; } = DateTime.Now;
    }
}