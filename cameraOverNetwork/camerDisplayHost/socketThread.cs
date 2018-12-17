using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace camerDisplayHost
{
    public class socketThread
    {
        private System.Windows.Forms.RichTextBox _rtb;

        Thread thSocketListening = null;

        int _port;

        string _serverIP;

        public socketThread(object rtb, string addr, int port)
        {
            _rtb = ( System.Windows.Forms.RichTextBox)rtb;

            thSocketListening = new Thread(threadProc);

            _serverIP = addr;

            _port = port;

            thSocketListening.Start();

        }

        private void threadProc()
        {
            string Text = "\n Started ...";
            _rtb.Invoke(new Action(() => _rtb.AppendText( Text)));

            //_rtb.AppendText("\n Started...");

            //---listen at the specified IP and port no.---
            IPAddress localAdd = IPAddress.Parse(_serverIP);
            TcpListener listener = new TcpListener(localAdd, _port);

            Text = "\n Server is Listening...";
            _rtb.Invoke(new Action(() => _rtb.AppendText(Text)));
            listener.Start();

            //---incoming client connected---
            TcpClient client = listener.AcceptTcpClient();
            
            Text = "\n Client accepted...";
            _rtb.Invoke(new Action(() => _rtb.AppendText(Text)));

            //---get the incoming data through a network stream---

            byte[] buffer = new byte[client.ReceiveBufferSize];

            int counter = 0;
            while (++counter < 10)
            {
                NetworkStream nwStream = client.GetStream();
                //---read incoming stream---
                int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);

                //---convert the data received into a string---
                string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                Text = "\n Server Received : " + dataReceived;
                _rtb.Invoke(new Action(() => _rtb.AppendText(Text)));


                //---write back the text to the client---

                Text = "\n Server Sending back : " + dataReceived;
                _rtb.Invoke(new Action(() => _rtb.AppendText(Text)));

                nwStream.Write(buffer, 0, bytesRead);

                //Thread.Sleep(2000);

            }

            client.Close();

            listener.Stop();

            Text = "\n Connection Closed...";
            _rtb.Invoke(new Action(() => _rtb.AppendText(Text)));
        }
    }
}
