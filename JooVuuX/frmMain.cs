using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using Vlc.DotNet.Core.Medias;
using Vlc.DotNet.Core;
using Vlc.DotNet.Forms;

namespace JooVuuX
{
    public partial class frmMain : Form
    {
        mIniFile ini;
        public static String mPath = AppDomain.CurrentDomain.BaseDirectory;
        public static String FileStorePath = "C:\\Temp\\";

        public static List<String> list_str = new List<string>();
        //List<String> ip_list = new List<string>();
        private bool form_active = true;

        //Moving form
        private bool _dragging = false;
        private Point _start_point = new Point(0, 0);
        private FormWindowState form_state = FormWindowState.Normal;

        //ShowForms
        public static int MainWindow_Width = 0;
        public static int MainWindow_Height = 0;
        public static int MainWindow_Top = 0;
        public static int MainWindow_Left = 0;

        public static bool minButtonSelect = false;
        public static bool maxButtonSelect = false;
        public static bool hidButtonSelect = false;

        byte[] bytes = new byte[1024];
        public Socket mySocket;
        public string token;
        public string msg_id;
        public bool isConnection = false;
        public bool isRecord = false;
        public bool isStream = false;

        public Control_ViewTable currUserControl = null;
        public string Select_ID_Files = "0";
        public string Select_Name_Files = "";

        //selected wifi connnection or settings
        public bool isSettings = false;

        bool isFillFilesList = false;
        public string file_path = "/tmp/fuse_d/DCIM/100MEDIA/";

        private static String response = String.Empty;
        private static ManualResetEvent receiveDone = new ManualResetEvent(false);

        public List<Ls_Structure> files_List = new List<Ls_Structure>();
        string selected_ip = "";
        string myIP = "";

        public frmMain()
        {
            InitializeComponent();

            this.FormBorderStyle = FormBorderStyle.None;
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
        }
        
        public static void CreateIni(mIniFile ini)
        {
            ini.IniWriteValue("System", "FileStorePath", FileStorePath);

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Load ini
            ini = new mIniFile(frmMain.mPath + "JooVuuX.ini");
            if (!File.Exists(mPath + "JooVuuX.ini"))
            {
                CreateIni(ini);
            }
            FileStorePath = ini.IniReadValue("System", "FileStorePath");

            //this.FormBorderStyle = FormBorderStyle.None;
            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            control_ViewFiles.SelectEvent += new EventHandler(control_Files_SelectEvent);
            control_ViewFiles.DoubleClickEvent += new EventHandler(control_Files_DoubleClick);
            control_ViewFiles.DoubleClick += new EventHandler(control_Files_DoubleClick);
            
            rightTabCtrl.Appearance = TabAppearance.FlatButtons;
            rightTabCtrl.ItemSize = new Size(0, 1);
            rightTabCtrl.SizeMode = TabSizeMode.Fixed;

            leftTabCtrl.Appearance = TabAppearance.FlatButtons;
            leftTabCtrl.ItemSize = new Size(0, 1);
            leftTabCtrl.SizeMode = TabSizeMode.Fixed;

            myIP = GetMyIP();
            string[] diappazon = myIP.Split('.');
           
        }

        private void Form1_Closing(object sender, EventArgs e)
        {
                                
        }

        protected void control_Files_SelectEvent(object sender, EventArgs e)
        {
            if (control_ViewFiles.selectElement.ID == "0")
            {
                btnGalleryDelete.Enabled = false;
            }
            else
            {
                Select_ID_Files = control_ViewFiles.selectElement.ID;
                Select_Name_Files = control_ViewFiles.Select_Detail_Name;
                label1.Text = Select_Name_Files;
                btnGalleryDelete.Enabled = true;
                btnGalleryPlay.Text = "Play";
                vlcControl.Stop();
                //FillOsnastkaList(); /tmp/fuse_d/DCIM/100MEDIA
            }

        }

        private void control_Files_DoubleClick(object sender, EventArgs e)
        {
            //currUserControl = (Control_ViewTable)sender;
            //string s = Select_Name_Files.Trim(' ');
            //s = s.Trim('/');
            //if (file_path != "/") file_path = file_path + "/" + s;
            //else file_path = file_path + s;
            //textBox1.Text = file_path;
            //AmbaCD(file_path);
            //FillFilesList("");
            //            axVLCPlugin21.playlist.stop();
            vlcControl.Stop();
            btnGalleryPlay.Text = "Play";
            ShowImage("", false);
        }

        private void FillFilesList(string path)
        {
            if (!isFillFilesList)
            {
                control_ViewFiles.ID_Field_Name = "id";
                control_ViewFiles.tableStructure.Clear();
                control_ViewFiles.tableStructure.Add(new Control_ViewTable.TableStructure("path", "string", "File Name", 150, HorizontalAlignment.Left, 0));
                control_ViewFiles.tableStructure.Add(new Control_ViewTable.TableStructure("date", "string", "Date", 130, HorizontalAlignment.Left, 0));
                control_ViewFiles.createStructure();
                isFillFilesList = true;
            }

            files_List.Clear();
            dynamic mData = AmbaLs(path);

            try
            {
                JObject mJson = JObject.Parse(mData.ToString());
                IList<JToken> results = mJson["listing"].Children().ToList();
                // serialize JSON results into .NET objects
                IList<Ls_Structure> LSResults = new List<Ls_Structure>();
                int id = 1;
                foreach (JToken result in results)
                {
                    string s = result.ToString();
                    s = s.Trim('{');
                    s = s.Trim('}');
                    s = s.Replace(": ", ";");
                    s = s.Replace("\r\n", " ");
                    s = s.Replace("\"", " ");
                    s = s.Trim(' ');
                    string[] lines = s.Split(';');
                    if (lines.Length > 1)
                    {
                        files_List.Add(new Ls_Structure(id.ToString(), lines[0], lines[1]));
                    }

                    id++;
                }
                control_ViewFiles.FillDataList(files_List, "");
            }
            catch (Exception) { }

        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {
            Panel p = (Panel)sender;
            Pen mPen = new Pen(Color.WhiteSmoke, 2);
            int x = 13;
            int y = 4;
            e.Graphics.DrawLine(mPen, new Point(x, y), new Point(x + 8, y + 8));
            e.Graphics.DrawLine(mPen, new Point(x, y + 8), new Point(x + 8, y));
        }

        private void panel5_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void panel5_MouseEnter(object sender, EventArgs e)
        {
            Panel p = (Panel)sender;
            p.BackColor = Color.FromArgb(224, 67, 67); //224
        }

        private void panel5_MouseLeave(object sender, EventArgs e)
        {
            Panel p = (Panel)sender;
            p.BackColor = Color.IndianRed;
        }

        private void panel3_Click(object sender, EventArgs e)
        {
            if (form_state == FormWindowState.Normal)
            {
                form_state = FormWindowState.Maximized;
                WindowState = FormWindowState.Maximized;
                //this.Bounds = Screen.PrimaryScreen.WorkingArea;
            }
            else
            {
                form_state = FormWindowState.Normal;
                WindowState = FormWindowState.Normal;
            }
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {
            Panel p = (Panel)sender;
            Pen mPen = new Pen(Color.Black, 1);
            if (maxButtonSelect == false)
            {
                mPen = new Pen(Color.Black, 1);
            }
            else
            {
                mPen = new Pen(Color.WhiteSmoke, 1);
            }
            int x = 8;
            int y = 5;
            e.Graphics.DrawLine(mPen, new Point(x, y), new Point(x + 8, y));
            e.Graphics.DrawLine(mPen, new Point(x, y + 1), new Point(x + 8, y + 1));
            e.Graphics.DrawLine(mPen, new Point(x + 8, y + 1), new Point(x + 8, y + 6));
            e.Graphics.DrawLine(mPen, new Point(x + 8, y + 6), new Point(x, y + 6));
            e.Graphics.DrawLine(mPen, new Point(x, y + 6), new Point(x, y + 1));
        }

        private void panel3_MouseEnter(object sender, EventArgs e)
        {
            maxButtonSelect = true;
            Panel p = (Panel)sender;
            p.BackColor = Color.SteelBlue;
        }

        private void panel3_MouseLeave(object sender, EventArgs e)
        {
            maxButtonSelect = false;
            Panel p = (Panel)sender;
            p.BackColor = Color.WhiteSmoke;
        }

        private void panel4_Click(object sender, EventArgs e)
        {
            form_state = FormWindowState.Minimized;
            WindowState = FormWindowState.Minimized;
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {
            Panel p = (Panel)sender;
            Pen mPen = new Pen(Color.Black, 1);
            if (minButtonSelect == false)
            {
                mPen = new Pen(Color.Black, 1);
            }
            else
            {
                mPen = new Pen(Color.WhiteSmoke, 1);
            }
            int x = 8;
            int y = 5;
            e.Graphics.DrawLine(mPen, new Point(x + 8, y + 5), new Point(x, y + 5));
            e.Graphics.DrawLine(mPen, new Point(x + 8, y + 6), new Point(x, y + 6));
        }

        private void panel4_MouseEnter(object sender, EventArgs e)
        {
            minButtonSelect = true;
            Panel p = (Panel)sender;
            p.BackColor = Color.SteelBlue;
        }

        private void panel4_MouseLeave(object sender, EventArgs e)
        {
            minButtonSelect = false;
            Panel p = (Panel)sender;
            p.BackColor = Color.WhiteSmoke;
        }

        private void panel2_DoubleClick(object sender, EventArgs e)
        {
            if (form_state == FormWindowState.Normal)
            {
                form_state = FormWindowState.Maximized;
                WindowState = FormWindowState.Maximized;
                //this.Bounds = Screen.PrimaryScreen.WorkingArea;
            }
            else
            {
                form_state = FormWindowState.Normal;
                WindowState = FormWindowState.Normal;
            }
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            Panel p = (Panel)sender;
            string title = "JooVuuX";
            FontFamily family = new FontFamily("Verdana");
            Font font = new Font(family, 10.0f, FontStyle.Regular);

            Size size = TextRenderer.MeasureText(e.Graphics, title, font);
            int LeftX = (int)(p.Width / 2) - (int)(size.Width / 2);

            //if (form_active) p.BackColor = Color.WhiteSmoke;
            //else p.BackColor = Color.LightGray;
            if (form_active) e.Graphics.DrawString(title, font, SystemBrushes.GrayText, LeftX, 6);
            else e.Graphics.DrawString(title, font, SystemBrushes.ControlDark, LeftX, 6);
        }

        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            _dragging = true;  // _dragging is your variable flag
            _start_point = new Point(e.X, e.Y);
        }

        private void panel2_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging)
            {
                Point p = PointToScreen(e.Location);
                Location = new Point(p.X - this._start_point.X, p.Y - this._start_point.Y);

                MainWindow_Left = Location.X;
                MainWindow_Top = Location.Y;
            }
        }

        private void panel2_MouseUp(object sender, MouseEventArgs e)
        {
            _dragging = false;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            form_state = WindowState;
        }

        private void Form1_Shown(object sender, EventArgs e)
        {           
            //leftTabCtrl.SelectedTab = tabPage6;
            //show tabs
            if (isSettings == true)
            {
                toolTabCtrl.TabPages.Remove(tabPageAction);
                toolTabCtrl.SelectedTab = tabPageSettings;
                btnDownload.Visible = true;
                enableToolButtons(true);
                btnSettingsMain_Click(sender, e);
            }
            else
            {
                toolTabCtrl.SelectedTab = tabPageAction;
                btnDownload.Visible = false;
                enableToolButtons(false);
                leftTabCtrl.SelectedTab = tabPageConnect;
            }
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            form_active = true;
            panel2.Refresh();
        }

        private void Form1_Deactivate(object sender, EventArgs e)
        {
            form_active = false;
            panel2.Refresh();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            MainWindow_Width = Width;
            MainWindow_Height = Height;

            MainWindow_Top = Location.Y;
            MainWindow_Left = Location.X;

            //progressPanel1.Left = (int)(Width / 2) - (int)(progressPanel1.Width / 2);
            //progressPanel1.Top = (int)(Height / 2) - (int)(progressPanel1.Height / 2);
        }


        //***********************************************************
        //This gives us the ability to resize the borderless from any borders instead of just the lower right corner
        protected override void WndProc(ref Message m)
        {
            const int wmNcHitTest = 0x84;
            const int htLeft = 10;
            const int htRight = 11;
            const int htTop = 12;
            const int htTopLeft = 13;
            const int htTopRight = 14;
            const int htBottom = 15;
            const int htBottomLeft = 16;
            const int htBottomRight = 17;

            if (m.Msg == wmNcHitTest)
            {
                int x = (int)(m.LParam.ToInt64() & 0xFFFF);
                int y = (int)((m.LParam.ToInt64() & 0xFFFF0000) >> 16);
                Point pt = PointToClient(new Point(x, y));
                Size clientSize = ClientSize;
                ///allow resize on the lower right corner
                if (pt.X >= clientSize.Width - 16 && pt.Y >= clientSize.Height - 16 && clientSize.Height >= 16)
                {
                    m.Result = (IntPtr)(IsMirrored ? htBottomLeft : htBottomRight);
                    return;
                }
                ///allow resize on the lower left corner
                if (pt.X <= 16 && pt.Y >= clientSize.Height - 16 && clientSize.Height >= 16)
                {
                    m.Result = (IntPtr)(IsMirrored ? htBottomRight : htBottomLeft);
                    return;
                }
                ///allow resize on the upper right corner
                if (pt.X <= 16 && pt.Y <= 16 && clientSize.Height >= 16)
                {
                    m.Result = (IntPtr)(IsMirrored ? htTopRight : htTopLeft);
                    return;
                }
                ///allow resize on the upper left corner
                if (pt.X >= clientSize.Width - 16 && pt.Y <= 16 && clientSize.Height >= 16)
                {
                    m.Result = (IntPtr)(IsMirrored ? htTopLeft : htTopRight);
                    return;
                }
                ///allow resize on the top border
                if (pt.Y <= 16 && clientSize.Height >= 16)
                {
                    m.Result = (IntPtr)(htTop);
                    return;
                }
                ///allow resize on the bottom border
                if (pt.Y >= clientSize.Height - 16 && clientSize.Height >= 16)
                {
                    m.Result = (IntPtr)(htBottom);
                    return;
                }
                ///allow resize on the left border
                if (pt.X <= 16 && clientSize.Height >= 16)
                {
                    m.Result = (IntPtr)(htLeft);
                    return;
                }
                ///allow resize on the right border
                if (pt.X >= clientSize.Width - 16 && clientSize.Height >= 16)
                {
                    m.Result = (IntPtr)(htRight);
                    return;
                }
            }
            base.WndProc(ref m);
        }
        //***********************************************************
        //***********************************************************
        //This gives us the ability to drag the borderless form to a new location
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private void YOURCONTROL_MouseDown(object sender, MouseEventArgs e)
        {
            //ctrl-leftclick anywhere on the control to drag the form to a new location 
            if (e.Button == MouseButtons.Left && Control.ModifierKeys == Keys.Control)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        //***********************************************************
        //***********************************************************
        //This gives us the drop shadow behind the borderless form
        private const int CS_DROPSHADOW = 0x20000;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                //cp.ExStyle |= 0x00080000; //transparent
                return cp;
            }
        }

