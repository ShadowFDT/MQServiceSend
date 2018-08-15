using System;
using RabbitMQ.Client;
using System.Text;

namespace Send
{
    class SendMQ
    {
        static void Main(string[] args)
        {            
            Console.Title = "Send in microservice";            
            string Name = ReadFromConsole();
            RMQService rmqService = new RMQService();
            rmqService.GetAppsettings();
            string msg = rmqService.ConsoleRead(Name);
            rmqService.SendMessage(msg);
            Console.WriteLine("Press [enter] to exit.");
            Console.ReadKey();
            Environment.Exit(-1);
        }
        
        private static string ReadFromConsole(string promptMessage = "")
        {
            string _readPrompt = "Name> ";
            Console.Write(_readPrompt + promptMessage);
            return Console.ReadLine();
        }
    }   
}
