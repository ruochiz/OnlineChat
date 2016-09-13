using System.Text;
using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Forms;
using System.IO;

namespace OnlineChat
{
    //后台，实现监听别人的TCP请求
    class ChatListen
    {
        public static int Port = 1234;
        private IPAddress IP_Address;//定义IP地址（类型）
        public static bool ServiceFlag = false;

        private TcpListener listener;

        Thread ListenThread;

        public Main main;

        public event ChatMessage chatmessage;
        public delegate void ChatMessage(object sender, ChatListenEventArgs e);

        Socket currentSocket;

        //得到本地IP
        private void GetLocalIP()
        {
            IPAddress[] ips = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress ip in ips)
            {
                if (ip.AddressFamily.Equals(AddressFamily.InterNetwork))
                {
                    IP_Address = ip;
                    break;
                }
            }
        }

        //开启TCP Listen同时开启监听线程
        public void StartListening()
        {
            GetLocalIP();
            listener = new TcpListener(IP_Address,Port);
            while(true)
            {
                try
                {
                    listener.Start();
                    ServiceFlag = true;
                    ListenThread = new Thread(Listening);
                    ListenThread.IsBackground = true;
                    ListenThread.Start();
                    break;
                }
                catch (Exception e)
                {
                    break;
                }
            }
        }

        private void Listening()
        {
            while(ServiceFlag)
            {
                try
                {
                    if(listener.Pending())
                    {
                        Socket mySocket = listener.AcceptSocket();
                        currentSocket = mySocket;
                        //开启新的线程处理新的连接请求
                        Client client = new Client(this, mySocket);
                        Thread clientservice = new Thread(new ThreadStart(client.ServiceClient));
                        clientservice.IsBackground = true;
                        clientservice.Start();
                    }
                    Thread.Sleep(200);
                }
                catch(Exception e)
                {
                    Thread.Sleep(200);
                    //break;
                }
            }
        }

        public class Client
        {
            private Socket currentSocket = null;
            private string ipAddress;
            private ChatListen server;

            public Client(ChatListen server,Socket clientSocket)
            {
                this.server = server;
                this.currentSocket = clientSocket;
                ipAddress= getRemoteIPAddress();
            }

            private string getRemoteIPAddress()
            {
                return ((IPEndPoint)currentSocket.RemoteEndPoint).Address.ToString();
            }

            //接收消息并解包然后中断将消息送回给UI层
            public void ServiceClient()
            {
                byte[] buff = new byte[1024];
                bool keepConnect = true;
                string clientCommand = "";

                while (keepConnect && ServiceFlag)
                {
                    //tokens=null;
                    try
                    {
                        if (currentSocket == null || currentSocket.Available < 1)
                        {
                            Thread.Sleep(300);
                            continue;
                        }
                        int len = currentSocket.Receive(buff);
                        clientCommand = Encoding.GetEncoding("gb2312").GetString(buff, 0, len);
                        ChatListenEventArgs msg = Unpack(clientCommand);
                        if(msg.to==Main.myID)
                            server.chatmessage(this, msg);

                        Thread.Sleep(200);
                    }
                    catch (Exception e)
                    {
                        Thread.Sleep(200);
                        break;
                    }
                }
            }
        }

        //解包数据
        private static ChatListenEventArgs Unpack(string pack)
        {          
            string[] tokens = pack.Split(new char[] { '|' });
            ChatListenEventArgs temp = new ChatListenEventArgs();
            switch (tokens[0])
            {
                case "HELLO":
                    temp.type = ChatMessageType.HELLO;
                    break;
                case "CHAT":
                    temp.type = ChatMessageType.CHAT;
                    break;
                case "PICTURE":
                    temp.type = ChatMessageType.PICTURE;
                    break;
                case "FILE":
                    temp.type = ChatMessageType.FILE;
                    break;
                case "FILEREQUEST":
                    temp.type = ChatMessageType.FILEREQUEST;
                    break;
                case "VIDEO":
                    temp.type = ChatMessageType.VIDEO;
                    break;
                case "REQUEST":
                    temp.type = ChatMessageType.REQUEST;
                    break;
                case "SHAKE":
                    temp.type = ChatMessageType.SHAKE;
                    break;
                default:
                    temp.type = ChatMessageType.ERROR;
                    break;
            }
            temp.hour = Convert.ToInt32(tokens[1]);
            temp.minute = Convert.ToInt32(tokens[2]);
            temp.from = tokens[3];
            temp.to = tokens[4];
            temp.header = tokens[5];
            for(int i=6;i<tokens.Length;i++)
            {
                if (i != 6)
                    temp.message += "|";
                temp.message += tokens[i];
            }
            return temp;
        }
    }

    public class ChatListenEventArgs : System.EventArgs
    {
        public ChatMessageType type;
        public int hour;
        public int minute;
        public string from;
        public string to;
        public string message;
        public string header;

        public ChatListenEventArgs(ChatMessageType type, int hour,int minute, string from, string to, string message,string header)
        {
            this.type = type;
            this.hour = hour;
            this.minute = minute;
            this.from = from;
            this.to = to;
            this.message = message;
            this.header = header;
        }

        public ChatListenEventArgs(ChatMessageType type, string message)
        {
            this.type = type;
            this.message = message;
        }

        public ChatListenEventArgs()
        {

        }
    }

}
