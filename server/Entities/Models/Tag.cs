using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Models
{
    [Table("Tags")]
    public class Tag
    {
        [Key]
        [Column("tag_id")]
        public int Id { get; set; }

        [Column("value")]
        [StringLength(45, ErrorMessage = "Value can't be longer than 45 characters")]
        public string Value { get; set; }
    }
}
