using System;
using System.Windows;

namespace OnlineChat
{
    class Message
    {
        //提供了三种输出消息窗口的接口
        public static MessageResult Show(MessageFunction function,string title,string message)
        {
            MessageWindow window = new MessageWindow(function, title, message);
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                window.ShowDialog();
            }));
            return window.Result;
        }
        public static MessageResult Show(string title, string message)
        {
            MessageWindow window = new MessageWindow(MessageFunction.Normal, title, message);
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                window.ShowDialog();
            }));
            return window.Result;
        }

        public static MessageResult Show(string message)
        {
            MessageWindow window = new MessageWindow(MessageFunction.Normal, "Message", message);
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                window.ShowDialog();
            }));
            return window.Result;
        }
    }
}
