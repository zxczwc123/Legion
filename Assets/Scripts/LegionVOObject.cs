using System;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class LegionVOObject : MonoBehaviour
{
    [HideInInspector]
    public Vector2 velocity;
    public float speed;
    [HideInInspector]
    public Vector3 dest;
    public Vector3 dest1;
    public Vector3 dest2;
    public float radius;
    private bool m_isForward;

    private void Awake()
    {
        LegionVO.Instance.AddObject(this);
        var position = transform.position;
        velocity = (dest - new Vector3(position.x, position.y)).normalized;
        dest = m_isForward ? dest1 : dest2;
    }

    private void Update()
    {
        if (speed == 0)
        {
            return;
        }
        var distance = Vector3.Distance(transform.position, dest);
        if (distance < 0.1)
        {
            m_isForward = !m_isForward;
            dest = m_isForward ? dest1 : dest2;
            return;
        }
        LegionVO.Instance.Calc(this);
        var delta = Time.deltaTime * velocity;
        transform.position += new Vector3(delta.x,delta.y);
    }
}