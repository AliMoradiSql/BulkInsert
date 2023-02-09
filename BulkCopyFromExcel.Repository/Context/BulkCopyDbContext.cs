using BulkCopyFromExcel.Repository.Entities;
using Microsoft.EntityFrameworkCore;
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
        public virtual DbSet<BulkCopy> BulkCopy { get; set; }

        
    }
}
