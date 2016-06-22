using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace JooVuuX
{
    public partial class FirstNotification : Form
    {
        public FirstNotification()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            frmMain frm1 = new frmMain();
            frm1.isSettings = false;
            frm1.ShowDialog();  
                      
        }

        private void button2_Click(object sender, EventArgs e)
        {
            frmMain frm1 = new frmMain();
            frm1.isSettings = true;           
            frm1.ShowDialog(this);                   
        }
         
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.joovuu.com/community");
        }

        private void FirstNotification_Load(object sender, EventArgs e)
        {
           webBrowser1.Navigate("http://www.joovuu.com/firmware.php");            
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.joovuu-x.com/submit-ticket");
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.joovuu-x.com/knowledgebase/");
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.joovuu-x.com/software-apps/");
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            //scrape latest firmware
            WebBrowser wb = (WebBrowser)sender;
            HtmlElementCollection inputCol = wb.Document.GetElementsByTagName("body");
            foreach (HtmlElement el in inputCol)
            {               
                String strHtml = el.InnerText;                    
                linkLabel4.Text = "Latest Firmware: " + strHtml;
                break;              

            }
        }
    }
}
