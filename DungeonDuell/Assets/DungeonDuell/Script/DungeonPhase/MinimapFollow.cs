using UnityEngine;

public class MinimapFollow : MonoBehaviour
{
    private Transform player;

    void Start()
    {
        // Pr�ft den Namen der Minimap-Kamera und sucht den richtigen Spieler
        if (gameObject.name == "Minimap_Camera_Player1")
        {
            player = GameObject.Find("PlayerNew_1")?.transform;
        }
        else if (gameObject.name == "Minimap_Camera_Player2")
        {
            player = GameObject.Find("PlayerNew_2")?.transform;
        }

        if (player == null)
        {
            Debug.LogError("Kein Spieler gefunden f�r " + gameObject.name);
        }
    }

    void LateUpdate()
    {
        if (player != null)
        {
            Vector3 newPosition = player.position;
            newPosition.z = transform.position.z; // Falls notwendig, H�he beibehalten
            transform.position = newPosition;
        }
    }
}
