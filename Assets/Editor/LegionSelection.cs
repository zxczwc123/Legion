using System;

public class LegionSelection
{
    public static Action OnSelectTowerChange;

    private static LegionTower selectTower;

    public static LegionTower SelectTower
    {
        set
        {
            if (selectTower != value)
            {
                selectTower = value;
                if (OnSelectTowerChange != null) OnSelectTowerChange();
            }
        }
        get => selectTower;
    }
}