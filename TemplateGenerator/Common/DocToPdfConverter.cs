using System.IO;

namespace Common
{
    public class DocToPdfConverter
    {
        private readonly string _docFilePath;
        private readonly string _pdfFilePath;

        public DocToPdfConverter(string docFilePath, string pdfFilePath)
        {
            _docFilePath = docFilePath;
            _pdfFilePath = pdfFilePath;
        }

        public void Convert()
        {
            if (!File.Exists(_docFilePath))
                return;

            Microsoft.Office.Interop.Word.Application app = new Microsoft.Office.Interop.Word.Application();
            var document = app.Documents.Open(_docFilePath);
            document.ExportAsFixedFormat(_pdfFilePath, Microsoft.Office.Interop.Word.WdExportFormat.wdExportFormatPDF);
            document.Close();
        }
    }
}
