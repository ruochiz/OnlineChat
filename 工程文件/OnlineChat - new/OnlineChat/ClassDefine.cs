
using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace OnlineChat
{
    public class Friends
    {
        public string Name { get; set; }
        public string IP { get; set; }
        public string ID { get; set; }
        public string State { get; set; }
        public string Header { get; set; }
    }

    public enum MessageResult
    {
        //用户直接关闭了消息窗口
        None = 0,
        //用户点击是按钮
        Yes = 1,
        //用户点击否按钮
        No = 2
    }
    public enum MessageFunction
    {
        Normal = 0,
        Autoclose = 1,
        YesNo = 2,
        OK = 3,
        OKCancel = 4
    }
    public enum ServerMessageType
    {
        LOGIN,
        LOGOUT,
        QUERYONLINE,
        QUERYOFFLINE,
        OTHER,
        ERROR
    }
    public enum ChatMessageType
    {
        HELLO,
        CHAT,
        PICTURE,
        FILE,
        FILEREQUEST,
        VIDEO,
        REQUEST,
        ERROR,
        SHAKE
    }

    public static class MyColor
    {
        public static SolidColorBrush Trans = Function.str2color("#00FFFFFF");
        public static SolidColorBrush TransGrey = Function.str2color("#33252525");
        public static SolidColorBrush White = Function.str2color("#FFFFFFFF");
        public static SolidColorBrush LightBlue = Function.str2color("#CC5FCFFF");
        public static Color lightBlue = (Color)ColorConverter.ConvertFromString("#CC5FCFFF");
    }
    class ClassDefine
    {
    }
}
