namespace HireSphereApi.core.services
{
    using System.IO;
    using iText.Kernel.Pdf;
    using iText.Kernel.Pdf.Canvas.Parser;
    using DocumentFormat.OpenXml.Packaging;
    using DocumentFormat.OpenXml.Wordprocessing;

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
            using var memoryStream = new MemoryStream();
            fileStream.CopyTo(memoryStream);
            memoryStream.Position = 0;

            using var wordDoc = WordprocessingDocument.Open(memoryStream, false);
            var body = wordDoc.MainDocumentPart.Document.Body;
            return body.InnerText;
        }
    }

}
