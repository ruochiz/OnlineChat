using System;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using System.Windows.Media;
using System.Collections;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Windows.Input;
using System.Windows.Forms;
using MahApps.Metro.Controls.Dialogs;

namespace OnlineChat
{
    /// <summary>
    /// Main.xaml 的交互逻辑
    /// </summary>
    public partial class Main : MetroWindow
    {
        FriendList f;

        int number = 0;

        //通过学号找到对应的窗口编号
        public Hashtable id2number = new Hashtable();
        public Hashtable id2window = new Hashtable();
        public Hashtable id2tabitem = new Hashtable();
        public Hashtable id2userinfo = new Hashtable();

        ChatListen cl = new ChatListen();

        public static Friends myself = new Friends();
        public static string myheader = "-1";

        public static string myID="2013011551";

        bool Flag = false;

        FileProgress fp;
        FileProgress fpr;

        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();

        Thread thread;

        NotifyIcon notifyIcon;

        public Main(string ID)
        {
            InitializeComponent();

            //初始化参数
            myID = ID;
            myself.Header = "7";
            myself.ID = myID;
            myself.IP = ChatSend.GetLocalIP();
            myself.Name = "User1";
            myself.State = "Online";
            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Tick += timer_tick;

            //绑定事件，起始操作
            UserInfo.userclick += OnClick;
            cl.chatmessage += OnCall;
            cl.main = this;
            cl.StartListening();

            f = new FriendList(this);
            //刷新好友列表,新线程防止界面卡住
            refreshfriendlist();

            myselfitem.Visibility = Visibility.Collapsed;
            myselfgrid.Visibility = Visibility.Collapsed;

            //初始化UI
            //grid.Background = new ImageBrush(Function.img2source(Properties.Resources.bg));
            bgimage.Source= Function.img2source(Properties.Resources.bg);
            label.Content = "";
            Chat.Source = Function.img2source(Properties.Resources.Chat);
            Add.Source = Function.img2source(Properties.Resources.Add);
            Myself.Source = Function.img2source(Properties.Resources.myself);
            ImageBrush b = new ImageBrush(Function.img2source(Properties.Resources.welcomeimage));
            welcomegrid.Background = b;
            flipview.IsEnabled = false;

            timer.Start();

            icon();

        }

