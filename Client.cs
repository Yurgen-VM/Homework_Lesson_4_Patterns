using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Task_1
{
    internal class Client
    {
        public static async Task ClientListener(UdpClient udpClient)
        {
            while (true)
            {
                var reciveResult = await udpClient.ReceiveAsync();
                string str = Encoding.UTF8.GetString(reciveResult.Buffer);
                Message? msg = Message.FromJson(str);
                Console.WriteLine($"\n{msg}");
            }
        }

        public static async Task ClientSender(string Name, UdpClient udpClient)
        {            
            IPEndPoint clientEP = ServerConnect.EndPointSetup(); // Реализация паттерна Singleton (Одиночка). Создаем глобальную точку подключения к серверу.
            
            while (true)
            {
                await Console.Out.WriteLineAsync("\nВведите имя получателя:");
                string? toName = Console.ReadLine();
                if (String.IsNullOrEmpty(toName))
                {
                    await Console.Out.WriteLineAsync("Вы не указали имя получателя!");
                    continue;
                }
                Console.WriteLine("\nВведите сообщение:");
                string? text = Console.ReadLine();

                if (text != "Exit")
                {
                    try
                    {
                        Message newMessage = new Message(Name, text);
                        newMessage.ToName = toName;
                        await Sender.SendMessage(newMessage, clientEP, udpClient);                        
                    }
                    catch (Exception) { Console.WriteLine("Сервер не доступен"); }
                }
                else
                {
                    Console.WriteLine("\nРабота программы завершена");
                    return;
                }
                await Task.Delay(500);
            }
        }
    }
}
