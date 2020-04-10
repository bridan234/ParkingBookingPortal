using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PBP.Main.View_Models
{
    public class ReceiptVM
    {
        public ReceiptVM()
        {
        }

        public ReceiptVM(string fullName, Guid reservationId, string reservedDates, string carPlateNumber, decimal amountPaid, string merchantId)
        {
            FullName = fullName;
            ReservationId = reservationId;
            ReservedDates = reservedDates;
            CarPlateNumber = carPlateNumber;
            AmountPaid = amountPaid;
            MerchantId = merchantId;
        }

        public string FullName { get; set; }
        public Guid ReservationId { get; set; }
        public string ReservedDates { get; set; }
        public string CarPlateNumber { get; set; }
        public decimal AmountPaid { get; set; }
        public string MerchantId { get; set; }
    }
}