        //关键部分，处理对方发来的信息
        private void OnCall(object sender, ChatListenEventArgs e)
        {
            //聊天类型
            if (e.type == ChatMessageType.CHAT)
            {
                this.Dispatcher.Invoke(new Action(delegate
                {
                    //如果那个窗口曾经打开过，或者打开着
                    if (id2window.Contains(e.from))
                    {
                        //显示信息
                        ChatBox temp = id2window[e.from] as ChatBox;
                        temp.Show_Time();
                        temp.ChatRecord.Document.Blocks.Add(Function.ToShow(TextAlignment.Left, e.message, 0, e.header));
                        temp.ChatRecord.ScrollToEnd();

                        //增加提示
                        if (tabControl.SelectedIndex != Convert.ToInt32(id2number[e.from]))
                            (id2tabitem[e.from] as TabItem).Background = MyColor.White;
                        //不管是不是被关了，都让其可见
                        (id2tabitem[e.from] as TabItem).Visibility = Visibility.Visible;
                        ((id2tabitem[e.from] as TabItem).Content as Grid).Visibility = Visibility.Visible;

                    }
                    //有这么个人但是从没开过窗口
                    else if (FriendList.id2ip.Contains(e.from))
                    {
                        try
                        {
                            //先开这个窗口，但是不把焦点移过去
                            TabItem temp = new TabItem();
                            Grid grid = new Grid();
                            grid.Margin = new Thickness(0, -1, 0, 1);
                            ChatBox cb = new ChatBox(this, FriendList.id2ip[e.from] as string, e.from);
                            grid.Children.Add(cb);
                            grid.Visibility = Visibility.Visible;
                            grid.Background = MyColor.Trans;
                            temp.Content = grid;
                            temp.Header = (FriendList.id2friends[e.from] as Friends).Name;
                            temp.Background = MyColor.White;

                            cb.Show_Time();
                            cb.ChatRecord.Document.Blocks.Add(Function.ToShow(TextAlignment.Left, e.message, 0, e.header));

                            number++;

                            tabControl.Items.Add(temp);
                            id2number.Add(e.from, number);
                            id2window.Add(e.from, cb);
                            id2tabitem.Add(e.from, temp);
                        }
                        catch (Exception ex)
                        {

                        }
                    }

                    //添加消息预览
                    UserInfo temp2 = id2userinfo[e.from] as UserInfo;
                    temp2.label1.Content = e.message.Substring(0, min(10, e.message.Length)) + "...";
                }));
            }
            //加好友类型
            else if (e.type == ChatMessageType.REQUEST)
            {
                this.Dispatcher.Invoke(new Action(delegate
                {
                    //如果点了同意
                    if (Message.Show(MessageFunction.YesNo, "Request", e.from + "申请加您为好友,是否接受？") == MessageResult.Yes)
                    {
                        string[] tokens = e.message.Split(new char[] { '\t' });
                        //发送确认信息
                        ChatSend cs = new ChatSend(tokens[0], e.from, this);
                        cs.ConnectFriend();
                        cs.SendMessage(ChatMessageType.HELLO, myself.IP + "\t" + myself.Name + "\t" + myself.Header);
                        if (!FriendList.id2friends.Contains(e.from))
                        {
                            //把对方加到好友列表并刷新
                            f.WriteFriendsList(e.from, tokens[1], tokens[2]);
                            refreshfriendlist();
                        }
                    }
                }));
            }
            //对方同意加好友类型
            else if (e.type == ChatMessageType.HELLO)
            {
                this.Dispatcher.Invoke(new Action(delegate
                {
                    Message.Show(MessageFunction.Autoclose, "Success", "对方已经接受你的好友请求");
                    string[] tokens = e.message.Split(new char[] { '\t' });
                    f.WriteFriendsList(e.from, tokens[1], tokens[2]);
                    refreshfriendlist();
                }));
            }
            //对方请求发文件
            else if (e.type == ChatMessageType.FILE)
            {
                this.Dispatcher.Invoke(new Action(delegate
                {
                    string[] tokens = e.message.Split(new char[] { '~' });
                    string filePath = tokens[1];
                    string fileName = tokens[0];
                    string fileLength = tokens[2];
                    if (Message.Show(MessageFunction.YesNo, "收到文件请求\n",
                        "收到文件" + tokens[0] + "\n大小为" + tokens[2] + "MB\n是否接收？") == MessageResult.Yes)
                    {
                        System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
                        //设置默认文件名
                        sfd.FileName = fileName;
                        if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            string FilePath = sfd.FileName;
                            if (!string.IsNullOrEmpty(FilePath))
                            {
                                fpr = new FileProgress(fileName, 0, e.header);

                                FileRecv fr = new FileRecv(FilePath, fpr);
                                fr.main = this;
                                fr.Start();
                            }
                            //如果已经打开着窗口
                            if (id2window.Contains(e.from))
                            {
                                ChatBox temp = id2window[e.from] as ChatBox;

                                Thread.Sleep(500);

                                temp.Show_Time();

                                temp.SendMessage(ChatMessageType.FILEREQUEST, tokens[1] + "~" + tokens[0]);

                                temp.ChatRecord.Document.Blocks.Add(Function.ToShow(TextAlignment.Left, fpr));
                                temp.ChatRecord.ScrollToEnd();


                                (id2tabitem[e.from] as TabItem).Visibility = Visibility.Visible;
                                ((id2tabitem[e.from] as TabItem).Content as Grid).Visibility = Visibility.Visible;

                                if (tabControl.SelectedIndex != Convert.ToInt32(id2number[e.from]))
                                    (id2tabitem[e.from] as TabItem).Background = MyColor.White;
                            }
                            //如果在好友列表里但没开过窗口
                            else if (FriendList.id2ip.Contains(e.from))
                            {
                                TabItem temp = new TabItem();
                                Grid grid = new Grid();
                                grid.Margin = new Thickness(0, -1, 0, 1);
                                ChatBox cb = new ChatBox(this, FriendList.id2ip[e.from] as string, e.from);

                                Thread.Sleep(500);
                                cb.Show_Time();
                                cb.SendMessage(ChatMessageType.FILEREQUEST, tokens[1] + "~" + tokens[0]);
                                cb.ChatRecord.Document.Blocks.Add(Function.ToShow(TextAlignment.Left, fpr));
                                cb.ChatRecord.ScrollToEnd();

                                grid.Children.Add(cb);
                                grid.Visibility = Visibility.Visible;
                                grid.Background = MyColor.Trans;
                                temp.Content = grid;
                                temp.Header = (FriendList.id2friends[e.from] as Friends).Name;
                                temp.Background = MyColor.White;

                                number++;

                                tabControl.Items.Add(temp);
                                id2number.Add(e.from, number);
                                id2window.Add(e.from, cb);
                                id2tabitem.Add(e.from, temp);
                            }
                        }
                    }
                }));
            }
            //发来图片
            else if (e.type == ChatMessageType.PICTURE)
            {
                string[] tokens = e.message.Split(new char[] { '~' });
                string fileName = tokens[0];

                string FilePath = "./ChatImage/" + fileName;

                FileRecv fr = new FileRecv();
                if (!string.IsNullOrEmpty(FilePath))
                {
                    fr = new FileRecv(FilePath);
                    fr.main = this;
                    fr.Start();
                }

                //等待图片传送完毕
                while (!fr.Finish)
                {
                    Thread.Sleep(1000);
                }

                this.Dispatcher.Invoke(new Action(delegate
                {

                    //如果已经打开着窗口
                    if (id2window.Contains(e.from))
                    {
                        ChatBox temp = id2window[e.from] as ChatBox;
                        temp.Show_Time();
                        temp.ChatRecord.Document.Blocks.Add(Function.ToShow(TextAlignment.Left, new BitmapImage(new Uri(FilePath, UriKind.Relative)), 0, e.header));
                        temp.ChatRecord.ScrollToEnd();
                        (id2tabitem[e.from] as TabItem).Visibility = Visibility.Visible;
                        ((id2tabitem[e.from] as TabItem).Content as Grid).Visibility = Visibility.Visible;

                        if (tabControl.SelectedIndex != Convert.ToInt32(id2number[e.from]))
                            (id2tabitem[e.from] as TabItem).Background = MyColor.White;
                    }
                    //如果在好友列表里但没开过窗口
                    else if (FriendList.id2ip.Contains(e.from))
                    {
                        TabItem temp = new TabItem();
                        Grid grid = new Grid();
                        grid.Margin = new Thickness(0, -1, 0, 1);
                        ChatBox cb = new ChatBox(this, FriendList.id2ip[e.from] as string, e.from);


                        cb.Show_Time();
                        cb.SendMessage(ChatMessageType.FILEREQUEST, tokens[1] + "~" + tokens[0]);

                        cb.ChatRecord.Document.Blocks.Add(Function.ToShow(TextAlignment.Left, new BitmapImage(new Uri(FilePath)), 0, e.header));
                        cb.ChatRecord.ScrollToEnd();

                        grid.Children.Add(cb);
                        grid.Visibility = Visibility.Visible;
                        grid.Background = MyColor.Trans;
                        temp.Content = grid;
                        temp.Header = (FriendList.id2friends[e.from] as Friends).Name;
                        temp.Background = MyColor.White;

                        number++;

                        tabControl.Items.Add(temp);
                        id2number.Add(e.from, number);
                        id2window.Add(e.from, cb);
                        id2tabitem.Add(e.from, temp);
                    }
                }));
            }
            //如果是对方同意发送文件
            else if (e.type == ChatMessageType.FILEREQUEST)
            {
                string[] tokens = e.message.Split(new char[] { '~' });
                string filePath = tokens[0];
                string fileName = tokens[1];
                this.Dispatcher.Invoke(new Action(delegate
                {
                    fp = new FileProgress(fileName, 1, myheader);


                    if (id2window.Contains(e.from))
                    {
                        ChatBox temp = id2window[e.from] as ChatBox;

                        temp.Show_Time();
                        temp.ChatRecord.Document.Blocks.Add(Function.ToShow(TextAlignment.Right, fp));
                        temp.ChatRecord.ScrollToEnd();

                        (id2tabitem[e.from] as TabItem).Visibility = Visibility.Visible;
                        ((id2tabitem[e.from] as TabItem).Content as Grid).Visibility = Visibility.Visible;
                        if (tabControl.SelectedIndex != Convert.ToInt32(id2number[e.from]))
                            (id2tabitem[e.from] as TabItem).Background = MyColor.White;

                        UserInfo temp2 = id2userinfo[e.from] as UserInfo;

                        temp2.label1.Content = e.message.Substring(0, min(10, e.message.Length)) + "...";

                    }
                    else if (FriendList.id2ip.Contains(e.from))
                    {

                        TabItem temp = new TabItem();
                        Grid grid = new Grid();
                        grid.Margin = new Thickness(0, -1, 0, 1);
                        ChatBox cb = new ChatBox(this, FriendList.id2ip[e.from] as string, e.from);
                        grid.Children.Add(cb);
                        grid.Visibility = Visibility.Visible;
                        grid.Background = MyColor.Trans;
                        temp.Content = grid;
                        temp.Header = (FriendList.id2friends[e.from] as Friends).Name;
                        temp.Background = MyColor.White;

                        cb.Show_Time();
                        cb.ChatRecord.Document.Blocks.Add(Function.ToShow(TextAlignment.Right, fp));
                        cb.ChatRecord.ScrollToEnd();

                        number++;

                        tabControl.Items.Add(temp);
                        id2number.Add(e.from, number);
                        id2window.Add(e.from, cb);
                        id2tabitem.Add(e.from, temp);

                        UserInfo temp2 = id2userinfo[e.from] as UserInfo;
                        temp2.label1.Content = e.message.Substring(0, min(10, e.message.Length)) + "...";
                    }


                    FileSend fs = new FileSend(FriendList.id2ip[e.from] as string, filePath, fileName, fp);
                    fs.SendFile();
                }));
            }
            else if (e.type == ChatMessageType.SHAKE)
            {
                this.Dispatcher.Invoke(new Action(delegate
                {
                    int i, j, k; //定义三个变量
                    for (i = 1; i <= 3; i++) //循环次数
                    {
                        for (j = 1; j <= 10; j++)
                        {

                            this.Top += 1;
                            this.Left += 1;
                            Thread.Sleep(5);
                        }
                        for (k = 1; k <= 10; k++)
                        {
                            this.Top -= 1;
                            this.Left -= 1;
                            Thread.Sleep(5);
                        }
                    }
                }));
            }
        }

