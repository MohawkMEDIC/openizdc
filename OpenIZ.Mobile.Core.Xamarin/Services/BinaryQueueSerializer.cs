using OpenIZ.Mobile.Core.Synchronization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Xamarin.Services
{
    /// <summary>
    /// Represets a serializer that goes to binary
    /// </summary>
    public class BinaryQueueSerializer : IQueueDataSerializer
    {
        /// <summary>
        /// Deserialize object
        /// </summary>
        public object DeserializeObject(byte[] data, Type type)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(data))
                return bf.Deserialize(ms);
        }

        /// <summary>
        /// Serialize object
        /// </summary>
        public byte[] SerializeObject(object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream()) {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
    }
}
