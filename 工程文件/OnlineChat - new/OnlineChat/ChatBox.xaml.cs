using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Windows.Media;

namespace OnlineChat
{
    /// <summary>
    /// ChatBox.xaml 的交互逻辑
    /// </summary>
    public partial class ChatBox : UserControl
    {

        ChatSend cs = null;

        private readonly RisCaptureLib.ScreenCaputre screenCaputre = new RisCaptureLib.ScreenCaputre();
        System.Windows.Threading.DispatcherTimer cuttimer = new System.Windows.Threading.DispatcherTimer();
        private Size? lastSize;

        //ChatListen cl = new ChatListen();

        string ip = "";

        //调用自己的主窗口
        Main m;

        //记录最后一条消息的发送时间，决定是否更新时间标签
        public int hourcurrent = -1;
        public int minutecurrent = -1;

        public ChatBox()
        {
        }

        public ChatBox(Main main, string ip, string id)
        {
            InitializeComponent();


            //初始化参数
            this.ip = ip;
            m = main;
            eb.cb = this;
            cs = new ChatSend(ip, id, main);

            //绑定图片
            fileimage.Source = Function.img2source(Properties.Resources.file);
            emotionimage.Source = Function.img2source(Properties.Resources.emotion);
            picimage.Source = Function.img2source(Properties.Resources.pic);
            shakeimage.Source = Function.img2source(Properties.Resources.shake);
            cutimage.Source = Function.img2source(Properties.Resources.cut);
            eb.Visibility = Visibility.Collapsed;
            eb.IsEnabled = false;

            ChatRecord.FocusVisualStyle = null;
            ChatRecord.SelectionBrush = null;
            ChatRecord.IsDocumentEnabled = true;
            cs.ConnectFriend();

            screenCaputre.ScreenCaputred += OnScreenCaputred;
            screenCaputre.ScreenCaputreCancelled += OnScreenCaputreCancelled;
            cuttimer.Interval = TimeSpan.FromMilliseconds(10);
            cuttimer.Tick += new EventHandler(cuttimer_Tick);
        }

        public void SendMessage(ChatMessageType type, string message)
        {
            //调用发送信息的函数
            cs.SendMessage(type, message);
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                cs.ConnectFriend();
            }
            catch { }
            try
            {
                cs.SendMessage(ChatMessageType.CHAT, Tosend.Text);
            }
            catch (Exception ex)
            {
                Thread.Sleep(200);
            }

            //判断一下是否需要更新时间

            Show_Time();
            //在窗口显示内容、清空输入框、确保窗口自动滚动至下方
            ChatRecord.Document.Blocks.Add(Function.ToShow(TextAlignment.Right, Tosend.Text, 1, Main.myheader));
            Tosend.Clear();
            ChatRecord.ScrollToEnd();
        }

        //显示时间标签
        public bool Show_Time()
        {
            if (DateTime.Now.Hour != hourcurrent || DateTime.Now.Minute - minutecurrent > 1)
            {
                hourcurrent = DateTime.Now.Hour;
                minutecurrent = DateTime.Now.Minute;
                Paragraph paragraph = new Paragraph();
                paragraph.TextAlignment = TextAlignment.Center;
                Label temp = new Label();
                temp.Background = Function.str2color("#3F252525");
                temp.Foreground = MyColor.White;
                temp.Content = DateTime.Now.ToString("HH:mm");
                paragraph.Inlines.Add(temp);
                ChatRecord.Document.Blocks.Add(paragraph);
            }
            return true;
        }

