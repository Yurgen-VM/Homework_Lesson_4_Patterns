using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Task_1
{
    // Реализация паттерная Chain of responsibility/ Цепочка ответственности

    internal abstract class Package
    {
        protected Package successor;

        public void SetSuccessor(Package successor)
        {
            this.successor = successor;
        }
        public abstract Message PackageImpl(Message message, Dictionary<string, IPEndPoint> clients, UdpClient udpClient, IPEndPoint clientEp);
    }

    class PackageUserDelete : Package
    {
        Message servResp = new Message("Server", "принял!");
        public override Message PackageImpl(Message message, Dictionary<string, IPEndPoint> clients, UdpClient udpClient, IPEndPoint clientEp)
        {
            if (message.Text.ToLower().Equals("delete"))
            {
                clients.Remove(message.FromName);
                servResp = new Message("Server", $"Пользователь {message.FromName} удален с сервера");
                return servResp;                        
            }
            else if (successor != null)
            {
                return successor.PackageImpl(message, clients,udpClient,clientEp);
            }
            return servResp;
        }
    }

    class PackageUserReg : Package
    {
        Message servResp = new Message("Server", "принял!");
        public override Message PackageImpl(Message message, Dictionary<string, IPEndPoint> clients, UdpClient udpClient, IPEndPoint clientEp)
        {           
            if (message.Text.ToLower().Equals("register"))
            {
                if (clients.TryAdd(message.FromName, clientEp))
                {
                    servResp = new Message("Server", $"Пользователь {message.FromName} добавлен на сервер");
                    return servResp;
                }
            }
            else if (successor != null)
            {
                return successor.PackageImpl(message, clients, udpClient, clientEp);
            }
            return servResp;
        }

    }
    class PackageUserList : Package
    {
        Message servResp;
        public override Message PackageImpl(Message message, Dictionary<string, IPEndPoint> clients, UdpClient udpClient, IPEndPoint clientEp)
        {
            if (message.Text.ToLower().Equals("list"))
            {
                StringBuilder stringClients = new StringBuilder();
                foreach (var client in clients)
                {
                    stringClients.Append(client.Key + "\n");
                }
                servResp = new Message("Server", $"Список клиентов\n{stringClients.ToString()}");
                return servResp;                
            }
            else if (successor != null)
            {
                return successor.PackageImpl(message, clients, udpClient, clientEp);
            }
            servResp = new Message("Server", "принял!");
            return servResp;
        }
    }
}
