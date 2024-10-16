using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Npgsql;
using NpgsqlTypes;
using PowerliftingCompareResult.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

                // Mapowanie kolumn do bazy danych
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

                // Mapowanie System.Type na NpgsqlDbType
                var typeMapping = new Dictionary<Type, NpgsqlDbType>
                {
                    { typeof(string), NpgsqlDbType.Text },
                    { typeof(int), NpgsqlDbType.Integer },
                    { typeof(long), NpgsqlDbType.Bigint },
                    { typeof(float), NpgsqlDbType.Real },
                    { typeof(double), NpgsqlDbType.Double },
                    { typeof(decimal), NpgsqlDbType.Numeric },
                    { typeof(bool), NpgsqlDbType.Boolean },
                    { typeof(DateTime), NpgsqlDbType.Timestamp },
                    // Dodaj inne mapowania typów, jeśli potrzebne
                };

                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    // Przygotowanie listy kolumn do wstawienia
                    var columnsToWrite = selectedColumns.Select(columnName => new
                    {
                        SourceColumnName = columnName,
                        TargetColumnName = columnMappings[columnName],
                        ColumnType = columnTypeMapping.ContainsKey(columnName) ? columnTypeMapping[columnName] : typeof(string)
                    }).ToList();

                    // Konstrukcja polecenia COPY
                    string copyCommand = $"COPY {TableName} ({string.Join(",", columnsToWrite.Select(c => c.TargetColumnName))}) FROM STDIN (FORMAT BINARY)";

                    using (var writer = connection.BeginBinaryImport(copyCommand))
                    {
                        using (StreamReader sr = new StreamReader(csvFilePath))
                        {
                            // Odczyt nagłówków i stworzenie mapy nazwa kolumny -> indeks
                            string headerLine = sr.ReadLine();
                            string[] headers = headerLine.Split(',');
                            var headerIndexMap = headers.Select((header, index) => new { header, index })
                                                        .ToDictionary(h => h.header, h => h.index);

                            // Sprawdzenie, czy wszystkie wybrane kolumny istnieją w pliku CSV
                            foreach (var column in selectedColumns)
                            {
                                if (!headerIndexMap.ContainsKey(column))
                                {
                                    throw new Exception($"Kolumna '{column}' nie istnieje w pliku CSV.");
                                }
                            }

                            int totalRowCount = 0;

                            while (!sr.EndOfStream)
                            {
                                string line = sr.ReadLine();
                                if (string.IsNullOrWhiteSpace(line))
                                    continue; // Pomijanie pustych linii

                                string[] rows = line.Split(',');

                                // Rozpoczęcie nowego wiersza w imporcie
                                writer.StartRow();

                                foreach (var column in columnsToWrite)
                                {
                                    string sourceColumnName = column.SourceColumnName;
                                    int columnIndex = headerIndexMap[sourceColumnName];
                                    string cellValue = rows[columnIndex];

                                    Type targetType = column.ColumnType;

                                    // Konwersja wartości komórki na odpowiedni typ
                                    object value = null;
                                    try
                                    {
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
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"Błąd konwersji kolumny '{sourceColumnName}' z wartością '{cellValue}': {ex.Message}");
                                        value = DBNull.Value;
                                    }

                                    // Zapisanie wartości do importera
                                    if (value == DBNull.Value)
                                    {
                                        writer.WriteNull();
                                    }
                                    else
                                    {
                                        // Pobranie odpowiedniego NpgsqlDbType
                                        if (typeMapping.TryGetValue(targetType, out NpgsqlDbType npgsqlDbType))
                                        {
                                            writer.Write(value, npgsqlDbType);
                                        }
                                        else
                                        {
                                            // Jeśli typ nie jest zmapowany, użyj metody Write(object)
                                            writer.Write(value);
                                        }
                                    }
                                }

                                totalRowCount++;
                                if (totalRowCount % 10000 == 0)
                                {
                                    Console.WriteLine($"{totalRowCount} wierszy przetworzono.");
                                }
                            }
                        }

                        writer.Complete(); // Zakończenie importu
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