        //发文件
        private void file_Click(object sender, RoutedEventArgs e)
        {
            string filePath = null;
            string fileName = null;
            double fileLength = 0;
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                fileName = dialog.SafeFileName;
                filePath = dialog.FileName;
            }
            if (!string.IsNullOrEmpty(filePath))
            {
                fileLength = new FileInfo(filePath).Length;
                fileLength /= 1024;
                fileLength /= 1024;

                cs.SendMessage(ChatMessageType.FILE, fileName + "~" + filePath + "~" + fileLength.ToString("0.00"));
            }
        }

        private void pic_Click(object sender, RoutedEventArgs e)
        {
            string filePath = null;
            string fileName = null;
            double fileLength = 0;
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.Filter = "图像文件|*.jpg;*.jpeg;*.png;*.bmp";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                fileName = dialog.SafeFileName;
                filePath = dialog.FileName;
            }
            if (!string.IsNullOrEmpty(filePath))
            {
                fileLength = new FileInfo(filePath).Length;
                fileLength /= 1024;
                fileLength /= 1024;

                int failtime = 0;
                cs.SendMessage(ChatMessageType.PICTURE, fileName + "~" + filePath + "~" + fileLength.ToString("0.00"));
                //等待对方开启照片接收线程
                Thread.Sleep(1000);
                while (true)
                {
                    try
                    {
                        FileSend fs = new FileSend(ip, filePath, fileName);
                        fs.SendFile();
                        break;
                    }
                    catch (Exception ex)
                    {
                        Thread.Sleep(200);
                        failtime++;
                        if (failtime == 10)
                            break;
                    }
                }
                Show_Time();
                ChatRecord.Document.Blocks.Add(Function.ToShow(TextAlignment.Right, new BitmapImage(new Uri(filePath, UriKind.Relative)), 1, Main.myheader));
                ChatRecord.ScrollToEnd();

            }
        }

        private void shake_Click(object sender, RoutedEventArgs e)
        {
            cs.SendMessage(ChatMessageType.SHAKE, "");
            m.Dispatcher.Invoke(new Action(delegate
            {
                int i, j, k; //定义三个变量
                for (i = 1; i <= 3; i++) //循环次数
                {
                    for (j = 1; j <= 10; j++)
                    {

                        m.Top += 1;
                        m.Left += 1;
                        Thread.Sleep(5); //当前线程指定挂起的时间
                    }
                    for (k = 1; k <= 10; k++)
                    {
                        m.Top -= 1;
                        m.Left -= 1;
                        Thread.Sleep(5);
                    }
                }
            }));
        }

        private void cut_Click(object sender, RoutedEventArgs e)
        {
            m.Dispatcher.Invoke(new Action(delegate
            {
                m.Hide();
            }));
            cuttimer.Start();
            
        }
        void cuttimer_Tick(object sender, EventArgs e)
        {
            screenCaputre.StartCaputre(30, lastSize);
            cuttimer.Stop();
        }
        private void OnScreenCaputreCancelled(object sender, System.EventArgs e)
        {
            m.Dispatcher.Invoke(new Action(delegate
            {
                m.Show();
                m.Focus();
            }));
        }

        private void OnScreenCaputred(object sender, RisCaptureLib.ScreenCaputredEventArgs e)
        {
            //set last size
            lastSize = new Size(e.Bmp.Width, e.Bmp.Height);


            m.Dispatcher.Invoke(new Action(delegate
            {
                m.Show();
                System.Drawing.Bitmap bmpImg = e.BmpImg;
                string fileName = new Random().NextDouble().ToString() + ".jpg";
                string filePath = "./ChatImageSend/" + new Random().NextDouble().ToString() + ".jpg";
                bmpImg.Save(filePath);
                FileInfo f = new FileInfo(filePath);
                double fileLength = f.Length;
                Show_Time();
                ChatRecord.Document.Blocks.Add(Function.ToShow(TextAlignment.Right,e.Bmp, 1, Main.myheader));
                ChatRecord.ScrollToEnd();
                cs.SendMessage(ChatMessageType.PICTURE, fileName + "~" + filePath + "~" + fileLength.ToString("0.00"));
                //等待对方开启照片接收线程
                Thread.Sleep(2000);
                while (true)
                {
                    try
                    {
                        FileSend fs = new FileSend(ip, filePath, fileName);
                        fs.SendFile();
                        break;
                    }
                    catch (Exception ex)
                    {
                        Thread.Sleep(200);
                    }
                }

            }));


        }

        private void emotion_Click(object sender, RoutedEventArgs e)
        {
            if (eb.IsEnabled)
            {
                eb.Visibility = Visibility.Collapsed;
                eb.IsEnabled = false;
            }
            else
            {
                eb.Visibility = Visibility.Visible;
                eb.IsEnabled = true;
            }
        }
    }
}
