using System;
using System.Windows.Media.Imaging;

namespace RisCaptureLib
{
    public class ScreenCaputredEventArgs : EventArgs
    {
        public BitmapSource Bmp
        {
            get;
            private set;
        }
        public System.Drawing.Bitmap BmpImg
        {
            get;
            private set;
        }
        public ScreenCaputredEventArgs(BitmapSource bmp, System.Drawing.Bitmap bmpImg)
        {
            Bmp = bmp;
            BmpImg = bmpImg;
        }
    }
}
