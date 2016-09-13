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
    /// EmojiBox.xaml 的交互逻辑
    /// </summary>
    public partial class EmojiBox : UserControl
    {
        //36
        private string[] emojilist= { "00E","11B","40A","40B","40C",
                                      "40D","40E","40F","056","057",
                                      "058","059","105","106","107",
                                      "108","401","402","403","404",
                                      "405","406","407","408","409",
                                      "410","411","412","413","414",
                                      "415","416","417","418","419","420"};
        public ChatBox cb;

        public EmojiBox()
        {
            InitializeComponent();
            
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 36; i++)
            {
                Emoji em = new Emoji(emojilist[i], cb);

                emojipanel.Children.Add(em);
            }
        }
    }
}
