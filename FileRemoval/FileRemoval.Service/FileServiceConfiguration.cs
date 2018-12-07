using System;
using System.Collections.Generic;

namespace FileRemoval.Service
{
    public class FileServiceConfiguration
    {
        public long MinimumSize { get; set; }
        public DateTime MinimumAge { get; set; }

        private Dictionary<FileSizeEnum, long> _mappings = new Dictionary<FileSizeEnum, long>
        {
            { FileSizeEnum.MB,  1000000 },
            { FileSizeEnum.GB, 1000000000 },
            { FileSizeEnum.TB, 1000000000000 }
        };

        public FileServiceConfiguration(long minimumSize, DateTime minimumAge)
        {
            MinimumSize = minimumSize == 0 ? _mappings[FileSizeEnum.GB] : minimumSize;
            InitializeDate(minimumAge);
        }

        public FileServiceConfiguration(DateTime minimumAge = default(DateTime), FileSizeEnum fileSizeEnum = FileSizeEnum.GB)
        {
            MinimumSize = _mappings[fileSizeEnum];
            InitializeDate(minimumAge);
        }

        private void InitializeDate(DateTime date)
        {
            if (date == default(DateTime))
            {
                date = DateTime.Now.AddDays(-2);
                MinimumAge = date;
            }
        }
    }
}
