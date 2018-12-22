using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace camSerializerDeserialzerLib
{

    [Serializable]
    public class matFrameWrapper : ISerializable
    {

        public matFrameWrapper()
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

        public matFrameWrapper(Mat f)
        {
            MyProperty = f;
        }

        // Implement this method to serialize data. The method is called 
        // on serialization.
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Use the AddValue method to specify serialized values.
            info.AddValue("props", myProperty_value, typeof(Mat));

        }

        // The special constructor is used to deserialize values.
        public matFrameWrapper(SerializationInfo info, StreamingContext context)
        {
            // Reset the property value using the GetValue method.
            myProperty_value = (Mat)info.GetValue("props", typeof(Mat));
        }
    }

}
