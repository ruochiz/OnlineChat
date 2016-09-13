using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace OnlineChat
{
    class FileSend
    {
        Socket socketClient = null;
        public const int SendBufferSize = 20 * 1024;
        public const int ReceiveBufferSize = 80 * 1024;
        public IPAddress IP_Address;
        public const int Port = 1235;
        public string FilePath;
        public string FileName;
        public FileProgress fp;
        IPEndPoint endpoint = null;
        public Thread sendthread = null;

        public FileSend(string ip,string path,string name,FileProgress fp)
        {
            IP_Address = IPAddress.Parse(ip);
            FilePath = path;
            FileName = name;
            socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
            //将获取的ip地址和端口号绑定到网络节点endpoint上
            IPEndPoint endpoint = new IPEndPoint(IP_Address,Port);
            //向指定的ip和端口号的服务端发送连接请求 用的方法是Connect 不是Bind
            socketClient.Connect(endpoint);
            this.fp = fp;
        }

        public FileSend(string ip, string path, string name)
        {
            IP_Address = IPAddress.Parse(ip);
            FilePath = path;
            FileName = name;
            socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //将获取的ip地址和端口号绑定到网络节点endpoint上
            endpoint = new IPEndPoint(IP_Address, Port);
            //向指定的ip和端口号的服务端发送连接请求 用的方法是Connect 不是Bind
            socketClient.Connect(endpoint);
        }

        private void SendMsg(string sendMsg,byte symbol)
        {
            byte[] arrClientMsg = Encoding.UTF8.GetBytes(sendMsg);
            //实际发送的字节数组比实际输入的长度多1 用于存取标识符
            byte[] arrClientSendMsg = new byte[arrClientMsg.Length + 1];
            arrClientSendMsg[0] = symbol;  //在索引为0的位置上添加一个标识符
            Buffer.BlockCopy(arrClientMsg, 0, arrClientSendMsg, 1, arrClientMsg.Length);

            socketClient.Send(arrClientSendMsg);
        }
        public void SendFile()
        {
            sendthread = new Thread(new ThreadStart(Sendfile));
            sendthread.IsBackground = true;
            sendthread.Start();
        }
        public void Sendfile()
        {
            if(!string.IsNullOrEmpty(FilePath))
            {
                double fileLength = new FileInfo(FilePath).Length;
                //先发文件名和文件长度确认信息
                string totalMsg = string.Format("{0}/{1}/", FileName, fileLength);
                SendMsg(totalMsg, 2);
                Thread.Sleep(200);
                byte[] buffer = new byte[SendBufferSize];

                using (FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
                {
                    int readLength = 0;
                    bool firstRead = true;
                    double sentFileLength = 0;
                    while ((readLength = fs.Read(buffer, 0, buffer.Length)) > 0 && sentFileLength < fileLength)
                    {
                        sentFileLength += readLength;
                        //在第一次发送的字节流上加个前缀1
                        if (firstRead)
                        {
                            byte[] firstBuffer = new byte[readLength + 1];
                            firstBuffer[0] = 1; //告诉机器该发送的字节数组为文件
                            Buffer.BlockCopy(buffer, 0, firstBuffer, 1, readLength);

                            socketClient.Send(firstBuffer, 0, readLength + 1, SocketFlags.None);
                            //Stream.Write(firstBuffer, 0, readLength+1);


                            firstRead = false;

                            continue;
                        }
                        try
                        {
                            //之后发送的均为直接读取的字节流
                            socketClient.Send(buffer, 0, readLength, SocketFlags.None);
                            //Stream.Write(buffer, 0, readLength);

                        }
                        catch
                        {

                        }
                        if (fp != null)
                        {
                            fp.Dispatcher.BeginInvoke(new Action(delegate
                            {
                                fp.Update(sentFileLength / fileLength * 100);
                            }));
                        }

                    }
                    fs.Close();
                    socketClient.Close();
                }
            }
        }
    }
}
