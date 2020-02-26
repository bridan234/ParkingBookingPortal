using PBP.Pocos;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace PBP.DataAccess
{
    public interface IDataRepository<TPoco> 
    {
        void Add(TPoco poco);
        void Remove(TPoco poco);
        void Update(TPoco poco); 
        TPoco Get(Expression<Func<TPoco, bool>> Condition, Expression<Func<TPoco, object>> navigationProperty = null);
        IList<TPoco> GetList(Expression<Func<TPoco, bool>> Condition, Expression<Func<TPoco, object>> navigationProperty = null);

    }
}
