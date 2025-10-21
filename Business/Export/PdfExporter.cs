using DinkToPdf;
using DinkToPdf.Contracts;
using DataAccess.Entities;
using System.Text;

namespace Business.Export
{
    public static class PdfExporter
    {
        public static byte[] GeneratePdf(List<JobApplication> jobs)
        {
            var html = new StringBuilder();
            html.Append("<h1>Job Applications</h1><table border='1'><tr><th>Company</th><th>Position</th><th>Status</th></tr>");

            foreach (var job in jobs)
            {
                html.Append($"<tr><td>{job.CompanyName}</td><td>{job.PositionTitle}</td><td>{job.Status}</td></tr>");
            }

            html.Append("</table>");

            var converter = new SynchronizedConverter(new PdfTools());
            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = { PaperSize = PaperKind.A4 },
                Objects = { new ObjectSettings { HtmlContent = html.ToString() } }
            };

            return converter.Convert(doc);
        }
    }
}
