using System;
//Namespace to make borderless form moveable
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Lab6_1820151020
{
    public partial class formMain : formDesign
    {
        public formMain()
        {
            InitializeComponent();
        }
        #region Button Events
        private void button7_Click(object sender, EventArgs e)
        {
            taskManager form = new taskManager();
            form.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            memoryMonitor form = new memoryMonitor();
            form.Show();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Explorer form = new Explorer();
            form.Show();
        }


        private void button3_Click(object sender, EventArgs e)
        {
            imageViewer form = new imageViewer();
            form.Show();
        }
        #endregion

        #region FormMain Events
        private void formMain_Load(object sender, EventArgs e)
        {
            TimeProcess.startTime = TimeProcess.getTime();
        }

        private void formMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            TimeProcess.endTime = TimeProcess.getTime();
            processManagement m = new processManagement();
            m.ShowDialog();
        }
        #endregion

        #region Moveable
        //To make the borderless form moveable
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd,
                         int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private void formMain_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        #endregion
    }
}
