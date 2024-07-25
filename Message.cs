using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Task_1
{
    internal class Message
    {        
        public string ToName { get; set; }    
        public string FromName { get; set; }
        public string Text { get; set; }
        public DateTime SetDate { get; set; }
        public Message() { }
        public Message(string Name, string Text)
        {
            this.FromName = Name;
            this.Text = Text;
            this.SetDate = DateTime.Now;
        }

        public override string ToString()
        {
            return $"Получено сообщение от {FromName} {SetDate.ToString("HH:mm:ss")}:\n{Text} ";
        }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }

        public static Message? FromJson(string message)
        {
            return JsonSerializer.Deserialize<Message>(message);
        }       
    }
}
