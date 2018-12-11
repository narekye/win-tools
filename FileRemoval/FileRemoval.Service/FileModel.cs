namespace FileRemoval.Service
{
    public class FileModel
    {
        public string ComputerName { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public long Size { get; set; }
        public string ReadableSize { get; set; }
        public string FullName { get; set; }
        public string Note { get; set; }

        public StatusEnum Status { get; set; }
    }
}
