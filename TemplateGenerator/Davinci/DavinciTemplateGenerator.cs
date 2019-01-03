using Common;
using System;
using System.IO;
using System.Linq;
using Xceed.Words.NET;

namespace Davinci
{
    public class DavinciTemplateGenerator
    {
        private readonly DataModel _dataModel;
        private readonly DavinciAddressModel _davinciAddressModel;
        private DocToPdfConverter _docToPdfConverter;

        private const string CalibriFont = "Calibri";

        public DavinciTemplateGenerator(DataModel dataModel, DavinciAddressModel davinciAddressModel = null)
        {
            _dataModel = dataModel;
            _davinciAddressModel = davinciAddressModel;

            if (_davinciAddressModel == null)
            {
                _davinciAddressModel = new DavinciAddressModel();
            }
        }

        public void GenerateDoc(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return;

            DocX doc = DocX.Load("Template/template.docx");

            Paragraph cubicleAddress = doc.InsertParagraph();

            cubicleAddress.Alignment = Alignment.left;

            cubicleAddress.AppendLine();
            cubicleAddress.AppendLine();
            cubicleAddress.AppendLine();

            cubicleAddress.Append(_dataModel.AddressLine).Font(CalibriFont).FontSize(9);
            cubicleAddress.AppendLine();

            var phoneString = $"Phone {_dataModel.Phone}";

            cubicleAddress.Append(phoneString).Font(CalibriFont).FontSize(9);

            Table faxAndInvoiceInfo = doc.AddTable(2, 2);
            faxAndInvoiceInfo.SetWidthsPercentage(new float[] { 50, 50 }, null);

            var faxString = $"Fax {_dataModel.Fax}";

            faxAndInvoiceInfo.Rows[0].Cells[0].Paragraphs.FirstOrDefault().Append(faxString)
                .Font(CalibriFont).FontSize(9).Alignment = Alignment.left;

            var invoiceString = $"INVOICE # { _dataModel.InvoiceId}";

            faxAndInvoiceInfo.Rows[0].Cells[1].Paragraphs.FirstOrDefault().Append(invoiceString)
                .Font(CalibriFont).FontSize(8).Alignment = Alignment.right;

            var dateString = $"DATE: {_dataModel.InvoiceDate.ToShortDateString()}";

            faxAndInvoiceInfo.Rows[1].Cells[1].Paragraphs.FirstOrDefault().Append(dateString)
                .Font(CalibriFont).FontSize(8).Alignment = Alignment.right;

            cubicleAddress.InsertTableAfterSelf(faxAndInvoiceInfo);

            Paragraph emptyParagraph = doc.InsertParagraph();

            emptyParagraph.AppendLine();
            emptyParagraph.AppendLine();
            emptyParagraph.AppendLine();

            Paragraph forParagraph = doc.InsertParagraph();

            forParagraph.Alignment = Alignment.center;

            forParagraph.Append($"FOR: {_dataModel.PersonalName}").Font(CalibriFont).FontSize(11).Bold();

            Table businessNameTable = doc.AddTable(6, 2);

            businessNameTable.SetWidthsPercentage(new float[] { 45.5f, 54.5f }, null);

            businessNameTable.Rows[0].Cells[0].Paragraphs.FirstOrDefault().Append("TO:")
                .Font(CalibriFont).FontSize(8).Bold().Alignment = Alignment.left;
            businessNameTable.Rows[0].Cells[1].Paragraphs.FirstOrDefault().Append(_dataModel.BusinessName)
                .Font(CalibriFont).FontSize(11).Bold().Alignment = Alignment.left;

            businessNameTable.Rows[1].Cells[0].Paragraphs.FirstOrDefault().Append(_davinciAddressModel.Name)
                .Font(CalibriFont).FontSize(9).Alignment = Alignment.left;

            businessNameTable.Rows[2].Cells[0].Paragraphs.FirstOrDefault().Append(_davinciAddressModel.AddressLine1)
                .Font(CalibriFont).FontSize(9).Alignment = Alignment.left;

            businessNameTable.Rows[3].Cells[0].Paragraphs.FirstOrDefault().Append(_davinciAddressModel.AddressLine2)
                .Font(CalibriFont).FontSize(9).Alignment = Alignment.left;

            var davinciPhone = $"Phone: {_davinciAddressModel.Phone}";

            businessNameTable.Rows[4].Cells[0].Paragraphs.FirstOrDefault().Append(davinciPhone)
                .Font(CalibriFont).FontSize(9).Alignment = Alignment.left;

            var davinciFax = $"Fax: {_davinciAddressModel.Fax}";

            businessNameTable.Rows[5].Cells[0].Paragraphs.FirstOrDefault().Append(davinciFax)
                .Font(CalibriFont).FontSize(9).Alignment = Alignment.left;

            forParagraph.InsertTableAfterSelf(businessNameTable);

            Paragraph emptyParagraph2 = doc.InsertParagraph();

            emptyParagraph2.AppendLine();

            Paragraph actionsParagraph = doc.InsertParagraph();

            Table actionsTable = doc.AddTable(15, 4);

            actionsTable.SetWidthsPercentage(new float[] { 60, 20, 10, 10 }, null);

            // Borders

            for (int i = 0; i < actionsTable.RowCount; i++)
            {
                actionsTable.Rows[i].Height = 18;

                if (actionsTable.RowCount - 1 == i)
                    continue;

                for (int j = 0; j < actionsTable.Rows[i].Cells.Count; j++)
                {
                    actionsTable.Rows[i].Cells[j].SetBorder(TableCellBorderType.Bottom, new Border { Size = BorderSize.one });
                    actionsTable.Rows[i].Cells[j].SetBorder(TableCellBorderType.Left, new Border { Size = BorderSize.one });
                    actionsTable.Rows[i].Cells[j].SetBorder(TableCellBorderType.Right, new Border { Size = BorderSize.one });
                    actionsTable.Rows[i].Cells[j].VerticalAlignment = VerticalAlignment.Center;
                }
            }

            actionsTable.SetBorder(TableBorderType.Top, new Border { Size = BorderSize.one });

            actionsTable.Rows[0].Cells[0].Paragraphs.FirstOrDefault().Append("DESCRIPTION")
                .Bold().Font(CalibriFont).FontSize(8).Alignment = Alignment.center;

            actionsTable.Rows[0].Cells[1].Paragraphs.FirstOrDefault().Append("DATE")
                .Bold().Font(CalibriFont).FontSize(8).Alignment = Alignment.center;

            actionsTable.Rows[0].Cells[2].Paragraphs.FirstOrDefault().Append("RATE")
                .Bold().Font(CalibriFont).FontSize(8).Alignment = Alignment.center;

            actionsTable.Rows[0].Cells[3].Paragraphs.FirstOrDefault().Append("AMOUNT")
                .Bold().Font(CalibriFont).FontSize(8).Alignment = Alignment.center;

            if (_dataModel.Actions != null && _dataModel.Actions.Count > 0)
            {
                int i = 1;

                foreach (var record in _dataModel.Actions)
                {
                    actionsTable.Rows[i].Cells[0].Paragraphs.FirstOrDefault().Append(record.Description);

                    var formattedDate = $"{record.StartDate.ToShortDateString()} - {record.EndDate.ToShortDateString()}";

                    actionsTable.Rows[i].Cells[1].Paragraphs.FirstOrDefault()
                        .Append(formattedDate).Alignment = Alignment.center;

                    actionsTable.Rows[i].Cells[2].Paragraphs.FirstOrDefault()
                        .Append(record.Rate).Alignment = Alignment.right;

                    actionsTable.Rows[i].Cells[3].Paragraphs.FirstOrDefault()
                        .Append($"{record.AmountSign} {record.Amount.ToString("0.00")}").Alignment = Alignment.right;

                    i++;
                }
            }

            var total = $"{_dataModel.Actions.FirstOrDefault().AmountSign} {_dataModel.Actions.Sum(x => x.Amount).ToString("0.00")}";

            var totalCell = actionsTable.Rows.LastOrDefault().Cells[3];

            var totalWord = actionsTable.Rows.LastOrDefault().Cells[2];
            totalWord.VerticalAlignment = VerticalAlignment.Center;

            totalWord.Paragraphs.FirstOrDefault().Append("TOTAL").Font(CalibriFont).FontSize(8).Alignment = Alignment.right;

            totalCell.VerticalAlignment = VerticalAlignment.Center;

            totalCell.SetBorder(TableCellBorderType.Left, new Border { Size = BorderSize.one });
            totalCell.SetBorder(TableCellBorderType.Right, new Border { Size = BorderSize.one });
            totalCell.SetBorder(TableCellBorderType.Bottom, new Border { Size = BorderSize.one });

            totalCell.Paragraphs.FirstOrDefault().Append(total).Alignment = Alignment.right;

            actionsParagraph.InsertTableAfterSelf(actionsTable);

            Paragraph emptyParagraph3 = doc.InsertParagraph();

            emptyParagraph3.AppendLine();

            Paragraph noteParagraph = doc.InsertParagraph();

            noteParagraph.Append($"NOTES: {_dataModel.Notes}")
                .Font(CalibriFont).FontSize(9).Alignment = Alignment.left;

            noteParagraph.AppendLine();
            noteParagraph.AppendLine();

            Paragraph thankYouParagraph = doc.InsertParagraph();

            thankYouParagraph.AppendLine();

            thankYouParagraph.Append("THANK YOU FOR YOUR BUSINESS!")
                .Font(CalibriFont).FontSize(8).Bold().Alignment = Alignment.center;

            doc.SaveAs(filePath);

            doc.Dispose();
        }

        public void GeneratePdf(string pdfFilePath)
        {
            var tempFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "temp1.doc");
            GenerateDoc(tempFileName);

            _docToPdfConverter = new DocToPdfConverter(tempFileName, pdfFilePath);
            _docToPdfConverter.Convert();

            if (File.Exists(tempFileName))
            {
                File.Delete(tempFileName);
            }
        }
    }
}
