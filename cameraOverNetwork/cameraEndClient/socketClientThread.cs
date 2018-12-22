using Emgu.CV;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace camerEndClient
{
    public class socketClientThread
    {
        private System.Windows.Forms.RichTextBox _rtb;
        private System.Windows.Forms.Button _btStart = null;
        
        TcpClient _client = null;
        
        BinaryFormatter _formatter = null;

        byte[] header = new byte[1024];

        int _port;
        string _serverIP;
        private int _packetCounter = 0;


        public byte[] SubArray(byte[] data, int index, int length)
        {
            byte[] result = new byte[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }


        public void destroyConnection()
        {
            _client.Client.Close();
            _client = null;
        }


        private void SetText(string v)
        {
            _rtb.Invoke(new Action(() => _rtb.AppendText(v)));
        }
        public TcpClient TryConnecting(int counter, string addr, int port)
        {
            try
            {
                _client = new TcpClient(_serverIP, _port);

                SetText("Client: Successfully connected to display IP = " + _serverIP + ", Port = " + _port + "..\r\n");
            }
            catch (SocketException se)
            {
                if (se.ErrorCode == 10061)
                {
                    try
                    {
                        SetText("Client: Server not running....retry attempt = " + counter + "\r\n");

                    }
                    catch (System.InvalidOperationException e)
                    {

                    }

                }
            }
            return _client;

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

            _client.SendTimeout = 60000000;
            _client.ReceiveTimeout = 600000;

            _btStart.Invoke(new Action(() => _btStart.Enabled = true));

        }

        public socketClientThread(object btStart, object rtb, string addr, int port)
        {
            _btStart = (System.Windows.Forms.Button)btStart;
            _rtb = (System.Windows.Forms.RichTextBox)rtb;

            _formatter = new BinaryFormatter();

            SetText("Client : Launching to connect to = " + addr + ", Port = " + port + "..\r\n");

            _serverIP = addr;
            _port = port;

            TryConnecting(0, _serverIP, _port);
            if (null == _client)
            {
                Thread t = new Thread(serverPollingThread);
                t.Start();
            }

        }

        internal void sendMemoryStream(byte[] stream)
        {
            if (null == _client)
                return;


            string headerStr = "Content-length:" + stream.Length.ToString();
            Array.Copy(Encoding.ASCII.GetBytes(headerStr), header, Encoding.ASCII.GetBytes(headerStr).Length);


            _client.Client.Send(header);
            //SetText("Client : Sent header filesize"+stream.Length+". Header length = " + header.Length + "\r\n");

            //int bufferSize = 1024;

            //int bufferCount = Convert.ToInt32(Math.Ceiling((double)stream.Length / (double)bufferSize));
            //byte[] buffer = new byte[bufferSize];

            //int totalBytesSent = 0;
            
            //for (int i = 0; i < bufferCount; i++)
            //{
            //    if (stream.Length - totalBytesSent < bufferSize)
            //        bufferSize = stream.Length - totalBytesSent;

            //    buffer = SubArray(stream, i*bufferSize, bufferSize);

            //    _client.Client.Send(buffer, buffer.Length, SocketFlags.Partial);

            //    totalBytesSent += bufferSize;
            //}

            _client.Client.Send(stream, stream.Length, SocketFlags.None);

            _client.GetStream().Flush();
            byte[] bufferAck = new byte[16];           


            int sz = _client.Client.Receive(bufferAck);
            bufferAck = SubArray(bufferAck, 0, sz);
            string result = System.Text.Encoding.UTF8.GetString(bufferAck);

            if ( result == "DONE")
            {
                //SetText("Client : File Transfer Successful..\r\n");
            }
            

        }


    }
}
