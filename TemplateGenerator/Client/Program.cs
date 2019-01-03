using Davinci;
using System;
using System.Collections.Generic;
using System.IO;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var dataModel = new DataModel
            {
                InvoiceId = "4651231864",
                InvoiceDate = DateTime.Now,
                PersonalName = "Narek Yegoryan",
                BusinessName = "Business name",
                Notes = "Sample note... Very long text...",
                Actions = new List<ActionModel>
                {
                    new ActionModel {Description = "AAA", Amount = 12.5, Rate = "12.5 P/M"},
                    new ActionModel {Description = "BBB", Amount = 15.5, Rate = "15.5 P/M"},
                    new ActionModel {Description = "CCC", Amount = 12.5, Rate = "12.5 P/M"},
                    new ActionModel {Description = "DDD", Amount = 12.5, Rate = "12.5 P/M"},
                }
            };

            DavinciTemplateGenerator generator = new DavinciTemplateGenerator(dataModel);

            var filePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            var fileName = "davinci_mailbox_template.doc";

            generator.GenerateDoc(Path.Combine(filePath, fileName));

            generator.GeneratePdf(Path.Combine(filePath, "davinci_mailbox_template.pdf"));

            Console.WriteLine("Saved... {0}", Path.Combine(filePath, fileName));

            Console.Read();
        }
    }
}
