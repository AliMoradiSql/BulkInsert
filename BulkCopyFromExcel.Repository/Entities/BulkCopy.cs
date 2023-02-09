using BulkCopyFromExcel.Repository.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkCopyFromExcel.Repository.Entities
{
    public class BulkCopy : Entity
    {
        public DateTime Date { get; set; }
        [MaxLength(50)]
        public string Description { get; set; }
        public double Deposits { get; set; }
        public double Withdrawls { get; set; }
        public double Balance { get; set; }

    }
}
