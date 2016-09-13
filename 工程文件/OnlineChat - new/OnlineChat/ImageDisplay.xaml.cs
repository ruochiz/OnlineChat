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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using System.Drawing;

namespace OnlineChat
{
    /// <summary>
    /// ImageDisplay.xaml 的交互逻辑
    /// </summary>
    public partial class ImageDisplay : Window
    {
        public ImageDisplay()
        {
            InitializeComponent();
        }

        private DoubleAnimation width;
        private DoubleAnimation height;
        private DoubleAnimation opacity;
        private Storyboard storyboard = new Storyboard();
        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
        public Bitmap Img = null;

        public ImageDisplay(ImageSource img)
        {
            InitializeComponent();
            image.Source = img;
            close.Source = Function.img2source(Properties.Resources.close);
            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.Fant);
            this.WindowState = System.Windows.WindowState.Normal;
            this.WindowStyle = System.Windows.WindowStyle.None;
            this.ResizeMode = System.Windows.ResizeMode.NoResize;
            this.Topmost = true;

            this.Left = 0.0;
            this.Top = 0.0;
            this.Width = 0;
            this.Height = 0;
            this.Opacity = 0;

            width = new DoubleAnimation(System.Windows.SystemParameters.PrimaryScreenWidth, TimeSpan.FromSeconds(0.5));
            height = new DoubleAnimation(System.Windows.SystemParameters.PrimaryScreenHeight, TimeSpan.FromSeconds(1));
            opacity = new DoubleAnimation(1, TimeSpan.FromSeconds(1));

            timer.Interval = TimeSpan.FromSeconds(1.0);
            timer.Tick += finish;
            Storyboard.SetTarget(width, grid);
            Storyboard.SetTarget(height, grid);
            Storyboard.SetTarget(opacity, grid);

            Storyboard.SetTargetProperty(width, new PropertyPath(WidthProperty));
            Storyboard.SetTargetProperty(height, new PropertyPath(HeightProperty));
            Storyboard.SetTargetProperty(opacity, new PropertyPath(OpacityProperty));

        }

        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            MessageBox.Show(e.Key.ToString());
            if (e.Key == Key.Escape)
                this.Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            width = new DoubleAnimation(0, TimeSpan.FromSeconds(0.5));
            height = new DoubleAnimation(0, TimeSpan.FromSeconds(1));
            opacity = new DoubleAnimation(0, TimeSpan.FromSeconds(1));
            Storyboard.SetTarget(width, this);
            Storyboard.SetTarget(height, this);
            Storyboard.SetTarget(opacity, this);

            Storyboard.SetTargetProperty(width, new PropertyPath(WidthProperty));
            Storyboard.SetTargetProperty(height, new PropertyPath(HeightProperty));
            Storyboard.SetTargetProperty(opacity, new PropertyPath(OpacityProperty));
            storyboard.Children.Add(width);
            storyboard.Children.Add(height);
            storyboard.Children.Add(opacity);
            storyboard.Begin(this);
            timer.Start();
        }

        private void Grid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            this.BeginAnimation(WidthProperty, width);
            this.BeginAnimation(HeightProperty, height);
            this.BeginAnimation(OpacityProperty, opacity);
        }

        private void finish(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
            //设置默认文件名
            sfd.Filter = "图像文件|*.jpg;*.jpeg;*.png;*.bmp";
            sfd.FileName = "*.jpg";
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string FilePath = sfd.FileName;
                Img.Save(FilePath);
                this.Close();
            }
        }
    }
}
