using System;
using System.Linq;
using System.IO;
using System.Threading;
using System.Collections;
using System.Collections.ObjectModel;

namespace OnlineChat
{
    class FriendList
    {

        public ToServer toserver = new ToServer();

        public Friends[] temp=new Friends[300];
        public int count = 0;
        public int count1 = 0;
        public int friendcount = 0;

        private Main main;

        public static Hashtable id2friends = new Hashtable();
        public static Hashtable id2ip = new Hashtable();

        public ObservableCollection<Friends> memberData = new ObservableCollection<Friends>();

        public FriendList(Main main)
        {
            toserver.servermessage += OnServerCall;
            ObservableCollection<Friends> memberData = new ObservableCollection<Friends>();
            this.main = main;
        }

        public void WriteFriendsList(string ID, string name, string header)
        {
            FileStream fs = new FileStream("FriendsList_" + Main.myID + ".txt", FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(fs);
            sw.BaseStream.Seek(0, SeekOrigin.End);
            //开始写入
            sw.Write(ID + "|" + name + "|" + header + "\r\n");
            //清空缓冲区
            sw.Flush();
            //关闭流
            sw.Close();
            fs.Close();
            ReadFriendsList();
        }

        public void ReadFriendsList()
        {
            count = 0;
            count1 = 0;
            friendcount = 0;
            id2ip.Clear();
            id2friends.Clear();
            memberData.Clear();
            FileStream fs = new FileStream("FriendsList_"+Main.myID+".txt", FileMode.OpenOrCreate);
            StreamReader sr = new StreamReader(fs);
            String line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] tokens = line.Split(new char[] { '|' });
                if (id2friends.Contains(tokens[0]))
                    continue;
                temp[count] = new Friends() { Name = tokens[1], IP = "", ID = tokens[0], State = "", Header = tokens[2] };
                count++;
            }
            friendcount = count;
            for(int i=0;i<friendcount;i++)
            {
                toserver.Query(temp[i].ID);
                Thread.Sleep(200);
            }
            sr.Close();
            fs.Close();
            memberData = new ObservableCollection<Friends>(memberData.OrderByDescending(item => item.State));
        }

        public void OnServerCall(object sender, ServerEventArgs e)
        {
            try
            {
                if (e.type == ServerMessageType.QUERYONLINE)
                {
                    temp[count1].IP = e.message;
                    temp[count1].State = "Online";
                    id2ip.Add(temp[count1].ID, temp[count1].IP);
                    id2friends.Add(temp[count1].ID, temp[count1]);
                }
                else if (e.type == ServerMessageType.QUERYOFFLINE)
                {
                    temp[count1].IP = "None";
                    temp[count1].State = "Offline";
                    id2friends.Add(temp[count1].ID, temp[count1]);
                }
                else if (e.type == ServerMessageType.ERROR)
                {
                    //temp.IP = "None";
                    //temp.State = "***";
                }
                memberData.Add(temp[count1]);
                count1++;
            }
            catch(Exception ex)
            {
                //Thread.Sleep(200);
            }
    }
    }
}
