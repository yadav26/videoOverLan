

using Emgu.CV;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
// Add references to Soap and Binary formatters.
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace camerEndClient
{

    [Serializable]
    public class myFramWrapper : ISerializable
    {
        
        public myFramWrapper()
        {
            // Empty constructor required to compile.
        }

    // The value to serialize.
    private Mat myProperty_value;

    public Mat MyProperty
    {
        get { return myProperty_value; }
        set { myProperty_value = value; }
    }
	
        public myFramWrapper(Mat f)
        {
           
        }
        
        // Implement this method to serialize data. The method is called 
        // on serialization.
    public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Use the AddValue method to specify serialized values.
        info.AddValue("props", myProperty_value, typeof(Mat));

        }

        // The special constructor is used to deserialize values.
        public myFramWrapper(SerializationInfo info, StreamingContext context)
        {
            // Reset the property value using the GetValue method.
            myProperty_value = (Mat)info.GetValue("props", typeof(Mat));
        }
    }

    // This is a console application. 
    public static class SerializerDeserialzer
    {
        private static FileStream sfstream = null;
        private static FileStream sOfstream = null;

        private static IFormatter _formatter = null;
        private static string fileName = "dataStuff.myData";
        static FileStream getStreamCreateMode()
        {
            if (sfstream == null)
                sfstream = new FileStream(fileName, FileMode.Create);

            return sfstream;
        }

        static FileStream getStreamOpenMode()
        {
            if (sOfstream == null)
                sOfstream = new FileStream(fileName, FileMode.Open);

            return sOfstream;
        }

        static IFormatter getFormatterHandle()
        {
            if (_formatter == null)
                _formatter = new BinaryFormatter();

            return _formatter;
        }

        static void TestFunction(Mat frame)
        {
            // This is the name of the file holding the data. You can use any file extension you like.
            string fileName = "dataStuff.myData";

            // Use a BinaryFormatter or SoapFormatter.
            IFormatter formatter = new BinaryFormatter();
            //IFormatter formatter = new SoapFormatter();

            SerializerDeserialzer.SerializeItem(fileName, formatter, frame); // Serialize an instance of the class.
            SerializerDeserialzer.DeserializeItem(fileName, formatter); // Deserialize the instance.
            //Console.WriteLine("Done");
            //Console.ReadLine();
        }

        public static void SerializeItem(string fileName, IFormatter formatter, Mat frame)
        {
            // Create an instance of the type and serialize it.
            myFramWrapper t = new myFramWrapper();
            t.MyProperty = frame;

            FileStream s = new FileStream(fileName, FileMode.Create);
            formatter.Serialize(s, t);
            s.Close();
        }


        public static Mat DeserializeItem(string fileName, IFormatter formatter)
        {
            FileStream s = new FileStream(fileName, FileMode.Open);
            myFramWrapper t = (myFramWrapper)formatter.Deserialize(s);
            //Console.WriteLine(t.MyProperty);
            s.Close();
            return t.MyProperty;
        }

        public static void SerializeFrame( Mat frame, IFormatter formatter)
        {
            //IFormatter formatter = new BinaryFormatter();
            SerializeItem(fileName, formatter, frame);
        }

        public static Mat DeSerializeFrame(IFormatter formatter)
        {
            //IFormatter formatter = new BinaryFormatter();
            return DeserializeItem(fileName, formatter);
        }

    }
}
