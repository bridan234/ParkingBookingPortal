using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PBP.Pocos
{
    [Table("Reservations")]
    public class ReservationPoco : IPoco
    {
        [Key]
        public Guid Id { get; set ; }
        [Column("Car_Plate_Number")]
        public string CarPlateNumber { get; set; }
        [Column("Full_Name")]
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public DateTime Date { get; set; }
        [Column("Dates_Reserved")]
        public string DatesReserved { get; set; }
        [Column("Number_Of_Days_Reserved")] 
        public int NumberOfDaysReserved { get; set; }

        public TransactionPoco Transaction { get; set; }
    }
}
