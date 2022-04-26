using System.Collections.Generic;
using UnityEngine;

public class LegionVO : MonoBehaviour
{
    private static LegionVO m_instance;

    public static LegionVO Instance
    {
        get
        {
            if (m_instance == null)
            {
                var obj = GameObject.Find(nameof(LegionVO));
                if (obj == null)
                {
                    obj = new GameObject(nameof(LegionVO));
                }
                m_instance = obj.GetComponent<LegionVO>();
                if (m_instance == null)
                {
                    m_instance = obj.AddComponent<LegionVO>();
                }
            }
            return m_instance;
        }
    }

    private List<LegionVOObject> m_objs = new List<LegionVOObject>();

    public void AddObject(LegionVOObject obj)
    {
        if (!m_objs.Contains(obj))
        {
            m_objs.Add(obj);
        }
    }

    public void Calc(LegionVOObject obj)
    {
        foreach (var obstacle in m_objs)
        {
            if(obj == obstacle) continue;
            CalcVo(obj,obstacle);
        }
    }
    
    public void CalcVo(LegionVOObject obj,LegionVOObject obstacle)
    {
        // P 切点  O 原点 A 外点
        float radius = obj.radius + obstacle.radius;
        Vector3 destN = obj.dest - obj.transform.position;
        CalcQieDian(obstacle.transform.position, obj.transform.position, radius, out Vector3 positionP1,out Vector3 positionP2);
        if (positionP1 == obj.transform.position)
        {
            return;
        }
        Vector3 p1N = positionP1 - obj.transform.position;
        Vector3 p2N = positionP2 - obj.transform.position;
        Vector3 ON = obstacle.transform.position - obj.transform.position;
        Vector2 destVelocity = destN * obj.speed;
        Vector2 destVelocityN = destVelocity - obstacle.velocity;
        float angleDest = Vector2.Angle(destVelocityN, ON);
        float angleP1N =  Vector2.Angle(p1N, ON);
        if (angleDest > angleP1N)
        {
            obj.velocity = destN.normalized * obj.speed;
        }
        else
        {
            float angleP1DestN = Vector2.Angle(p1N, destVelocityN);
            float angleP2DestN = Vector2.Angle(p2N, destVelocityN);
            if (angleP1DestN > angleP2DestN)
            {
                // obj.velocity = (p2N.normalized * obj.speed + new Vector3(obstacle.velocity.x,obstacle.velocity.y)).normalized * obj.speed;
                obj.velocity = p2N.normalized * obj.speed;
            }
            else
            {
                // obj.velocity = (p1N.normalized * obj.speed + new Vector3(obstacle.velocity.x,obstacle.velocity.y)).normalized * obj.speed;
                obj.velocity = p1N.normalized * obj.speed;
            }
            
            // float angleP1DestN = Vector2.Angle(p1N, destN);
            // float angleP2DestN = Vector2.Angle(p2N, destN);
            // if (angleP1DestN > angleP2DestN)
            // {
            //     obj.velocity = p2N.normalized * obj.speed;
            // }
            // else
            // {
            //     obj.velocity = p1N.normalized * obj.speed;
            // }
        }
        
    }
    
    public void CalcVoOld(LegionVOObject obj,LegionVOObject obstacle)
    {
        // P 切点  O 原点 A 外点
        float radius = obj.radius + obstacle.radius;
        Vector3 destN = obj.dest - obj.transform.position;
        CalcQieDian(obstacle.transform.position, obj.transform.position, radius, out Vector3 positionP1,out Vector3 positionP2);
        if (positionP1 == obj.transform.position)
        {
            return;
        }
        Vector3 p1N = positionP1 - obj.transform.position;
        Vector3 p2N = positionP2 - obj.transform.position;
        Vector3 ON = obstacle.transform.position - obj.transform.position;
        float angleDest = Vector2.Angle(destN, ON);
        float angleP1N =  Vector2.Angle(p1N, ON);
        if (angleDest > angleP1N)
        {
            obj.velocity = destN.normalized * obj.speed;
        }
        else
        {
            Vector2 destVelocity = destN * obj.speed;
            Vector2 destVelocityN = destVelocity - obstacle.velocity;
            float angleP1DestN = Vector2.Angle(p1N, destVelocityN);
            float angleP2DestN = Vector2.Angle(p2N, destVelocityN);
            if (angleP1DestN > angleP2DestN)
            {
                // obj.velocity = (p2N.normalized * obj.speed + new Vector3(obstacle.velocity.x,obstacle.velocity.y)).normalized * obj.speed;
                obj.velocity = p2N.normalized * obj.speed;
            }
            else
            {
                // obj.velocity = (p1N.normalized * obj.speed + new Vector3(obstacle.velocity.x,obstacle.velocity.y)).normalized * obj.speed;
                obj.velocity = p1N.normalized * obj.speed;
            }
            
            // float angleP1DestN = Vector2.Angle(p1N, destN);
            // float angleP2DestN = Vector2.Angle(p2N, destN);
            // if (angleP1DestN > angleP2DestN)
            // {
            //     obj.velocity = p2N.normalized * obj.speed;
            // }
            // else
            // {
            //     obj.velocity = p1N.normalized * obj.speed;
            // }
        }
        
    }
    
    /// <summary>
    /// 计算出切点坐标
    /// </summary>
    /// <param name="ptCenter">圆心坐标</param>
    /// <param name="ptOutside">第三点坐标</param>
    /// <param name="dbRadious">半径</param>
    /// <returns></returns>
    private void CalcQieDian(Vector3 ptCenter, Vector3 ptOutside, float dbRadious,out Vector3 P1,out Vector3 P2)
    {
 
        Vector3 E, F, G1,G2 ;
        E = new Vector3();
        F = new Vector3();
        G1 = new Vector3();
        P1 = new Vector3();
        G2 = new Vector3();
        P2 = new Vector3();
        float r = dbRadious;
        //1. 坐标平移到圆心ptCenter处,求园外点的新坐标E
        E.x = ptOutside.x - ptCenter.x;
        E.y = ptOutside.y - ptCenter.y; //平移变换到E
 
        //2. 求圆与OE的交点坐标F, 相当于E的缩放变换
        float t = r / Mathf.Sqrt(E.x * E.x + E.y * E.y); //得到缩放比例
        F.x = E.x * t;
        F.y = E.y * t; //缩放变换到F
 
        //3. 将E旋转变换角度a到切点G，其中cos(a)=r/OF=t, 所以a=arccos(t);
        float a = Mathf.Acos(t); //得到旋转角度
        if (a == float.NaN)
        {
            P1 = ptOutside;
            P2 = ptOutside;
            return;
        }
        
        G1.x = F.x * Mathf.Cos(a) - F.y * Mathf.Sin(a);
        G1.y = F.x * Mathf.Sin(a) + F.y * Mathf.Cos(a); //旋转变换到G
 
        //4. 将G平移到原来的坐标下得到新坐标H
        P1.x = G1.x + ptCenter.x;
        P1.y = G1.y + ptCenter.y; //平移变换到H

        a = -a;
        G2.x = F.x * Mathf.Cos(a) - F.y * Mathf.Sin(a);
        G2.y = F.x * Mathf.Sin(a) + F.y * Mathf.Cos(a); //旋转变换到G
 
        //4. 将G平移到原来的坐标下得到新坐标H
        P2.x = G2.x + ptCenter.x;
        P2.y = G2.y + ptCenter.y; //平移变换到H
        
    }
}