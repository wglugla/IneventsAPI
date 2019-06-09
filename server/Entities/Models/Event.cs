using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Models
{
    [Table("Events")]
    public class Event
    {
        [Key]
        [Column("event_id")]
        public int Id { get; set; }

        [Column("owner_id")]
        public int OwnerId { get; set; }

        [Column("title")]
        [StringLength(45, ErrorMessage = "Username can't be longer than 45 characters")]
        public string Title { get; set; }

        [Column("date")]
        public DateTime Date { get; set; }

        [Column("place")]
        [StringLength(45, ErrorMessage = "Place can't be longer than 45 characters")]
        public string Place { get; set; }

        [Column("description")]
        [StringLength(1000, ErrorMessage = "Description can't be longer than 1000 characters")]
        public string Description { get; set; }
    }
}
