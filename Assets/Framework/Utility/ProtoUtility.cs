// ========================================================
// 描 述：ProtoUtility.cs 
// 作 者：郑贤春 
// 时 间：2017/02/23 15:49:18 
// 版 本：5.4.1f1 
// ========================================================
using System;
using System.IO;

namespace Framework.Utility
{
    public class ProtoUtility
    {
        public static T FileToProtoData<T>(string path) where T : ProtoBuf.IExtensible
        {
            using (FileStream stream = File.OpenRead(path))
            {
                T t = ProtoBuf.Serializer.Deserialize<T>(stream);
                return t;
            }
        }

        public static object FileToProtoData(string path,Type type)
        {
            using (FileStream stream = File.OpenRead(path))
            {
                object obj = ProtoBuf.Serializer.Deserialize(type,stream);
                return obj;
            }
        }

        public static void ProtoDataToFile(string path,object proto)
        {
            using (FileStream stream = File.Create(path))
            {
                ProtoBuf.Serializer.Serialize(stream, proto);
            }
        }

        public static void ProtoDataToFile<T>(string path,T proto)
        {
            using (FileStream stream = File.Create(path))
            {
                ProtoBuf.Serializer.Serialize(stream, proto);
            }
        }
    }
}
