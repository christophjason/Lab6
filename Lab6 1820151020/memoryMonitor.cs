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
using System.Threading;

//Namespace for speech
using System.Speech.Synthesis;

//Namespace to make borderless form moveable
using System.Runtime.InteropServices;

namespace Lab6_1820151020
{
    public partial class memoryMonitor : formDesign
    {
        public memoryMonitor()
        {
            InitializeComponent();
        }
        
        void GetMemory()
        {
            SpeechSynthesizer synth = new SpeechSynthesizer();
            //Current CPU Load in percentage
            PerformanceCounter perfCpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");

            //Current available memory in megabytes
            PerformanceCounter perfMemCounter = new PerformanceCounter("Memory", "Available MBytes");

            //System Uptime in seconds
            PerformanceCounter perfUptimeCounter = new PerformanceCounter("System", "System Up Time");

            TimeSpan uptimeSpan = TimeSpan.FromSeconds(perfUptimeCounter.NextValue());
            string systemUptime = string.Format("{0} days, {1} hours, {2} minutes, {3} seconds",
                uptimeSpan.TotalDays,
                uptimeSpan.TotalHours,
                uptimeSpan.TotalMinutes,
                uptimeSpan.TotalSeconds
                );
            float currentCpuPercentage = perfCpuCounter.NextValue();
            float currentAvailableMemory = perfMemCounter.NextValue();

            label1.Text = currentCpuPercentage + "%";
            label2.Text = currentAvailableMemory + "MB";
            label4.Text = perfUptimeCounter.NextValue() + "";

                if (currentCpuPercentage > 80)
                {
                    if(currentCpuPercentage == 100)
                    {
                        string cpuLoadVocalMessage = String.Format("WARNING: cpu load is at maximum!");
                        synth.Speak(cpuLoadVocalMessage);
                    }
                    else
                    {
                        string cpuLoadVocalMessage = String.Format("The Current Cpu Load is {0} percent", currentCpuPercentage);
                        synth.Speak(cpuLoadVocalMessage);
                    }
                }
        }

        private void memoryMonitor_Load(object sender, EventArgs e)
        {
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            GetMemory();
        }
        #region Moveable
        //To make the borderless form moveable
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd,
                         int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private void memoryMonitor_MouseDown(object sender, MouseEventArgs e)
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
