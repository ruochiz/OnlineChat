using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace OnlineChat
{
    class FileRecv
    {
        public Thread threadWatch = null;
        public Socket socketWatch = null;

        public const int SendBufferSize = 20 * 1024;
        public const int ReceiveBufferSize = 80 * 1024;

        public IPAddress IP_Address;
        public const int Port = 1235;
        public string FilePath;
        public Main main;

        public static bool ServiceFlag;
        public bool Finish = false;

        public FileProgress fp = null;

        Socket socConnection = null;

        public FileRecv()
        { }
        public FileRecv(string path,FileProgress fp)
        {
            IP_Address = IPAddress.Parse(ChatSend.GetLocalIP());
            socketWatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //将获取的ip地址和端口号绑定到网络节点endpoint上
            IPEndPoint endpoint = new IPEndPoint(IP_Address, Port);
            if (!FileRecv.ServiceFlag)
            {
                socketWatch.Bind(endpoint);
                socketWatch.Listen(20);
                FileRecv.ServiceFlag = true;
            }

            FilePath = path;
            this.fp = fp;

            Finish = false;
        }

        public FileRecv(string path)
        {
            IP_Address = IPAddress.Parse(ChatSend.GetLocalIP());
            socketWatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //将获取的ip地址和端口号绑定到网络节点endpoint上
            IPEndPoint endpoint = new IPEndPoint(IP_Address, Port);
            if (!FileRecv.ServiceFlag)
            {
                socketWatch.Bind(endpoint);
                socketWatch.Listen(20);
                FileRecv.ServiceFlag = true;
            }

            FilePath = path;
            this.fp = null;

            Finish = false;
        }

        public void Start()
        {
            threadWatch = new Thread(WatchConnecting);
            threadWatch.IsBackground = true;
            threadWatch.Start();
        }
        private void WatchConnecting()
        {
            while (true)
            {
                try
                {
                    socConnection = socketWatch.Accept();
                }
                catch
                {
                    break;
                }
                //创建通信线程 
                ParameterizedThreadStart pts = new ParameterizedThreadStart(ServerRecMsg);
                Thread thr = new Thread(pts);
                thr.IsBackground = true;
                //启动线程 
                thr.Start(socConnection);
            }
        }

        string strSRecMsg = null;

        private void ServerRecMsg(object socketClientPara)
        {
            Socket socketServer = socketClientPara as Socket;
            double fileLength = 0;
            double receivedTotalFilelength = 0;

            while (true)
            {
                int firstReceived = 0;
                byte[] buffer = new byte[ReceiveBufferSize];
                try
                {
                    //获取接收的数据,并存入内存缓冲区  返回一个字节数组的长度
                    if (socketServer != null) firstReceived = socketServer.Receive(buffer);

                    if (firstReceived > 0) //接受到的长度大于0 说明有信息或文件传来
                    {
                        if (buffer[0] == 0) //0为文字信息
                        {
                            strSRecMsg = Encoding.UTF8.GetString(buffer, 1, firstReceived - 1);//真实有用的文本信息要比接收到的少1(标识符)
                        }
                        if (buffer[0] == 2)//2为文件名字和长度
                        {
                            string fileNameWithLength = Encoding.UTF8.GetString(buffer, 1, firstReceived - 1);
                            string [] tokens = fileNameWithLength.Split(new char [] { '/' });
                            strSRecMsg = tokens[0]; //文件名
                            fileLength = Convert.ToInt64(tokens[1]);//文件长度
                        }
                        if (buffer[0] == 1)//1为文件
                        {
                            //string fileNameSuffix = strSRecMsg.Substring(strSRecMsg.LastIndexOf('.')); //文件后缀
                            string savePath = FilePath; //获取文件的全路径
                                                        //保存文件
                            int received = 0;
                            receivedTotalFilelength = 0;
                            bool firstWrite = true;
                            using (FileStream fs = new FileStream(savePath, FileMode.Create, FileAccess.Write))
                            {
                                while (receivedTotalFilelength < fileLength) //之后收到的文件字节数组
                                {
           
                                    if (firstWrite)
                                    {
                                        fs.Write(buffer, 1, firstReceived - 1); //第一次收到的文件字节数组 需要移除标识符1 后写入文件
                                        fs.Flush();

                                        receivedTotalFilelength += firstReceived - 1;

                                        firstWrite = false;

                                        continue;
                                    }
                                    received = socketServer.Receive(buffer); //之后每次收到的文件字节数组 可以直接写入文件
                                    fs.Write(buffer, 0, received);
                                    fs.Flush();

                                    receivedTotalFilelength += received;

                                    try
                                    {
                                        if (fp != null)
                                        {
                                            fp.Dispatcher.BeginInvoke(new Action(delegate
                                           {
                                               fp.Update(receivedTotalFilelength / fileLength * 100);
                                           }));
                                        }
                                    }
                                    catch { }

                                }
                                Finish = true;
                                fs.Close();
                                socketWatch.Close();
                                FileRecv.ServiceFlag = false;
                                threadWatch.Abort();
                            }

                            string fName = savePath.Substring(savePath.LastIndexOf("\\") + 1); //文件名 不带路径
                            string fPath = savePath.Substring(0, savePath.LastIndexOf("\\")); //文件路径 不带文件名
                        }
                    }
                    if(receivedTotalFilelength==fileLength)
                    {
                        socketWatch.Close();
                        ServiceFlag = false;
                        threadWatch.Abort();
                    }
                }
                catch
                {
                    break;
                }
            }
        }

    }
}
