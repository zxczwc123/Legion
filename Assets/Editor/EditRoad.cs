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
}