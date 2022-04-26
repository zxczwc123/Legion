using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class AttributeObject : MonoBehaviour
{
    [ComponentAttribute("Name")]
    private Text m_textName;


    // Start is called before the first frame update
    void Start()
    {
        var fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
        foreach (var field in fields)
        {
            var attribute = field.GetCustomAttribute<ComponentAttribute>();
            if (attribute != null)
            {
                var type = field.FieldType;
                var component = transform.Find(attribute.transformPath).GetComponent(type);
                field.SetValue(this,component);
            }
        }
        Debug.Log(m_textName.text);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[AttributeUsage(AttributeTargets.Field)]
public class ComponentAttribute : Attribute
{
    public string transformPath;

    public ComponentAttribute(string path)
    {
        transformPath = path;
    }
    
}
