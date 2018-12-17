using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace camerEndClient
{
    public class socketClientThread
    {
        private System.Windows.Forms.RichTextBox _rtb;
        private System.Windows.Forms.Button _btStart = null;
        Thread thSocketListening = null;
        TcpClient _client = null;
        NetworkStream _nwStream = null;

        int _port;
        string _serverIP;
        private int _packetCounter = 0;

        public TcpClient TryConnecting(int counter, string addr, int port)
        {
            try
            {
                _client = new TcpClient(_serverIP, _port);
            }
            catch (SocketException se)
            {
                if (se.ErrorCode == 10061)
                {
                    _rtb.Invoke(new Action(() => _rtb.AppendText("\n Server not running....retry attempt= " + counter)));
                }
            }
            return _client;

        }

        public socketClientThread(object btStart, object rtb, string addr, int port)
        {
            _btStart = (System.Windows.Forms.Button)btStart;
            _rtb = (System.Windows.Forms.RichTextBox)rtb;


            _serverIP = addr;

            _port = port;

            TryConnecting(0, _serverIP, _port);
            if (null == _client)
            {
                Thread t = new Thread(serverPollingThread);
                t.Start();
            }

        }

        public void destroyConnection()
        {
            _client.Close();
            _client = null;
        }


        public void serverPollingThread()
        {
            if (_client == null)
            {
                int counter = 0;
                while (_client == null)
                {
                    TryConnecting(++counter, _serverIP, _port);
                    Thread.Sleep(3000);
                }
            }

            _btStart.Invoke(new Action(() => _btStart.Enabled=true ));

            _rtb.Invoke(new Action(() => _rtb.AppendText("\n Client connected Successfully...." )));

        }

        public void launchClientConnectionThread()
        {
            thSocketListening.Start();
        }

        public bool sendFrameToServer(string frame)
        {
            string Text = "\n Client connected Successfully ...";
            //---create a TCPClient object at the IP and port no.---
            _nwStream = _client.GetStream();

            _packetCounter++;
            //while (++counter < 10)

            // string textToSend = DateTime.Now.ToString();
            byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(frame);

            //---send the text---
            Text = "\n Sending Packet : " + _packetCounter;
            _rtb.Invoke(new Action(() => _rtb.AppendText(Text)));


            _nwStream.Write(bytesToSend, 0, bytesToSend.Length);

            //---read back the text---
            byte[] bytesToRead = new byte[_client.ReceiveBufferSize];
            int bytesRead = _nwStream.Read(bytesToRead, 0, _client.ReceiveBufferSize);

            Text = "\n Received : " + Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);
            _rtb.Invoke(new Action(() => _rtb.AppendText(Text)));

            //Thread.Sleep(2000);


            return true;
        }

        private void sendRecieveThreadServer()
        {

            string Text = "\n Client connected Successfully ...";
            //---create a TCPClient object at the IP and port no.---
            NetworkStream nwStream = _client.GetStream();

            int counter = 0;
            while (++counter < 10)
            {
                string textToSend = DateTime.Now.ToString();
                byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(textToSend);

                //---send the text---
                Text = "\n Sending Packet : " + counter;
                _rtb.Invoke(new Action(() => _rtb.AppendText(Text)));


                nwStream.Write(bytesToSend, 0, bytesToSend.Length);

                //---read back the text---
                byte[] bytesToRead = new byte[_client.ReceiveBufferSize];
                int bytesRead = nwStream.Read(bytesToRead, 0, _client.ReceiveBufferSize);

                Text = "\nReceived : " + Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);
                _rtb.Invoke(new Action(() => _rtb.AppendText(Text)));

                Thread.Sleep(2000);

            }


            _rtb.Invoke(new Action(() => _rtb.AppendText("\n Connection Closed...")));

            _client.Close();






        }
    }
}
