using ProcessController.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace LicenseChecker.Service
{
    public class LicenseCheckerService
    {
        private Library _library;
        private IEnumerable<string> _computersToBeChecked;

        public LicenseCheckerService(IEnumerable<string> computers = null)
        {
            _library = new Library();

            if (computers == null)
                computers = _library.GetCurrentDomainComputers();

            _computersToBeChecked = computers;
            _computersToBeChecked = _computersToBeChecked.Where(x => x.EndsWith("pc", StringComparison.InvariantCultureIgnoreCase)).ToList();
        }


        public Dictionary<string, LicenseStatusTypeEnum> RunWindowsLicenseCheck()
        {
            var result = new Dictionary<string, LicenseStatusTypeEnum>();

            foreach (var computer in _computersToBeChecked)
            {
                var alive = _library.Ping(computer, 10);

                if (!alive)
                    continue;

                try
                {
                    var scope = new ManagementScope(@"\\" + computer + @"\root\cimv2", null);
                    var queryString = "SELECT * FROM SoftwareLicensingProduct WHERE ApplicationID = '55c92734-d682-4d71-983e-d6ec3f16059f' and LicenseStatus = 1";
                    var query = new ObjectQuery(queryString);

                    ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

                    var searcherResult = searcher.Get();

                    using (searcherResult)
                    {
                        var isActivated = Convert.ToInt32(searcherResult.Count > 0);
                        result.Add(computer, (LicenseStatusTypeEnum)isActivated);
                    }
                }
                catch (Exception ex)
                {

                }
            }

            return result;
        }
    }
}
