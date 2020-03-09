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

        public TPoco Get(Expression<Func<TPoco, bool>> Condition)
        {
            var dbQuery = db.Set<TPoco>();
            return dbQuery.Where(Condition).FirstOrDefault();

        }

        public IList<TPoco> GetList(Expression<Func<TPoco, bool>> Condition)
        {
            var dbQuery = db.Set<TPoco>();
            //dbQuery = db
            return dbQuery.Where(Condition).ToList();
        }

        public void Remove(Guid Id)
        {
            db.Set<TPoco>();
            var poco = db.Find<TPoco>(Id);
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
