using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessController.Lib
{
    public enum ProgramTypeEnum
    {
        Msi, Other, Both
    }

    public class ProgramController
    {
        public List<string> GetInstalledPrograms(string machineName, ProgramTypeEnum type)
        {
            RegistryKey rHive = null;
            try
            {
                rHive = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, machineName, RegistryView.Registry64);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            var resultList = new List<string>();

            switch (type)
            {
                case ProgramTypeEnum.Both:
                    GetForMsi();
                    GetForOther();
                    break;
                case ProgramTypeEnum.Msi:
                    GetForMsi();
                    break;
                case ProgramTypeEnum.Other:
                    GetForOther();
                    break;
            }

            void GetForMsi() { }
            void GetForOther()
            {
                var rKey = rHive.OpenSubKey(@"Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall");

                var t = rKey.GetSubKeyNames();

                foreach (var item in t)
                {
                    Guid guid;

                    string val = item.Replace("<", string.Empty).Replace(">", string.Empty);

                    bool b = Guid.TryParse(val, out guid);

                    if (Guid.Empty != guid)
                    {
                        continue;
                    }
                }
            }

            return resultList;
        }
    }
}
