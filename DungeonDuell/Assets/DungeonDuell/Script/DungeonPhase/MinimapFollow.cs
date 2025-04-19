using UnityEngine;
using MoreMountains.TopDownEngine;

public class MinimapFollow : MonoBehaviour
{
    [SerializeField] private bool isPlayer1; // True = Kamera f�r Player1, False = Kamera f�r Player2
    private Transform player;

    void Start()
    {
        // Findet alle Charaktere in der Szene
        var characters = FindObjectsByType<Character>(FindObjectsSortMode.None);

        foreach (var character in characters)
        {
            if (character.CharacterType == Character.CharacterTypes.Player) // Nur Spieler-Charaktere ber�cksichtigen
            {
                // Pr�ft das PlayerID-Feld, um den richtigen Spieler zu finden
                if ((isPlayer1 && character.PlayerID == "Player1") || (!isPlayer1 && character.PlayerID == "Player2"))
                {
                    player = character.transform;
                    break;
                }
            }
        }

        if (player == null)
        {
            Debug.LogError($"Kein Spieler gefunden f�r Minimap-Kamera (isPlayer1: {isPlayer1})");
        }
    }

    void LateUpdate()
    {
        if (player != null)
        {
            Vector3 newPosition = player.position;
            newPosition.z = transform.position.z; // H�he beibehalten
            transform.position = newPosition;
        }
    }
}