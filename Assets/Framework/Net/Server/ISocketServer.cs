using UnityEngine;
using System.Collections;

namespace Framework.Net
{
    public interface ISocketServer
    {
        void Receive(byte[] bytes);
    }
}

