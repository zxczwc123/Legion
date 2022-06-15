
using UnityEngine;

namespace DefaultNamespace
{
    public class MathTool
    {
        //求直线和平面的交点 
        public static Vector3 IntersectLineAndPlane(Vector3 _linePt, Vector3 _lineDir, Vector3 _plnNormal,
            Vector3 _plnPt)
        {
            float d = Vector3.Dot(_plnPt - _linePt, _plnNormal) / Vector3.Dot(_lineDir, _plnNormal);
            return d * _lineDir.normalized + _linePt;
        }
        
        /// <summary>
        /// 点到直线距离
        /// </summary>
        /// <param name="point">点坐标</param>
        /// <param name="linePoint1">直线上一个点的坐标</param>
        /// <param name="linePoint2">直线上另一个点的坐标</param>
        /// <returns></returns>
        public static float DisPoint2Line(Vector3 point,Vector3 linePoint1,Vector3 linePoint2)
        {
            Vector3 vec1 = point - linePoint1;
            Vector3 vec2 = linePoint2 - linePoint1;
            Vector3 vecProj = Vector3.Project(vec1, vec2);
            float dis =  Mathf.Sqrt(Mathf.Pow(Vector3.Magnitude(vec1), 2) - Mathf.Pow(Vector3.Magnitude(vecProj), 2));
            return dis;
        }

        //求两异面直线距离
        public static float DisOfLine(Vector3 _l1, Vector3 _l1Dir, Vector3 _l2, Vector3 _l2Dir)
        {
            Vector3 n = Vector3.Cross(_l1Dir, _l2Dir).normalized;
 
            Vector3 dir = Vector3.ProjectOnPlane(_l2, n);
 
            Vector3 cPt = IntersectLineAndPlane(_l2, (dir - _l2).normalized, n, _l1);
 
            return (_l2 - cPt).magnitude;
        }
    }
}