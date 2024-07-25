using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Task_1
{
    internal class ServerConnect // Реализация паттерна "Одиночка" (Singleton). Создаем глобальную точку подключения к серверу 
    {
        private static readonly Lazy<ServerConnect> LazyInstatns = new Lazy<ServerConnect>(() => new ServerConnect());               
        private static IPEndPoint serverEp = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 54321);
        private ServerConnect() { }
        public static ServerConnect Instance => LazyInstatns.Value;
        public static IPEndPoint EndPointSetup()
        {
            return serverEp;
        }        
    }
}
