using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace camerDisplayHost
{
    public class webCamDisplayThread
    {
        cameraDisplayClass webcam = null;
        Thread thDisplay = null;
        public webCamDisplayThread(object ci)
        {
            thDisplay = new Thread(webCamDisplayThread.threadProc);
            thDisplay.Start(ci);
        }

        private static void threadProc(object ci)
        {
            cameraDisplayClass webcam = new cameraDisplayClass(ci);
            webcam.startcamera();
        }

        public void destroyCam()
        {
            webcam.ReleaseData();

            webcam.stopcamera();

            if (thDisplay!=null)
                thDisplay.Abort();

        }

    }
}
