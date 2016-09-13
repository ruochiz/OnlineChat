using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Collections;
using System.IO;
using System.Drawing;

namespace OnlineChat
{
    public class Function
    {
        //得到信息窗口
        public static Paragraph ToShow(TextAlignment Align, string Message, int type,string header)
        {
            Paragraph paragraph = new Paragraph();
            paragraph.TextAlignment = Align;            
            ChatMessage ctmessage = new ChatMessage(Message,type,header);
            paragraph.Inlines.Add(ctmessage);
            paragraph.Padding = new Thickness(1, 1, 1, 1);

            return paragraph;
        }

        public static Paragraph ToShow(TextAlignment Align, ImageSource Image, int type, string header)
        {
            Paragraph paragraph = new Paragraph();
            paragraph.TextAlignment = Align;
            ChatMessage ctmessage = new ChatMessage(Image, type, header);
            paragraph.Inlines.Add(ctmessage);
            paragraph.Padding = new Thickness(1, 1, 1, 1);

            return paragraph;
        }

        //添加其他自定义控件
        public static Paragraph ToShow(TextAlignment Align,object a)
        {
            Paragraph paragraph = new Paragraph();
            paragraph.TextAlignment = Align;
            paragraph.Inlines.Add(a as UserControl);
            paragraph.Padding = new Thickness(1, 1, 1, 1);

            return paragraph;
        }

        public static SolidColorBrush str2color(string str)
        {
            return new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(str));
        }

        //设置动画插值函数
        public static void SetAnimation(int type, DoubleAnimation da)
        {
            da.From = type;
            da.To = 1 - type;
            da.Duration = TimeSpan.FromSeconds(0.3);
        }

        //设置动画插值函数
        public static void SetAnimation(Thickness from, Thickness to, ThicknessAnimation da)
        {
            da.From = from;
            da.To = to;
            da.Duration = TimeSpan.FromSeconds(0.3);
        }

        public static ImageSource img2source(System.Drawing.Bitmap img)
        {
            var testImg = img;
            MemoryStream memory = new MemoryStream();
            if(testImg!=null)
            {
                testImg.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                ImageSourceConverter converter = new ImageSourceConverter();
                ImageSource source = (ImageSource)converter.ConvertFrom(memory);
                return source;
            }
            return null;
        }

        public static ImageSource img2source(System.Drawing.Image img)
        {
            var testImg = img;
            MemoryStream memory = new MemoryStream();
            if (testImg != null)
            {
                testImg.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                ImageSourceConverter converter = new ImageSourceConverter();
                ImageSource source = (ImageSource)converter.ConvertFrom(memory);
                return source;
            }
            return null;
        }

        public static ImageSource img2source(System.Windows.Controls.Image img)
        {
            var testImg = img;
            if (testImg != null)
            {
                ImageSource source = testImg.Source;
                return source;
            }
            return null;
        }

        public static System.Drawing.Image BytToImg(byte[] byt)
        {
            try
            {
                MemoryStream ms = new MemoryStream(byt);
                System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
                return img;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        ///  图片转换成字节流 
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static byte[] ImageToByteArray(System.Drawing.Image img)
        {
            ImageConverter imgconv = new ImageConverter();
            byte[] b = (byte[])imgconv.ConvertTo(img, typeof(byte[]));
            return b;
        }
    }
}
