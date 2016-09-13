using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MahApps.Metro.Controls;
using System.Windows.Media.Animation;

namespace OnlineChat
{
    public partial class Login : MetroWindow
    {
        string ID_Public;//弹窗显示的ID
        string state = "CLOSED";//定义是否在线的状态

        //动画相关
        DoubleAnimation da_op = new DoubleAnimation();
        DoubleAnimation da_op2 = new DoubleAnimation();
        ThicknessAnimation da_po = new ThicknessAnimation();
        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
        System.Windows.Threading.DispatcherTimer successtimer = new System.Windows.Threading.DispatcherTimer();


        bool success = false;

        Main mainchar;

        ToServer toserver = new ToServer();

        public Login()
        {
            InitializeComponent();
            //加特效！
            this.Opacity = 0;
            progressring.Opacity = 0;
            Function.SetAnimation(0, da_op);
            Function.SetAnimation(1, da_op2);

            //duangduangduang得加特效
            this.BeginAnimation(TextBlock.OpacityProperty, da_op);

            //绑定事件
            da_op.Completed += da_Completed;

            successtimer.Interval = TimeSpan.FromSeconds(1);
            successtimer.Tick += new EventHandler(successtimer_Tick);

            toserver.servermessage += OnServerCall;

            //给LoginButton加上背景图片
            login.Background = new ImageBrush(Function.img2source(Properties.Resources.check));
        }

        //与逻辑层后台的交互
        private void OnServerCall(object sender,ServerEventArgs e)
        {
            if (e.type == ServerMessageType.LOGIN)
            {
                state = "CONNECTED";
                ID_Public = e.message;
            }
            else if (e.type == ServerMessageType.ERROR)
            {
                this.Dispatcher.Invoke(new Action(delegate
                {
                    Message.Show(MessageFunction.Autoclose,"Error!", e.message + "\nLogin.cs/69");

                }));
            }
            else
            {
                this.Dispatcher.Invoke(new Action(delegate
                {
                    Message.Show(MessageFunction.Autoclose, "Error!", "你收到了你不该收到的消息！\n程序猿一定写错了什么！"+"\nLogin.cs/73");
                }));
            }
        }

        /// <summary>
        /// 以下都是动画和逻辑没关系
        /// </summary>

        //动画结束后调用
        private void da_Completed(object sender, EventArgs e)
        {
            //动画完成后暂停1秒
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        //每秒调用一次执行动画
        void timer_Tick(object sender, EventArgs e)
        {
            if (state == "CONNECTED")
            {
                //提示上线
                Message.Show(MessageFunction.Autoclose, "Sucess", ID_Public + "上线啦");
                //停止监听服务器

                //进度环的透明度和位置动画
                progressring.BeginAnimation(OpacityProperty, da_op2);
                progressring.BeginAnimation(MarginProperty, da_po);
                //输入框的透明度动画
                Function.SetAnimation(0, da_op2);
                username.BeginAnimation(OpacityProperty, da_op2);
                password.BeginAnimation(OpacityProperty, da_op2);
                //停止循环
                timer.Stop();

                login.IsEnabled = true;

                successtimer.Start();

                mainchar = new Main(username.Text);

            }
            else
            {
                //失败一定次数停止重复发送
                timer.Stop();
                login.IsEnabled = true;
                //进度环位置动画
                Thickness temp = progressring.Margin;
                temp.Bottom -= progressring.RenderSize.Height;
                temp.Top += progressring.RenderSize.Height;
                da_po.From = progressring.Margin;
                da_po.To = temp;
                da_po.Duration = TimeSpan.FromSeconds(0.3);
                progressring.BeginAnimation(MarginProperty, da_po);

                //输入框的透明度动画
                Function.SetAnimation(0, da_op2);
                username.BeginAnimation(OpacityProperty, da_op2);
                password.BeginAnimation(OpacityProperty, da_op2);
                Function.SetAnimation(1, da_op2);
                progressring.BeginAnimation(OpacityProperty, da_op2);

            }
        }

        //计时1s然后发出成功登陆的标志
        void successtimer_Tick(object sender, EventArgs e)
        {
            if (mainchar != null)
            {
                success = true;
                successtimer.Stop();
                this.Close();
            }
        }

        private void login_Click(object sender, RoutedEventArgs e)
        {
            //开始进度环动画
            progressring.BeginAnimation(TextBlock.OpacityProperty, da_op);
            //进度环位置动画
            Thickness temp = progressring.Margin;
            temp.Bottom += progressring.RenderSize.Height;
            temp.Top -= progressring.RenderSize.Height;
            da_po.From = progressring.Margin;
            da_po.To = temp;
            da_po.Duration = TimeSpan.FromSeconds(0.3);
            progressring.BeginAnimation(MarginProperty, da_po);
            //输入框透明度动画
            username.BeginAnimation(TextBlock.OpacityProperty, da_op2);
            password.BeginAnimation(TextBlock.OpacityProperty, da_op2);

            //将按钮暂时禁用
            login.IsEnabled = false;
            //toserver.Login(username.Text);
            try
            {
                toserver.Login(username.Text);
            }
            catch(Exception ex)
            {
                Message.Show(MessageFunction.Autoclose, "Error", ex.Message+"\nLogin.cs/168");
                //this.Close(); 
            }
            
        }

        //弹出设置窗口
        private void settings_Click(object sender, RoutedEventArgs e)
        {
            MessageResult result= Message.Show(MessageFunction.YesNo, "setting", "setting");
        }

        //gif动画
        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            ((MediaElement)sender).Position = ((MediaElement)sender).Position.Add(TimeSpan.FromMilliseconds(1));
        }

        //允许拖动窗口
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            // 获取鼠标相对标题栏位置  
            Point position = e.GetPosition(element);

            // 如果鼠标位置在标题栏内，允许拖动  
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (position.X >= 0 && position.X < element.ActualWidth && position.Y >= 0 && position.Y < element.ActualHeight)
                {
                    this.DragMove();
                }
            }
        }

        //关闭判断是否登陆成功
        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (success)
            {
                mainchar.Show();
                timer.Stop();
                successtimer.Stop();
            }
            else
            {
                e.Cancel = false;
                Environment.Exit(0);
            }
        }
    }
}
