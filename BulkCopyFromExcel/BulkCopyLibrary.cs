using BulkCopyFromExcel.Repository.Context;
using BulkCopyFromExcel.Repository.Dto;
using BulkCopyFromExcel.Repository.Entities;
using ExcelDataReader;
using Microsoft;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Transactions;

namespace BulkCopyFromExcel
{
    public class BulkCopyLibrary
    {

        public HttpStatusCode BulkCopyFun(IFormFile? files)
        {

            List<BulkCopy> bulkCopies = new List<BulkCopy>();
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            if (files == null)
                 return HttpStatusCode.BadRequest;
            using (var ms = new MemoryStream())
            {
                files.CopyTo(ms);
                ms.Position = 0;
                using (var reader = ExcelReaderFactory.CreateReader(ms))
                {
                    reader.Read();
                    var obj = reader.GetValue(0);


                    while (reader.Read()) //Each row of the file
                    {
                        if (obj != typeof(string))
                        {
                            bulkCopies.Add(new BulkCopy
                            {
                                Date = DateConvertor(reader.GetValue(0)),
                                Description = reader.GetValue(1).ToString(),
                                Deposits = (double)reader.GetDouble(2),
                                Withdrawls = (double)reader.GetDouble(3),
                                Balance = (double)reader.GetDouble(4),
                            });
                        }
                        else
                        {
                            obj = null;
                        }
                    }
                }

            }
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 5, 0)))
            {
                BulkCopyDbContext context = null;

                try
                {
                    context = new BulkCopyDbContext();
                    context.ChangeTracker.AutoDetectChangesEnabled = false;

                    int count = 0;
                    foreach (var item in bulkCopies)
                    {
                        ++count;
                        context = AddToContext(new AddToContextInputDto
                        {
                            context = context,
                            Entity = item,
                            Count = count,
                            CommitCount = 100,
                            RecreateContext = true
                        }
                            );
                        //context = AddToContext(context, item, count, 50000, true);
                    }
                    context.SaveChanges();
                }
                finally
                {
                    if (context != null)
                        context.Dispose();
                }
                scope.Complete();
            }
            return HttpStatusCode.OK;

        }

        private BulkCopyDbContext AddToContext(AddToContextInputDto input)
        {
            input.context.Set<BulkCopy>().Add(input.Entity);

            if (input.Count % input.CommitCount == 0)
            {
                input.context.SaveChanges();

                if (input.RecreateContext)
                {
                    input.context.Dispose();
                    input.context = new BulkCopyDbContext();
                    input.context.ChangeTracker.AutoDetectChangesEnabled = false;
                }
            }
            return input.context;
        }

        private DateTime DateConvertor(object input)
        {
            DateTime dateTime;
            if (input is null) return DateTime.UtcNow;
            DateTime.TryParse(input.ToString(), out dateTime);
            return dateTime;

        }
    }
}