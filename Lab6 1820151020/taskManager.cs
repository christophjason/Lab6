using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;
//Namespace for Icons
using System.Drawing;
//Namespace for expandoobject
using System.Dynamic;
//Namespace to get processes
using System.Diagnostics;
//Namespace to make borderless form moveable
using System.Runtime.InteropServices;

namespace Lab6_1820151020
{
    public partial class taskManager : taskManagerDesign
    {
        public taskManager()
        {
            InitializeComponent();
        }

        Process[] proc;

        #region Form Setup
        private void taskManager_Load(object sender, EventArgs e)
        {
            GetAllProcess();
        }
        #endregion

        #region Functions
        
        void GetAllProcess()
        {
            //Get list of processes and puts it to an array
            proc = Process.GetProcesses();

            //Image List is created that will store the icons of the processes
            ImageList imglist = new ImageList();

            //Clears the List View
            listView2.Items.Clear();

            foreach (Process p in proc)
            {
                //Define the status wheter it is responding or not
                string status = (p.Responding == true ? "Responding" : "Not responding");

                dynamic details = GetDetails(p.Id);

                // An Array of string that will display 
                string[] row = {
                    p.ProcessName,
                    p.Id.ToString(),
                    status,
                    details.Username,
                    bytesConversion(p.PrivateMemorySize64),
                    details.Description
                };
                //To prevent crashes due to some process might not have an icon
                try
                {
                    //Add icon to the list
                    imglist.Images.Add(p.Id.ToString(),
                    Icon.ExtractAssociatedIcon(p.MainModule.FileName).ToBitmap());
                }
                catch { }
                // Create a new Item to add into the list view that expects the row of information as first argument
                ListViewItem item = new ListViewItem(row)
                {
                    // Set the ImageIndex of the item as the same defined in the previous try-catch
                    ImageIndex = imglist.Images.IndexOfKey(p.Id.ToString())
                };

                // Add the Item
                listView2.Items.Add(item);
            }
            listView2.SmallImageList = imglist;
        }


        public string bytesConversion(long number)
        {
            List<string> units = new List<string> { " B", " KB", " MB", " GB", " TB", " PB" };

            for (int i = 0; i < units.Count; i++)
            {
                long temp = number / (int)Math.Pow(1024, i + 1);

                if (temp == 0)
                {
                    return (number / (int)Math.Pow(1024, i)) + units[i];
                }
            }
            return number.ToString();
        }

        //ExpandoObject allows it so it can be dynamically added and removed at run time
        public ExpandoObject GetDetails(int processId)
        {
            // Query the Processes
            string sql = "Select * From Win32_Process Where ProcessID = " + processId;
            ManagementObjectSearcher search = new ManagementObjectSearcher(sql);
            ManagementObjectCollection proc = search.Get();

            // Create a dynamic object to store some properties on it
            dynamic response = new ExpandoObject();
            response.Description = "";
            response.Username = "Unknown";

            foreach (ManagementObject obj in proc)
            {
                // Retrieve username 
                string[] argList = new string[] { string.Empty, string.Empty };
                int returnVal = Convert.ToInt32(obj.InvokeMethod("GetOwner", argList));
                if (returnVal == 0)
                {
                    // return system's username
                    response.Username = argList[0];

                }

                // Retrieve process description if exists
                if (obj["ExecutablePath"] != null)
                {
                    try
                    {
                        FileVersionInfo info = FileVersionInfo.GetVersionInfo(obj["ExecutablePath"].ToString());
                        response.Description = info.FileDescription;
                    }
                    catch { }
                }
            }
            return response;
        }
        #endregion

        #region EndTask
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                int index = 0;
                if (listView2.SelectedItems.Count > 0)
                {
                    var selItems = listView2.SelectedItems;
                    foreach (ListViewItem selectedItem in selItems)
                    {
                        index = selectedItem.Index;
                    }
                }

                proc[index].Kill();
                GetAllProcess();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region RunNewTask
        private void runNewTaskToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (formRunNewTask form = new formRunNewTask())
            {
                if (form.ShowDialog() == DialogResult.OK)
                    GetAllProcess();
            }
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

        private void taskManager_MouseDown(object sender, MouseEventArgs e)
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
