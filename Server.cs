using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Task_1
{
    internal class Server
    {
        static private CancellationTokenSource cts = new CancellationTokenSource();
        static private CancellationToken ct = cts.Token;
        static private Dictionary<string, IPEndPoint> clients = new Dictionary<string, IPEndPoint>();


        public static async Task AcceptMess(string nikName)
        {
            UdpClient udpClient = new UdpClient(54321);
            IPEndPoint clientEp = new IPEndPoint(IPAddress.Any, 0);

            Console.WriteLine("Сервер ожидает сообщение от клиента.");
            Console.WriteLine("Для завершения работы нажмите \"Esc\"\n");
            Task programmClose = Task.Run(() =>
            {
                try
                {
                    while (!ct.IsCancellationRequested)
                    {
                        var key = Console.ReadKey(intercept: true);
                        if (key.Key == ConsoleKey.Escape)
                        {
                            cts.Cancel();
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Работа сервера остановлена ");
                }
            });

            // Реализация паттерная Chain of responsibility/ Цепочка ответственности

            Package userDel = new PackageUserDelete();
            Package userReg = new PackageUserReg();
            Package userList = new PackageUserList();
            userReg.SetSuccessor(userDel);
            userDel.SetSuccessor(userList);

            while (!ct.IsCancellationRequested)
            {
                try
                {
                    if (udpClient.Available > 0)
                    {
                        var data = udpClient.Receive(ref clientEp);
                        string dataB = Encoding.UTF8.GetString(data);

                        await Task.Run(async () =>
                        {
                            Message? msg = Message.FromJson(dataB);
                            Message servResp = new Message("Server", "принял!");

                            if (msg != null)
                            {
                                if (msg.ToName.Equals("Server"))
                                {
                                    servResp = userReg.PackageImpl(msg, clients, udpClient, clientEp); // Реализация паттерна "Цепочка ответственности"                                   
                                }                               
                                else if (msg.ToName.ToLower().Equals("all"))
                                {
                                    servResp = new Message("Server", "Cooбщение отправлено всем клиентам");
                                    foreach (var client in clients)
                                    {
                                        msg.ToName = client.Key;
                                        await Sender.SendMessage(msg, client.Value, udpClient); // Метод для отправки сообщений. Убрал дублирующийся код                                   
                                    }
                                }

                                else if (clients.TryGetValue(msg.ToName, out IPEndPoint? value))
                                {
                                    await Sender.SendMessage(msg, value, udpClient);
                                    servResp = new Message("Server", $"Пользователю {msg.ToName} отправлено сообщение");
                                }
                                else
                                {
                                    servResp = new Message("Server", $"Пользователя {msg.ToName} не существует");
                                }
                                Console.WriteLine($"{msg}\n");
                                await Sender.SendMessage(servResp, clientEp, udpClient);

                            }
                            else { Console.WriteLine("Некорректное сообщение"); }
                        });
                    }
                    else
                    {
                        await Task.Delay(200);
                    }
                    ct.ThrowIfCancellationRequested();
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Работа сервера остановлена");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Ошибка {e.Message}");
                }
            }
        }
    }
}
