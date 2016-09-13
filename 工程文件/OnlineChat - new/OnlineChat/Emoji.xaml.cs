using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OnlineChat
{
    /// <summary>
    /// Emoji.xaml 的交互逻辑
    /// </summary>
    public partial class Emoji : UserControl
    {
        public ChatBox cb;
        public string emojistr;
        public Emoji()
        {
            InitializeComponent();
        }

        public Emoji(string emo,ChatBox cb)
        {
            InitializeComponent();

            emojistr = "[emoji]emoji-E" + emo + "[emoji]";
            this.cb = cb;

            //初始化图像
            switch(emo)
            {
                case "00E":
                    EMOJI.Source = Function.img2source(Properties.Resources.emoji_E00E);
                    break;
                case "11B":
                    EMOJI.Source = Function.img2source(Properties.Resources.emoji_E11B);
                    break;
                case "40A":
                    EMOJI.Source = Function.img2source(Properties.Resources.emoji_E40A);
                    break;
                case "40B":
                    EMOJI.Source = Function.img2source(Properties.Resources.emoji_E40B);
                    break;
                case "40C":
                    EMOJI.Source = Function.img2source(Properties.Resources.emoji_E40C);
                    break;
                case "40D":
                    EMOJI.Source = Function.img2source(Properties.Resources.emoji_E40D);
                    break;
                case "40E":
                    EMOJI.Source = Function.img2source(Properties.Resources.emoji_E40E);
                    break;
                case "40F":
                    EMOJI.Source = Function.img2source(Properties.Resources.emoji_E40F);
                    break;
                case "056":
                    EMOJI.Source = Function.img2source(Properties.Resources.emoji_E056);
                    break;
                case "057":
                    EMOJI.Source = Function.img2source(Properties.Resources.emoji_E057);
                    break;
                case "058":
                    EMOJI.Source = Function.img2source(Properties.Resources.emoji_E058);
                    break;
                case "059":
                    EMOJI.Source = Function.img2source(Properties.Resources.emoji_E059);
                    break;
                case "105":
                    EMOJI.Source = Function.img2source(Properties.Resources.emoji_E105);
                    break;
                case "106":
                    EMOJI.Source = Function.img2source(Properties.Resources.emoji_E106);
                    break;
                case "107":
                    EMOJI.Source = Function.img2source(Properties.Resources.emoji_E107);
                    break;
                case "108":
                    EMOJI.Source = Function.img2source(Properties.Resources.emoji_E108);
                    break;
                case "401":
                    EMOJI.Source = Function.img2source(Properties.Resources.emoji_E401);
                    break;
                case "402":
                    EMOJI.Source = Function.img2source(Properties.Resources.emoji_E402);
                    break;
                case "403":
                    EMOJI.Source = Function.img2source(Properties.Resources.emoji_E403);
                    break;
                case "404":
                    EMOJI.Source = Function.img2source(Properties.Resources.emoji_E404);
                    break;
                case "405":
                    EMOJI.Source = Function.img2source(Properties.Resources.emoji_E405);
                    break;
                case "406":
                    EMOJI.Source = Function.img2source(Properties.Resources.emoji_E406);
                    break;
                case "407":
                    EMOJI.Source = Function.img2source(Properties.Resources.emoji_E407);
                    break;
                case "408":
                    EMOJI.Source = Function.img2source(Properties.Resources.emoji_E408);
                    break;
                case "409":
                    EMOJI.Source = Function.img2source(Properties.Resources.emoji_E409);
                    break;
                case "410":
                    EMOJI.Source = Function.img2source(Properties.Resources.emoji_E410);
                    break;
                case "411":
                    EMOJI.Source = Function.img2source(Properties.Resources.emoji_E411);
                    break;
                case "412":
                    EMOJI.Source = Function.img2source(Properties.Resources.emoji_E412);
                    break;
                case "413":
                    EMOJI.Source = Function.img2source(Properties.Resources.emoji_E413);
                    break;
                case "414":
                    EMOJI.Source = Function.img2source(Properties.Resources.emoji_E414);
                    break;
                case "415":
                    EMOJI.Source = Function.img2source(Properties.Resources.emoji_E415);
                    break;
                case "416":
                    EMOJI.Source = Function.img2source(Properties.Resources.emoji_E416);
                    break;
                case "417":
                    EMOJI.Source = Function.img2source(Properties.Resources.emoji_E417);
                    break;
                case "418":
                    EMOJI.Source = Function.img2source(Properties.Resources.emoji_E418);
                    break;
                case "419":
                    EMOJI.Source = Function.img2source(Properties.Resources.emoji_E419);
                    break;
                case "420":
                    EMOJI.Source = Function.img2source(Properties.Resources.emoji_E420);
                    break;
            }
        }

        private void EMOJI_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            cb.Dispatcher.Invoke(new Action(delegate
            {
                cb.Tosend.AppendText(emojistr);
                cb.Tosend.SelectionStart = cb.Tosend.Text.Length;
                cb.Tosend.Focus();
                cb.eb.Visibility = Visibility.Collapsed;
                cb.eb.IsEnabled = false;
            }));
        }
    }
}
