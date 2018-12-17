using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace camerDisplayHost
{
    public partial class Form1 : Form
    {


        webCamDisplayThread thWebCamDisplay = null;
        socketThread thSocketConn = null;

        const int PORT_NO = 5000;
        const string SERVER_IP = "127.0.0.1";
        public Form1()
        {
            InitializeComponent();
            thWebCamDisplay = new webCamDisplayThread(this.captureImageBox);

            thSocketConn = new socketThread(this.richTextBox1, SERVER_IP, PORT_NO);
        }
        public void destroyCam()
        {
            thWebCamDisplay.destroyCam();
            
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }
    }
}
