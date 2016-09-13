using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Security.Cryptography;
using System.IO;

namespace OnlineChat
{
    class ChatSend
    {
        private static string IP = "127.0.0.1";
        private static string ID;
        public static int Port = 1234;
        private IPAddress IP_Address = IPAddress.Parse(IP);//定义IP地址（类型）

        TcpClient tcpClient;
        NetworkStream Stream;
        Thread connectthread;

        Main main;

        public ChatSend(string ip, string id,Main main)
        {
            IP = ip;
            ID = id;
            this.main = main;
            IP_Address = IPAddress.Parse(IP);
        }

        //开启连接进程（后台）
        public void ConnectFriend()
        {
            try
            {
                //建立连接
                tcpClient = new TcpClient();
                tcpClient.Connect(IP_Address, Port);
                Stream = tcpClient.GetStream();
            }
            catch
            {
                main.Dispatcher.Invoke(new Action(delegate
                {
                    Message.Show(MessageFunction.Autoclose, "Error", "对方拒绝，可能对方使用的客户端和这个不同");
                }));
            }
        }

        //得到本地IP
        public static string GetLocalIP()
        {
            IPAddress[] ips = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress ip in ips)
            {
                if (ip.AddressFamily.Equals(AddressFamily.InterNetwork))
                    return ip.ToString();
            }
            return null;
        }

        public void SendMessage(ChatMessageType type, string message)
        {
            string cmd = Pack(type, DateTime.Now.Hour, DateTime.Now.Minute, Main.myID, ID, message);
            Byte[] outbytes = Encoding.GetEncoding("gb2312").GetBytes(cmd.ToCharArray());
            Stream.Write(outbytes, 0, outbytes.Length);
        }

        public void SendMessage(ChatMessageType type, int hour, int minute, string from, string to, string message)
        {
            string cmd = Pack(type, hour, minute, from, to, message);
            Byte[] outbytes = Encoding.GetEncoding("gb2312").GetBytes(cmd.ToCharArray());

            Stream.Write(outbytes, 0, outbytes.Length);
        }

        private string Pack(ChatMessageType type, int hour, int minute, string from, string to, string message)
        {
            string temp = type.ToString() + "|" + hour.ToString() + "|" + minute.ToString() + "|" + from + "|" + to + "|" + Main.myheader + "|" + message;
            return temp;
        }
    }
}
