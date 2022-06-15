using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;

namespace Framework.Net
{
    public class MarshalBuffer
    {
        public static byte[] StructToByteData(object stuctObj)
        {
            int size = Marshal.SizeOf(stuctObj); //返回对象的非托管大小（以字节为单位）  
            IntPtr buffer = Marshal.AllocHGlobal(size); //通过使用指定的字节数，从进程的非托管内存中分配内存  
            try
            {
                Marshal.StructureToPtr(stuctObj, buffer, false); //将数据从托管对象封送到非托管内存块  
                byte[] bytes = new byte[size];
                Marshal.Copy(buffer, bytes, 0, size); //将数据从非托管内存指针复制到托管8位无符号整数数组。  
                return bytes;
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);//释放从非托管内存中申请的内存  
            }

        }//因为C#不能直接对内存进行申请访问，所以借用非托管内存，来申请和管理内存，但是需要手动释放申请的内存  


        public static object ByteToStructData(byte[] bytes, Type stuctType)
        {
            int size = Marshal.SizeOf(stuctType);//返回对象的非托管大小，（以字节为单位）  
            IntPtr buffer = Marshal.AllocHGlobal(size);//根据指定大小，申请非托管内存  
            try
            {
                Marshal.Copy(bytes, 0, buffer, size);//将一维的托管 16 位有符号整数数组中的数据复制到非托管内存指针  
                return Marshal.PtrToStructure(buffer, stuctType);//从非托管内存中复制数据到托管内存块  
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);//释放非托管内存块  
            }
        }
    }

    public struct MarshalStruct
    {
        public int type;
        public string data;
        //public byte[] bytes;
    }
}
