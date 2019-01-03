using System;
using System.Collections.Generic;

namespace Davinci
{
    public class DataModel
    {
        public string AddressLine { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }

        public DataModel()
        {
            if (string.IsNullOrWhiteSpace(AddressLine))
            {
                AddressLine = "167 Division Avenue, Brooklyn NY 11211";
            }
            if (string.IsNullOrWhiteSpace(Phone))
            {
                Phone = "718.972.8242";
            }
            if (string.IsNullOrWhiteSpace(Fax))
            {
                Fax = "718.576.3497";
            }
        }

        public string InvoiceId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string PersonalName { get; set; }
        public string BusinessName { get; set; }
        public string Notes { get; set; }
        public List<ActionModel> Actions { get; set; }
    }

    public class ActionModel
    {
        public string Description { get; set; }

        public DateTime EndDate { get; set; }
        public DateTime StartDate { get; set; }

        public string AmountSign { get; set; } = "$";

        public double Amount { get; set; }
        public string Rate { get; set; }
    }
}
