using cameraEndClient;
using camSerializerDeserialzerLib;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace camerEndClient
{
    public partial class Form1 : Form
    {
        private VideoCapture _capture = null;
        private camMemoryStreamSerializerDeserialzer _cMemSerDeser= null;

        private Mat _frame;

        private socketClientThread _sthClientConn = null;

        int PORT_NO = 5000;

        string SERVER_IP = "127.0.0.1";

        IFormatter _formatter = null;

        public Form1()
        {
            InitializeComponent();

            _formatter = new BinaryFormatter();
            _cMemSerDeser = new camMemoryStreamSerializerDeserialzer();

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
                //string fname = SerializerDeserialzer.SerializeFrame( _frame, _formatter ); // Serialize an instance of the class.
                
                matFrameWrapper t = new matFrameWrapper(_frame);
                byte[] stream = _cMemSerDeser.Serialize(t);
                try
                {
                    
                    _sthClientConn.sendMemoryStream(stream);
                }
                catch( Exception ex)
                {

                }


                //_frame = _cMemSerDeser.Deserialize<myFramWrapper>(stream).MyProperty;


             //   captureImageBox.Image = _frame;

            }
        }
        private void btStart_Click(object sender, EventArgs e)
        {
            this.richTextBox1.AppendText("Client : Streaming started..\r\n");
            _capture.Start();
        }

        private void btStop_Click(object sender, EventArgs e)
        {
            this.richTextBox1.AppendText("Client : Streaming paused..\r\n");
            _capture.Stop();
        }
        private void ReleaseData()
        {
            _capture.Stop();
            Thread.Sleep(2000);
            if (_capture != null)
                _capture.Dispose();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            // scroll it automatically
            richTextBox1.ScrollToCaret();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
                SERVER_IP = textBox1.Text;
            if (textBox2.Text != "")
                PORT_NO = Convert.ToInt32(textBox2.Text);

            _sthClientConn = new socketClientThread(this.btStart, this.richTextBox1, SERVER_IP, PORT_NO);
        }
    }
}
