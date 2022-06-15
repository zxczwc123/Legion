using UnityEngine;
using System.Collections;

namespace Framework.Net
{
    public interface IRequestHandler
    {
        byte[] Code();

        bool IsRequest();
    }
}


