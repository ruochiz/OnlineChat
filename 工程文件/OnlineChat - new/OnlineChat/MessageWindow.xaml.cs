using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using MahApps.Metro.Controls;

namespace OnlineChat
{
    /// <summary>
    /// Message.xaml 的交互逻辑
    /// </summary>
    public partial class MessageWindow : MetroWindow
    {
        //本类实现了MessageBox的重绘
        //这个主要负责UI层的交互，逻辑层在Message.cs中实现
        DoubleAnimation animation = new DoubleAnimation();
        public System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
        private MessageFunction function;
        public MessageResult Result;
        bool closing = false;

        public MessageWindow(MessageFunction function, string title, string message)
        {
            InitializeComponent();
            this.Opacity = 0;
            Function.SetAnimation(0, animation);
            animation.Completed += animation_Completed;
            this.BeginAnimation(TextBlock.OpacityProperty, animation);//开始动画

            this.Title = title;
            this.function = function;
            //label.Content = message;
            labeltext.Text = message;
            Result = MessageResult.None;
        }





        //关闭动画
        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Function.SetAnimation(1, animation);
            this.BeginAnimation(OpacityProperty, animation);//开始动画
            if (closing)
                e.Cancel = false;
        }

        //动画完成
        private void animation_Completed(object sender, EventArgs e)
        {
            if (this.Opacity <= 0.1)
            {
                closing = true;
                this.Close();
            }
            if (function == MessageFunction.Autoclose && !closing)
            {
                //延时1s
                timer.Interval = new TimeSpan(0, 0, 1);
                timer.Tick += new EventHandler(timer_Tick);
                timer.Start();
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            Function.SetAnimation(1, animation);         //动画持续时间
            this.BeginAnimation(OpacityProperty, animation);//开始动画
            timer.Stop();
        }

        private void yes_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageResult.Yes;
            this.Close();
        }

        private void no_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageResult.No;
            this.Close();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            if (function == MessageFunction.YesNo)
            {
                Thickness temp = label.Margin;
                temp.Bottom  += label.ActualHeight/3;
                label.Margin = temp;
                //label.Height = 50;
                yes.Opacity = 1;
                no.Opacity = 1;
            }
            else if (function == MessageFunction.OKCancel)
            {
                //label.Margin = new Thickness(30, 0, 30, 80);
                Thickness temp = label.Margin;
                temp.Bottom += label.ActualHeight / 3;
                label.Margin = temp;
                //label.Height = 50;
                yes.Opacity = 1;
                no.Opacity = 1;
                yes.Content = "OK";
                no.Content = "Cancel";
            }
            else if (function == MessageFunction.OK)
            {
                //label.Margin = new Thickness(30, 0, 30, 80);
                //label.Height = 50;
                Thickness temp = label.Margin;
                temp.Bottom += label.ActualHeight / 3;
                label.Margin = temp;
                yes.Opacity = 1;
                no.Opacity = 0;
                yes.Content = "OK";
                yes.Margin = new Thickness(0, 0, -yes.Width / 2, 26);
            }
            else
            {
                //label.Margin = new Thickness(30, 0, 30, 35);
                //label.Height = 80;
                yes.Opacity = 0;
                no.Opacity = 0;
            }
        }
    }
}
