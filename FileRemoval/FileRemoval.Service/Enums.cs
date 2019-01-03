using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileRemoval.Service
{
    public enum FileSizeEnum
    {
        MB, GB, TB
    }

    public enum StatusEnum
    {
        Fail,
        Success
    }

    public enum LogBehaviorEnum
    {
        LogEverything, 
        LogOnlyExceptions,
        LogOnlyInformation
    }
}
