using UnityEngine;

public class LegionUtil
{
    public static Color GetColor(int legion)
    {
        switch (legion)
        {
            case (int)LegionType.Blue:
                return Color.blue;
            case (int)LegionType.Red:
                return Color.red;
            default:
                return Color.white;
        }
    }
}