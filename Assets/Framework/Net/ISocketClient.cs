using UnityEngine;
using System.Collections;

namespace Framework.Net
{
    public interface ISocketClient
    {
        void Receive(byte[] bytes);
    }
}

