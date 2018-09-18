using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessController.Lib
{
    public class ProcessModel
    {
        public string Caption { get; set; }
        public string CsName { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string Priority { get; set; }
        public string ProcessId { get; set; }
        public string SessionId { get; set; }
    }
}
