using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace OnlineChat
{
    /// <summary>
    /// ChatMessage.xaml 的交互逻辑
    /// </summary>
    public partial class ChatMessage : UserControl
    {
        private Image im = null;
        public ChatMessage()
        {
            InitializeComponent();
        }

        public ChatMessage(string Message,int type,string header)
        {
            InitializeComponent();
            ColorAnimation BlueToWhite = new ColorAnimation((Color)ColorConverter.ConvertFromString("#CC5FCFFF"), Colors.White, TimeSpan.FromSeconds(1));
            BlueToWhite.AutoReverse = true;
            BlueToWhite.RepeatBehavior = new RepeatBehavior(3);

            ColorAnimation WhiteToBlue = new ColorAnimation(Colors.White,(Color)ColorConverter.ConvertFromString("#CC5FCFFF"),TimeSpan.FromSeconds(1));
            WhiteToBlue.AutoReverse = true;
            WhiteToBlue.RepeatBehavior = new RepeatBehavior(3);

            while (Message.Contains("[emoji]"))
            {
                try
                {
                    int point = Message.IndexOf("[emoji]");
                    string Message1 = Message.Remove(point, Message.Length - point);
                    string emoji = Message.Substring(point + 7, 10);
                    Message = Message.Remove(0, point + 24);
                    textblock.Inlines.Add(Message1);
                    Image i = new Image();
                    i.Source = new BitmapImage(new Uri("./Resources/emoji/" + emoji + ".png", UriKind.Relative));
                    //i.Source = Function.img2source(Properties.Resources.emoji_E107);
                    i.Stretch = Stretch.None;

                    textblock.Inlines.Add(new InlineUIContainer(i));
                }
                catch { }
            }
            textblock.Inlines.Add(Message);
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
            //区分是收到的消息还是发送的消息
            if(type==1)
            {
                image.HorizontalAlignment = HorizontalAlignment.Right;
                label.HorizontalAlignment = HorizontalAlignment.Right;
                label.HorizontalContentAlignment = HorizontalAlignment.Left;
                textblock.TextAlignment = TextAlignment.Left;
                label.Margin = new Thickness(10, 8, 60, 0);
                SolidColorBrush scb = new SolidColorBrush(Colors.White);
                //SolidColorBrush scb = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CC5FCFFF"));
                //scb.BeginAnimation(SolidColorBrush.ColorProperty, BlueToWhite);
                label.BorderBrush = scb;
                label.Background = Function.str2color("#FFFFFFFF");
                label.BorderThickness = new Thickness(2, 2, 2, 2);
            }
            else
            {
                SolidColorBrush scb = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CC5FCFFF"));
                //SolidColorBrush scb = new SolidColorBrush(Colors.White);
                //scb.BeginAnimation(SolidColorBrush.ColorProperty, WhiteToBlue);
                label.BorderBrush = scb;
                label.Background = Function.str2color("#CC5FCFFF");
                label.Foreground = new SolidColorBrush(Colors.White);
                label.BorderThickness = new Thickness(2, 2, 2, 2);
            }
        }

        public ChatMessage(ImageSource Image, int type, string header)
        {
            InitializeComponent();
            ColorAnimation BlueToWhite = new ColorAnimation((Color)ColorConverter.ConvertFromString("#CC5FCFFF"), Colors.White, TimeSpan.FromSeconds(1));
            BlueToWhite.AutoReverse = true;
            BlueToWhite.RepeatBehavior = new RepeatBehavior(3);

            ColorAnimation WhiteToBlue = new ColorAnimation(Colors.White, (Color)ColorConverter.ConvertFromString("#CC5FCFFF"), TimeSpan.FromSeconds(1));
            WhiteToBlue.AutoReverse = true;
            WhiteToBlue.RepeatBehavior = new RepeatBehavior(3);

            Image i = new Image();
            i.Source = Image;
            i.Stretch = Stretch.Uniform;
            im = i;
            RenderOptions.SetBitmapScalingMode(i, BitmapScalingMode.Fant);
            InlineUIContainer iu = new InlineUIContainer(i);
            iu.Focusable = true;
            textblock.Inlines.Add(iu);
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
            //区分是收到的消息还是发送的消息
            if (type == 1)
            {
                image.HorizontalAlignment = HorizontalAlignment.Right;
                label.HorizontalAlignment = HorizontalAlignment.Right;
                label.HorizontalContentAlignment = HorizontalAlignment.Left;
                textblock.TextAlignment = TextAlignment.Left;
                label.Margin = new Thickness(10, 8, 60, 0);
                SolidColorBrush scb = new SolidColorBrush(Colors.White);
                //SolidColorBrush scb = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CC5FCFFF"));
                //scb.BeginAnimation(SolidColorBrush.ColorProperty, BlueToWhite);
                label.BorderBrush = scb;
                label.Background = Function.str2color("#FFFFFFFF");
                label.BorderThickness = new Thickness(2, 2, 2, 2);
            }
            else
            {
                //SolidColorBrush scb = new SolidColorBrush(Colors.White);
                //scb.BeginAnimation(SolidColorBrush.ColorProperty, WhiteToBlue);
                SolidColorBrush scb = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CC5FCFFF"));
                label.BorderBrush = scb;
                label.Background = Function.str2color("#CC5FCFFF");
                label.Foreground = new SolidColorBrush(Colors.White);
                label.BorderThickness = new Thickness(2, 2, 2, 2);
            }
        }

        private void textblock_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (im != null)
            {
                ImageDisplay id = new ImageDisplay(im.Source);
                id.Show();
                id.Focus();
            }
        }
    }
}
