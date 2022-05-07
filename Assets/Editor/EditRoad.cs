using UnityEngine;

public class EditRoad
{
    public GameObject gameObject;
    public EditTower[] towers = new EditTower[2];

    public void UpdatePosition()
    {
        var start = towers[0];
        var end = towers[1];
        gameObject.transform.position = (start.gameObject.transform.position + end.gameObject.transform.position) * 0.5f;
        gameObject.transform.LookAt(start.gameObject.transform);
        var distance = Vector3.Distance(end.gameObject.transform.position, start.gameObject.transform.position);
        gameObject.transform.localScale = new Vector3(1,1,distance);
    }

    public void Link(EditTower start, EditTower end)
    {
        start.linkRoads.Add(this);
        start.linkTowers.Add(end);
                
        end.linkRoads.Add(this);
        end.linkTowers.Add(start);

        towers[0] = start;
        towers[1] = end;
        
        UpdatePosition();
    }

    public void Destroy()
    {
        
    }
}