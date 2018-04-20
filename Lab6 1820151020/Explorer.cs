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
//Namespace for Hashtable
using System.Collections;
//Namespace for File System Object
using IWshRuntimeLibrary;
//Namespace for Drive Information
using System.IO;
//Namespace to make borderless form moveable
using System.Runtime.InteropServices;
namespace Lab6_1820151020
{
    public partial class Explorer : Form
    {
        #region Declaration of Variables
        private FileSystemObject fso = new FileSystemObject();
        private DriveInfo[] allDrives = DriveInfo.GetDrives();
        private Hashtable hFileExt = new Hashtable();
        private int foldercount;
        private int filecount;
        private long filesize;
        private int index = 1;
        public string path;
        #endregion


        #region Image Explorer Setup
        public Explorer()
        {
            InitializeComponent();
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            foreach (DriveInfo di in allDrives)
            {
                if (di.IsReady)
                {
                    comboBox1.Items.Add(di.ToString());
                }
            }
            comboBox1.Text = path;
            comboBox1.SelectedIndex = 1;
        }
        #endregion

        #region ControlBar
        private void button2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region BackButton
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            int poz = path.LastIndexOf('\\');
            if (poz > 2)
            {
                path = path.Substring(0, poz);
                comboBox1.Text = path;
                ScanFolder(1, path);
                label9.Text = string.Format("Folders:{0}, Files:{1}, Size:{2}", foldercount, filecount,
                  SizeFilesToString(filesize));
            }
        }
        #endregion

