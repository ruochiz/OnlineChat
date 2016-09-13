using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace OnlineChat
{
    /// <summary>
    /// FileProgress.xaml 的交互逻辑
    /// </summary>
    public partial class FileProgress : UserControl
    {
        public FileProgress(string filename,int type,string header)
        {
            InitializeComponent();
            ColorAnimation BlueToWhite = new ColorAnimation((Color)ColorConverter.ConvertFromString("#CC5FCFFF"), Colors.White, TimeSpan.FromSeconds(1));
            BlueToWhite.AutoReverse = true;
            BlueToWhite.RepeatBehavior = new RepeatBehavior(3);

            ColorAnimation WhiteToBlue = new ColorAnimation(Colors.White, (Color)ColorConverter.ConvertFromString("#CC5FCFFF"), TimeSpan.FromSeconds(1));
            WhiteToBlue.AutoReverse = true;
            WhiteToBlue.RepeatBehavior = new RepeatBehavior(3);

            image.Source = Function.img2source(Properties.Resources.header7);
            switch (header)
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
            textblock.Text = filename;
            if (type == 1)
            {
                image.HorizontalAlignment = HorizontalAlignment.Right;
                label.HorizontalAlignment = HorizontalAlignment.Right;
                label.HorizontalContentAlignment = HorizontalAlignment.Left;
                textblock.TextAlignment = TextAlignment.Left;
                label.Margin = new Thickness(5, 0, 55, 15);
                progressbar.Margin = new Thickness(5, 0, 55, 5);
                SolidColorBrush scb = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CC5FCFFF"));
                scb.BeginAnimation(SolidColorBrush.ColorProperty, BlueToWhite);
                label.BorderBrush = scb;
                label.Background = Function.str2color("#FFFFFFFF");
                label.BorderThickness = new Thickness(2, 2, 2, 2);
            }
            else
            {
                SolidColorBrush scb = new SolidColorBrush(Colors.White);
                scb.BeginAnimation(SolidColorBrush.ColorProperty, WhiteToBlue);
                label.BorderBrush = scb;
                label.Background = Function.str2color("#CC5FCFFF");
                label.Foreground = new SolidColorBrush(Colors.White);
                label.BorderThickness = new Thickness(2, 2, 2, 2);
            }
        }

        //更新进度条
        public void Update(double progress)
        {
            progressbar.Value = progress;
            if(progress==100)
            {
                DoubleAnimation da_op = new DoubleAnimation();
                Function.SetAnimation(1, da_op);
                progressbar.BeginAnimation(OpacityProperty, da_op);

                ThicknessAnimation ta = new ThicknessAnimation();
                Thickness temp = label.Margin;
                temp.Bottom -= 10;
                temp.Top += 10;
                ta.From = label.Margin;
                ta.To = temp;
                ta.Duration = TimeSpan.FromSeconds(0.3);
                label.BeginAnimation(MarginProperty, ta);
            }
        }
        
    }
}
