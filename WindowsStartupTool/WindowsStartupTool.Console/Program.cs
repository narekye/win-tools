using System;
using WindowsStartupTool.Lib;

namespace WindowsStartupTool.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            Start();
            System.Console.Read();
        }

        static void Start()
        {
            var emailSender = new EmailSender();
            var _pingTimeout = 200;
            var emailBuilder = new EmailBuilder("cubicle23-pc");
            string emailMessage = emailBuilder.BuildReportAndRemoveServices(_pingTimeout);
            emailSender.Send(emailMessage, "yegoryan.narek@gmail.com");
        }
    }
}
