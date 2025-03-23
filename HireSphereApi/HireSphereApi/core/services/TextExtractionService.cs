namespace HireSphereApi.core.services
{
    using System.IO;
    using Tesseract;
    using Xceed.Words.NET;
    using iText.Kernel.Pdf;
    using iText.Kernel.Pdf.Canvas.Parser;
    using System.Reflection.PortableExecutable;

    public class TextExtractionService
    {
        public string ExtractTextFromFile(Stream fileStream, string fileName)
        {
            if (fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                return ExtractTextFromPdf(fileStream);

            if (fileName.EndsWith(".docx", StringComparison.OrdinalIgnoreCase))
                return ExtractTextFromDocx(fileStream);

            return string.Empty;
        }

        private string ExtractTextFromPdf(Stream fileStream)
        {
            using var pdfReader = new PdfReader(fileStream);
            using var pdfDoc = new PdfDocument(pdfReader);
            var text = "";

            for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
            {
                text += PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(i)) + "\n";
            }

            return text;
        }

        private string ExtractTextFromDocx(Stream fileStream)
        {
            using var doc = DocX.Load(fileStream);
            return doc.Text;
        }
    }

}
