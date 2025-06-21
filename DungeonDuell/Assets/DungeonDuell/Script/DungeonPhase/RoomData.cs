using UnityEngine;

public class RoomData : MonoBehaviour
{
    [Header("Map-Zeug")]
    public Transform roomCenter;
    public GameObject mapWalls; // Die weißen Map-Walls

    public void SetMapWallsActive(bool active)
    {
        if (mapWalls != null)
            mapWalls.SetActive(active);
    }
}