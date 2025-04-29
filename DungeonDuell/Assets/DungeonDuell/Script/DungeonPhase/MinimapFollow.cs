using UnityEngine;
using MoreMountains.TopDownEngine;
using MoreMountains.Tools;
using UnityEngine.EventSystems;
using System.Collections;

public class MinimapFollow : MonoBehaviour
{
    [SerializeField] private bool isPlayer1; // True = Kamera für Player1, False = Kamera für Player2
    private Transform player;

    void Start()
    {
        StartCoroutine(StartLate());
        // Findet alle Charaktere in der Szene
    }

    private IEnumerator StartLate()
    {
        yield return new WaitForEndOfFrame();
        AfterStart();
    }

    private void AfterStart()
    {
        var characters = FindObjectsOfType<Character>();

        foreach (var character in characters)
        {
            if (character.CharacterType == Character.CharacterTypes.Player) // Nur Spieler-Charaktere berücksichtigen
            {
                // Prüft das PlayerID-Feld, um den richtigen Spieler zu finden
                if ((isPlayer1 && character.PlayerID == "Player1") || (!isPlayer1 && character.PlayerID == "Player2"))
                {
                    player = character.transform;
                    Debug.Log(player + " gefunden");
                    break;
                }
            }
        }

        if (player == null)
        {
            Debug.LogError($"Kein Spieler gefunden für Minimap-Kamera (isPlayer1: {isPlayer1})");
        }
    }


    void LateUpdate()
    {
        if (player != null)
        {
            Vector3 newPosition = player.position;
            newPosition.z = transform.position.z; // Höhe beibehalten
            transform.position = newPosition;
        }
    }
}
