using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace OnlineChat
{
    /// <summary>
    /// UserInfo.xaml 的交互逻辑
    /// </summary>
    public partial class UserInfo : UserControl
    {
        public Friends friend;
        public Main main;

        public static event UserClick userclick;
        public delegate void UserClick(object sender, UserClickEventArgs e);
        DoubleAnimation da_op = new DoubleAnimation();


        public UserInfo()
        {
            InitializeComponent();
        }

        public UserInfo(Friends friend,Main main)
        {
            InitializeComponent();

            //初始化参数
            this.friend = friend;
            this.main = main;
            Function.SetAnimation(0, da_op);

            //初始化样式
            label2.Content = friend.Name;
            image1.Source = Function.img2source(Properties.Resources.close);
            image1.Opacity = 0;
            image.Source = Function.img2source(Properties.Resources.header7);
            switch (friend.Header)
            {
                case "1":
                    image.Source = Function.img2source(Properties.Resources.header1);
                    break;
                case "2":
                    image.Source = Function.img2source(Properties.Resources.header2);
                    break;
                case "3":
                    image.Source = Function.img2source(Properties.Resources.header3);
                    break;
                case "4":
                    image.Source = Function.img2source(Properties.Resources.header4);
                    break;
                case "5":
                    image.Source = Function.img2source(Properties.Resources.header5);
                    break;
                case "6":
                    image.Source = Function.img2source(Properties.Resources.header6);
                    break;
                case "7":
                    image.Source = Function.img2source(Properties.Resources.header7);
                    break;
                default:
                    image.Source = Function.img2source(Properties.Resources.header7);
                    break;
            }

            //如果不在线则变颜色
            if (friend.State!="Online")
            {
                this.Background = Function.str2color("#7F252525");
            }
        }

        private void image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            userclick(this, new UserClickEventArgs(friend, this));
        }

        private void label2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            userclick(this, new UserClickEventArgs(friend, this));
        }

        private void label1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            userclick(this, new UserClickEventArgs(friend, this));
        }


        //以下三段实现，鼠标移入移出固定区域是否出现关闭符号，以及点击关闭
        private void image1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //只是隐藏，保留聊天记录
            (main.id2tabitem[friend.ID] as TabItem).Visibility = Visibility.Collapsed;
            ((main.id2tabitem[friend.ID] as TabItem).Content as Grid).Visibility = Visibility.Collapsed;
            
            //切换到第一个可见的窗体
            foreach(TabItem item in main.tabControl.Items)
            {
                if (item.Visibility == Visibility.Visible)
                    main.tabControl.SelectedItem = item;
            }
            Function.SetAnimation(1, da_op);
            image1.BeginAnimation(OpacityProperty, da_op);
            image1.Opacity = 0;
            image1.IsEnabled = false;
        }

        private void grid_MouseEnter(object sender, MouseEventArgs e)
        {
            if (main.id2window.Contains(friend.ID))
            {
                if ((main.id2tabitem[friend.ID] as TabItem).Visibility == Visibility.Visible)
                {
                    Function.SetAnimation(0, da_op);
                    image1.BeginAnimation(OpacityProperty, da_op);
                    image1.IsEnabled = true;
                }
            }
        }

        private void grid_MouseLeave(object sender, MouseEventArgs e)
        {
            if(image1.IsEnabled==true)
            {
                Function.SetAnimation(1, da_op);
                image1.BeginAnimation(OpacityProperty, da_op);
            }
        }
    }
    public class UserClickEventArgs : System.EventArgs
    {
        public Friends friend;
        public UserInfo thisuser;
        public UserClickEventArgs()
        {
        }
        public UserClickEventArgs(Friends f,UserInfo u)
        {
            friend = f;
            thisuser = u;
        }

    }
}

