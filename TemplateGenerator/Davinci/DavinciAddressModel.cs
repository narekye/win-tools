namespace Davinci
{
    public class DavinciAddressModel
    {
        public string Name { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }

        public DavinciAddressModel()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                Name = "Davinci Virtual";
            }

            if (string.IsNullOrWhiteSpace(AddressLine1))
            {
                AddressLine1 = "2150 S 1300 E, Suite 200";
            }

            if (string.IsNullOrWhiteSpace(AddressLine2))
            {
                AddressLine2 = "Salt Lake City, UT 84106";
            }

            if (string.IsNullOrWhiteSpace(Phone))
            {
                Phone = "866-661-5588";
            }

            if (string.IsNullOrWhiteSpace(Fax))
            {
                Fax = "801-990-4266";
            }
        }
    }
}
