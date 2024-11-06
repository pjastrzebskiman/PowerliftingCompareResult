using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;
using NpgsqlTypes;
using PowerliftingCompareResult.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace PowerliftingCompareResult.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CsvToSqlController : ControllerBase
    {
        private readonly CsvSettings _csvToSql;
        private readonly HttpClient _httpClient;
        private readonly ResultContext _context;

        public CsvToSqlController(IOptions<CsvSettings> csvToSql, HttpClient httpClient, ResultContext context)
        {
            _csvToSql = csvToSql.Value;
            _httpClient = httpClient;
             _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> PostDataFromFile()
        {
            try
            {
                string connectionString = _context.Database.GetDbConnection().ConnectionString;
                bool isSuccess = await ImportSelectedColumsFromCsvToDbAsync(_csvToSql.FilePath, _csvToSql.TableName, _csvToSql.SelectedColumns);

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


        private async Task<bool> ImportSelectedColumsFromCsvToDbAsync(string csvFilePath, string TableName, string[] selectedColumns)
        {
            try
            {
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

                var typeMapping = new Dictionary<Type, NpgsqlDbType>
        {
            { typeof(string), NpgsqlDbType.Text },
            { typeof(int), NpgsqlDbType.Integer },
            { typeof(long), NpgsqlDbType.Bigint },
            { typeof(float), NpgsqlDbType.Real },
            { typeof(double), NpgsqlDbType.Double },
            { typeof(decimal), NpgsqlDbType.Numeric },
            { typeof(bool), NpgsqlDbType.Boolean },
            { typeof(DateTime), NpgsqlDbType.Timestamp }
        };

            using (var connection = _context.Database.GetDbConnection() as NpgsqlConnection)

                {
                    await connection.OpenAsync();

                    var columnsToWrite = selectedColumns.Select(columnName => new
                    {
                        SourceColumnName = columnName,
                        TargetColumnName = columnMappings[columnName],
                        ColumnType = columnTypeMapping.ContainsKey(columnName) ? columnTypeMapping[columnName] : typeof(string)
                    }).ToList();

                    string TableName2 = "public.\"LiftResults\"";
                    string copyCommand = $"COPY {TableName2} ({string.Join(",", columnsToWrite.Select(c => "\"" + c.TargetColumnName + "\""))}) FROM STDIN (FORMAT BINARY)";
                    Console.WriteLine($"Polecenie COPY: {copyCommand}");
                    Console.WriteLine($"Pobieranie pliku CSV z adresu: {csvFilePath}");
                    using (var writer = connection.BeginBinaryImport(copyCommand))
                    {
                        using (var response = await _httpClient.GetAsync(csvFilePath, HttpCompletionOption.ResponseHeadersRead))
                        {
                        Console.WriteLine($"Pobieranie pliku CSV z adresu: {csvFilePath}");
                            response.EnsureSuccessStatusCode();
                            using (var responseStream = await response.Content.ReadAsStreamAsync())
                            using (var sr = new StreamReader(responseStream))
                            {
                                string headerLine = sr.ReadLine();
                                if (string.IsNullOrWhiteSpace(headerLine))
                                {
                                    Console.WriteLine("Plik CSV jest pusty lub nie zawiera nagłówków.");
                                    return false;
                                }

                                string[] headers = headerLine.Split(',');
                                var headerIndexMap = headers.Select((header, index) => new { header = header.Trim(), index })
                                                            .ToDictionary(h => h.header, h => h.index);

                                foreach (var column in selectedColumns)
                                {
                                    if (!headerIndexMap.ContainsKey(column))
                                    {
                                        throw new Exception($"Kolumna '{column}' nie istnieje w pliku CSV.");
                                    }
                                }

                                int totalRowCount = 0;
                                int batchRowCount = 0;
                                int batchSize = 10000;

                                while (!sr.EndOfStream)
                                {
                                    string line = sr.ReadLine();
                                    if (string.IsNullOrWhiteSpace(line)) continue;

                                    string[] rows = line.Split(',');
                                    writer.StartRow();

                                    foreach (var column in columnsToWrite)
                                    {
                                        string sourceColumnName = column.SourceColumnName;
                                        int columnIndex = headerIndexMap[sourceColumnName];
                                        string cellValue = rows[columnIndex];
                                        Type targetType = column.ColumnType;
                                        object value = null;

                                        try
                                        {
                                            // value = string.IsNullOrWhiteSpace(cellValue) ? DBNull.Value : Convert.ChangeType(cellValue, targetType);
                                            if (string.IsNullOrWhiteSpace(cellValue))
                                            {
                                                value = DBNull.Value;
                                            }
                                            else if (targetType == typeof(float) || targetType == typeof(double) || targetType == typeof(decimal))
                                            {
                                                value = Convert.ChangeType(Convert.ToDouble(cellValue, System.Globalization.CultureInfo.InvariantCulture), targetType);
                                            }
                                            else if (targetType == typeof(int) || targetType == typeof(long))
                                            {
                                                value = Convert.ChangeType(Convert.ToInt64(cellValue, System.Globalization.CultureInfo.InvariantCulture), targetType);
                                            }
                                            else if (targetType == typeof(DateTime))
                                            {
                                                if (DateTime.TryParse(cellValue, out DateTime dateValue))
                                                {
                                                    value = dateValue;
                                                }
                                                else
                                                {
                                                    value = DBNull.Value;
                                                }
                                            }
                                            else
                                            {
                                                value = Convert.ChangeType(cellValue, targetType);
                                            }
                                        }
                                        catch
                                        {
                                            value = DBNull.Value;
                                        }

                                        if (value == DBNull.Value)
                                            writer.WriteNull();
                                        else
                                            writer.Write(value, typeMapping[targetType]);
                                    }

                                    totalRowCount++;
                                    batchRowCount++;

                                    // Wyświetlanie komunikatu co `batchSize` wierszy
                                    if (batchRowCount >= batchSize)
                                    {
                                        Console.WriteLine($"{totalRowCount} wierszy przetworzono.");
                                        batchRowCount = 0;
                                    }
                                }

                                // Po zakończeniu całego procesu zapisu danych wykonujemy `Complete`.
                                writer.Complete();
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd: {ex.Message}");
                return false;
            }
        }


    }
}