        //关键部分，处理点击用户列表的信息
        private void OnClick(object sender,UserClickEventArgs e)
        {
            if((sender as UserInfo).friend==e.friend)
            {
                welcome.Visibility = Visibility.Collapsed;
                welcomegrid.Visibility = Visibility.Collapsed;
                Friends mySelectedElement = e.friend;
                string selectedid = mySelectedElement.ID;
                string selectedip = mySelectedElement.IP;

                if (mySelectedElement.State == "Online")
                {

                    if (selectedid != null)
                    {
                        if (id2number.Contains(selectedid))
                        {
                            tabControl.SelectedIndex = Convert.ToInt32(id2number[selectedid]);
                            (id2tabitem[selectedid] as TabItem).Visibility = Visibility.Visible;
                            ((id2tabitem[selectedid] as TabItem).Content as Grid).Visibility = Visibility.Visible;

                        }
                        else
                        {
                            TabItem temp = new TabItem();
                            Grid grid = new Grid();
                            grid.Margin = new Thickness(0, -1, 0, 1);
                            ChatBox cb = new ChatBox(this, selectedip, selectedid);
                            grid.Children.Add(cb);
                            grid.Visibility = Visibility.Visible;
                            temp.Content = grid;
                            temp.Header = mySelectedElement.Name;


                            number++;

                            tabControl.Items.Add(temp);
                            tabControl.SelectedIndex = number;
                            id2number.Add(selectedid, number);
                            id2window.Add(selectedid, cb);
                            id2tabitem.Add(selectedid, temp);
                            //, mid2userinfo.Add(selectedid, e.thisuser);
                        }
                    }
                }
                else
                {
                    Message.Show(MessageFunction.Autoclose, "Error", "对方不在线\n尚不提供离线消息服务。");
                }
            }
        }


