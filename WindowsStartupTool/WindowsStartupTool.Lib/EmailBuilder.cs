using ProcessController.Lib;
using System;
using System.IO;
using System.Linq;
using System.Web.UI;

namespace WindowsStartupTool.Lib
{
    public class EmailBuilder
    {
        private StringWriter StringWriter;

        HtmlTextWriter CreateHTMLDocument()
        {
            StringWriter = new StringWriter();
            var document = new HtmlTextWriter(StringWriter);

            document.RenderBeginTag(HtmlTextWriterTag.Html);
            document.RenderBeginTag(HtmlTextWriterTag.Body);
            document.RenderBeginTag(HtmlTextWriterTag.Div);

            return document;
        }

        void InsertTr(HtmlTextWriter document, params string[] columns)
        {
            document.RenderBeginTag(HtmlTextWriterTag.Tr);
            foreach (var item in columns)
            {
                document.RenderBeginTag(HtmlTextWriterTag.Td);
                document.Write(item);
                document.RenderEndTag();
            }
            document.RenderEndTag();
        }

        public string BuildReport(int pingTimeout)
        {
            var lib = new Library();

            var _computers = lib.GetCurrentDomainComputers();

            if (_computers == null)
                return "No comptuers found";

            var document = CreateHTMLDocument();

            foreach (var computer in _computers)
            {
                bool isOnline = lib.Ping(computer, 200);

                document.AddStyleAttribute("Border", "3px solid gray");
                document.AddStyleAttribute(HtmlTextWriterStyle.Padding, "5px");
                document.RenderBeginTag(HtmlTextWriterTag.Div); // Start position of each computer information
                document.RenderBeginTag(HtmlTextWriterTag.Div);
                document.RenderBeginTag(HtmlTextWriterTag.Label);
                document.Write($"PC Name: {computer} | Status: ");
                if (isOnline)
                    document.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, "Green");
                else
                    document.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, "Red");
                document.AddStyleAttribute(HtmlTextWriterStyle.Color, "White");
                document.RenderBeginTag(HtmlTextWriterTag.Label);

                document.Write(isOnline ? "Online" : "Offline");
                document.RenderEndTag();
                document.RenderEndTag();
                document.RenderEndTag(); // DIV

                if (!isOnline)
                {
                    document.RenderEndTag(); // DIV
                    continue;
                }

                // available space on C drive
                document.RenderBeginTag(HtmlTextWriterTag.Div);
                document.RenderBeginTag(HtmlTextWriterTag.Label);

                var readableSpace = lib.GetDiskAvailableSpace(computer);
                document.Write($"Available space on drive: {readableSpace}");

                document.RenderEndTag();
                document.RenderEndTag();

                document.RenderBeginTag(HtmlTextWriterTag.Hr);
                document.RenderEndTag();

                document.RenderBeginTag(HtmlTextWriterTag.Div);
                document.RenderBeginTag(HtmlTextWriterTag.Label);
                document.Write("Found these apps from startup registry");
                document.RenderEndTag();

                try
                {
                    using (var regedit = new RegistryEditor(computer, startServiceIfNeeded: true)) // default value, service may not start
                    {
                        var startupApps = regedit.GetAllStartupAppsFromRegistry(SkippableSourceEnum.None);

                        document.AddStyleAttribute("Border", "1px solid black");
                        document.AddAttribute(HtmlTextWriterAttribute.Border, "1");
                        document.RenderBeginTag(HtmlTextWriterTag.Table);
                        document.RenderBeginTag(HtmlTextWriterTag.Thead);

                        InsertTr(document, "Name", "Location");
                        document.RenderEndTag();
                        document.RenderBeginTag(HtmlTextWriterTag.Tbody);

                        foreach (var app in startupApps)
                        {
                            InsertTr(document, app.Key, app.Value);
                        }

                        document.RenderEndTag();
                        document.RenderEndTag();
                        document.RenderEndTag(); // DIV end

                        document.RenderBeginTag(HtmlTextWriterTag.Hr);
                        document.RenderEndTag();

                        // missing from default apps

                        var missingFromDefaultApps = regedit.DefaultStartupApps.Where(da => !startupApps.Any(x => x.Key == da)).ToList();

                        document.RenderBeginTag(HtmlTextWriterTag.Div);
                        document.RenderBeginTag(HtmlTextWriterTag.Label);
                        document.Write("Missing from default apps");
                        document.RenderEndTag();
                        document.AddStyleAttribute("Border", "1px solid black");
                        document.AddAttribute(HtmlTextWriterAttribute.Border, "1");
                        document.RenderBeginTag(HtmlTextWriterTag.Table);
                        document.RenderBeginTag(HtmlTextWriterTag.Thead);

                        InsertTr(document, "Name");

                        document.RenderEndTag();
                        document.RenderBeginTag(HtmlTextWriterTag.Tbody);

                        foreach (var item in missingFromDefaultApps)
                        {
                            InsertTr(document, item);
                        }

                        document.RenderEndTag();
                        document.RenderEndTag();
                        document.RenderEndTag();

                        document.RenderBeginTag(HtmlTextWriterTag.Hr);
                        document.RenderEndTag();
                        // extra apps

                        var extraApps = startupApps.Where(x => !regedit.DefaultStartupApps.Any(z => z == x.Key)).ToList();

                        document.RenderBeginTag(HtmlTextWriterTag.Div);
                        document.RenderBeginTag(HtmlTextWriterTag.Label);
                        document.Write("Extra apps");
                        document.RenderEndTag();
                        document.AddStyleAttribute("border", "1px solid black");
                        document.AddAttribute(HtmlTextWriterAttribute.Border, "1");
                        document.RenderBeginTag(HtmlTextWriterTag.Table);
                        document.RenderBeginTag(HtmlTextWriterTag.Thead);

                        InsertTr(document, "Name", "Location", "State / Error Message");

                        document.RenderEndTag();
                        document.RenderBeginTag(HtmlTextWriterTag.Tbody);

                        foreach (var item in extraApps)
                        {
                            document.RenderBeginTag(HtmlTextWriterTag.Tr);

                            bool removed = false;
                            string errorMessage = string.Empty;
                            string color = "red";
                            try
                            {
                                regedit.RemoveStartupAppByKey(item.Key);
                                removed = true;
                            }
                            catch (Exception ex)
                            {
                                errorMessage = ex.Message;
                                removed = false;
                            }

                            if (removed)
                                color = "Green";

                            document.RenderBeginTag(HtmlTextWriterTag.Td);
                            document.Write(item.Key);
                            document.RenderEndTag();

                            document.RenderBeginTag(HtmlTextWriterTag.Td);
                            document.Write(item.Value);
                            document.RenderEndTag();

                            document.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, color);
                            document.AddStyleAttribute(HtmlTextWriterStyle.Color, "white");
                            document.RenderBeginTag(HtmlTextWriterTag.Td);
                            document.Write(removed ? "Removed" : errorMessage);
                            document.RenderEndTag();

                            document.RenderEndTag();
                        }

                        document.RenderEndTag();
                        document.RenderEndTag();
                        document.RenderEndTag();
                    }
                }
                catch (Exception ex)
                {
                    document.RenderBeginTag(HtmlTextWriterTag.Hr);
                    document.RenderBeginTag(HtmlTextWriterTag.Label);
                    document.Write(ex.Message, ex.InnerException?.Message);
                    document.RenderEndTag();
                    document.RenderEndTag();
                    document.RenderEndTag();
                }

                document.RenderEndTag();
            }
            document.RenderEndTag();
            document.RenderEndTag();
            document.RenderEndTag();

            return StringWriter.ToString();
        }
    }
}
