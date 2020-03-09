using PBP.DataAccess;
using PBP.Pocos;
using System;
using System.Collections.Generic;
using System.Text;

namespace PBP.BusinessLogic
{
    public class CalendarLogic : BaseLogic<CalendarPoco>
    {
        public CalendarLogic(IDataRepository<CalendarPoco> repository) : base(repository)
        {

        }
    }
}
