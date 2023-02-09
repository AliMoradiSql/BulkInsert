using BulkCopyFromExcel.Repository.Context;
using BulkCopyFromExcel.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkCopyFromExcel.Repository.Dto
{
    public class AddToContextInputDto
    {
        public BulkCopyDbContext context { get; set; }
        public BulkCopy Entity { get; set; }
        public int Count { get; set; }
        public int CommitCount { get; set; }
        public bool RecreateContext { get; set; }

    }
}
