using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PBP.Pocos
{
    [Table("Transactions")]
    public class TransactionPoco : IPoco
    {
        [Key]
        public Guid Id { get; set; }
        [Column("Reservation")]
        public Guid ReservationId { get; set; }
        [Column("Amount_Paid")]
        public decimal AmountPaid { get; set; }
        public string Details { get; set; }
        public string MerchantId { get; set; }

        public ReservationPoco Reservation { get; set; }
    }
}
