// ========================================================
// 描 述：AsyncSocketMarshalCoder.cs 
// 作 者：郑贤春 
// 时 间：2019/06/15 15:57:23 
// 版 本：2018.3.12f1 
// ========================================================
using UnityEngine;
using System.Collections;

namespace Framework.Net
{
    public class AsyncSocketMarshalCoder : AsyncSocketDataCoder<MarshalStruct>
    {
        public MarshalStruct Decode(byte[] bytes)
        {
            object data = MarshalBuffer.ByteToStructData(bytes, typeof(MarshalStruct));
            MarshalStruct msg = (MarshalStruct)data;
            Debug.Log(string.Format("接收到服务器消息type{0},data{1}", msg.type, msg.data));
            return msg;
        }

        public byte[] Encode(MarshalStruct obj)
        {
            byte[] bytes = MarshalBuffer.StructToByteData(obj);
            return bytes;
        }
    }
}

