using BulkCopyFromExcel.Repository.Context;
using BulkCopyFromExcel.Repository.Entities;
using FastMember;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.SqlClient;

namespace BulkCopyFromExcel.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly BulkCopyDbContext bulkCopyDbContext;

        public IndexModel(ILogger<IndexModel> logger, BulkCopyDbContext bulkCopyDbContext)
        {
            _logger = logger;
            this.bulkCopyDbContext = bulkCopyDbContext;
        }

        public void OnGet()
        {
        }

        public void OnPut()
        {
            var builder = WebApplication.CreateBuilder();
            var myConnection = builder.Configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection connection = new SqlConnection(myConnection))
            {
                //sqlConnection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(connection,SqlBulkCopyOptions.Default,transaction))
                        try
                        {
                            using (var reader = ObjectReader.Create(new List<BulkCopy>()))// I should pass all data as a list hear
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
        }

        public void OnSubmit()
        {

        }
    }
}