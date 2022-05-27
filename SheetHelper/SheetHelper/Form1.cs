using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SheetHelper
{
    public partial class Form1 : Form
    {
        private const int HT_CAPTION = 0x2;
        private const int WM_NCLBUTTONDOWN = 0xA1;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        public Form1()
        {
            InitializeComponent();
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void topPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void buttonSelectFolder_Click(object sender, EventArgs e)
        {
            DataManager.OpenFolderPicker();
        }

        private void panel2_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void panel2_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            DataManager.AddFiles(files.ToList());
        }

        private void buttonMapFiles_Click(object sender, EventArgs e)
        {
            DataManager.Test();
            //DataManager.MapAllFiles();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }
    }
}