using UnityEngine;
using System.Collections.Generic;


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
    
    public void UpdateDoorTransparency(List<ConnectionDir> usedPorts)
    {
        foreach (Transform wallSide in mapWalls.transform)
        {
            // Parent-Name ist z. B. "TopLeft", "Right" etc.
            string sideName = wallSide.name;

            if (!System.Enum.TryParse(sideName, out ConnectionDir dir))
            {
                Debug.LogWarning($"[RoomData] Unbekannte Richtung: {sideName}");
                continue;
            }

            // Ist diese Seite offen (Tür vorhanden)?
            bool hasDoor = usedPorts.Contains(dir);

            // Erstes Kind ist immer DoorXY
            if (wallSide.childCount > 0)
            {
                var doorObject = wallSide.GetChild(0); // DoorTL, DoorRight etc.
                var sr = doorObject.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.color = hasDoor
                        ? new Color(1f, 1f, 1f, 0.2f) // Tür transparent machen
                        : new Color(1f, 1f, 1f, 1f);  // Tür deckend lassen
                }
            }
        }
    }

    
}