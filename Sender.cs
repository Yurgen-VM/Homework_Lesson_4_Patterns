using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Task_1
{
    internal class Sender
    {
        public static async Task SendMessage(Message servResp,IPEndPoint clientEp, UdpClient udpClient)
        {
            string strServResp = servResp.ToJson(); // Конвертируем наше сообщение в JSON
            byte[] byteServResp = Encoding.UTF8.GetBytes(strServResp); // Кодируем JSON в массив байтов
            await udpClient.SendAsync(byteServResp, clientEp); // Отправляем пакет клиенту
        }
    }
}
