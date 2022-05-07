using System;

public class LegionSelection
{

    public static Action OnSelectTowerChange;
    
    private static EditTower selectTower;
    public static EditTower SelectTower
    {
        set
        {
            if (selectTower != value)
            {
                selectTower = value;
                if (OnSelectTowerChange != null) OnSelectTowerChange();
            }
        }
        get
        {
            return selectTower;
        }
    }
}