        //切换到聊天窗口
        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(tabControl.SelectedItem!=null)
                (tabControl.SelectedItem as TabItem).Background = Function.str2color("#00FFFFFF");
        }

        //关闭前下线
        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            f.toserver.Logout(myID);
            Thread.Sleep(200);
            Environment.Exit(0);
        }

        //比较大小
        private int min(int a, int b)
        {
            return a <= b ? a : b;
        }

        //取消设置个人信息
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            tabControl1.SelectedIndex = 0;
        }

        //加好友按钮
        private void Send_Click(object sender, RoutedEventArgs e)
        {
            if(CheckID()==true)
            {
                if (FriendList.id2friends.Contains(FriendID.Text))
                {
                    Message.Show(MessageFunction.Autoclose,"Error","对方已经是你的好友了！");
                }
                else
                {
                    f.temp[f.count] = new Friends();
                    f.temp[f.count].ID = FriendID.Text;
                    f.toserver.Query(FriendID.Text);
                    //执行上一步操作后，id2ip中肯定只有在线的，id2friend和memberdata中都会有，然而对方还不一定是好友，所以下一步要处理
                    Thread.Sleep(500);
                    if (FriendList.id2ip.Contains(FriendID.Text))
                    {
                        Message.Show(MessageFunction.Autoclose, "Success", "对方在线，正在发送好友请求");
                        ChatSend cs = new ChatSend(FriendList.id2ip[FriendID.Text].ToString(), FriendID.Text,this);
                        cs.ConnectFriend();
                        cs.SendMessage(ChatMessageType.REQUEST, myself.IP+"\t"+myself.Name+"\t"+myself.Header);
                    }
                    else
                    {
                        Message.Show(MessageFunction.Autoclose, "Error", "对方离线或不存在该用户,无法接收到好友请求");
                    }
                    //因为人家还不是你的好友，所以删掉咯
                    FriendList.id2friends.Remove(FriendID.Text);
                    FriendList.id2ip.Remove(FriendID.Text);
                    f.memberData.Remove(f.temp[f.count]);
                    f.count1--;
                }
            }
        }

        //确保输入的和学号比较接近
        private bool CheckID()
        {
            int temp;
            if (int.TryParse(FriendID.Text, out temp) == false)
            {
                Message.Show(MessageFunction.Autoclose, "Error!", "是学号哦~~不要输入其他字符");
                return false;
            }
            else if (FriendID.Text.Length > 10)
            {
                Message.Show(MessageFunction.Autoclose, "Error!", "我大清的学号是10位的");
                return false;
            }
            else if(FriendID.Text.Contains("2013")==false)
            {
                Message.Show(MessageFunction.Autoclose, "Error!", "本程序仅限3字班使用");
                return false;
            }
            return true;
        }

        private void FriendID_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FriendID.SelectAll();
        }

        public void refreshfriendlist()
        {
            if (thread == null || !thread.IsAlive)
            {
                try { thread.Abort(); } catch { }
                thread = new Thread(new ThreadStart(RefreshFriendList));
                thread.IsBackground = true;
                thread.Start();
            }
        }

        public void RefreshFriendList()
        {
            f.ReadFriendsList();
            while(true)
            {
                if(f.count1<=f.friendcount)
                {
                    this.Dispatcher.Invoke(new Action(delegate
                    {
                        friendpanel.Children.Clear();
                        foreach (Friends friend in f.memberData)
                        {
                            UserInfo u = new UserInfo(friend, this);
                            if(!id2userinfo.Contains(friend.ID))
                                id2userinfo.Add(friend.ID, u);
                            friendpanel.Children.Add(u);
                        }
                    }));
                    break;
                }
                Thread.Sleep(500);
            }
        }

        private void friendpanel_Loaded(object sender, RoutedEventArgs e)
        {
            refreshfriendlist();
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            if (Flag)
            {
                myheader = (flipview.SelectedIndex + 1).ToString();
                myself.Header = (flipview.SelectedIndex + 1).ToString();
                Flag = false;
            }

            myself.Name = myselfname.Text;
            f.WriteFriendsList(myID, myself.Name, myheader);
            refreshfriendlist();
            Message.Show(MessageFunction.Autoclose, "", "成功录入信息");
            myselfitem.Visibility = Visibility.Collapsed;
            myselfgrid.Visibility = Visibility.Collapsed;
            tabControl1.SelectedIndex = 0;
        }

        private void tabControl1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch(tabControl1.SelectedIndex)
            {
                case 0:
                    label.Content = "Chat";
                    break;
                case 1:
                    label.Content = "Add Friend";
                    break;
                case 2:
                    label.Content = "Information";
                    break;
            }
        }
    
        private void myselfgrid_Loaded(object sender, RoutedEventArgs e)
        {
            header1.Background = new ImageBrush(Function.img2source(Properties.Resources.header1_big));
            header2.Background = new ImageBrush(Function.img2source(Properties.Resources.header2_big));
            header3.Background = new ImageBrush(Function.img2source(Properties.Resources.header3_big));
            header4.Background = new ImageBrush(Function.img2source(Properties.Resources.header4_big));
            header5.Background = new ImageBrush(Function.img2source(Properties.Resources.header5_big));
            header6.Background = new ImageBrush(Function.img2source(Properties.Resources.header6_big));
            header7.Background = new ImageBrush(Function.img2source(Properties.Resources.header7_big));
        }

        private void grid_Loaded(object sender, RoutedEventArgs e)
        {
            //Thread.Sleep(1000);
            foreach (Friends person in f.memberData)
            {
                //查找个人的信息
                if (person.ID == myID)
                {
                    myself = person;
                    myheader = person.Header;
                }
            }
            if (myheader == "-1")
            {
                Flag = true;
                Message.Show(MessageFunction.Autoclose, "", "新用户，请先设置个人信息！");
                myselfitem.Visibility = Visibility.Visible;
                myselfgrid.Visibility = Visibility.Visible;
                flipview.IsEnabled = true;
                tabControl1.SelectedItem = myselfitem;
            }
        }

        private void timer_tick(object sender, EventArgs e)
        {
            refreshfriendlist();
        }

        private void settings_Click(object sender, RoutedEventArgs e)
        {

        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            // 获取鼠标相对标题栏位置  
            Point position = e.GetPosition(bgimage);

            // 如果鼠标位置在标题栏内，允许拖动  
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (position.X >= 0 && position.X < bgimage.ActualWidth && position.Y >= 0 && position.Y < bgimage.ActualHeight)
                {
                    this.DragMove();
                }
            }
        }

        private void MetroWindow_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
                this.Hide();
        }

        private void icon()
        {
            this.notifyIcon = new NotifyIcon();
            this.notifyIcon.Icon = Properties.Resources.logo16;//程序图标
            this.notifyIcon.Visible = true;
            notifyIcon.MouseDoubleClick += OnNotifyIconDoubleClick;
        }

        private void OnNotifyIconDoubleClick(object sender, EventArgs e)
        {
            this.Show();
            WindowState = WindowState.Normal;
        }

    }
}
