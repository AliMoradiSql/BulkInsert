using BulkCopyFromExcel.Repository.Dto;
using BulkCopyFromExcel.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkCopyFromExcel.Repository.Context
{
    public class BulkCopyDbContext : DbContext
    {

        public BulkCopyDbContext()
        {

        }

        public BulkCopyDbContext(DbContextOptions<BulkCopyDbContext> options) : base(options)
        {
        }
        public DbSet<BulkCopy> BulkCopy { get; set; }
        public class DbContextConfiguration
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=BulkCopyDb;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            builder.Entity<BulkCopy>().HasKey(x => x.Id);
            builder.Entity<BulkCopy>().ToTable("BulkCopy");

            base.OnModelCreating(builder);
        }


    }
}
