using System;
using System.Windows.Forms;

namespace SheetHelper
{
    internal static class Debug
    {
        private static TextBox text;

        public static void InitWith(TextBox boxForDebug)
        {
            text = boxForDebug;
        }

        public static void Log(object obj)
        {
            text.AppendText("\r\n" + obj);
        }

        public static void Log(string str)
        {
            text.AppendText("\r\n" + str);
        }
    }
}