        //***********************************************************

        private void btnGallery_Click(object sender, EventArgs e)
        {
            if (isConnection == false)
            {
                DialogResult result = MessageBox.Show("Camera is not connected.", "Important Note",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button1);
                return;
            }
            leftTabCtrl.SelectedTab = tabPageFilelist;
//            String test_data = @"{
//              'rval': 0,
//              'msg_id': 1282,
//              'listing': [
//                {
//                  'bin/': '2015-10-15 18:49:38'
//                },
//                {
//                  'dev/': '1970-01-01 00:01:17'
//                },
//                {
//                  'etc/': '2015-10-15 18:49:40'
//                },
//                {
//                  'lib/': '2015-10-15 18:49:40'
//                },
//                {
//                  'mnt/': '2015-03-03 13:38:30'
//                },
//                {
//                  'oem/': '2015-03-03 13:54:06'
//                },
//                {
//                  'opt/': '2015-03-03 13:38:30'
//                },
//                {
//                  'tmp/': '1970-01-01 00:05:36'
//                },
//                {
//                  'sys/': '1970-01-01 00:00:10'
//                },
//                {
//                  'var/': '2015-10-15 18:49:41'
//                },
//                {
//                  'usr/': '2015-03-03 13:54:04'
//                },
//                {
//                  'home/': '2015-03-03 13:54:17'
//                },
//                {
//                  'pref/': '2015-01-01 17:03:32'
//                },
//                {
//                  'proc/': '1970-01-01 00:00:00'
//                },
//                {
//                  'sbin/': '2015-10-15 18:49:38'
//                },
//                {
//                  'root/': '2015-03-03 13:38:30'
//                },
//                {
//                  'linuxrc': '2015-10-15 18:49:38'
//                }
//              ]
//            }";

            //files_List.Clear();
            //files_List.Add(new Ls_Structure("test", "20015"));
            //FillFilesList();

            //file_path = "/";
            //AmbaCD("/");
            AmbaCD(file_path);
            FillFilesList("");
        }

