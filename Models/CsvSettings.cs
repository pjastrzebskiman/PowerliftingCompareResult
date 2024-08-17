namespace PowerliftingCompareResult.Models
{
    public class CsvSettings
    {
        public string FilePath { get; set; }
        public string TableName { get; set; }
        public string[] SelectedColumns { get; set; }
        public string Connectionstring { get; set; }
    }
}
