using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace camSerializerDeserialzerLib
{
    public class camMemoryStreamSerializerDeserialzer
    {
        private BinaryFormatter _binSerializer = null;

        public camMemoryStreamSerializerDeserialzer()
        {
            _binSerializer = new BinaryFormatter();
        }


        // Serialize collection of any type to a byte stream
        public byte[] Serialize<T>(T obj)
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                _binSerializer.Serialize(memStream, obj);
                return memStream.ToArray();
            }
        }

        // DSerialize collection of any type to a byte stream
        public T Deserialize<T>(byte[] serializedObj)
        {
            T obj = default(T);
            try
            {
                using (MemoryStream memStream = new MemoryStream(serializedObj))
                {
                    memStream.Position = 0;
                    obj = (T)_binSerializer.Deserialize(memStream);
                    memStream.Flush();
                }
                

            }
            catch ( Exception e)
            {
                
            }
            return obj;
        }
    }

}
