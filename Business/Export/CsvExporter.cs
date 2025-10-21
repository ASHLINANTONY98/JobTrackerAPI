using CsvHelper;
using CsvHelper.Configuration;
using DataAccess.Entities;
using System.Globalization;

namespace Business.Export
{
    public static class CsvExporter
    {
        public static byte[] GenerateCsv(List<JobApplication> jobs)
        {
            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream);
            using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture));

            csv.WriteRecords(jobs);
            writer.Flush();

            return memoryStream.ToArray();
        }
    }
}
