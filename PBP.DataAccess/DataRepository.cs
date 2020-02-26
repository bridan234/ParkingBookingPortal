using Microsoft.EntityFrameworkCore;
using PBP.Pocos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;


namespace PBP.DataAccess
{
    public class DataRepository<TPoco> : IDataRepository<TPoco> where TPoco : class
    {
        private DBModel db;

        public DataRepository()
        {
            db = new DBModel();
        }
        public void Add(TPoco poco)
        {
            db.Entry(poco).State = EntityState.Added;
            db.SaveChanges();
        }

        public TPoco Get(Expression<Func<TPoco, bool>> Condition, Expression<Func<TPoco, object>> navigationProperty = null)
        {
            var dbQuery = db.Set<TPoco>();
            if (navigationProperty == null)
                return dbQuery.Where(Condition).FirstOrDefault();
            
            return dbQuery.Where(Condition).Include(navigationProperty).FirstOrDefault();

        }

        public IList<TPoco> GetList(Expression<Func<TPoco, bool>> Condition, Expression<Func<TPoco, object>> navigationProperty = null)
        {
            var dbQuery = db.Set<TPoco>();
            if (navigationProperty == null)
                return dbQuery.Where(Condition).ToList();

            return dbQuery.Where(Condition).Include(navigationProperty).ToList();
        }

        public void Remove(TPoco poco)
        {
            db.Entry(poco).State = EntityState.Deleted;
            db.SaveChanges();
        }

        public void Update(TPoco poco)
        {
            db.Entry(poco).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}
