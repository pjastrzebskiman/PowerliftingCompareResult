using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using PowerliftingCompareResult.Models;
using System.Data;
using System.IO;

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


        // [HttpPost("execute")]
        [HttpPost]
        public IActionResult PostDataFromFile()
        {
            try
            {
                bool isSuccess = ImportSelectedColumsFromCsvToDb(_csvToSql.FilePath, _csvToSql.Connectionstring, _csvToSql.TableName, _csvToSql.SelectedColumns);

                if (isSuccess)
                {
                    return NoContent();
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
                // Definiowanie typów danych dla wybranych kolumn
                var columnTypeMapping = new Dictionary<string, Type>
        {
            { "Name", typeof(string) },
            { "Sex", typeof(string) },
            { "Equipment", typeof(string) },
            { "Age", typeof(float) },
            { "AgeClass", typeof(string) },
            { "BodyweightKg", typeof(float) },
            { "WeightClassKg", typeof(string) },
            { "Squat1Kg", typeof(float) },
            { "Squat2Kg", typeof(float) },
            { "Squat3Kg", typeof(float) },
            { "Bench1Kg", typeof(float) },
            { "Bench2Kg", typeof(float) },
            { "Bench3Kg", typeof(float) },
            { "Deadlift1Kg", typeof(float) },
            { "Deadlift2Kg", typeof(float) },
            { "Deadlift3Kg", typeof(float) },
            { "Best3SquatKg", typeof(float) },
            { "Best3BenchKg", typeof(float) },
            { "Best3DeadliftKg", typeof(float) },
            { "TotalKg", typeof(float) },
            { "Country", typeof(string) },
            { "Federation", typeof(string) },
            { "Date", typeof(DateTime) },
            { "MeetCountry", typeof(string) },
            { "MeetName", typeof(string) }
        };

                // Mappings kolumn do bazy danych
                var columnMappings = new Dictionary<string, string>
        {
            { "Name", "Name" },
            { "Equipment", "EQ" },
            { "Age", "Age" },
            { "AgeClass", "AgeClass" },
            { "Sex", "Sex" },
            { "BodyweightKg", "BodyWeight" },
            { "WeightClassKg", "WeightClass" },
            { "Squat1Kg", "Squat1" },
            { "Squat2Kg", "Squat2" },
            { "Squat3Kg", "Squat3" },
            { "Bench1Kg", "Bench1" },
            { "Bench2Kg", "Bench2" },
            { "Bench3Kg", "Bench3" },
            { "Deadlift1Kg", "Deadlift1" },
            { "Deadlift2Kg", "Deadlift2" },
            { "Deadlift3Kg", "Deadlift3" },
            { "Best3SquatKg", "Squat" },
            { "Best3BenchKg", "Bench" },
            { "Best3DeadliftKg", "Deadlift" },
            { "TotalKg", "Total" },
            { "Country", "Country" },
            { "Federation", "Federation" },
            { "Date", "Date" },
            { "MeetCountry", "MeetCountry" },
            { "MeetName", "MeetName" }
        };

                int batchSize = 10000; // Ustal rozmiar partii według potrzeb

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                    {
                        bulkCopy.DestinationTableName = TableName;

                        // Dodaj mapowania kolumn
                        foreach (var mapping in columnMappings)
                        {
                            bulkCopy.ColumnMappings.Add(mapping.Key, mapping.Value);
                        }

                        // Inicjalizuj DataTable
                        DataTable dataTable = new DataTable();

                        // Dodaj kolumny do DataTable
                        foreach (var column in selectedColumns)
                        {
                            if (columnTypeMapping.ContainsKey(column))
                            {
                                dataTable.Columns.Add(column, columnTypeMapping[column]);
                            }
                            else
                            {
                                dataTable.Columns.Add(column, typeof(string)); // Domyślnie string
                            }
                        }

                        using (StreamReader sr = new StreamReader(csvFilePath))
                        {
                            string[] headers = sr.ReadLine().Split(',');
                            var selectedIndexes = headers.Select((header, index) => new { header, index })
                                .Where(h => selectedColumns.Contains(h.header))
                                .Select(h => h.index)
                                .ToList();

                            int rowCount = 0;
                            int totalRowCount = 0;

                            while (!sr.EndOfStream)
                            {
                                string[] rows = sr.ReadLine().Split(',');
                                DataRow dr = dataTable.NewRow();

                                for (int i = 0; i < selectedIndexes.Count; i++)
                                {
                                    string columnName = selectedColumns[i];
                                    string cellValue = rows[selectedIndexes[i]];

                                    // Przypisanie wartości z konwersją na odpowiedni typ
                                    if (columnTypeMapping.ContainsKey(columnName))
                                    {
                                        Type targetType = columnTypeMapping[columnName];
                                        try
                                        {
                                            if (targetType == typeof(float) || targetType == typeof(double) || targetType == typeof(decimal))
                                            {
                                                if (string.IsNullOrWhiteSpace(cellValue))
                                                {
                                                    dr[columnName] = DBNull.Value;
                                                }
                                                else
                                                {
                                                    dr[columnName] = Convert.ChangeType(
                                                        Convert.ToDouble(cellValue, System.Globalization.CultureInfo.InvariantCulture),
                                                        targetType);
                                                }
                                            }
                                            else if (targetType == typeof(DateTime))
                                            {
                                                if (DateTime.TryParse(cellValue, out DateTime dateValue))
                                                {
                                                    dr[columnName] = dateValue;
                                                }
                                                else
                                                {
                                                    dr[columnName] = DBNull.Value;
                                                }
                                            }
                                            else
                                            {
                                                dr[columnName] = Convert.ChangeType(cellValue, targetType);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine($"Error column convert '{columnName}' with value '{cellValue}': {ex.Message}");
                                            dr[columnName] = DBNull.Value;
                                        }
                                    }
                                    else
                                    {
                                        dr[columnName] = cellValue;
                                    }
                                }

                                dataTable.Rows.Add(dr);
                                rowCount++;
                                totalRowCount++;

                                // Jeśli osiągnięto rozmiar partii, wyślij dane do bazy
                                if (rowCount >= batchSize)
                                {
                                    try
                                    {
                                        bulkCopy.WriteToServer(dataTable);
                                        dataTable.Clear();
                                        rowCount = 0;
                                        Console.WriteLine($"{totalRowCount} rows processed and save to databaseh.");
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"Error during sending data: {ex.Message}");
                                        return false;
                                    }
                                }
                            }

                            // Wyślij pozostałe dane
                            if (dataTable.Rows.Count > 0)
                            {
                                try
                                {
                                    bulkCopy.WriteToServer(dataTable);
                                    dataTable.Clear();
                                    Console.WriteLine($"Last part {dataTable.Rows.Count} rows processed and save to database.");
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Error during sending data: {ex.Message}");
                                    return false;
                                }
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

    }
}
