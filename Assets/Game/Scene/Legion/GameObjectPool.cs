using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool
{
    private List<GameObject> m_paths = new List<GameObject>();

    public void Init(GameObject poolObj)
    {
        m_paths.Add(poolObj);
    }

    public GameObject Get()
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

    private GameObject Clone()
    {
        var obj = m_paths[0].gameObject;
        var cloneObj = GameObject.Instantiate(obj);
        return cloneObj;
    }

    public void Release(GameObject path)
    {
        m_paths.Add(path);
    }
}