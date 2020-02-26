using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PBP.Pocos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PBP.DataAccess
{
    public class DBModel : DbContext
    {
        private string _Constr;
        public DbSet<CalendarPoco> Calendar { get; set; }
        public DbSet<ReservationPoco> Reservations { get; set; }
        public DbSet<TransactionPoco> Transactions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var config = new ConfigurationBuilder();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            config.AddJsonFile(path, false);
            var root = config.Build();
            _Constr = root.GetSection("ConnectionStrings").GetSection("DataConnection").Value;

            optionsBuilder.UseSqlServer(_Constr);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<TransactionPoco>()
                .HasOne(t => t.Reservation)
                .WithOne(r => r.Transaction)
                .HasForeignKey<TransactionPoco>(k => k.ReservationId);
            
            base.OnModelCreating(mb);
        }
    }
}
