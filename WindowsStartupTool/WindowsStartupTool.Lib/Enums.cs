namespace WindowsStartupTool.Lib
{
    public enum StartUpLookupEnum
    {
        ShellStartupFolder = 1,
        Registry
    }

    public enum RegistryLookupSourceEnum
    {
        User = 1,
        Machine
    }

    public enum TargetPlatformEnum
    {
        x32 = 1,
        x64
    }

    public enum SkippableSourceEnum
    {
        None,
        Default,
        File,
        DefaultWithFile
    }

    public enum ExportFileTypeEnum
    {
        Csv = 1,
        Json
    }
}
