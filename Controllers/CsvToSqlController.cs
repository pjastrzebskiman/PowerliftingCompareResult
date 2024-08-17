using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using PowerliftingCompareResult.Models;
using System.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PowerliftingCompareResult.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CsvToSqlController : ControllerBase
    {

        private readonly CsvSettings _csvToSql;

        public CsvToSqlController(IOptions<CsvSettings> csvToSql) 
        {
         _csvToSql = csvToSql.Value;
        }


       [HttpPost("execute")]
        public IActionResult PostDataFromFile()
        {
            try
            {
                bool isSuccess = ImportSelectedColumsFromCsvToDb(_csvToSql.FilePath, _csvToSql.Connectionstring, _csvToSql.TableName, _csvToSql.SelectedColumns);

                if (isSuccess)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        private bool ImportSelectedColumsFromCsvToDb(string csvFilePath, string connectionString, string TableName, string[] selectedColumns)
        {
            try
            {
                DataTable dataTable = new DataTable();
                using (StreamReader sr = new StreamReader(csvFilePath))
                {
                    string[] headers = sr.ReadLine().Split(',');
                    var selectedIndexes = headers.Select((header, index) => new { header, index })
                        .Where(h => selectedColumns.Contains(h.header))
                        .Select(h => h.index)
                        .ToList();
                    foreach (var column in selectedColumns)
                    {
                        dataTable.Columns.Add(column);
                    }

                    while (!sr.EndOfStream)
                    {
                        string[] rows = sr.ReadLine().Split(',');
                        DataRow dr = dataTable.NewRow();
                        for (int i = 0; i < selectedIndexes.Count; i++)
                        {
                            dr[i] = rows[selectedIndexes[i]];
                        }
                    }

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                        {
                            bulkCopy.DestinationTableName = TableName;
                            foreach (var column in selectedColumns)
                            {
                                bulkCopy.ColumnMappings.Add(column, column);
                            }
                            bulkCopy.WriteToServer(dataTable);
                        }

                    }
                    return true;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

    }
}
