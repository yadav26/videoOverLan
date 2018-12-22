

using camSerializerDeserialzerLib;
using Emgu.CV;
using Emgu.CV.UI;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace camerDisplayHost
{
    public class socketThread
    {
        private System.Windows.Forms.RichTextBox _rtb;
        private Emgu.CV.UI.ImageBox _cibox;
        private ManageDisplayControls _manageDispCtrl = null;
        TcpListener _listener = null;

        camMemoryStreamSerializerDeserialzer _camMemSerialDeserial = null;
        Thread thSocketListening = null;

        int _port;

        string _serverIP;

        webCamDisplayThread thWebCamDisplay = null;

        
        byte[] header = new byte[1024];
        //private int lastfilesize = 1024;
        byte[] buffer = new byte[1024];

        public socketThread(object rtb, object manageDisp, string addr, int port)
        {
            _serverIP = addr;
            _port = port;
            _rtb = (System.Windows.Forms.RichTextBox)rtb;

            _manageDispCtrl = (ManageDisplayControls)manageDisp;

            _camMemSerialDeserial = new camMemoryStreamSerializerDeserialzer();

            thSocketListening = new Thread(ListeningServerLaunchClientThreads);

            thSocketListening.Start();

            thWebCamDisplay = new webCamDisplayThread(_cibox);

        }

        public byte[] SubArray(byte[] data, int index, int length)
        {
            byte[] result = new byte[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        private void networkByteStreamToMatFrame(object sock)
        {
            Socket socket = (Socket)sock;

            byte[] extaLeftOver = { };

            bool done = false;
            int lastfilesize = 0;

            SetText("Server[" + Thread.CurrentThread.ManagedThreadId + "] : Streaming Started..\r\n");

            displayControl dc = (displayControl)_manageDispCtrl.getAvailableDisplayNotInUse();
            if( dc == null )
            {
                SetText("Server[" + Thread.CurrentThread.ManagedThreadId + "] : All screens occupied cannot add more connections. **ERROR** ..\r\n");
                return;
            }
            _cibox = (dc)._cibox;
            _manageDispCtrl.setDisplayIndexInUSe(_cibox);

            SetText("Server[" + Thread.CurrentThread.ManagedThreadId + "] : delegated to screen  ["+ dc.id +"]..\r\n");

            //ImageBox im3 = ((displayControl)_manageDispCtrl.getAvailableDisplayNotInUse())._cibox;

            //_manageDispCtrl.setDisplayIndexInUSe(im3);

            //ImageBox im4 = ((displayControl)_manageDispCtrl.getAvailableDisplayNotInUse())._cibox;

            //_manageDispCtrl.setDisplayIndexInUSe(im4);

            while (!done)
            {
                Dictionary<string, string> headers = new Dictionary<string, string>();
                
                if (extaLeftOver.Length == 0)
                {
                    try
                    {
                        socket.Receive(header);
                    }
                    catch (ObjectDisposedException ode)
                    {
                        done = true;
                    }
                    catch (SocketException se)
                    {
                        if (se.ErrorCode == 10054)
                        {
                            socket.Close();
                        }

                        done = true;
                    }
                }
                else
                {
                    byte[] headerExtracted = SubArray(extaLeftOver, 0, 1024); // assuming constant hearder size 1024
                    byte[] streamBuffer = SubArray(extaLeftOver, 1024, extaLeftOver.Length - 1024);

                }

                //SetText("Server[" + Thread.CurrentThread.ManagedThreadId + "] : Header received..\r\n");

                string headerStr = Encoding.ASCII.GetString(header);


                string[] splitted = headerStr.Split(new string[] { "\r\n" }, StringSplitOptions.None);

                foreach (string s in splitted)
                {
                    if (s.Contains(":"))
                    {
                        headers.Add(s.Substring(0, s.IndexOf(":")), s.Substring(s.IndexOf(":") + 1));
                    }

                }


                try
                {
                    int filesize = Convert.ToInt32(headers["Content-length"]);
                    //Get filesize from header

                    if (filesize != lastfilesize)
                    {
                        SetText("Server[" + Thread.CurrentThread.ManagedThreadId + "] : Updated file size = " + filesize + "..\r\n");
                        Array.Resize(ref buffer, filesize);
                    }


                    lastfilesize = filesize;

                    byte[] temp = { };

                    int recvsize = 0;
                    while (recvsize < filesize)
                    {
                        int size = socket.Receive(buffer, SocketFlags.None);
                        if (size > filesize - recvsize)
                        {
                            int wastedBytes = (size - (filesize - recvsize));
                            //SetText("Server[" + Thread.CurrentThread.ManagedThreadId + "] : Data Wasted = " + wastedBytes + "....\r\n");

                            size = filesize - recvsize;
                            extaLeftOver = SubArray(buffer, size - 1, wastedBytes);
                        }

                        temp = temp.Concat(buffer.Take(size)).ToArray();
                        recvsize += size;
                    }

                    
                    _cibox.Image = _camMemSerialDeserial.Deserialize<matFrameWrapper>(temp).MyProperty;

                    string ackString = "DONE";
                    byte[] ackFrame = new byte[ackString.Length];
                    Array.Copy(Encoding.ASCII.GetBytes(ackString), ackFrame, Encoding.ASCII.GetBytes(ackString).Length);

                    socket.Send(ackFrame);

                }
                catch (Exception e)
                {
                    continue;
                }


            }

            SetText("Server[" + Thread.CurrentThread.ManagedThreadId + "] : Streaming Stopped..\r\n");
            _manageDispCtrl.ResetDisplayIndexInUSe(_cibox);

        }

        private void ListeningServerLaunchClientThreads()
        {

            TcpListener listener = new TcpListener(IPAddress.Any, _port);

            bool done_listening = false;
            while (!done_listening)
            {
                SetText("Server : Launched listening at port = " + _port + "..\r\n");
                listener.Start();

                Socket socket = listener.AcceptSocket();
                socket.ReceiveTimeout = 60000000;

                SetText("Server : Client connection accepted..\r\n");

                displayControl dc = (displayControl)_manageDispCtrl.getAvailableDisplayNotInUse();
                if (dc == null)
                {
                    SetText("Server[" + Thread.CurrentThread.ManagedThreadId + "] : All screens occupied cannot add more connections. **ERROR** ..\r\n");
                    continue;
                }

                Thread thNewClient = new Thread(networkByteStreamToMatFrame);
                thNewClient.Start(socket);
            }

        }


        private void SetText(string v)
        {
            _rtb.Invoke(new Action(() => _rtb.AppendText(v)));
        }

    }
}
