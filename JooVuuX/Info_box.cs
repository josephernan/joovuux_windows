using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace JooVuuX
{
    public partial class Info_box : Form
    {
        public string fName = "";
        public ulong size_File = 0;
        public string selected_ip = "";
        public Socket File_Socket = null;
        public string fPath = "C:\\temp\\";
        public bool isCopy = true;

        public Info_box()
        {
            InitializeComponent();
        }

        private void Info_box_Load(object sender, EventArgs e)
        {
            textBox1.Text = fPath;
            this.Text = fName;
            this.progressBar1.Value = 0;
            this.progressBar1.Maximum = (int)size_File;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //folderBrowserDialog1.RootFolder = textBox1.Text;
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                //string[] files = Directory.GetFiles(folderBrowserDialog1.SelectedPath);
                textBox1.Text = folderBrowserDialog1.SelectedPath + "\\";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //IPEndPoint EndPoint = new IPEndPoint(IPAddress.Parse(selected_ip), 8787);
            //Socket File_Socket = new Socket(EndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            //File_Socket.Connect(EndPoint);

            //File.Delete("C:\\temp\\" + fName);
            //calc download speed
            DateTime started = DateTime.Now;
            if (textBox1.Text.Substring(textBox1.Text.Length - 1) != "\\") textBox1.Text = textBox1.Text + "\\";
            fPath = textBox1.Text;
            File.Delete(textBox1.Text + fName);
            byte[] buffer_1 = new byte[1024]; //Буфер для файла
            FileStream stream = new FileStream(textBox1.Text + fName, FileMode.CreateNew, FileAccess.Write);
            BinaryWriter f = new BinaryWriter(stream);
            ulong processed = 0; //Байт принято
            isCopy = true;
            while ((processed < size_File) && (isCopy == true)) //Принимаем файл
            {
                if ((size_File - processed) < 1024)
                {
                    int bytes_f = (int)(size_File - processed);
                    byte[] buf = new byte[bytes_f];
                    bytes_f = File_Socket.Receive(buf);
                    f.Write(buf, 0, bytes_f);
                    processed = processed + (ulong)bytes_f;
                    progressBar1.Value = (int)processed;
                    Application.DoEvents();
                }
                else
                {
                    int bytes_f = File_Socket.Receive(buffer_1);
                    f.Write(buffer_1, 0, bytes_f);
                    processed = processed + (ulong)bytes_f;
                    progressBar1.Value = (int)processed;
                    Application.DoEvents();
                }
                TimeSpan elapsedTime = DateTime.Now - started;
                TimeSpan estimatedTime = TimeSpan.FromSeconds((size_File - processed) / ((double)processed / elapsedTime.TotalSeconds));
                lblEstimation.Text = "Time Remaining: " + Convert.ToInt32(estimatedTime.TotalSeconds)+" Seconds, "+ Convert.ToInt32(processed /1024)+"/"+ Convert.ToInt32(size_File /1024)+" Kbytes";
            }
            if (f != null)
            {
                f.Close();
            }
            stream.Close();


            //// Получаем ответ от сервера
            //int bytesRec = mySocket.Receive(bytes);
            //string responce = Encoding.UTF8.GetString(bytes, 0, bytesRec);



            //Image img = new Bitmap("C:\\temp\\" + sName);
            //pictureBox1.Image = new Bitmap(img);
            //img.Dispose();
            File_Socket.Close();

            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void Info_box_Shown(object sender, EventArgs e)
        {
            textBox1.Text = fPath;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.DoEvents();
            isCopy = false;
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
