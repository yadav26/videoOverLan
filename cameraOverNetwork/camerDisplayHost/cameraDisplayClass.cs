using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;


using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.Util;


namespace camerDisplayHost
{
    public class cameraDisplayClass
    {
        private VideoCapture _capture = null;
        private Mat _frame;
        private Emgu.CV.UI.ImageBox captureImageBox;

        public cameraDisplayClass(object cibox)
        {
            captureImageBox = (Emgu.CV.UI.ImageBox)cibox;
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
                captureImageBox.Image = _frame;

            }
        }
        public void ReleaseData()
        {
            if (_capture != null)
                _capture.Dispose();
        }

        public void startcamera()
        {
            _capture.Start();
        }
        public void stopcamera()
        {
            _capture.Stop();
        }
    }
}
