using BulkCopyFromExcel.Repository.Entities;
using ExcelDataReader;
using FastMember;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;
using System.Web;

namespace BulkCopyFromExcel.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        private DateTime convertor(object input)
        {
            DateTime dateTime;
            DateTime.TryParse(input.ToString(), out dateTime);
            return dateTime;

        }
        [HttpPost]
        public IActionResult Index(IFormFile? files)
        {
            List<BulkCopy> bulkCopies = new List<BulkCopy>();
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            if (files == null)
                return View(StatusCode(300));
            using (var ms = new MemoryStream())
            {
                files.CopyTo(ms);
                ms.Position = 0;
                DataTable dt = new DataTable();
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
                                Date = convertor(reader.GetValue(0)),
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


            var builder = WebApplication.CreateBuilder();
            var myConnection = builder.Configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection connection = new SqlConnection(myConnection))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
                        try
                        {
                            using (var reader = ObjectReader.Create(bulkCopies))// I should pass all data as a list hear
                            {
                                sqlBulkCopy.BatchSize = 100000;
                                sqlBulkCopy.DestinationTableName = "BulkCopy";
                                sqlBulkCopy.ColumnMappings.Add("Date", "Date");
                                sqlBulkCopy.ColumnMappings.Add("Description", "Description");
                                sqlBulkCopy.ColumnMappings.Add("Deposits", "Deposits");
                                sqlBulkCopy.ColumnMappings.Add("Withdrawls", "Withdrawls");
                                sqlBulkCopy.ColumnMappings.Add("Balance", "Balance");
                                sqlBulkCopy.WriteToServer(reader);
                            }

                            transaction.Commit();
                        }
                        catch (Exception)
                        {

                            transaction.Rollback();
                            connection.Close();
                            throw;
                        }
                }
            }
            return View("Index");
        }

    }
}
