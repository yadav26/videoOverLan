using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace camerEndClient
{
    public partial class Form1 : Form
    {
        private VideoCapture _capture = null;
        private Mat _frame;
        private socketClientThread _sthClientConn = null;
        const int PORT_NO = 5000;
        const string SERVER_IP = "127.0.0.1";

        public Form1()
        {
            InitializeComponent();

            CvInvoke.UseOpenCL = false;
            try
            {
                _capture = new VideoCapture();
                _capture.ImageGrabbed += ProcessFrame;
            }
            catch (NullReferenceException excpt)
            {
                MessageBox.Show(excpt.Message);
            }
            _frame = new Mat();
        }
        private void ProcessFrame(object sender, EventArgs arg)
        {
            if (_capture != null && _capture.Ptr != IntPtr.Zero)
            {
                _capture.Retrieve(_frame, 0);
                
                _sthClientConn .sendFrameToServer(_frame.ToString());
                captureImageBox.Image = _frame;

            }
        }
        private void btStart_Click(object sender, EventArgs e)
        {
            _capture.Start();
        }

        private void btStop_Click(object sender, EventArgs e)
        {
            _capture.Stop();
        }
        private void ReleaseData()
        {
            if (_capture != null)
                _capture.Dispose();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _sthClientConn = new socketClientThread(this.btStart, this.richTextBox1, SERVER_IP, PORT_NO);
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            // scroll it automatically
            richTextBox1.ScrollToCaret();
        }
    }
}
