using Emgu.CV.UI;
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
        const int _totalImageContainers = 4;

        ManageDisplayControls _manageDC = null;

        socketThread thSocketConn = null;

        const int PORT_NO = 5000;
        const string SERVER_IP = "127.0.0.1";

        public Form1()
        {
            InitializeComponent();

            _manageDC = new ManageDisplayControls();
            _manageDC.InitializeDispCtrls(this.splitContainer1.Panel1, new ImageBox());
            _manageDC.InitializeDispCtrls(this.splitContainer1.Panel2, new ImageBox());
            _manageDC.InitializeDispCtrls(this.splitContainer2.Panel1, new ImageBox());
            _manageDC.InitializeDispCtrls(this.splitContainer2.Panel2, new ImageBox());

        }

        public void destroyCam()
        {
           // thWebCamDisplay.destroyCam();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            thSocketConn = new socketThread(this.richTextBox1, _manageDC, SERVER_IP, PORT_NO);
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
