using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Windows;

namespace OnlineChat
{
    //后台，实现与服务器的交互
    class ToServer
    {
        public static string IP = "166.111.140.14";
        public static int Port = 8000;
        public static IPAddress IP_Address = IPAddress.Parse(IP);//定义IP地址（类型）

        string ID_Public;//弹窗显示的ID

        //网络Socket相关
        TcpClient tcpClient;
        NetworkStream Stream;

        //与服务器交互的线程
        Thread ServerThread;

        public event ServerMessage servermessage;
        public delegate void ServerMessage(object sender, ServerEventArgs e);

        //连接服务器
        public void ConnectServer()
        {
            try
            {
                //建立连接
                tcpClient = new TcpClient();
                tcpClient.Connect(IP_Address, Port);
                Stream = tcpClient.GetStream();
            }
            catch (Exception ex)
            {
                Message.Show(MessageFunction.Autoclose, "Error!", ex.Message + "服务器错误,服务器将关闭"+"\nToServer.cs/45");
            }
        }

        //开始监听回复
        public void StartListening()
        {
            ServerThread = new Thread(new ThreadStart(this.ServerResponse));
            ServerThread.IsBackground = true;
            ServerThread.Start();
        }

        //发送上线命令
        public void Login(string ID)
        {
            ID_Public = ID;
            ConnectServer();
            string cmd = ID + "_net2015";
            Byte[] outbytes = Encoding.GetEncoding("gb2312").GetBytes(cmd.ToCharArray());

            Stream.Write(outbytes, 0, outbytes.Length);

            StartListening();
        }

        //发送下线命令
        public void Logout(string ID)
        {
            ID_Public = ID;
            ConnectServer();
            string cmd = "logout" + ID;
            Byte[] outbytes = Encoding.GetEncoding("gb2312").GetBytes(cmd.ToCharArray());

            Stream.Write(outbytes, 0, outbytes.Length);

            StartListening();
        }

        //发送查询是否在线命令
        public void Query(string ID)
        {
            ID_Public = ID;
            ConnectServer();
            string cmd = "q" + ID;
            Byte[] outbytes = Encoding.GetEncoding("gb2312").GetBytes(cmd.ToCharArray());

            Stream.Write(outbytes, 0, outbytes.Length);

            StartListening();
        }

        public void ServerResponse()//响应服务器的命令
        {
            //定义一个byte数组，用于接收从服务器端发来的数据
            //每次所能接受的数据包的最大长度为1024个字节
            byte[] Response = new byte[1024];
            string Msg;
            int Len;
            try
            {
                if (Stream.CanRead == false)
                {
                    return;
                }
                while (true)
                {
                    Len = Stream.Read(Response, 0, Response.Length);
                    if (Len < 1)
                    {
                        Thread.Sleep(200);
                        continue;
                    }
                    Msg = Encoding.GetEncoding("gb2312").GetString(Response, 0, Len);
                    Msg.Trim();
                    //下面为处理各种命令


                    if (Msg == "lol")
                    {
                        ServerEventArgs login = new ServerEventArgs(ServerMessageType.LOGIN, ID_Public);
                        servermessage(this, login);
                        break;
                    }
                    else if (Msg == "loo")
                    {
                        ServerEventArgs logout = new ServerEventArgs(ServerMessageType.LOGOUT, ID_Public);
                        servermessage(this, logout);
                        break;

                    }
                    else if (Msg == "n")
                    {
                        ServerEventArgs query = new ServerEventArgs(ServerMessageType.QUERYOFFLINE, ID_Public);
                        servermessage(this, query);
                        break;
                    }
                    else
                    {
                        IPAddress ip = null;
                        if (IPAddress.TryParse(Msg, out ip))
                        {
                            ServerEventArgs query = new ServerEventArgs(ServerMessageType.QUERYONLINE, Msg);
                            servermessage(this, query);
                            break;
                        }
                        else
                        {
                            ServerEventArgs error = new ServerEventArgs(ServerMessageType.ERROR, Msg);
                            servermessage(this, error);
                            break;
                        }
                    }
                }
                //关闭连接
                tcpClient.Close();
            }
            catch (Exception ex)
            {
                //向UI层返回一个出错的信号
                ServerEventArgs error = new ServerEventArgs(ServerMessageType.ERROR, ex.Message);
                servermessage(this, error);
            }
        }
    }

    class ServerEventArgs:System.EventArgs
    {
        public ServerMessageType type;
        public string message;

        public ServerEventArgs(ServerMessageType type,string message)
        {
            this.type = type;
            this.message = message;
        }
    }
}
