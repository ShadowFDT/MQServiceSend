using System;
using System.Collections.Generic;
using RabbitMQ.Client;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Send
{
    public class RMQService : IDisposable
    {
        private string username { get; set; }
        private string password { get; set; }
        private string virtualhost { get; set; }
        private string hostname { get; set; }
        private string queuekey { get; set; }
        private IConnection conn { get; set; }
        private IModel mod { get; set; }

        public void GetAppsettings()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();
            username = configuration["username"].ToString();
            password = configuration["password"].ToString();
            virtualhost = configuration["virtualhost"].ToString();
            hostname = configuration["hostname"].ToString();
            queuekey = configuration["queuekey"].ToString();
        }
        
        private bool RMQConnect()
        {            
            ConnectionFactory connFact = new ConnectionFactory();
            connFact.UserName = username;
            connFact.Password = password;
            connFact.VirtualHost = virtualhost;
            connFact.HostName = hostname;
            connFact.RequestedHeartbeat = 60;

            int attempts = 0;
            while (attempts < 5)
            {
                attempts++;

                try
                {
                    conn = connFact.CreateConnection();
                    CreateModel();
                    return true;
                }
                catch (System.IO.EndOfStreamException e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
                catch (RabbitMQ.Client.Exceptions.BrokerUnreachableException e)
                {
                    Console.WriteLine(e.Message);
                    Thread.Sleep(5000);
                    return false;
                }
            }
            if (conn != null) conn.Dispose();

            return false;
        }

        private void CreateModel()
        {
            mod = conn.CreateModel();
            mod.QueueDeclare(queue: queuekey, durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        public string ConsoleRead(string NameInput)
        {
            string result = "";
            while (true)
            {                
                if (string.IsNullOrWhiteSpace(NameInput)) continue;
                try
                {
                    result = string.Format("Hello my name is, {0}", NameInput);
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }
            return result;
        }
                      
        public void SendMessage(string message)
        {
            if (conn is null)
            {
                RMQConnect();
            }

            byte[] msgbytes = Encoding.UTF8.GetBytes(message);
            mod.BasicPublish(exchange: "", routingKey: queuekey, basicProperties: null, body: msgbytes);
            Console.WriteLine("Sent: {0}", message);           
        }
        
        void IDisposable.Dispose()
        {
            if (mod != null) { mod.Close(); }
            if (conn != null) { conn.Close(); conn.Dispose(); }
            GC.SuppressFinalize(this);
        }
    }
}
