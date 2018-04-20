using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Diagnostics;
//Namespace to make borderless form moveable
using System.Runtime.InteropServices;

namespace Lab6_1820151020
{
    public partial class processManagement : formDesign
    {
        public processManagement()
        {
            InitializeComponent();
        }
        #region GetStartingTime
        public void getStartingTime()
        {
            label3.Text = TimeProcess.getTime().ToString("h:mm:ss tt");
        }
        #endregion

        #region GetFinishingTime
        public void getFinishingTime()
        {
            label4.Text = TimeProcess.getTime().ToString("h:mm:ss tt");
        }
        #endregion

        #region GetRunTime
        public void getRunTime()
        {
            label6.Text = (TimeProcess.endTime - TimeProcess.startTime).ToString();
        }
        #endregion

        #region OnLoad
        private void processManagement_Load(object sender, EventArgs e)
        {
            label3.Text = TimeProcess.startTime.ToString();
            label4.Text = TimeProcess.endTime.ToString();
            getRunTime();
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

        private void processManagement_MouseDown(object sender, MouseEventArgs e)
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
