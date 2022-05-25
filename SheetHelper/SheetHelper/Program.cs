using System;
using System.Windows.Forms;

namespace SheetHelper
{
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 form = new Form1();
            Debug.InitWith(form.debugTextBox);
            DataManager.LoadSetting();
            Application.Run(form);
        }
    }
}
