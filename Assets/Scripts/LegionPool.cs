using System.Collections.Generic;
using UnityEngine;

public abstract class LegionPool<T> : MonoBehaviour where T : MonoBehaviour
{
    private List<T> m_paths = new List<T>();

    protected void Init(GameObject poolObj)
    {
        var obj = poolObj.GetComponent<T>();
        if (obj == null)
        {
            obj = poolObj.AddComponent<T>();
        }
        m_paths.Add(obj);
    }

    public T Get()
    {
        if (m_paths.Count > 1)
        {
            var path = m_paths[1];
            m_paths.RemoveAt(1);
            return path;
        }
        var clonePath = Clone();
        return clonePath;
    }

    private T Clone()
    {
        var obj = m_paths[0].gameObject;
        var cloneObj = GameObject.Instantiate(obj);
        var path = InitObj(cloneObj);
        return path;
    }
    
    private T InitObj(GameObject pathObj)
    {
        var path = pathObj.GetComponent<T>();
        if (path == null)
        {
            path = pathObj.AddComponent<T>();
        }
        return path;
    }

    public void Release(T path)
    {
        m_paths.Add(path);
    }
}