        #region ComboBox
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            path = comboBox1.Text;
            ScanFolder(1, path);
            listView1.Focus();
            label9.Text = string.Format("Folders:{0}, Files:{1}, Size:{2}", foldercount, filecount,
              SizeFilesToString(filesize));
        }
        #endregion

        #region ListView
        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            int i = listView1.SelectedIndices[0];
            if (i < foldercount)
            {
                path = path + "\\" + listView1.SelectedItems[0].Text;
                comboBox1.Text = path;
                ScanFolder(1, path);
                label9.Text = string.Format("Folders:{0}, Files:{1}, Size:{2}", foldercount, filecount,
                  SizeFilesToString(filesize));
            }
            else
            {
                try
                {
                    Process proc = new Process();
                    proc.EnableRaisingEvents = false;
                    proc.StartInfo.FileName = path + "\\" + listView1.SelectedItems[0].Text;
                    proc.Start();
                    proc.Close();
                }
                catch
                {
                    MessageBox.Show("No application associated with this type file", "Error type file");
                }
                
            }
        }
        #endregion

        #region ContextMenuStrip on ListView Right Click
        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (listView1.SelectedIndices.Count == 0)
            {
                contextMenuStrip1.Items[0].Enabled = false; //Open
                contextMenuStrip1.Items[2].Enabled = false; //Delete
                contextMenuStrip1.Items[4].Enabled = true;  //New Folder
            }
            else if (listView1.SelectedIndices.Count == 1)
            {
                contextMenuStrip1.Items[0].Enabled = true; //Open
                contextMenuStrip1.Items[2].Enabled = true; //Delete
                contextMenuStrip1.Items[4].Enabled = true;  //New Folder
            }
            else
            {
                contextMenuStrip1.Items[0].Enabled = false;
                contextMenuStrip1.Items[2].Enabled = true;
                contextMenuStrip1.Items[4].Enabled = false;
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {

            int i = listView1.SelectedIndices[0];
            if (i < foldercount)
            {
                path = path + "\\" + listView1.SelectedItems[0].Text;
                comboBox1.Text = path;
                ScanFolder(1, path);
                label9.Text = string.Format("Folders:{0}, Files:{1}, Size:{2}", foldercount, filecount,
                  SizeFilesToString(filesize));
            }
            else
            {
                try
                {
                    Process proc = new Process();
                    proc.EnableRaisingEvents = false;
                    proc.StartInfo.FileName = path + "\\" + listView1.SelectedItems[0].Text;
                    proc.Start();
                    proc.Close();
                }
                catch
                {
                    MessageBox.Show("No application associated with this type file", "Error type file");
                }
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (NewFolder(path + "\\NewFolder"))
            {
                ScanFolder(1, path);
                label9.Text = string.Format("Folders:{0}, Files:{1}, Size:{2}", foldercount, filecount,
                  SizeFilesToString(filesize));
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            confirmDelete form = new confirmDelete();
            if (form.ShowDialog() == DialogResult.Yes)
            {
                Delete();
            }
            form.Dispose();
        }
        #endregion

        #region Functions
        public void ScanFolder(int n, string path)
        {
            int i = 0;
            Icon IC16;
            DirectoryInfo d = new DirectoryInfo(path);
            DirectoryInfo[] folders = d.GetDirectories();
            FileInfo[] files = d.GetFiles();
            try
            {
                listView1.Clear();
                listView1.Cursor = Cursors.WaitCursor;
                foldercount = 0;
                foreach (DirectoryInfo di in folders)
                {
                    if ((di.Attributes & FileAttributes.Hidden) == 0)
                    {
                        listView1.Items.Add(di.Name);
                        listView1.Items[i].ImageIndex = 0;
                        i++;
                        foldercount++;
                    }
                }
                filecount = 0;
                filesize = 0;
                Application.DoEvents();
                listView1.BeginUpdate();
                foreach (FileInfo file in files)
                {
                    if ((file.Attributes & FileAttributes.Hidden) == 0)
                    {
                        if (file.Extension == ".exe")
                        {
                            if (!hFileExt.ContainsKey(file.Name))
                            {
                                IC16 = IconImport.Import(file.FullName);
                                imageList1.Images.Add(IC16);
                                hFileExt.Add(file.Name, index);
                                index++;
                            }
                            listView1.Items.Add(file.Name);
                            listView1.Items[i].ImageIndex = (int)hFileExt[file.Name];
                        }
                        else
                        {
                            if (!hFileExt.ContainsKey(file.Extension))
                            {
                                IC16 = IconImport.Import(file.FullName);
                                imageList1.Images.Add(IC16);
                                hFileExt.Add(file.Extension, index);
                                index++;
                            }
                            listView1.Items.Add(file.Name);
                            listView1.Items[i].ImageIndex = (int)hFileExt[file.Extension];
                        }
                        listView1.Items[i].ToolTipText = SizeFilesToString(file.Length);
                        i++;
                        filecount++;
                        filesize += file.Length;
                    }
                }
                listView1.EndUpdate(); listView1.Cursor = Cursors.Default;
            }
            catch { }
        }

        //public long GetFolderSize(string path)
        //{
        //    return Convert.ToInt64(fso.GetFolder(path).Size);
        //    return 100;
        //}

        public long GetFileSize(string path)
        {
            FileInfo f = new FileInfo(path);
            return f.Length;
        }

        public string SizeFilesToString(long a)
        {
            if (a < 1024) return string.Format("{0} bytes", a);
            else if (a < 1048576) return string.Format("{0:0.00} KB", a / 1024.0);
            else if (a < 1073741824) return string.Format("{0:0.00} MB", a / 1048576.0);
            else return string.Format("{0:0.00} GB", a / 1073741824.0);
        }

        public bool NewFolder(string path)
        {
            int i = 0;
            string pth = path;
            try
            {
                while (Directory.Exists(pth))
                {
                    i++;
                    pth = path + string.Format("({0})", i);
                }
                DirectoryInfo nd = Directory.CreateDirectory(pth);
                return true;
            }
            catch { return false; }
        }

        public bool Delete()
        {
            foreach (int i in listView1.SelectedIndices)
            {
                try
                {
                    if (i < foldercount) Directory.Delete(path + "\\" + listView1.Items[i].Text, true);
                    else System.IO.File.Delete(path + "\\" + listView1.Items[i].Text);
                }
                catch { }
            }
            ScanFolder(1, path);
            label9.Text = string.Format("Folders:{0}, Files:{1}, Size:{2}", foldercount, filecount,
                SizeFilesToString(filesize));
            return true;
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

        private void Explorer_MouseDown(object sender, MouseEventArgs e)
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
