using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace WindowsStartupTool.Lib
{
    public class EmailBuilder
    {
        private StringBuilder _builder;

        public EmailBuilder()
        {
            _builder = new StringBuilder();
        }

        void CreateHTMLDocument()
        {
            var stringWriter = new StringWriter();

            using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Html);
                writer.RenderBeginTag(HtmlTextWriterTag.Body);
                writer.RenderBeginTag(HtmlTextWriterTag.Div);

                writer.RenderEndTag();
                writer.RenderEndTag();
                writer.RenderEndTag();
            }    
            
        }
    }
}
