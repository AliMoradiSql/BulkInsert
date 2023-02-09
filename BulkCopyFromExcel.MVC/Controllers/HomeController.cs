using BulkCopyFromExcel.MVC.Models;
using BulkCopyFromExcel.Repository.Context;
using BulkCopyFromExcel.Repository.Dto;
using BulkCopyFromExcel.Repository.Entities;
using ExcelDataReader;
using FastMember;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.Transactions;

namespace BulkCopyFromExcel.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult Index(IFormFile? files)
        {
            BulkCopyLibrary bulk = new BulkCopyLibrary();

            var response = bulk.BulkCopyFun(files);
            if (response != HttpStatusCode.OK)
            {
                throw new Exception("Status Code 400");
            }
            return View();
        }

        //[HttpPost]
        //public IActionResult Index(IFormFile? files)
        //{
        //    List<BulkCopy> bulkCopies = new List<BulkCopy>();
        //    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        //    if (files == null)
        //        return View(StatusCode(400));
        //    using (var ms = new MemoryStream())
        //    {
        //        files.CopyTo(ms);
        //        ms.Position = 0;
        //        DataTable dt = new DataTable();
        //        using (var reader = ExcelReaderFactory.CreateReader(ms))
        //        {
        //            reader.Read();
        //            var obj = reader.GetValue(0);


        //            while (reader.Read()) //Each row of the file
        //            {
        //                if (obj != typeof(string))
        //                {
        //                    bulkCopies.Add(new BulkCopy
        //                    {
        //                        Date = DateConvertor(reader.GetValue(0)),
        //                        Description = reader.GetValue(1).ToString(),
        //                        Deposits = (double)reader.GetDouble(2),
        //                        Withdrawls = (double)reader.GetDouble(3),
        //                        Balance = (double)reader.GetDouble(4),
        //                    });
        //                }
        //                else
        //                {
        //                    obj = null;
        //                }
        //            }
        //        }

        //    }
        //    using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 1, 0)))
        //    {
        //        BulkCopyDbContext context = null;

        //        try
        //        {
        //            context = new BulkCopyDbContext();
        //            context.ChangeTracker.AutoDetectChangesEnabled = false;

        //            int count = 0;
        //            foreach (var item in bulkCopies)
        //            {
        //                ++count;
        //                context = AddToContext(new AddToContextInputDto
        //                {
        //                    context = context,
        //                    Entity = item,
        //                    Count = count,
        //                    CommitCount = 100,
        //                    RecreateContext = true
        //                }
        //                    );
        //                //context = AddToContext(context, item, count, 50000, true);
        //            }
        //            context.SaveChanges();
        //        }
        //        finally
        //        {
        //            if (context != null)
        //                context.Dispose();
        //        }
        //        scope.Complete();
        //    }
        //    //var builder = WebApplication.CreateBuilder();
        //    //var myConnection = builder.Configuration.GetConnectionString("DefaultConnection");
        //    //using (SqlConnection connection = new SqlConnection(myConnection))
        //    //{
        //    //    connection.Open();
        //    //    using (SqlTransaction transaction = connection.BeginTransaction())
        //    //    {
        //    //        using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
        //    //            try
        //    //            {
        //    //                using (var reader = ObjectReader.Create(bulkCopies))// I should pass all data as a list hear
        //    //                {
        //    //                    sqlBulkCopy.BatchSize = 50000;
        //    //                    sqlBulkCopy.DestinationTableName = "BulkCopy";
        //    //                    sqlBulkCopy.ColumnMappings.Add("Date", "Date");
        //    //                    sqlBulkCopy.ColumnMappings.Add("Description", "Description");
        //    //                    sqlBulkCopy.ColumnMappings.Add("Deposits", "Deposits");
        //    //                    sqlBulkCopy.ColumnMappings.Add("Withdrawls", "Withdrawls");
        //    //                    sqlBulkCopy.ColumnMappings.Add("Balance", "Balance");
        //    //                    sqlBulkCopy.WriteToServer(reader);
        //    //                }

        //    //                transaction.Commit();
        //    //            }
        //    //            catch (Exception)
        //    //            {

        //    //                transaction.Rollback();
        //    //                connection.Close();
        //    //                throw;
        //    //            }
        //    //    }
        //    //}
        //    return View("Index");
        //}

        private DateTime DateConvertor(object input)
        {
            DateTime dateTime;
            if (input is null) return DateTime.UtcNow;
            DateTime.TryParse(input.ToString(), out dateTime);
            return dateTime;

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
    }
}