using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PBP.Pocos
{
    [Table("Calendar")]
    public class CalendarPoco : IPoco
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        [Column("Available_Slots")]
        public int AvailableSlots { get; set; }
        [Column("Reserved_Slots")]
        public int ReservedSlots { get; set; }
        [Column("Total_Slots")]
        public int TotalSlots { get; set; }
    }
}
