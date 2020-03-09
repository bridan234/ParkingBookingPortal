using PBP.DataAccess;
using PBP.Pocos;
using System;
using System.Collections.Generic;
using System.Text;

namespace PBP.BusinessLogic
{
    public class ReservationLogic: BaseLogic<ReservationPoco>
    {
        public ReservationLogic(IDataRepository<ReservationPoco> repository) :  base (repository)
        {

        }
    }
}
