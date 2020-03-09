using PBP.DataAccess;
using PBP.Pocos;
using System;
using System.Collections.Generic;
using System.Text;

namespace PBP.BusinessLogic
{
    public class TransactionLogic : BaseLogic<TransactionPoco>
    {
        public TransactionLogic(IDataRepository<TransactionPoco> repository) :  base (repository)
        {
        }

        public override void Verify(TransactionPoco poco)
        {
            //base.Verify(poco);
        }
    }
}
