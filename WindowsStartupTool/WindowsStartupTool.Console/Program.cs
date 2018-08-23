using WindowsStartupTool.Lib;

namespace WindowsStartupTool.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            string machine = "";

            using (var editor = new RegistryEditor(machine, RegistryLookupSourceEnum.User))
            {
                var apps = editor.GetStartupAppsFromRegistry();

                foreach (var app in apps)
                    System.Console.WriteLine($"{app.Key} - {app.Value}");
            }

            System.Console.ReadKey();
        }
    }
}
