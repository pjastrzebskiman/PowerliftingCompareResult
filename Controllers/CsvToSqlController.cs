﻿using Microsoft.AspNetCore.Mvc;
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
                DataTable dataTable = new DataTable();

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

                using (StreamReader sr = new StreamReader(csvFilePath))
                {
                    string[] headers = sr.ReadLine().Split(',');
                    var selectedIndexes = headers.Select((header, index) => new { header, index })
                        .Where(h => selectedColumns.Contains(h.header))
                        .Select(h => h.index)
                        .ToList();

                    // Dodawanie kolumn z odpowiednimi typami do DataTable
                    foreach (var column in selectedColumns)
                    {
                        if (columnTypeMapping.ContainsKey(column))
                        {
                            dataTable.Columns.Add(column, columnTypeMapping[column]);
                        }
                        else
                        {
                            dataTable.Columns.Add(column, typeof(string)); // Domyślnie string, jeśli brak mapowania
                        }
                    }

                    int rowCount = 0; // Licznik przetworzonych wierszy
                    int logInterval = 1000; // Jak często logować (co 1000 wierszy)

                    // Wypełnianie DataTable
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
                                //     dr[columnName] = Convert.ChangeType(cellValue, targetType);
                                try
                                {
                                    if (targetType == typeof(float) || targetType == typeof(double) || targetType == typeof(decimal))
                                    {
                                        // Użyj konwersji z CultureInfo.InvariantCulture dla typów zmiennoprzecinkowych
                                        if (string.IsNullOrWhiteSpace(cellValue))
                                        {
                                            // Jeśli cellValue jest pusty, ustaw DBNull.Value lub domyślną wartość
                                            dr[columnName] = DBNull.Value; // Lub np. 0f dla domyślnej wartości
                                        }
                                        else
                                        {
                                            // Użyj konwersji z CultureInfo.InvariantCulture dla typów zmiennoprzecinkowych
                                            dr[columnName] = Convert.ChangeType(
                                                Convert.ToDouble(cellValue, System.Globalization.CultureInfo.InvariantCulture),
                                                targetType);
                                        }
                                    }
                                    else
                                    {
                                        // Standardowa konwersja dla innych typów
                                        dr[columnName] = Convert.ChangeType(cellValue, targetType);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Error converting column '{columnName}' with value '{cellValue}': {ex.Message}");
                                    dr[columnName] = DBNull.Value; // Lub inna logika obsługi błędu
                                }
                            }
                            else
                            {
                                dr[columnName] = cellValue;
                            }
                        }
                        dataTable.Rows.Add(dr);
                        rowCount++; // Zwiększenie licznika wierszy
                                    // Logowanie co określoną liczbę wierszy
                        if (rowCount % logInterval == 0)
                        {
                            Console.WriteLine($"{rowCount} rows processed.");
                        }
                       
                    }
                    sr.Dispose();
                }
              

                //
             //   dataTable.EndLoadData();


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


                int batchSize = 100000;
                int totalRows = dataTable.Rows.Count;
                int batchCount = (int)Math.Ceiling((double)totalRows / batchSize);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    for (int batchIndex = 0; batchIndex < batchCount; batchIndex++)
                    {
                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                        {
                            bulkCopy.DestinationTableName = TableName;
                            /* foreach (var column in selectedColumns)
                             {
                                 bulkCopy.ColumnMappings.Add(column, column);
                             }*/

                            foreach (var mapping in columnMappings)
                            {
                                bulkCopy.ColumnMappings.Add(mapping.Key, mapping.Value);
                            }

                            try
                            {
                                // bulkCopy.WriteToServer(dataTable);
                                bulkCopy.WriteToServer(dataTable.AsEnumerable()
                              .Skip(batchIndex * batchSize)
                              .Take(batchSize)
                              .CopyToDataTable());
                                Console.WriteLine($"Batch {batchIndex + 1} of {batchCount} processed successfully.");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error during bulk copy: {ex.Message}");
                            }
                            bulkCopy.Close();
                        }


                    }
                    dataTable.Clear();
                    dataTable.Dispose();
                    connection.Dispose();
                    return true;
                   
                    //}

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