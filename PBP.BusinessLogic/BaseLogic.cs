using PBP.DataAccess;
using PBP.Pocos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace PBP.BusinessLogic
{
    public class BaseLogic<TPoco> where TPoco : IPoco
    {
        private readonly IDataRepository<TPoco> _repo;

        public BaseLogic(IDataRepository<TPoco> repository)
        {
            _repo = repository;
        }

        public virtual void Verify(TPoco poco)
        {
            return;
        }

		public virtual TPoco Get(Expression<Func<TPoco, bool>> Condition)
		{
			return _repo.Get(Condition);
		}

		public virtual List<TPoco> GetList(Expression<Func<TPoco, bool>> Condition)
		{
			return _repo.GetList(Condition).ToList();
		}

		public virtual void Add(TPoco poco)
		{
				if (poco.Id == Guid.Empty)
					poco.Id = Guid.NewGuid();
				
			_repo.Add(poco);
		}

		public virtual void Update(TPoco poco)
		{
			_repo.Update(poco);
		}

		public void Delete(Guid Id)
		{
			_repo.Remove(Id);
		}
	}
}