        //Connect
        public void Amba_Connect()
        {
            byte[] bytes = new byte[1024];
            if (selected_ip.Length == 0)
            {
                selected_ip = "192.168.42.1";
            }
            IPEndPoint EndPoint = new IPEndPoint(IPAddress.Parse(selected_ip), 7878);
            mySocket = new Socket(EndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try {
                mySocket.Connect(EndPoint);
            }
            catch(Exception)
            {
                DialogResult result1 = MessageBox.Show("Connection failed! Do you want to retry?", "Important Note",
                            MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                if (result1 == DialogResult.Retry)
                {
                    try
                    {
                        mySocket.Connect(EndPoint);
                    }
                    catch (Exception)
                    {
                        DialogResult result2 = MessageBox.Show("Connection failed! Do you want to retry?", "Important Note",
                            MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning,MessageBoxDefaultButton.Button1);
                        if (result2 == DialogResult.Retry)
                        {
                            try
                            {
                                mySocket.Connect(EndPoint);
                            }
                            catch (Exception)
                            {
                                DialogResult result3 = MessageBox.Show("Connection failed!", "Important Note",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                                return;
                            }
                        }
                        else
                            return;
                    }
                }
                else
                    return;
                
            }
            //mySocket = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            //mySocket.Connect(ipEndPoint);

            Byte[] SendBytes = Encoding.Default.GetBytes("{\"token\":0,\"msg_id\":257}");
            mySocket.Send(SendBytes);

            // Получаем ответ от сервера           
            int bytesRec = mySocket.Receive(bytes);
            string responce = Encoding.UTF8.GetString(bytes, 0, bytesRec);
            //textBox1.Text = responce;

            dynamic mData = JsonConvert.DeserializeObject(responce);
            msg_id = mData.msg_id;
            if (msg_id == "257")
            {
                token = mData.param;
                btnConnect.Image = ((System.Drawing.Image)(Properties.Resources.ic_settings_power_24_1));
                btnConnect.Text = "Disconnect";
                isConnection = true;               
                btnDownload.Enabled = true;              

                AmbaCD(file_path);
                FillFilesList("");
            }
            else
            {
                token = "0";
                btnConnect.Image = ((System.Drawing.Image)(Properties.Resources.ic_settings_power_24_gray));
                btnConnect.Text = "Connect";
                isConnection = false;               
                btnDownload.Enabled = false;               
            }

            enableToolButtons(isConnection);
        }

        //Amba_Ls
        public dynamic AmbaLs(string path)
        {
            byte[] bytes = new byte[2024];
            Amba_Ls amba_ls = new Amba_Ls();
            amba_ls.token = Convert.ToInt32(token);
            amba_ls.msg_id = 1282;
            amba_ls.param = path;
            //amba_ls.param = "/tmp/use_d/DCIM/100MEDIA/";
            string output = JsonConvert.SerializeObject(amba_ls);
            if (output == "") return "";
            Byte[] SendBytes = Encoding.Default.GetBytes(output);
            mySocket.Send(SendBytes);

            // Получаем ответ от сервера
            string responce = "";
            int bytesRec = mySocket.Receive(bytes);
            responce = responce + Encoding.UTF8.GetString(bytes, 0, bytesRec);
            Thread.Sleep(100);
            while (mySocket.Available > 0)
            {
                bytesRec = mySocket.Receive(bytes);
                responce = responce + Encoding.UTF8.GetString(bytes, 0, bytesRec);
                Thread.Sleep(100);
            }


           // int bytesRec = mySocket.Receive(bytes);
           // string responce = Encoding.UTF8.GetString(bytes, 0, bytesRec);
            //Проверяем ответ
            dynamic mData = JsonConvert.DeserializeObject(responce);
            msg_id = mData.msg_id;
            if (msg_id != "1282") return "";  //Error
            return mData;


        }

        //Amba_CD
        public dynamic AmbaCD(string path)
        {
            Amba_Cd amba_cd = new Amba_Cd();
            amba_cd.token = Convert.ToInt32(token);
            amba_cd.msg_id = 1283;
            amba_cd.param = path;
            //amba_ls.param = "/tmp/use_d/DCIM/100MEDIA/";
            string output = JsonConvert.SerializeObject(amba_cd);
            Byte[] SendBytes = Encoding.Default.GetBytes(output);
            mySocket.Send(SendBytes);

            // Получаем ответ от сервера
            byte[] bytes = new byte[1024];
            int bytesRec = mySocket.Receive(bytes);
            string responce = Encoding.UTF8.GetString(bytes, 0, bytesRec);
            //textBox1.Text = responce;

            //Проверяем ответ
            dynamic mData = JsonConvert.DeserializeObject(responce);
            msg_id = mData.msg_id;
            if (msg_id != "1283") return "";  //Error
            return mData;
        }

        public void Amba_DisConnect()
        {
            byte[] bytes = new byte[1024];

            Amba_Stop_Session amba_stop_session = new Amba_Stop_Session();
            amba_stop_session.token = Convert.ToInt32(token);
            amba_stop_session.msg_id = 258;
            string output = JsonConvert.SerializeObject(amba_stop_session);
            Byte[] SendBytes = Encoding.Default.GetBytes(output);
            mySocket.Send(SendBytes);

            // Получаем ответ от сервера
            int bytesRec = mySocket.Receive(bytes);
            string responce = Encoding.UTF8.GetString(bytes, 0, bytesRec);
            //textBox1.Text = responce;

            // Освобождаем сокет
            mySocket.Close();

            token = "0";
            btnConnect.Image = ((System.Drawing.Image)(Properties.Resources.ic_settings_power_24_gray));
            btnConnect.Text = "Connect";
            isConnection = false;
           
            enableToolButtons(false);             
        }

        public dynamic Amba_GetFile(string fileName)
        {

            Amba_Get_File amba_getFile = new Amba_Get_File();
            amba_getFile.token = Convert.ToInt32(token);
            amba_getFile.msg_id = 1285;
            amba_getFile.param = fileName;
            amba_getFile.offset = 0;
            amba_getFile.fetch_size = 0;
            //amba_ls.param = "/tmp/use_d/DCIM/100MEDIA/";
            string output = JsonConvert.SerializeObject(amba_getFile);
            Byte[] SendBytes = Encoding.Default.GetBytes(output);
            mySocket.Send(SendBytes);

            // Получаем ответ от сервера
            byte[] bytes = new byte[1024];
            int bytesRec = mySocket.Receive(bytes);
            string responce = Encoding.UTF8.GetString(bytes, 0, bytesRec);
            //textBox1.Text = responce;

            //Проверяем ответ
            dynamic mData = JsonConvert.DeserializeObject(responce);
            msg_id = mData.msg_id;
            if (msg_id != "1285") return "";  //Error
            return mData;
        }

        

        private void btnConnect_Click(object sender, EventArgs e)
        {
            leftTabCtrl.SelectedTab = tabPageConnect;            
            //if (isConnection == false) Amba_Connect();
            //else Amba_DisConnect();
        }

        private void btnToolShow_Click(object sender, EventArgs e)
        {
            ShowImage("", false);

        }

        private void ShowImage(string fName, bool only_download)
        {
            string resp = Send260();
            if (resp == "") return;

            string ext = Path.GetExtension(Select_Name_Files.Trim());
            if (ext == ".MOV" && only_download==false)
            {
                string fRest = Path.GetFileNameWithoutExtension(Select_Name_Files.Trim());
                if (fRest.Length > 4)
                {
                    string test_thm = fRest.Substring(fRest.Length - 3, 3);
                   // if (test_thm == "thm")
                    {
                        rightTabCtrl.SelectedTab = tabPage5;

                        
                        string s = "rtsp://" + selected_ip + "/tmp/fuse_d/DCIM/100MEDIA/" + Select_Name_Files.Trim();
                        LocationMedia media = new LocationMedia(s);
                        vlcControl.Stop();
                        vlcControl.Media = media;
                        vlcControl.Play();
                        
                        textBox1.Text = s;
                        return;
                    }
                    
                }
            }


            rightTabCtrl.SelectedTab = tabPage4;
            string sName = "";
            dynamic mData;
            if (fName == "")
            {
                mData = Amba_GetFile(file_path + Select_Name_Files.Trim());
                sName = Select_Name_Files.Trim();
            }
            else
            {
                mData = Amba_GetFile(fName.Trim());
                sName = fName.Trim();
            }
            try
            {
                if (mData.length==0) return;
                
                if (mData.rval < 0)
                {
                    return;
                }
            }
            catch (Exception)
            {
                return;
            }
            ulong size_File = mData.size;

            if (ext == ".jpg") //JPG===================================
            {
                //Создаем сокет, коннектимся
                //IPEndPoint EndPoint = new IPEndPoint(IPAddress.Parse("192.168.42.1"), 8787);
                IPEndPoint EndPoint = new IPEndPoint(IPAddress.Parse(selected_ip), 8787);
                Socket File_Socket = new Socket(EndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                File_Socket.Connect(EndPoint);


                // Получаем ответ от сервера
                //byte[] bytes_file = new byte[size_File];
                //int bytesRec_file = File_Socket.Receive(bytes_file);
                //File_Socket.Close();
                if (!Directory.Exists("C:\\temp"))
                {
                    Directory.CreateDirectory("C:\\temp");
                }
                String temp_url = "C:\\temp\\" + sName;
                if (File.Exists(temp_url))
                {
                    File.Delete(temp_url);
                }
                byte[] buffer_1 = new byte[1024]; //Буфер для файла
                FileStream stream = new FileStream("C:\\temp\\" + sName, FileMode.CreateNew, FileAccess.Write);
                BinaryWriter f = new BinaryWriter(stream);
                ulong processed = 0; //Байт принято
                while (processed < size_File) //Принимаем файл
                {
                    if ((size_File - processed) < 1024)
                    {
                        int bytes_f = (int)(size_File - processed);
                        byte[] buf = new byte[bytes_f];
                        bytes_f = File_Socket.Receive(buf);
                        f.Write(buf, 0, bytes_f);
                        processed = processed + (ulong)bytes_f;
                    }
                    else
                    {
                        int bytes_f = File_Socket.Receive(buffer_1);
                        f.Write(buffer_1, 0, bytes_f);
                        processed = processed + (ulong)bytes_f;
                    }
                }
                if (f != null)
                {
                    f.Close();
                }
                stream.Close();


                rightTabCtrl.SelectedTab = tabPage4;
                // Получаем ответ от сервера
                byte[] bytes = new byte[1024];
                int bytesRec = mySocket.Receive(bytes);
                string responce = Encoding.UTF8.GetString(bytes, 0, bytesRec);

                Image img = new Bitmap(@"C:\temp\" + sName);
                pictureBox1.Image = new Bitmap(img);
                img.Dispose();
                File_Socket.Close();
            }  //END JPG===================================
            else if (ext == ".MOV")
            {
                string fRest = Path.GetFileNameWithoutExtension(sName);
                if (fRest.Length > 4)
                {
                    string test_thm = fRest.Substring(fRest.Length - 3, 3);
                    if (test_thm == "thm")
                    {
                    //    tabControl1.SelectedTab = tabPage5;

                    //    Stream_Out_Type stream_out_type = new Stream_Out_Type();
                    //    stream_out_type.token = Convert.ToInt32(token);
                    //    stream_out_type.msg_id = 2;
                    //    stream_out_type.type = "save_low_resolution_clip";
                    //    stream_out_type.param = "off";
                    //    string output = JsonConvert.SerializeObject(stream_out_type);
                    //    Byte[] SendBytes = Encoding.Default.GetBytes(output);
                    //    mySocket.Send(SendBytes);

                    //    // Получаем ответ от сервера
                    //    int bytesRec = mySocket.Receive(bytes);
                    //    string responce = Encoding.UTF8.GetString(bytes, 0, bytesRec);
                    //    textBox1.Text = responce;

                    //    //Проверяем ответ
                    //    mData = JsonConvert.DeserializeObject(responce);
                    //    msg_id = mData.msg_id;
                    //    if (msg_id != "2") return;  //Error

                    //    //---------------
                    //    stream_out_type = new Stream_Out_Type();
                    //    stream_out_type = new Stream_Out_Type();
                    //    stream_out_type.token = Convert.ToInt32(token);
                    //    stream_out_type.msg_id = 2;
                    //    stream_out_type.type = "stream_out_type";
                    //    stream_out_type.param = "rtsp";
                    //    output = JsonConvert.SerializeObject(stream_out_type);
                    //    SendBytes = Encoding.Default.GetBytes(output);
                    //    output = JsonConvert.SerializeObject(stream_out_type);
                    //    SendBytes = Encoding.Default.GetBytes(output);
                    //    mySocket.Send(SendBytes);

                    //    // Получаем ответ от сервера
                    //    bytesRec = mySocket.Receive(bytes);
                    //    responce = Encoding.UTF8.GetString(bytes, 0, bytesRec);
                    //    textBox1.Text = responce;
                    //    textBox1.Text = textBox1 + " " + "rtsp://" + selected_ip + "/" + sName.Trim();

                    //    //Проверяем ответ
                    //    mData = JsonConvert.DeserializeObject(responce);
                    //    msg_id = mData.msg_id;
                    //    if (msg_id != "2") return;  //Error

                    //    //axVLCPlugin21.playlist.add("rtsp://192.168.42.1/live");
                    //    string s = "rtsp://" + selected_ip + "/tmp/fuse_d/DCIM/100MEDIA/" + sName.Trim();
                    //    //axVLCPlugin21.playlist.add("rtsp://" + selected_ip + "/tmp/fuse_d/DCIM/100MEDIA/" + sName.Trim());
                    //    axVLCPlugin21.playlist.stop();
                    //    axVLCPlugin21.playlist.clear();
                    //    axVLCPlugin21.playlist.add(s);
                    //    textBox1.Text = s;
                    //    //axVLCPlugin21.playlist.add("rtsp://" + selected_ip + "/" + sName.Trim());
                    //    axVLCPlugin21.playlist.play();
                    }
                    else //save file
                    {
                        //Создаем сокет, коннектимся
                        //IPEndPoint EndPoint = new IPEndPoint(IPAddress.Parse("192.168.42.1"), 8787);
                        IPEndPoint EndPoint = new IPEndPoint(IPAddress.Parse(selected_ip), 8787);
                        Socket File_Socket = new Socket(EndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                        File_Socket.Connect(EndPoint);

                        // Получаем ответ от сервера
                        //byte[] bytes_file = new byte[size_File];
                        //int bytesRec_file = File_Socket.Receive(bytes_file);
                        //File_Socket.Close();
                        Info_box info = new Info_box();
                        info.Left = this.Left + 50;
                        info.Top = this.Top + 50;
                        info.fName = sName;
                        info.selected_ip = selected_ip;
                        info.size_File = size_File;
                        info.File_Socket = File_Socket;
                        info.fPath = FileStorePath;
                        //info.Show();
                        DialogResult dr = (DialogResult)info.ShowDialog();
                        if (dr == DialogResult.Cancel) return;
                        rightTabCtrl.SelectedTab = tabPage5;

                        //info.ShowDialog();
                        //info.Text = sName;
                        //info.progressBar1.Value = 0;
                        //info.progressBar1.Maximum = (int)size_File;

                        //File.Delete("C:\\temp\\" + sName);
                        //byte[] buffer_1 = new byte[1024]; //Буфер для файла
                        //FileStream stream = new FileStream("C:\\temp\\" + sName, FileMode.CreateNew, FileAccess.Write);
                        //BinaryWriter f = new BinaryWriter(stream);
                        //ulong processed = 0; //Байт принято
                        //while (processed < size_File) //Принимаем файл
                        //{
                        //    if ((size_File - processed) < 1024)
                        //    {
                        //        int bytes_f = (int)(size_File - processed);
                        //        byte[] buf = new byte[bytes_f];
                        //        bytes_f = File_Socket.Receive(buf);
                        //        f.Write(buf, 0, bytes_f);
                        //        processed = processed + (ulong)bytes_f;
                        //        info.progressBar1.Value = (int)processed;
                        //        Application.DoEvents();
                        //    }
                        //    else
                        //    {
                        //        int bytes_f = File_Socket.Receive(buffer_1);
                        //        f.Write(buffer_1, 0, bytes_f);
                        //        processed = processed + (ulong)bytes_f;
                        //        info.progressBar1.Value = (int)processed;
                        //        Application.DoEvents();
                        //    }
                        //}
                        //if (f != null)
                        //{
                        //    f.Close();
                        //}
                        //stream.Close();
                        //info.Close();

                        // Получаем ответ от сервера
                        byte[] bytes = new byte[1024];
                        int bytesRec = mySocket.Receive(bytes);
                        string responce = Encoding.UTF8.GetString(bytes, 0, bytesRec);
                        ////Image img = new Bitmap("C:\\temp\\" + sName);
                        ////pictureBox1.Image = new Bitmap(img);
                        ////img.Dispose();
                        //File_Socket.Close();

                        ini.IniWriteValue("System", "FileStorePath", info.fPath);
                        FileStorePath = ini.IniReadValue("System", "FileStorePath");
                        rightTabCtrl.SelectedTab = tabPage5;
                        vlcControl.Stop();
                        LocationMedia media = new LocationMedia(@"file:///" + info.fPath + sName.Trim());
                        vlcControl.Media = media;                        
                        vlcControl.Play();
                    }
                }
            }

        }

        //---------------------------
        public class StateObject
        {
            // Client socket.
            public Socket workSocket = null;
            // Size of receive buffer.
            public const int BufferSize = 1024;
            // Receive buffer.
            public byte[] buffer = new byte[BufferSize];
            // Received data string.
            public StringBuilder sb = new StringBuilder();
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket 
                // from the asynchronous state object.
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                // Read data from the remote device.
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                    // Get the rest of the data.
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    // All the data has arrived; put it in response.
                    if (state.sb.Length > 1)
                    {
                        response = state.sb.ToString();
                    }
                    // Signal that all bytes have been received.
                    receiveDone.Set();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        //-----------------------------------------

        private void button4_Click(object sender, EventArgs e)
        {
            leftTabCtrl.SelectedTab = tabPageFilelist;
            if (isConnection == false)
            {
                DialogResult result = MessageBox.Show("Camera is not connected.", "Important Note",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button1);
                return;
            }
            dynamic mData = take_photo();
            if (mData.ToString == "") return;
            if (mData.rval < 0)
            {
                return;
            }
            string fName = mData.param;
            FillFilesList("");
            //ShowImage(fName);
        }

        public dynamic take_photo()
        {
            Amba_Take_Photo t_photo = new Amba_Take_Photo();
            t_photo.token = Convert.ToInt32(token);
            t_photo.msg_id = 769;

            string output = JsonConvert.SerializeObject(t_photo);
            Byte[] SendBytes = Encoding.Default.GetBytes(output);
            mySocket.Send(SendBytes);

            // Получаем ответ от сервера
            byte[] bytes = new byte[1024];
            int bytesRec = mySocket.Receive(bytes);
            string responce = Encoding.UTF8.GetString(bytes, 0, bytesRec);
            //textBox1.Text = responce;

            //Проверяем ответ
            dynamic mData = JsonConvert.DeserializeObject(responce);
            msg_id = mData.msg_id;
            int rval = mData.rval;
            if (rval < 0) return "";  //Error
            return mData;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (isConnection == false)
            {
                DialogResult result = MessageBox.Show("Camera is not connected.", "Important Note",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button1);
                return;
            }

            if (isRecord == false)
            {                
                btnVideo.FlatAppearance.BorderSize = 1;
                StartRecord();
                isRecord = true;
                
            }
            else
            {
                btnVideo.FlatAppearance.BorderSize = 0;
                StopRecord();
                FillFilesList("");
                isRecord = false;
            }
        }

        public dynamic StartRecord()
        {
            Amba_Start_Record start = new Amba_Start_Record();
            start.token = Convert.ToInt32(token);
            start.msg_id = 513;

            string output = JsonConvert.SerializeObject(start);
            Byte[] SendBytes = Encoding.Default.GetBytes(output);
            mySocket.Send(SendBytes);

            // Получаем ответ от сервера
            byte[] bytes = new byte[1024];
            int bytesRec = mySocket.Receive(bytes);
            string responce = Encoding.UTF8.GetString(bytes, 0, bytesRec);
            //textBox1.Text = responce;

            //Проверяем ответ
            dynamic mData = JsonConvert.DeserializeObject(responce);
            if (mData != null)
            {
                msg_id = mData.msg_id;
                int rval = mData.rval;
                if (rval < 0) return "";  //Error
            }
            return mData;
        }

        public dynamic StopRecord()
        {
            Amba_Start_Record stop = new Amba_Start_Record();
            stop.token = Convert.ToInt32(token);
            stop.msg_id = 514;

            string output = JsonConvert.SerializeObject(stop);
            Byte[] SendBytes = Encoding.Default.GetBytes(output);
            mySocket.Send(SendBytes);

            // Получаем ответ от сервера
            byte[] bytes = new byte[1024];
            int bytesRec = mySocket.Receive(bytes);
            string responce = Encoding.UTF8.GetString(bytes, 0, bytesRec);
            //textBox1.Text = responce;

            //Проверяем ответ
            dynamic mData = JsonConvert.DeserializeObject(responce);
            msg_id = mData.msg_id;
            int rval = mData.rval;
            if (rval < 0) return "";  //Error
            return mData;
        }

        public dynamic GetData(int msg_id, string type)
        {
            Amba_cType amba_type = new Amba_cType();
            amba_type.token = Convert.ToInt32(token);
            amba_type.msg_id = msg_id;
            amba_type.type = type;

            string output = JsonConvert.SerializeObject(amba_type);
            Byte[] SendBytes = Encoding.Default.GetBytes(output);
            mySocket.Send(SendBytes);

            // Получаем ответ от сервера
            byte[] bytes = new byte[1024];
            int bytesRec = mySocket.Receive(bytes);
            string responce = Encoding.UTF8.GetString(bytes, 0, bytesRec);
            //textBox1.Text = responce;

            //Проверяем ответ
            dynamic mData = JsonConvert.DeserializeObject(responce);
            msg_id = mData.msg_id;
            int rval = mData.rval;
            if (rval < 0) return "";  //Error
            return mData;
        }

        public Device_Info GetDeviceInfo()
        {
            Device_Info result = new Device_Info();
            result.api_ver = "Not available";
            result.chip = "Not available";
            result.fw_ver = "Not available";
            result.model = "Not available";

            Amba_cType amba_type = new Amba_cType();
            amba_type.token = Convert.ToInt32(token);
            amba_type.msg_id = 11;

            string output = JsonConvert.SerializeObject(amba_type);
            Byte[] SendBytes = Encoding.Default.GetBytes(output);
            mySocket.Send(SendBytes);

            // Получаем ответ от сервера
            byte[] bytes = new byte[1024];
            int bytesRec = mySocket.Receive(bytes);
            string responce = Encoding.UTF8.GetString(bytes, 0, bytesRec);
            //textBox1.Text = responce;

            //Проверяем ответ
            dynamic mData = JsonConvert.DeserializeObject(responce);
            msg_id = mData.msg_id;
            result.api_ver = mData.api_ver;
            result.chip = mData.achippi_ver;
            result.fw_ver = mData.fw_ver;
            result.model = mData.model;
            return result;
        }

        public dynamic SetData(int msg_id, string type, string param)
        {
            Amba_cParam amba_type = new Amba_cParam();
            amba_type.token = Convert.ToInt32(token);
            amba_type.msg_id = msg_id;
            amba_type.type = type;
            amba_type.param = param;

            string output = JsonConvert.SerializeObject(amba_type);
            Byte[] SendBytes = Encoding.Default.GetBytes(output);
            mySocket.Send(SendBytes);

            // Получаем ответ от сервера
            byte[] bytes = new byte[1024];
            int bytesRec = mySocket.Receive(bytes);
            string responce = Encoding.UTF8.GetString(bytes, 0, bytesRec);
            //textBox1.Text = responce;

            //Проверяем ответ
            dynamic mData = JsonConvert.DeserializeObject(responce);
            msg_id = mData.msg_id;
            int rval = mData.rval;
            if (rval < 0) return "";  //Error
            return mData;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            rightTabCtrl.SelectedTab = tabPage5;
            if (isConnection == false)
            {
                DialogResult result = MessageBox.Show("Camera is not connected.", "Important Note",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button1);
                return;
            }

            if (isStream == false)
            {                
                btnStream.FlatAppearance.BorderSize = 1;
                OpenStream();
                isStream = true;
            }
            else
            {
                btnStream.FlatAppearance.BorderSize = 0;
                CloseStream();
                isStream = false;
            }
        }

        public void OpenStream()
        {
            Send260();

            Stream_Out_Type stream_out_type = new Stream_Out_Type();
            stream_out_type.token = Convert.ToInt32(token);
            stream_out_type.msg_id = 2;
            stream_out_type.type = "save_low_resolution_clip";
            stream_out_type.param = "off";
            string output = JsonConvert.SerializeObject(stream_out_type);
            Byte[] SendBytes = Encoding.Default.GetBytes(output);
            mySocket.Send(SendBytes);

            // Получаем ответ от сервера
            byte[] bytes = new byte[1024];
            int bytesRec = mySocket.Receive(bytes);
            string responce = Encoding.UTF8.GetString(bytes, 0, bytesRec);
            textBox1.Text = responce;

            //Проверяем ответ
            dynamic mData = JsonConvert.DeserializeObject(responce);
            msg_id = mData.msg_id;
            if (msg_id != "2") return;  //Error


            //---------------
            stream_out_type = new Stream_Out_Type();
            stream_out_type.token = Convert.ToInt32(token);
            stream_out_type.msg_id = 2;
            stream_out_type.type = "stream_out_type";
            stream_out_type.param = "rtsp";
            output = JsonConvert.SerializeObject(stream_out_type);
            SendBytes = Encoding.Default.GetBytes(output);
            mySocket.Send(SendBytes);

            // Получаем ответ от сервера
            bytesRec = mySocket.Receive(bytes);
            responce = Encoding.UTF8.GetString(bytes, 0, bytesRec);
            textBox1.Text = responce;

            //Проверяем ответ
            mData = JsonConvert.DeserializeObject(responce);
            msg_id = mData.msg_id;
            if (msg_id != "2") return;  //Error

            Send259();

            LocationMedia media = new LocationMedia("rtsp://" + selected_ip + "/live");           
            vlcControl.Media = media;
            vlcControl.Play();

            /*axVLCPlugin21.playlist.clear();
            //axVLCPlugin21.playlist.add("rtsp://192.168.42.1/live");
            axVLCPlugin21.playlist.add("rtsp://" + selected_ip + "/live");
            axVLCPlugin21.playlist.play();*/
        }

        public void CloseStream()
        {
            vlcControl.Stop();
            //axVLCPlugin21.playlist.stop();
        }

        private void panel9_Paint(object sender, PaintEventArgs e)
        {
            Panel tp = (Panel)sender;
            int w = tp.Width;
            float[] dashValues = { 1, 2, 1, 2 };
            Pen mPen = new Pen(Color.Gray, 1);
            mPen.DashPattern = dashValues;
            e.Graphics.DrawLine(mPen, new Point(1, 30), new Point(w - 1, 30));
            
            Brush brush_s = new SolidBrush(System.Drawing.Color.WhiteSmoke);
            Brush brush_w = new SolidBrush(System.Drawing.Color.White);

            //e.Graphics.FillRectangle(brush_s, 2, 55, w + 4, 20);
            //e.Graphics.FillRectangle(brush_w, 2, 75, w - 4, 20);
            //e.Graphics.FillRectangle(brush_s, 2, 115, w - 4, 20);
            //e.Graphics.FillRectangle(brush_w, 2, 135, w - 4, 20);
            //e.Graphics.FillRectangle(brush_s, 2, 155, w - 4, 20);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            dynamic mData;
            leftTabCtrl.SelectedTab = tabPageStream;
            progressBar1.Maximum = 5;
            progressBar1.Value = 0;
            Application.DoEvents();
            mData = GetData(3, "");
            JObject mJson = JObject.Parse(mData.ToString());
            IList<JToken> results = mJson["param"].Children().ToList();
            IList<Ls_Structure> LSResults = new List<Ls_Structure>();

            dynamic sData = JsonConvert.DeserializeObject(results[0].ToString());
             comboBox4.Text = sData.stream_out_type;

            sData = JsonConvert.DeserializeObject(results[1].ToString());
            comboBox3.Text = sData.save_low_resolution_clip;

            sData = JsonConvert.DeserializeObject(results[2].ToString());
            comboBox5.Text = sData.video_resolution;

            sData = JsonConvert.DeserializeObject(results[3].ToString());
            comboBox6.Text = sData.video_quality;

            sData = JsonConvert.DeserializeObject(results[4].ToString());
            comboBox7.Text = sData.video_WDR;

            sData = JsonConvert.DeserializeObject(results[5].ToString());
            comboBox8.Text = sData.camera_clock;

            sData = JsonConvert.DeserializeObject(results[6].ToString());
            comboBox9.Text = sData.ext_gps;

            sData = JsonConvert.DeserializeObject(results[7].ToString());
            comboBox4.Text = sData.stream_out_type;
        }

        private void panel13_Paint(object sender, PaintEventArgs e)
        {
            Panel tp = (Panel)sender;
            int w = tp.Width;
            float[] dashValues = { 1, 2, 1, 2 };
            Pen mPen = new Pen(Color.Gray, 1);
            mPen.DashPattern = dashValues;
            e.Graphics.DrawLine(mPen, new Point(1, 28), new Point(w - 1, 28));

            Brush brush_s = new SolidBrush(System.Drawing.Color.WhiteSmoke);
            Brush brush_w = new SolidBrush(System.Drawing.Color.White);

            //e.Graphics.FillRectangle(brush_s, 2, 55, w + 4, 20);
            //e.Graphics.FillRectangle(brush_w, 2, 75, w - 4, 20);
            //e.Graphics.FillRectangle(brush_s, 2, 115, w - 4, 20);
            //e.Graphics.FillRectangle(brush_w, 2, 135, w - 4, 20);
            //e.Graphics.FillRectangle(brush_s, 2, 155, w - 4, 20);
        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetData(2, "video_quality", comboBox6.Text);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            button8_Click(sender, e);
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetData(2, "video_resolution", comboBox5.Text);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            SetData(2, "video_resolution", "848x480 30P 16:9");
        }

        private void btnSettingsMain_Click(object sender, EventArgs e)
        {
            leftTabCtrl.SelectedTab = tabPageSetMain;
            btnBeepNoises.Text = "ON";
            btnRecordingLed.Text = "ON";
            btnMotionDetection.Text = "OFF";
            btnGSensor.Text = "OFF";
            btnSpeedStamp.Text = "OFF";
            btnTimeMode.Text = "OFF";
            btnCarPlate.Text = "OFF";
            txtCarNumber.Text = "0000000000";
            txtWifiPassword.Text = "1234567890";

            cmbMotionDetection.Enabled = false;
            cmbGsensor.Enabled = false;
            cmbStartTime.Enabled = false;
            cmbEndTime.Enabled = false;
            cmbCarspeedUnit.Enabled = false;
            cmbDateFormat.SelectedIndex = 0;
            cmbDefaultModeCameraStart.SelectedIndex = 0;
            cmbPoweronAutoRecord.SelectedIndex = 1;
            cmbStandbyTime.SelectedIndex = 2;
            cmbMotionDetection.SelectedIndex = 0;
            cmbTvOut.SelectedIndex = 0;
            cmbGsensor.SelectedIndex = 0;
            cmbCarspeedUnit.SelectedIndex = 1;
            cmbStartTime.Text = "21";
            cmbEndTime.Text = "6";
            cmbPowerOffDisconnect.SelectedIndex = 0;

            try
            {
                string camera_time = GetCurrentOption("camera_time");
                string data_format = GetCurrentOption("data_format");
                string wifi_password = GetCurrentOption("wifi_password");
                string car_number = GetCurrentOption("car_number");
                string toggleBeepNoises = GetCurrentOption("toggleBeepNoises");
                string toggleRecordingLEDindicator = GetCurrentOption("toggleRecordingLEDindicator");
                string default_mode = GetCurrentOption("default_mode");
                string spinnerPowerOnAutoRecord = GetCurrentOption("spinnerPowerOnAutoRecord");

                string standby_time = GetCurrentOption("standby_time");
                string power_off_disconnect = GetCurrentOption("power_off_disconnect");
                string motion_detection = GetCurrentOption("motion_detection");
                string motion_det_sens = GetCurrentOption("motion_det_sens");
                string spinnerMotionTurnOff = GetCurrentOption("spinnerMotionTurnOff");
                string tv_type = GetCurrentOption("tv_type");
                string g_sensor = GetCurrentOption("g_sensor");
                string g_sensor_sensitivity = GetCurrentOption("g_sensor_sensitivity");
                string car_plate_stamp = GetCurrentOption("car_plate_stamp");
                string speed_stamp = GetCurrentOption("speed_stamp");
                string time_mode = GetCurrentOption("time_mode");
                string toggleConnectionLog = GetCurrentOption("toggleConnectionLog");
                string speed_unit = GetCurrentOption("speed_unit");
                //string g_sensor_sensitivity = GetCurrentOption("g_sensor_sensitivity");
                //string parking_mode = GetCurrentOption("parking_mode");   
                string time_mode_start_time = GetCurrentOption("time_mode_start_time");
                string time_mode_finish_time = GetCurrentOption("time_mode_finish_time");

                string[] ar_dt = camera_time.Split('_');
                string yy = ar_dt[0].Substring(0, 4);
                string mm = ar_dt[0].Substring(4, 2);
                string dd = ar_dt[0].Substring(6, 2);
                string h = ar_dt[1].Substring(0, 2);
                string m = ar_dt[1].Substring(2, 2);
                string s = ar_dt[1].Substring(4, 2);
                DateTime time = new DateTime(int.Parse(yy), int.Parse(mm), int.Parse(dd), int.Parse(h), int.Parse(m), int.Parse(s));
                dateTimePicker1.Value = time;
                dateTimePicker2.Value = time;

                cmbDateFormat.Text = data_format;
                txtWifiPassword.Text = wifi_password;
                txtCarNumber.Text = car_number;
                btnBeepNoises.Text = toggleBeepNoises.ToUpper();
                btnRecordingLed.Text = toggleRecordingLEDindicator.ToUpper();
                cmbDefaultModeCameraStart.Text = default_mode;
                cmbPoweronAutoRecord.Text = spinnerPowerOnAutoRecord;

                cmbStandbyTime.Text = standby_time;
                cmbPowerOffDisconnect.Text = power_off_disconnect;
                btnMotionDetection.Text = motion_detection.ToUpper();
                cmbMotionDetection.Text = motion_det_sens;
                cmbTvOut.Text = tv_type;
                btnGSensor.Text = g_sensor.ToUpper();
                btnCarPlate.Text = car_plate_stamp.ToUpper();
                btnSpeedStamp.Text = speed_stamp.ToUpper();
                //button21.Text = toggleConnectionLog.ToUpper();
                if (speed_stamp == "off")
                {
                    label84.ForeColor = Color.LightGray;
                    cmbCarspeedUnit.Enabled = false;
                }
                else
                {
                    label84.ForeColor = Color.Gray;
                    cmbCarspeedUnit.Enabled = true;
                }
                btnTimeMode.Text = time_mode.ToUpper();
                if (btnTimeMode.Text == "ON")
                {
                    cmbStartTime.Enabled = true;
                    cmbEndTime.Enabled = true;
                }
                else
                {
                    cmbStartTime.Enabled = false;
                    cmbEndTime.Enabled = false;
                }
                cmbCarspeedUnit.Text = speed_unit;
                cmbGsensor.Text = g_sensor_sensitivity;

                cmbStartTime.Text = time_mode_start_time;
                cmbEndTime.Text = time_mode_finish_time;

                if (time_mode == "off")
                {
                    btnTimeMode.Text = "OFF";
                    cmbStartTime.Enabled = false;
                    cmbEndTime.Enabled = false;                    
                }
                else
                {
                    btnTimeMode.Text = "ON";
                    cmbStartTime.Enabled = true;
                    cmbEndTime.Enabled = true;                    
                }

                if (car_plate_stamp == "off")
                {
                    btnCarPlate.Text = "OFF";
                    txtCarNumber.Enabled = false;
                    btnCarNumber.Enabled = false;                    
                }
                else
                {
                    btnCarPlate.Text = "ON";
                    txtCarNumber.Enabled = true;
                    btnCarNumber.Enabled = true;                    
                }
            }
            catch { }
        }

        public string GetCurrentOption(string type)
        {
            Stream_Out_Type json = new Stream_Out_Type();
            json.token = Convert.ToInt32(token);
            json.msg_id = 1;
            json.type = type;
            json.param = "";

            string output = JsonConvert.SerializeObject(json);
            Byte[] SendBytes = Encoding.Default.GetBytes(output);
            mySocket.Send(SendBytes);

            // Получаем ответ от сервера
            byte[] bytes = new byte[1024];
            int bytesRec = mySocket.Receive(bytes);
            string responce = Encoding.UTF8.GetString(bytes, 0, bytesRec);
            //textBox1.Text = responce;

            //Проверяем ответ
            dynamic mData = JsonConvert.DeserializeObject(responce);
            msg_id = mData.msg_id;
            int rval = mData.rval;
            if (rval < 0) return "";  //Error
            return mData.param;
        }

        public string SendSetting(string value, string type)
        {
            string resp = Send260();
            if (resp == "") return "";

            Stream_Out_Type json = new Stream_Out_Type();
            json.token = Convert.ToInt32(token);
            json.msg_id = 2;
            json.type = type;
            json.param = value;

            string output = JsonConvert.SerializeObject(json);
            Byte[] SendBytes = Encoding.Default.GetBytes(output);
            mySocket.Send(SendBytes);

            // Получаем ответ от сервера
            byte[] bytes = new byte[1024];
            int bytesRec = mySocket.Receive(bytes);
            string responce = Encoding.UTF8.GetString(bytes, 0, bytesRec);
            //textBox1.Text = responce;

            //Проверяем ответ
            dynamic mData = JsonConvert.DeserializeObject(responce);
            msg_id = mData.msg_id;
            int rval = mData.rval;
            if (rval < 0) return "";  //Error
            return mData.param;
        }

        public string Send260()
        {
            if (isConnection == false)
            {                
                return "";
            }
            Amba_Stop_Session json = new Amba_Stop_Session();
            json.token = Convert.ToInt32(token);
            json.msg_id = 260;
            
            string output = JsonConvert.SerializeObject(json);
            Byte[] SendBytes = Encoding.Default.GetBytes(output);
            mySocket.Send(SendBytes);

            // Получаем ответ от сервера
            byte[] bytes = new byte[1024];
            int bytesRec = mySocket.Receive(bytes);
            string responce = Encoding.UTF8.GetString(bytes, 0, bytesRec);
            //textBox1.Text = responce;

            //Проверяем ответ
            dynamic mData = JsonConvert.DeserializeObject(responce);
            msg_id = mData.msg_id;
            if (mData.rval == null)
                return "";

            int rval = mData.rval;
            if (rval < 0) return "";  //Error
            return mData.param;
        }

        public string Send259()
        {
            Amba_Stop_Session json = new Amba_Stop_Session();
            json.token = Convert.ToInt32(token);
            json.msg_id = 259;

            string output = JsonConvert.SerializeObject(json);
            Byte[] SendBytes = Encoding.Default.GetBytes(output);
            mySocket.Send(SendBytes);

            // Получаем ответ от сервера
            byte[] bytes = new byte[1024];
            int bytesRec = mySocket.Receive(bytes);
            string responce = Encoding.UTF8.GetString(bytes, 0, bytesRec);
            //textBox1.Text = responce;

            //Проверяем ответ
            dynamic mData = JsonConvert.DeserializeObject(responce);
            msg_id = mData.msg_id;
            int rval = mData.rval;
            if (rval < 0) return "";  //Error
            return mData.param;
        }

        private void button14_Click(object sender, EventArgs e)
        {
            DateTime d = DateTime.Now;
            string fD = d.ToString("yyyyMMdd_HHmmss");
            try
            {
                string currDate = SendSetting(fD, "toggleSyncFateAndTime");
                string camera_time = GetCurrentOption("camera_time");
                string[] ar_dt = camera_time.Split('_');
                string yy = ar_dt[0].Substring(0, 4);
                string mm = ar_dt[0].Substring(4, 2);
                string dd = ar_dt[0].Substring(6, 2);
                string h = ar_dt[1].Substring(0, 2);
                string m = ar_dt[1].Substring(2, 2);
                string s = ar_dt[1].Substring(4, 2);
                DateTime time = new DateTime(int.Parse(yy), int.Parse(mm), int.Parse(dd), int.Parse(h), int.Parse(m), int.Parse(s));
                dateTimePicker1.Value = time;
                dateTimePicker2.Value = time;
            }
            catch { }
            //string currDate = GetCurrentOption("wifi_password");
        }

        private void comboBox14_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string response = SendSetting(cmbDateFormat.Text, "data_format");
            }
            catch { }
        }

        private void button15_Click(object sender, EventArgs e)
        {   
            if (btnBeepNoises.Text == "OFF")
            {
                btnBeepNoises.Text = "ON";
                try
                {
                    string response = SendSetting("on", "toggleBeepNoises");
                    if (response == "on") btnBeepNoises.Text = "ON";
                }
                catch { }  
            }
            else
            {
                btnBeepNoises.Text = "OFF";
                try
                {
                    string response = SendSetting("off", "toggleBeepNoises");
                    if (response == "off") btnBeepNoises.Text = "OFF";
                }
                catch { }
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            if (btnRecordingLed.Text == "OFF")
            {
                btnRecordingLed.Text = "ON";
                try
                {
                    string response = SendSetting("on", "toggleRecordingLEDindicator");
                    if (response == "on") btnRecordingLed.Text = "ON";
                }
                catch { }
            }
            else
            {
                btnRecordingLed.Text = "OFF";
                try
                {
                    string response = SendSetting("off", "toggleRecordingLEDindicator");
                    if (response == "off") btnRecordingLed.Text = "OFF";
                }
                catch { }
            }
        }

        private void comboBox11_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string response = SendSetting(cmbDefaultModeCameraStart.Text, "default_mode");
            }
            catch { }
        }

        private void comboBox10_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string response = SendSetting(cmbPoweronAutoRecord.Text, "spinnerPowerOnAutoRecord");
            }
            catch { }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            btnSettingsMain_Click(sender, e);
        }

        private void comboBox13_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string response = SendSetting(cmbStandbyTime.Text, "standby_time");
            }
            catch { }
        }

