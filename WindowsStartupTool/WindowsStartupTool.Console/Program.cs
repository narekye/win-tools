using System;
using WindowsStartupTool.Lib;

namespace WindowsStartupTool.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            Start();
            Environment.Exit(0);
        }

        static void Start()
        {
            System.Console.WriteLine("Please do not close this app");
            var emailSender = new EmailSender();
            var _pingTimeout = 200;
            var emailBuilder = new EmailBuilder();
            string emailMessage = string.Empty;
            emailMessage = emailBuilder.BuildReport(_pingTimeout);
            emailSender.Send(emailMessage, "yegoryan.narek@gmail.com");
        }
    }
}