        private void comboBox12_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string response = SendSetting(cmbPowerOffDisconnect.Text, "power_off_disconnect");
            }
            catch { }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            if (btnMotionDetection.Text == "OFF")
            {
                btnMotionDetection.Text = "ON";
                try
                {
                    string response = SendSetting("on", "motion_detection");
                    if (response == "on") btnMotionDetection.Text = "ON";
                }
                catch { }
                label36.Enabled = true;
                cmbMotionDetection.Enabled = true;
                
            }
            else
            {
                btnMotionDetection.Text = "OFF";
                try
                {
                    string response = SendSetting("off", "motion_detection");
                    if (response == "off") btnMotionDetection.Text = "OFF";
                }
                catch { }
                label36.Enabled = false;
                cmbMotionDetection.Enabled = false;
                
                cmbMotionDetection.Text = "high";
            }
        }

        private void comboBox15_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string response = SendSetting(cmbMotionDetection.Text, "motion_det_sens");
            }
            catch { }
        }

        private void comboBox18_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string response = SendSetting(cmbTvOut.Text, "tv_type");
            }
            catch { }
        }

        private void button18_Click(object sender, EventArgs e)
        {
            if (btnGSensor.Text == "OFF")
            {
                btnGSensor.Text = "ON";
                try
                {
                    string response = SendSetting("on", "g_sensor");
                    if (response == "on") btnGSensor.Text = "ON";
                }
                catch { }
                label90.Enabled = true;
                cmbGsensor.Enabled = true;
                
            }
            else
            {
                btnGSensor.Text = "OFF";
                try
                {
                    string response = SendSetting("off", "g_sensor");
                    if (response == "off") btnGSensor.Text = "OFF";
                }
                catch { }
                label90.Enabled = false;
                cmbGsensor.Enabled = false;
                cmbGsensor.Text = "High";
                
            }
        }

        private void button19_Click(object sender, EventArgs e)
        {
            if (btnCarPlate.Text == "OFF")
            {
                btnCarPlate.Text = "ON";
                txtCarNumber.Enabled = true;
                btnCarNumber.Enabled = true;
                try
                {
                    string response = SendSetting("on", "car_plate_stamp");
                    if (response == "on") btnCarPlate.Text = "ON";
                }
                catch { }
            }
            else
            {
                btnCarPlate.Text = "OFF";
                txtCarNumber.Enabled = false;
                btnCarNumber.Enabled = false;
                try
                {
                    string response = SendSetting("off", "car_plate_stamp");
                    if (response == "off") btnCarPlate.Text = "OFF";
                }
                catch { }
            }
        }

        private void button20_Click(object sender, EventArgs e)
        {
            if (btnSpeedStamp.Text == "OFF")
            {
                btnSpeedStamp.Text = "ON";
                try
                {
                    string response = SendSetting("on", "speed_stamp");
                    if (response == "on") btnSpeedStamp.Text = "ON";
                }
                catch { }
                label84.Enabled = true;
                cmbCarspeedUnit.Enabled = true;
               
            }
            else
            {
                btnSpeedStamp.Text = "OFF";
                try
                {
                    string response = SendSetting("off", "speed_stamp");
                    if (response == "off") btnSpeedStamp.Text = "OFF";
                }
                catch { }
                label84.Enabled = false;
                cmbCarspeedUnit.Enabled = false;
                cmbCarspeedUnit.Text = "MPH";
                
            }
        }

        private void button22_Click(object sender, EventArgs e)
        {
            if (btnTimeMode.Text == "OFF")
            {
                btnTimeMode.Text = "ON";
                cmbStartTime.Enabled = true;
                cmbEndTime.Enabled = true;
                
                try
                {
                    string response = SendSetting("on", "time_mode");
                    if (response == "on") btnTimeMode.Text = "ON";
                }
                catch { }
            }
            else
            {
                btnTimeMode.Text = "OFF";
                cmbStartTime.Enabled = false;
                cmbEndTime.Enabled = false;
                try
                {
                    string response = SendSetting("off", "time_mode");
                    if (response == "off") btnTimeMode.Text = "OFF";
                }
                catch { }
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            try
            {                
                string response = SendSetting(txtWifiPassword.Text, "wifi_password");
            }
            catch { }
        }
                
        private void button23_Click(object sender, EventArgs e)
        {
            leftTabCtrl.SelectedTab = tabPageSetMode1;
            cmbVideoResolution.SelectedIndex = 2;
            cmbVideoClipLength.SelectedIndex = 2;
            cmbVideoBitRates.SelectedIndex = 0;
            cmbFieldofView.SelectedIndex = 0;
            cmbTimeLapseVideo.SelectedIndex = 0;
            btnMode1VideoTimeStamp.Text = "OFF";
            btnMode1Audio.Text = "ON";
            cmbMode1ImageRotation.SelectedIndex = 0;
            btnMode1LoopRecording.Text = "ON";

            try
            {
                string video_resolution = GetCurrentOption("video_resolution");
                string audio = GetCurrentOption("audio");
                string video_timestamp = GetCurrentOption("video_timestamp");
                //string rotate_video = GetCurrentOption("rotate_video");
                string loop_record = GetCurrentOption("loop_record");
                string video_length = GetCurrentOption("video_length");
                string video_quality = GetCurrentOption("video_quality");
                string image_rotation = GetCurrentOption("rotate_video");
                string wdr = GetCurrentOption("wdr");
                string fleld_view = GetCurrentOption("fleld_view");
                string timelapse_video = GetCurrentOption("timelapse_video");


                cmbVideoResolution.Text = video_resolution;
                btnMode1Audio.Text = audio.ToUpper();
                btnMode1VideoTimeStamp.Text = video_timestamp.ToUpper();
                cmbMode1ImageRotation.Text = image_rotation;
                btnMode1LoopRecording.Text = loop_record.ToUpper();

                cmbVideoClipLength.Text = video_length;
                cmbVideoBitRates.Text = video_quality;

                btnMode1WDR.Text = wdr.ToUpper();

                cmbFieldofView.Text = fleld_view;
                cmbTimeLapseVideo.Text = timelapse_video;
            }
            catch { }
        }

        private void button32_Click(object sender, EventArgs e)
        {
            button23_Click(sender, e);
        }

        private void comboBox24_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string response = SendSetting(cmbVideoResolution.Text, "video_resolution");
            }
            catch { }
            if (cmbVideoResolution.Text == "HDR 1920x1080 30P 16:9")
            {
                btnMode1WDR.Text = "OFF";
                btnMode1WDR.Enabled = false;
            }
            else
            {
                btnMode1WDR.Enabled = true;
            }
            if (cmbVideoResolution.Text == "HDR 1920x1080 30P 16:9" || cmbVideoResolution.Text == "1280x720 60P 16:9")
            {
                cmbTimeLapseVideo.Text = "off";
                cmbTimeLapseVideo.Enabled = false;
            }
            else
            {
                cmbTimeLapseVideo.Enabled = true;
            }
        }

        private void comboBox22_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string response = SendSetting(cmbVideoClipLength.Text, "video_length");
            }
            catch { }
        }

        private void comboBox21_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string response = SendSetting(cmbVideoBitRates.Text, "video_quality");
            }
            catch { }
        }

        private void comboBox19_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string response = SendSetting(cmbFieldofView.Text, "fleld_view");
            }
            catch { }
        }

        private void comboBox17_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string response = SendSetting(cmbTimeLapseVideo.Text, "timelapse_video");
            }
            catch { }
        }

        private void button31_Click(object sender, EventArgs e)
        {
            if (btnMode1Audio.Text == "OFF")
            {
                btnMode1Audio.Text = "ON";
                try
                {
                    string response = SendSetting("on", "audio");
                    if (response == "on") btnMode1Audio.Text = "ON";
                }
                catch { }
            }
            else
            {
                btnMode1Audio.Text = "OFF";
                try
                {
                    string response = SendSetting("off", "audio");
                    if (response == "off") btnMode1Audio.Text = "OFF";
                }
                catch { }
            }
        }

        private void button33_Click(object sender, EventArgs e)
        {
            if (btnMode1VideoTimeStamp.Text == "OFF")
            {
                btnMode1VideoTimeStamp.Text = "ON";
                try
                {
                    string response = SendSetting("on", "video_timestamp");
                    if (response == "on") btnMode1VideoTimeStamp.Text = "ON";
                }
                catch { }
            }
            else
            {
                btnMode1VideoTimeStamp.Text = "OFF";
                try
                {
                    string response = SendSetting("off", "video_timestamp");
                    if (response == "off") btnMode1VideoTimeStamp.Text = "OFF";
                }
                catch { }
            }
        }
               

        private void button29_Click(object sender, EventArgs e)
        {
            if (btnMode1LoopRecording.Text == "OFF")
            {
                btnMode1LoopRecording.Text = "ON";
                try
                {
                    string response = SendSetting("on", "loop_record");
                    if (response == "on") btnMode1LoopRecording.Text = "ON";
                }
                catch { }
            }
            else
            {
                btnMode1LoopRecording.Text = "OFF";
                try
                {
                    string response = SendSetting("off", "loop_record");
                    if (response == "off") btnMode1LoopRecording.Text = "OFF";
                }
                catch { }
            }
        }

        private void button27_Click(object sender, EventArgs e)
        {
            if (btnMode1WDR.Text == "OFF")
            {
                btnMode1WDR.Text = "ON";
                try
                {
                    string response = SendSetting("on", "wdr");
                    if (response == "on") btnMode1WDR.Text = "ON";
                }
                catch { }
            }
            else
            {
                btnMode1WDR.Text = "OFF";
                try
                {
                    string response = SendSetting("off", "wdr");
                    if (response == "off") btnMode1WDR.Text = "OFF";
                }
                catch { }
            }
        }

        private void button24_Click(object sender, EventArgs e)
        {
            leftTabCtrl.SelectedTab = tabPageSetMode2;
            cmbMode2VideoResolution.SelectedIndex = 0;
            cmbMode2VideoClipLength.SelectedIndex = 2;
            cmbMode2VideoBitRates.SelectedIndex = 0;
            cmbMode2FileldofView.SelectedIndex = 0;
            cmbMode2TimeLapseVideo.SelectedIndex = 0;
            btnMode2VideoTimeStamp.Text = "OFF";
            btnMode2Audio.Text = "ON";
            cmbMode2ImageRotation.SelectedIndex = 0;
            btnMode2LoopRecording.Text = "ON";
            
            try
            {
                string video_resolution_mode2 = GetCurrentOption("video_resolution_mode2");
                string audio_mode2 = GetCurrentOption("audio_mode2");
                string video_timestamp_mode2 = GetCurrentOption("video_timestamp_mode2");
                //string rotate_video_mode2 = GetCurrentOption("rotate_video_mode2");
                string image_rotation_mode2 = GetCurrentOption("rotate_video_mode2");
                string loop_record_mode2 = GetCurrentOption("loop_record_mode2");
                string video_length_mode2 = GetCurrentOption("video_length_mode2");
                string video_quality_mode2 = GetCurrentOption("video_quality_mode2");

                string wdr_mode2 = GetCurrentOption("wdr_mode2");
                string fleld_view_mode2 = GetCurrentOption("fleld_view_mode2");
                string timelapse_video_mode2 = GetCurrentOption("timelapse_video_mode2");


                cmbMode2VideoResolution.Text = video_resolution_mode2;
                btnMode2Audio.Text = audio_mode2.ToUpper();
                btnMode2VideoTimeStamp.Text = video_timestamp_mode2.ToUpper();
                cmbMode2ImageRotation.Text = image_rotation_mode2;
                btnMode2LoopRecording.Text = loop_record_mode2.ToUpper();

                cmbMode2VideoClipLength.Text = video_length_mode2;
                cmbMode2VideoBitRates.Text = video_quality_mode2;

                btnMode2WDR.Text = wdr_mode2.ToUpper();

                cmbMode2FileldofView.Text = fleld_view_mode2;
                cmbMode2TimeLapseVideo.Text = timelapse_video_mode2;
            }
            catch { }
        }

        private void comboBox16_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string response = SendSetting(cmbMode2VideoResolution.Text, "video_resolution_mode2");
            }
            catch { }
            if (cmbMode2VideoResolution.Text == "HDR 1920x1080 30P 16:9")
            {
                btnMode2WDR.Text = "OFF";
                btnMode2WDR.Enabled = false;
            }
            else
            {
                btnMode2WDR.Enabled = true;
            }
            if (cmbMode2VideoResolution.Text == "HDR 1920x1080 30P 16:9" || cmbMode2VideoResolution.Text == "1280x720 60P 16:9")
            {
                cmbMode2TimeLapseVideo.Text = "off";
                cmbMode2TimeLapseVideo.Enabled = false;
            }
            else
            {
                cmbMode2TimeLapseVideo.Enabled = true;
            }
        }

        private void button35_Click(object sender, EventArgs e)
        {
            if (btnMode2Audio.Text == "OFF")
            {
                btnMode2Audio.Text = "ON";
                try
                {
                    string response = SendSetting("on", "audio_mode2");
                    if (response == "on") btnMode2Audio.Text = "ON";
                }
                catch { }
            }
            else
            {
                btnMode2Audio.Text = "OFF";
                try
                {
                    string response = SendSetting("off", "audio_mode2");
                    if (response == "off") btnMode2Audio.Text = "OFF";
                }
                catch { }
            }
        }

        private void button25_Click(object sender, EventArgs e)
        {
            if (btnMode2VideoTimeStamp.Text == "OFF")
            {
                btnMode2VideoTimeStamp.Text = "ON";
                try
                {
                    string response = SendSetting("on", "video_timestamp_mode2");
                    if (response == "on") btnMode2VideoTimeStamp.Text = "ON";
                }
                catch { }
            }
            else
            {
                btnMode2VideoTimeStamp.Text = "OFF";
                try
                {
                    string response = SendSetting("off", "video_timestamp_mode2");
                    if (response == "off") btnMode2VideoTimeStamp.Text = "OFF";
                }
                catch { }
            }
        }
               

        private void button28_Click(object sender, EventArgs e)
        {
            if (btnMode2LoopRecording.Text == "OFF")
            {
                btnMode2LoopRecording.Text = "ON";
                try
                {
                    string response = SendSetting("on", "loop_record_mode2");
                    if (response == "on") btnMode2LoopRecording.Text = "ON";
                }
                catch { }
            }
            else
            {
                btnMode2LoopRecording.Text = "OFF";
                try
                {
                    string response = SendSetting("off", "loop_record_mode2");
                    if (response == "off") btnMode2LoopRecording.Text = "OFF";
                }
                catch { }
            }
        }

        private void comboBox26_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string response = SendSetting(cmbMode2VideoClipLength.Text, "video_length_mode2");
            }
            catch { }
        }

        private void comboBox25_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string response = SendSetting(cmbMode2VideoBitRates.Text, "video_quality_mode2");
            }
            catch { }
        }

        private void button26_Click(object sender, EventArgs e)
        {
            if (btnMode2WDR.Text == "OFF")
            {
                btnMode2WDR.Text = "ON";
                try
                {
                    string response = SendSetting("on", "wdr_mode2");
                    if (response == "on") btnMode2WDR.Text = "ON";
                }
                catch { }
            }
            else
            {
                btnMode2WDR.Text = "OFF";
                try
                {
                    string response = SendSetting("off", "wdr_mode2");
                    if (response == "off") btnMode2WDR.Text = "OFF";
                }
                catch { }
            }
        }

        private void comboBox23_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string response = SendSetting(cmbMode2FileldofView.Text, "fleld_view_mode2");
            }
            catch { }
        }

        private void comboBox20_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string response = SendSetting(cmbMode2TimeLapseVideo.Text, "timelapse_video_mode2");
            }
            catch { }
        }

        private void button36_Click(object sender, EventArgs e)
        {
            try
            {
                button24_Click(sender, e);
            }
            catch { }
        }

        private void button38_Click(object sender, EventArgs e)
        {           
            leftTabCtrl.SelectedTab = tabPageSetPhoto;
            string photo_resolution = GetCurrentOption("photo_resolution");
            string date_time_stamp = GetCurrentOption("date_time_stamp");
            string rotate_photo_180_degrees = GetCurrentOption("rotate_photo_180_degrees");
            string time_lapse_photo = GetCurrentOption("time_lapse_photo");
            string timelapse_video = GetCurrentOption("timelapse_video");//
            string spinnerBurstPhotoMode = GetCurrentOption("spinnerBurstPhotoMode");

            cmbPhotoResolution.Text = photo_resolution;
            button37.Text = date_time_stamp.ToUpper();
            button40.Text = rotate_photo_180_degrees.ToUpper();
            button39.Text = time_lapse_photo.ToUpper();
            button41.Text = timelapse_video.ToUpper();
            cmbBurstPhotoMode.Text = spinnerBurstPhotoMode;
        }

        private void comboBox27_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string response = SendSetting(cmbPhotoResolution.Text, "photo_resolution");
            }
            catch { }
        }

        private void comboBox30_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string response = SendSetting(cmbBurstPhotoMode.Text, "spinnerBurstPhotoMode");
            }
            catch { }
        }

        private void button37_Click(object sender, EventArgs e)
        {
            if (button37.Text == "OFF")
            {
                button37.Text = "ON";
                try
                {
                    string response = SendSetting("on", "date_time_stamp");
                    if (response == "on") button37.Text = "ON";
                }
                catch { }
            }
            else
            {
                button37.Text = "OFF";
                try
                {
                    string response = SendSetting("off", "date_time_stamp");
                    if (response == "off") button37.Text = "OFF";
                }
                catch { }
            }
        }

        private void button40_Click(object sender, EventArgs e)
        {
            if (button40.Text == "OFF")
            {
                button40.Text = "ON";
                try
                {
                    string response = SendSetting("on", "rotate_photo_180_degrees");
                    if (response == "on") button40.Text = "ON";
                }
                catch { }
            }
            else
            {
                button40.Text = "OFF";
                try
                {
                    string response = SendSetting("off", "rotate_photo_180_degrees");
                    if (response == "off") button40.Text = "OFF";
                }
                catch { }
            }
        }

        private void button39_Click(object sender, EventArgs e)
        {
            if (button39.Text == "OFF")
            {
                button39.Text = "ON";
                try
                {
                    string response = SendSetting("on", "time_lapse_photo");
                    if (response == "on") button39.Text = "ON";
                }
                catch { }
            }
            else
            {
                button39.Text = "OFF";
                try
                {
                    string response = SendSetting("off", "time_lapse_photo");
                    if (response == "off") button39.Text = "OFF";
                }
                catch { }
            }
        }

        private void button41_Click(object sender, EventArgs e)
        {
            if (button41.Text == "OFF")
            {
                button41.Text = "ON";
                try
                {
                    string response = SendSetting("on", "timelapse_video");
                    if (response == "on") button41.Text = "ON";
                }
                catch { }
            }
            else
            {
                button41.Text = "OFF";
                try
                {
                    string response = SendSetting("off", "timelapse_video");
                    if (response == "off") button41.Text = "OFF";
                }
                catch { }
            }
        }

        private void button42_Click(object sender, EventArgs e)
        {
            button38_Click(sender, e);
        }

        private void button43_Click(object sender, EventArgs e)
        {
            if (isConnection == false)
            {
                DialogResult result = MessageBox.Show("Camera is not connected.", "Important Note",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button1);
                return;
            }
            Amba_Ls json = new Amba_Ls();
            json.token = Convert.ToInt32(token);
            json.msg_id = 4;
            json.param = "D";
            string output = JsonConvert.SerializeObject(json);
            Byte[] SendBytes = Encoding.Default.GetBytes(output);
           
            mySocket.Send(SendBytes);

            // Получаем ответ от сервера
            byte[] bytes = new byte[1024];
            int bytesRec = mySocket.Receive(bytes);
            string responce = Encoding.UTF8.GetString(bytes, 0, bytesRec);
            textBox1.Text = responce;

            //Проверяем ответ
            dynamic mData = JsonConvert.DeserializeObject(responce);
            msg_id = mData.msg_id;
            if (msg_id != "4") return;  //Error

            btnInfo_Click(sender, e);
        }

        private void btnInfo_Click(object sender, EventArgs e)
        {
            if (isConnection == false)
            {
                DialogResult result = MessageBox.Show("Camera is not connected.", "Important Note", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button1);
                return;
            }
            btnInfo.Enabled = false;
            dynamic mData;
            leftTabCtrl.SelectedTab = tabPageSetSystem;
            progressBar1.Maximum = 5;
            progressBar1.Value = 0;
            Application.DoEvents();
            //mData = GetData(3, "");
            //return;
            //Get space
            mData = GetData(5, "total");
            try
            {
                if ((mData.ToString != "") && (mData.rval >= 0))
                {
                    label5.Text = mData.param;
                    progressBar1.Value++;
                }
            }
            catch (Exception)
            {
                return;
            }
            Application.DoEvents();
            mData = GetData(5, "free");
            if ((mData.ToString != "") && (mData.rval >= 0))
            {
                label6.Text = mData.param;
                progressBar1.Value++;
            }
            Application.DoEvents();
            mData = GetData(6, "total");
            if ((mData.ToString != "") && (mData.rval >= 0))
            {
                label7.Text = mData.param;
                progressBar1.Value++;
            }
            Application.DoEvents();
            mData = GetData(6, "photo");
            if ((mData.ToString != "") && (mData.rval >= 0))
            {
                label11.Text = mData.param;
                progressBar1.Value++;
            }
            Application.DoEvents();
            mData = GetData(6, "video");
            if ((mData.ToString != "") && (mData.rval >= 0))
            {
                label13.Text = mData.param;
                progressBar1.Value++;
            }
            Device_Info info = GetDeviceInfo();
            lblInfoFirmware.Text = info.fw_ver;
            lblInfoAPI.Text = info.api_ver;
            //progressBar1.Value = 0;
            Application.DoEvents();
            btnInfo.Enabled = true;
        }

        private void flatTabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (toolTabCtrl.SelectedIndex == 0)
            {
                if (isConnection == true) btnInfo_Click(sender, e);
            }
        }

        private string GetMyIP()
        {
            IPHostEntry host;
            string result = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    result = ip.ToString();
                    return result;
                }
            }
            return result;
        }
        private void btnStartScan_Click(object sender, EventArgs e)
        {
            btnConSelected.Enabled = false;
            selected_ip = "";
            progressConnect.Value = 0;
            btnStartScan.Enabled = false;
            //string my_address = GetMyIP();

            string[] b_address = "192.168.42.1".Split('.');
            //string[] b_address = txtStartIP.Text.Split('.');            
            string[] e_address = txtEndIP.Text.Split('.');
            if (b_address.Length != 4)
            {
                DialogResult result = MessageBox.Show("IP address is wrong!", "Important Note",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button1);
                btnStartScan.Enabled = true;
                return;
            }
            string body = b_address[0] + "." + b_address[1] + "." + b_address[2] + ".";

            progressConnect.Minimum = 0;
            progressConnect.Maximum = 100;
            list_str.Clear();


            //treeView1.Nodes.Clear();
            //Scan
            //byte[] bytes = new byte[1024];

            //IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(my_address), 7877);
            //Socket s = new Socket(endPoint.Address.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            //s.Bind(endPoint);

            //Receive("192.168.0.193");


            for (int i = 0; i < 254; i++)
            {
                string address = body + i.ToString();
                UDP_Listen udp = new UDP_Listen(address);
                Thread.Sleep(10);
                Application.DoEvents();
            }

            progressConnect.Minimum = 0;
            progressConnect.Maximum = 10;
            progressConnect.Value = 0;

            for (int i = 0; i < 10; i++)
            {
                Application.DoEvents();
                Thread.Sleep(1000);
                progressConnect.Value++;
            }
            progressConnect.Value = 0;
            Application.DoEvents();

            if (list_str.Count > 0)
            {
                foreach (string s in list_str)
                {
                    TreeNode n = treeCamera.Nodes[0];
                    //TreeNode tn = new TreeNode(s);
                    TreeNode tn = new TreeNode("name:JooVuuX");
                    n.Nodes.Add(tn);
                }
            }
            treeCamera.ExpandAll();
            btnStartScan.Enabled = true;
            return;

            //IPEndPoint snd = new IPEndPoint(IPAddress.Parse("192.168.0.193"), 7877);
            //EndPoint senderRemote = (EndPoint)snd;


            //while (index < max)
            //{
            //    string messageData = "";
            //    byte[] msg = new byte[256];
            //    string address = body + index.ToString();
            //    string error = "No error";
            //    address = "192.168.0.193";

            //    Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            //    sock.ReceiveTimeout = 300;
            //    sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
            //    sock.Connect(new IPEndPoint(IPAddress.Parse(address), 7877));
            //    //sock.Connect(new IPEndPoint(IPAddress.Parse("192.168.0.255"), 7877));
            //    sock.Send(System.Text.UTF8Encoding.UTF8.GetBytes("amba discovery"));

            //    try
            //    {
            //        int len = sock.ReceiveFrom(msg, SocketFlags.None, ref senderRemote);
            //        messageData = Encoding.ASCII.GetString(msg, 0, len);
            //    }
            //    catch (Exception ex)
            //    {
            //        error = "EndReceive";
            //    }
            //    if (error == "No error")
            //    {
            //        TreeNode n = treeView1.Nodes[0];
            //        string s = messageData + "-" + address;
            //        TreeNode tn = new TreeNode(s);
            //        n.Nodes.Add(tn);
            //    }
            //    sock.Close();
            //    index++;
            //    progressBar2.Value = index;
            //    Application.DoEvents();
            //}
        }
   
        private void buttonB_Click(object sender, EventArgs e)
        {
            rightTabCtrl.SelectedTab = tabPage5;
            LocationMedia media = new LocationMedia(textBox1.Text);
            vlcControl.Media = media;
            vlcControl.Play();
            
        }

        private void buttonE_Click(object sender, EventArgs e)
        {
            vlcControl.Stop();
            //axVLCPlugin21.playlist.stop();
        }

       
        private void btnSettingsAdvanced_Click(object sender, EventArgs e)
        {
            leftTabCtrl.SelectedTab = tabPageImgAdjust;
            cmbWhiteBalance.SelectedIndex = 0;
            cmbExposure.SelectedIndex = 6;
            cmbSharpness.SelectedIndex = 1;
            cmbContrast.SelectedIndex = 1;

            try
            {
                string White_balance = GetCurrentOption("wb");
                string Exposure = GetCurrentOption("Exposure");
                string Sharpness = GetCurrentOption("Sharpness");
                string Contrast = GetCurrentOption("Contrast");

                cmbWhiteBalance.Text = White_balance;
                cmbExposure.Text = Exposure;
                cmbSharpness.Text = Sharpness;
                cmbContrast.Text = Contrast;
            }
            catch { }
        }

        private void btnImageAdjustReload_Click(object sender, EventArgs e)
        {
            btnSettingsAdvanced_Click(sender, null);

        }

        private void comboBox35_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string response = SendSetting(cmbWhiteBalance.Text, "wb");
            }
            catch { }
        }

        private void comboBox34_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string response = SendSetting(cmbExposure.Text, "Exposure");
            }
            catch { }
        }

        private void comboBox33_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string response = SendSetting(cmbSharpness.Text, "Sharpness");
            }
            catch { }
        }

        private void comboBox32_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string response = SendSetting(cmbContrast.Text, "Contrast");
            }
            catch { }
        }

        private void comboBox36_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string response = SendSetting(cmbGsensor.Text, "g_sensor_sensitivity");
            }
            catch { }
        }

        private void comboBox31_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string response = SendSetting(cmbCarspeedUnit.Text, "speed_unit");
            }
            catch { }
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            CustomerAppPres settings = new CustomerAppPres();

            string dateFormat = cmbDateFormat.Text;
            byte dateFormatVal = 0;
            if (dateFormat == "YYYYMMDD")
                dateFormatVal = 0;
            else if (dateFormat == "DDMMYYYY")
                dateFormatVal = 1;
            else if (dateFormat == "MMDDYYYY")
                dateFormatVal = 2;
            settings.customer_press[CustomerAppPres.CustomerPresDateFormat] = dateFormatVal;

            string beepNoise = btnBeepNoises.Text;
            byte beepNoiseVal = 0;
            if (beepNoise == "ON")
                beepNoiseVal = 0;
            else if (beepNoise == "OFF")
                beepNoiseVal = 1;
            settings.customer_press[CustomerAppPres.CustomerPresBeepNoise] = beepNoiseVal;

            string recordLed = btnRecordingLed.Text;
            byte recordLedVal = 0;
            if (recordLed == "ON")
                recordLedVal = 0;
            else if (recordLed == "OFF")
                recordLedVal = 1;
            settings.customer_press[CustomerAppPres.CustomerPresRecordLED] = recordLedVal;

            string defaultMode = cmbDefaultModeCameraStart.Text;
            byte defaultModeVal = 0;
            if (defaultMode == "Mode1")
                defaultModeVal = 0;
            else if (defaultMode == "Mode2")
                defaultModeVal = 1;
            settings.customer_press[CustomerAppPres.CustomerPresDefaultMode] = defaultModeVal;

            string powerOnRecord = cmbPoweronAutoRecord.Text;
            byte powerOnRecordVal = 0;
            if (powerOnRecord == "Button only")
                powerOnRecordVal = 0;
            else if (powerOnRecord == "Auto Start")
                powerOnRecordVal = 1;
            settings.customer_press[CustomerAppPres.CustomerPresPowerOnRecord] = powerOnRecordVal;

            string standByTime = cmbStandbyTime.Text;
            byte standByTimeVal = 0;
            if (standByTime == "off")
                standByTimeVal = 0;
            else if (standByTime == "1 min")
                standByTimeVal = 3;
            else if (standByTime == "3 mins")
                standByTimeVal = 4;
            else if (standByTime == "5 mins")
                standByTimeVal = 5;
            settings.customer_press[CustomerAppPres.CustomerPresStandbyTime] = standByTimeVal;

            string powerOffDiscon = cmbPowerOffDisconnect.Text;
            byte powerOffDisconVal = 0;
            settings.customer_press[CustomerAppPres.CustomerPresPowerOffDisconnect] = powerOffDisconVal;

            string motionDet = btnMotionDetection.Text;
            byte motionDetVal = 0;
            if (motionDet == "OFF")
                motionDetVal = 0;
            else if (motionDet == "ON")
                motionDetVal = 1;
            settings.customer_press[CustomerAppPres.CustomerPresMotionDet] = motionDetVal;

            string motionDetSenti = cmbMotionDetection.Text;
            byte motionDetSentiVal = 0;
            if (motionDetSenti == "high")
                motionDetSentiVal = 0;
            else if (motionDetSenti == "medium")
                motionDetSentiVal = 1;
            else if (motionDetSenti == "low")
                motionDetSentiVal = 2;
            if (motionDetVal == 0) motionDetSentiVal = 0;
            settings.customer_press[CustomerAppPres.CustomerPresMotionDetSenti] = motionDetSentiVal;

            string tvType = cmbTvOut.Text;
            byte tvTypeVal = 0;
            if (tvType == "NTSC")
                tvTypeVal = 0;
            else if (tvType == "PAL")
                tvTypeVal = 1;
            settings.customer_press[CustomerAppPres.CustomerPresTVType] = tvTypeVal;

            string gSensor = btnGSensor.Text;
            byte gSensorVal = 0;
            if (gSensor == "OFF")
                gSensorVal = 0;
            else if (gSensor == "ON")
                gSensorVal = 1;
            settings.customer_press[CustomerAppPres.CustomerPresGsensor] = gSensorVal;

            string gSensorSenti = cmbGsensor.Text;
            byte gSensorSentiVal = 0;
            if (gSensorSenti == "High")
                gSensorSentiVal = 0;
            else if (gSensorSenti == "Medium")
                gSensorSentiVal = 1;
            else if (gSensorSenti == "Low")
                gSensorSentiVal = 2;
            settings.customer_press[CustomerAppPres.CustomerPresGsensorSenti] = gSensorSentiVal;

            string carNumberStamp = btnCarPlate.Text;
            byte carNumberStampVal = 0;
            if (carNumberStamp == "ON")
                carNumberStampVal = 0;
            else if (carNumberStamp == "OFF")
                carNumberStampVal = 1;
            settings.customer_press[CustomerAppPres.CustomerPresCarNumberStamp] = carNumberStampVal;

            string carSpeedStamp = btnSpeedStamp.Text;
            byte carSpeedStampVal = 0;
            if (carSpeedStamp == "ON")
                carSpeedStampVal = 0;
            else if (carSpeedStamp == "OFF")
                carSpeedStampVal = 1;
            settings.customer_press[CustomerAppPres.CustomerPresCarSpeedStamp] = carSpeedStampVal;

            string carSpeedUnit = cmbCarspeedUnit.Text;
            byte carSpeedUnitVal = 0;
            if (carSpeedUnit == "KPH")
                carSpeedUnitVal = 0;
            else if (carSpeedUnit == "MPH")
                carSpeedUnitVal = 1;
            settings.customer_press[CustomerAppPres.CustomerPresCarSpeedUnit] = carSpeedUnitVal;

            string timeMode = btnTimeMode.Text;
            byte timeModeVal = 0;
            if (timeMode == "OFF")
                timeModeVal = 0;
            else if (timeMode == "ON")
                timeModeVal = 1;
            settings.customer_press[CustomerAppPres.CustomerPresTimeMode] = timeModeVal;

            string startTime = cmbStartTime.Text;            
            settings.customer_press[CustomerAppPres.CustomerPresTimeModeStart] = Convert.ToByte(startTime);

            string endTime = cmbEndTime.Text;
            settings.customer_press[CustomerAppPres.CustomerPresTimeModeEnd] = Convert.ToByte(endTime);

            string resolType = cmbVideoResolution.Text;
            byte resolTypeVal = 0;
            if (resolType == "2560x1080 30P 21:9")
                resolTypeVal = 0;
            else if (resolType == "2304x1296 30P 16:9")
                resolTypeVal = 1;
            else if (resolType == "1920x1080 60P 16:9")
                resolTypeVal = 2;
            else if (resolType == "1920x1080 45P 16:9")
                resolTypeVal = 3;
            else if (resolType == "1920x1080 30P 16:9")
                resolTypeVal = 4;
            else if (resolType == "HDR 1920x1080 30P 16:9")
                resolTypeVal = 5;
            else if (resolType == "1280x720 60P 16:9")
                resolTypeVal = 6;
            else if (resolType == "1280x720 30P 16:9")
                resolTypeVal = 7;
            settings.customer_press[CustomerAppPres.CustomerPresResolutionType] = resolTypeVal;

            string audio = btnMode1Audio.Text;
            byte audioVal = 0;
            if (audio == "OFF")
                audioVal = 0;
            else if (audio == "ON")
                audioVal = 1;
            settings.customer_press[CustomerAppPres.CustomerPresAudio] = audioVal;

            string timeStamp = btnMode1VideoTimeStamp.Text;
            byte timeStampVal = 0;
            if (timeStamp == "ON")
                timeStampVal = 0;
            else if (timeStamp == "OFF")
                timeStampVal = 1;
            settings.customer_press[CustomerAppPres.CustomerPresTimeStamp] = timeStampVal;

            byte autoRotVal = (byte) cmbMode1ImageRotation.SelectedIndex;
            settings.customer_press[CustomerAppPres.CustomerPresAutoRotation] = autoRotVal;

            string loopRec = btnMode1LoopRecording.Text;
            byte loopRecVal = 0;
            if (loopRec == "OFF")
                loopRecVal = 0;
            else if (loopRec == "ON")
                loopRecVal = 1;
            settings.customer_press[CustomerAppPres.CustomerPresLoopRecord] = loopRecVal;

            string videoLen = cmbVideoClipLength.Text;
            byte videoLenVal = 0;
            if (videoLen == "continuous")
                videoLenVal = 0;
            else if (videoLen == "1 min")
                videoLenVal = 1;
            else if (videoLen == "2 min")
                videoLenVal = 2;
            else if (videoLen == "3 min")
                videoLenVal = 3;
            else if (videoLen == "5 min")
                videoLenVal = 4;
            else if (videoLen == "10 min")
                videoLenVal = 5;
            settings.customer_press[CustomerAppPres.CustomerPresVideoLength] = videoLenVal;

            string videoBitRate = cmbVideoBitRates.Text;
            byte videoBitRateVal = 0;
            if (videoBitRate == "High")
                videoBitRateVal = 0;
            else if (videoBitRate == "Medium")
                videoBitRateVal = 1;
            else if (videoBitRate == "Low")
                videoBitRateVal = 2;
            settings.customer_press[CustomerAppPres.CustomerPresVideoBitRate] = videoBitRateVal;

            string wdr = btnMode1WDR.Text;
            byte wdrVal = 0;
            if (wdr == "OFF")
                wdrVal = 0;
            else if (wdr == "ON")
                wdrVal = 1;
            settings.customer_press[CustomerAppPres.CustomerPresWDR] = wdrVal;

            string fieldView = cmbFieldofView.Text;
            byte fieldViewVal = 0;
            if (fieldView == "155")
                fieldViewVal = 0;
            else if (fieldView == "120")
                fieldViewVal = 1;
            else if (fieldView == "90")
                fieldViewVal = 2;
            else if (fieldView == "60")
                fieldViewVal = 3;
            settings.customer_press[CustomerAppPres.CustomerPresFieldOfView] = fieldViewVal;

            string timeLapse = cmbTimeLapseVideo.Text;
            byte timeLapseVal = 0;
            if (timeLapse == "off")
                timeLapseVal = 0;
            else if (timeLapse == "1_sec")
                timeLapseVal = 1;
            else if (timeLapse == "5_sec")
                timeLapseVal = 2;
            else if (timeLapse == "10_sec")
                timeLapseVal = 3;
            else if (timeLapse == "30_sec")
                timeLapseVal = 4;
            settings.customer_press[CustomerAppPres.CustomerPresTimeLapseVideo] = timeLapseVal;

            string resolType2 = cmbMode2VideoResolution.Text;
            byte resolTypeVal2 = 0;
            if (resolType2 == "2560x1080 30P 21:9")
                resolTypeVal2 = 0;
            else if (resolType2 == "2304x1296 30P 16:9")
                resolTypeVal2 = 1;
            else if (resolType2 == "1920x1080 60P 16:9")
                resolTypeVal2 = 2;
            else if (resolType2 == "1920x1080 45P 16:9")
                resolTypeVal2 = 3;
            else if (resolType2 == "1920x1080 30P 16:9")
                resolTypeVal2 = 4;
            else if (resolType2 == "HDR 1920x1080 30P 16:9")
                resolTypeVal2 = 5;
            else if (resolType2 == "1280x720 60P 16:9")
                resolTypeVal2 = 6;
            else if (resolType2 == "1280x720 30P 16:9")
                resolTypeVal2 = 7;
            settings.customer_press[CustomerAppPres.CustomerPresResolutionTypeMode2] = resolTypeVal2;

            string audio2 = btnMode2Audio.Text;
            byte audioVal2 = 0;
            if (audio2 == "OFF")
                audioVal2 = 0;
            else if (audio2 == "ON")
                audioVal2 = 1;
            settings.customer_press[CustomerAppPres.CustomerPresAudioMode2] = audioVal2;

            string timeStamp2 = btnMode2VideoTimeStamp.Text;
            byte timeStampVal2 = 0;
            if (timeStamp2 == "ON")
                timeStampVal2 = 0;
            else if (timeStamp2 == "OFF")
                timeStampVal2 = 1;
            settings.customer_press[CustomerAppPres.CustomerPresTimeStampMode2] = timeStampVal2;

            byte autoRotVal2 = (byte)cmbMode2ImageRotation.SelectedIndex;            
            settings.customer_press[CustomerAppPres.CustomerPresAutoRotationMode2] = autoRotVal2;

            string loopRec2 = btnMode2LoopRecording.Text;
            byte loopRecVal2 = 0;
            if (loopRec2 == "OFF")
                loopRecVal2 = 0;
            else if (loopRec2 == "ON")
                loopRecVal2 = 1;
            settings.customer_press[CustomerAppPres.CustomerPresLoopRecordMode2] = loopRecVal2;

            string videoLen2 = cmbMode2VideoClipLength.Text;
            byte videoLenVal2 = 0;
            if (videoLen2 == "continuous")
                videoLenVal2 = 0;
            else if (videoLen2 == "1 min")
                videoLenVal2 = 1;
            else if (videoLen2 == "2 min")
                videoLenVal2 = 2;
            else if (videoLen2 == "3 min")
                videoLenVal2 = 3;
            else if (videoLen2 == "5 min")
                videoLenVal2 = 4;
            else if (videoLen2 == "10 min")
                videoLenVal2 = 5;
            settings.customer_press[CustomerAppPres.CustomerPresVideoLengthMode2] = videoLenVal2;

            string videoBitRate2 = cmbMode2VideoBitRates.Text;
            byte videoBitRateVal2 = 0;
            if (videoBitRate2 == "High")
                videoBitRateVal2 = 0;
            else if (videoBitRate2 == "Medium")
                videoBitRateVal2 = 1;
            else if (videoBitRate2 == "Low")
                videoBitRateVal2 = 2;
            settings.customer_press[CustomerAppPres.CustomerPresVideoBitRateMode2] = videoBitRateVal2;

            string wdr2 = btnMode2WDR.Text;
            byte wdrVal2 = 0;
            if (wdr2 == "OFF")
                wdrVal2 = 0;
            else if (wdr2 == "ON")
                wdrVal2 = 1;
            settings.customer_press[CustomerAppPres.CustomerPresWDRMode2] = wdrVal2;

            string fieldView2 = cmbMode2FileldofView.Text;
            byte fieldViewVal2 = 0;
            if (fieldView2 == "155")
                fieldViewVal2 = 0;
            else if (fieldView2 == "120")
                fieldViewVal2 = 1;
            else if (fieldView2 == "90")
                fieldViewVal2 = 2;
            else if (fieldView2 == "60")
                fieldViewVal2 = 3;
            settings.customer_press[CustomerAppPres.CustomerPresFieldOfViewMode2] = fieldViewVal2;

            string timeLapse2 = cmbMode2TimeLapseVideo.Text;
            byte timeLapseVal2 = 0;
            if (timeLapse2 == "off")
                timeLapseVal2 = 0;
            else if (timeLapse2 == "1_sec")
                timeLapseVal2 = 1;
            else if (timeLapse2 == "5_sec")
                timeLapseVal2 = 2;
            else if (timeLapse2 == "10_sec")
                timeLapseVal2 = 3;
            else if (timeLapse2 == "30_sec")
                timeLapseVal2 = 4;
            settings.customer_press[CustomerAppPres.CustomerPresTimeLapseVideoMode2] = timeLapseVal2;

            settings.customer_press[CustomerAppPres.CustomerPresWhiteBalance] = (byte)cmbWhiteBalance.SelectedIndex;
            settings.customer_press[CustomerAppPres.CustomerPresSharpness] = (byte)cmbSharpness.SelectedIndex;
            settings.customer_press[CustomerAppPres.CustomerPresContrast] = (byte)cmbContrast.SelectedIndex;
            settings.customer_press[CustomerAppPres.CustomerPresExposure] = (byte)cmbExposure.SelectedIndex;

            string wifiPwd = txtWifiPassword.Text;
            settings.wifiPassword = new byte[wifiPwd.Length + 1];
            for (int i = 0; i < wifiPwd.Length; i++)
                settings.wifiPassword[i] = (byte)wifiPwd[i];

            string carnumber = txtCarNumber.Text;
            settings.carNumber = new byte[carnumber.Length + 1];
            for (int i = 0; i < carnumber.Length; i++)
                settings.carNumber[i] = (byte)carnumber[i];

            DateTime camera_d = dateTimePicker1.Value;
            string cameraDate = camera_d.ToString("yyyyMMdd");           
            settings.cameraDateBytes = new byte[cameraDate.Length + 1];
            for (int i = 0; i < cameraDate.Length; i++)
                settings.cameraDateBytes[i] = (byte)cameraDate[i];

            DateTime camera_t = dateTimePicker1.Value;
            string cameraTime = camera_t.ToString("HHmmss");
            settings.cameraTimeBytes = new byte[cameraTime.Length + 1];
            for (int i = 0; i < cameraTime.Length; i++)
                settings.cameraTimeBytes[i] = (byte)cameraTime[i];

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Text File(*.txt)|*.txt";
            sfd.FileName = "customer.txt";
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fileName = sfd.FileName;
                byte[] writeBytes = new byte[settings.customer_press.Length + settings.wifiPassword.Length + settings.carNumber.Length+ settings.cameraDateBytes.Length+ settings.cameraTimeBytes.Length];
                Array.Copy(settings.customer_press, 0, writeBytes, 0, settings.customer_press.Length);
                Array.Copy(settings.wifiPassword, 0, writeBytes, settings.customer_press.Length, settings.wifiPassword.Length);
                Array.Copy(settings.carNumber, 0, writeBytes, settings.customer_press.Length + settings.wifiPassword.Length, settings.carNumber.Length);
                Array.Copy(settings.cameraDateBytes, 0, writeBytes, settings.customer_press.Length + settings.wifiPassword.Length+ settings.carNumber.Length, settings.cameraDateBytes.Length);
                Array.Copy(settings.cameraTimeBytes, 0, writeBytes, settings.customer_press.Length + settings.wifiPassword.Length + settings.carNumber.Length+ settings.cameraDateBytes.Length, settings.cameraTimeBytes.Length);
                File.WriteAllBytes(fileName, writeBytes);
                MessageBox.Show("Successfully saved to customer.txt");
            }
        }

        private void btnCarNumber_Click(object sender, EventArgs e)
        {
            try
            {
                string response = SendSetting(txtCarNumber.Text, "car_number");
            }
            catch { }
        }

        private void btnConSelected_Click(object sender, EventArgs e)
        {
            btnVideo.FlatAppearance.BorderSize = 0;
            btnStream.FlatAppearance.BorderSize = 0;
            if (isConnection == false)
            {
                Amba_Connect();
                if(isConnection==true)
                    btnConSelected.Text = "Disconnect from JooVuuX";
            }
            else
            {
                Amba_DisConnect();
                btnConSelected.Text = "Connect to selected camera";
            }
           
        }

        private void enableToolButtons(bool isEnable)
        {
            btnFormat.Enabled = isEnable;
            btnGallery.Enabled = isEnable;
            btnPhoto.Enabled = isEnable;
            btnVideo.Enabled = isEnable;
            btnStream.Enabled = isEnable;
            btnInfo.Enabled = isEnable;
            btnSettingsMain.Enabled = isEnable;
            btnSettingsMode1.Enabled = isEnable;
            btnSettingsMode2.Enabled = isEnable;
            btnSettingsAdvanced.Enabled = isEnable;
        }

        private void treeCamera_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeCamera.SelectedNode == null) return;
            string s = treeCamera.SelectedNode.Text;
            if (s == "List of Cameras") return;
            s = list_str[treeCamera.SelectedNode.Index];
            string[] ip = s.Split('-');
            if (ip.Length > 0)
            {
                selected_ip = ip[1];
                btnConSelected.Enabled = true;
            }
        }

        private void cmbMode1ImageRotation_SelectedIndexChanged(object sender, EventArgs e)
        {
            string response = SendSetting(cmbMode1ImageRotation.Text, "rotate_video");
        }

        private void btnGalleryDelete_Click(object sender, EventArgs e)
        {
            string name = "/tmp/fuse_d/DCIM/100MEDIA/" + Select_Name_Files.Trim();
            Send260();
            deleteFile(name);
            FillFilesList("");
        }
        public string deleteFile(String name)
        {
            Amba_Ls json = new Amba_Ls();
            json.token = Convert.ToInt32(token);
            json.msg_id = 1281;
            json.param = name;

            string output = JsonConvert.SerializeObject(json);
            Byte[] SendBytes = Encoding.Default.GetBytes(output);
            mySocket.Send(SendBytes);

            // Получаем ответ от сервера
            byte[] bytes = new byte[1024];
            int bytesRec = mySocket.Receive(bytes);
            string responce = Encoding.UTF8.GetString(bytes, 0, bytesRec);

            //Проверяем ответ
            dynamic mData = JsonConvert.DeserializeObject(responce);
            msg_id = mData.msg_id;
            int rval = mData.rval;
            if (rval < 0) return "";  //Error
            return mData.param;
        }

        private void btnGalleryDownload_Click(object sender, EventArgs e)
        {
            vlcControl.Stop();
            ShowImage("", true);
        }

        private void btnGalleryPlay_Click(object sender, EventArgs e)
        {
            if (vlcControl.IsPlaying)
            {
                vlcControl.Pause();
                btnGalleryPlay.Text = "Play";
            }
            else if (vlcControl.IsPaused)
            {
                vlcControl.Play();
                btnGalleryPlay.Text = "Stop";
            }
            else
            {
                vlcControl.Stop();
                ShowImage("", false);
                btnGalleryPlay.Text = "Stop";
            }
        }

        private void cmbMode2ImageRotation_SelectedIndexChanged(object sender, EventArgs e)
        {
            string response = SendSetting(cmbMode1ImageRotation.Text, "rotate_video_mode2");
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                DateTime d = dateTimePicker1.Value;
                string fD = d.ToString("yyyyMMdd_HHmmss");
                string currDate = SendSetting(fD, "toggleSyncFateAndTime");
                string camera_time = GetCurrentOption("camera_time");

                string[] ar_dt = camera_time.Split('_');
                string yy = ar_dt[0].Substring(0, 4);
                string mm = ar_dt[0].Substring(4, 2);
                string dd = ar_dt[0].Substring(6, 2);
                string h = ar_dt[1].Substring(0, 2);
                string m = ar_dt[1].Substring(2, 2);
                string s = ar_dt[1].Substring(4, 2);
                DateTime time = new DateTime(int.Parse(yy), int.Parse(mm), int.Parse(dd), int.Parse(h), int.Parse(m), int.Parse(s));
                dateTimePicker1.Value = time;
                dateTimePicker2.Value = time;
            }
            catch (Exception)
            {

            }
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                DateTime d = dateTimePicker2.Value;
                string fD = d.ToString("yyyyMMdd_HHmmss");
                string currDate = SendSetting(fD, "toggleSyncFateAndTime");
                string camera_time = GetCurrentOption("camera_time");

                string[] ar_dt = camera_time.Split('_');
                string yy = ar_dt[0].Substring(0, 4);
                string mm = ar_dt[0].Substring(4, 2);
                string dd = ar_dt[0].Substring(6, 2);
                string h = ar_dt[1].Substring(0, 2);
                string m = ar_dt[1].Substring(2, 2);
                string s = ar_dt[1].Substring(4, 2);
                DateTime time = new DateTime(int.Parse(yy), int.Parse(mm), int.Parse(dd), int.Parse(h), int.Parse(m), int.Parse(s));
                dateTimePicker1.Value = time;
                dateTimePicker2.Value = time;
            }
            catch (Exception)
            {
            }
        }

        private void btnDirectConnect_Click(object sender, EventArgs e)
        {
            selected_ip = textBox6.Text;
            if (isConnection == false) Amba_Connect();
            else Amba_DisConnect();
        }

        //public void Receive(object obj)
        //{
        //    string ip = (String)obj;
        //    byte[] msg = new byte[256];
        //    IPEndPoint snd = new IPEndPoint(IPAddress.Parse(ip), 7877);
        //    EndPoint senderRemote = (EndPoint)snd;
        //    Thread.Sleep(300);

        //    if (ip == "192.168.0.193")
        //    {
        //        String a = "8";
        //    }
        //    Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        //    sock.ReceiveTimeout = 300;
        //    sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
        //    sock.Connect(new IPEndPoint(IPAddress.Parse(ip), 7877));
        //    sock.Send(System.Text.UTF8Encoding.UTF8.GetBytes("amba discovery"));

        //    string error = "No error";
        //    string messageData = "";
        //    try
        //    {
        //        int len = sock.ReceiveFrom(msg, SocketFlags.None, ref senderRemote);
        //        messageData = Encoding.ASCII.GetString(msg, 0, len);
        //        ip_list.Add(messageData);
        //    }
        //    catch (Exception ex)
        //    {
        //        error = "EndReceive";
        //    }
        //    //return messageData;
        //}

    }

    public class UDP_Listen
    {
        string ip = "0.0.0.0";
        //Socket socket;

        public UDP_Listen(string ip)
        {
            this.ip = ip;
            //this.socket = socket;
            Thread tr = new Thread(get_Receive);
            tr.Start();
        }

        public void get_Receive()
        {
            byte[] data = new byte[1024];
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint sender = new IPEndPoint(IPAddress.Parse(ip), 7877);
            EndPoint remoteIp = (EndPoint)sender;

            //Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            socket.ReceiveTimeout = 4000;
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
            socket.Connect(new IPEndPoint(IPAddress.Parse(ip), 7877));
            socket.Send(System.Text.UTF8Encoding.UTF8.GetBytes("amba discovery"));
            //EndPoint remoteIp = new IPEndPoint(IPAddress.Parse(ip), 7877);

            try
            {
                int len = socket.ReceiveFrom(data, SocketFlags.None, ref remoteIp);
                string stringData = Encoding.ASCII.GetString(data, 0, len);
                frmMain.list_str.Add(stringData + "-" + ip);
            }
            catch (Exception)
            {
                
            }
        }
    }



    class Amba_Start_Session
    {
        public string token { get; set; }
        public string msg_id { get; set; }
    }

    class Amba_Stop_Session
    {
        public int token { get; set; }
        public int msg_id { get; set; }
    }

    class Stream_Out_Type
    {
        public int token { get; set; }
        public int msg_id { get; set; }
        public string type { get; set; }
        public string param { get; set; }
    }

    class Amba_Ls
    {
        public int token { get; set; }
        public int msg_id { get; set; }
        public string param { get; set; }
    }

    class Amba_Cd
    {
        public int token { get; set; }
        public int msg_id { get; set; }
        public string param { get; set; }
    }

    class Amba_Get_File
    {
        public int token { get; set; }
        public int msg_id { get; set; }
        public string param { get; set; }
        public int offset { get; set; }
        public int fetch_size { get; set; }
    }

    class Amba_Take_Photo
    {
        public int token { get; set; }
        public int msg_id { get; set; }

        public Amba_Take_Photo()
        {
            this.token = 1;
            this.msg_id = 769;
        }
    }

    class Amba_Start_Record
    {
        public int token { get; set; }
        public int msg_id { get; set; }
    }

    class Amba_Stop_Record
    {
        public int token { get; set; }
        public int msg_id { get; set; }
    }

    public class Amba_Get_Space
    {
        public int token { get; set; }
        public int msg_id { get; set; }
        public string type { get; set; }
    }

    public class Amba_cType
    {
        public int token { get; set; }
        public int msg_id { get; set; }
        public string type { get; set; }
    }

    public class Amba_cParam
    {
        public int token { get; set; }
        public int msg_id { get; set; }
        public string type { get; set; }
        public string param { get; set; }
    }

    public class Device_Info
    {
        public string model { get; set; }
        public string chip { get; set; }
        public string fw_ver { get; set; }
        public string api_ver { get; set; }
    }

    public class Ls_Structure
    {
        public string path { get; set; }
        public string date { get; set; }
        public string id { get; set; }

        public Ls_Structure(string id, string path, string date)
        {
            this.id = id;
            this.path = path;
            this.date = date;
        }

        public Ls_Structure()
        {
            this.id = "0";
            this.path = "";
            this.date = "";
        }
    }


}
