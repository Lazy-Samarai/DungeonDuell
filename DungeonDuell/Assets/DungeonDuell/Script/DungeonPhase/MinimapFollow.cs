using MoreMountains.TopDownEngine;
using MoreMountains.Tools;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine;

public class MinimapFollow : MonoBehaviour
{
    [SerializeField] private bool isPlayer1; // True = Kamera f�r Player1, False = Kamera f�r Player2
    private Transform _player;

    private void Start()
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
        var characters = FindObjectsByType<Character>(FindObjectsSortMode.None);

        foreach (var character in characters)
            if (character.CharacterType == Character.CharacterTypes.Player) // Nur Spieler-Charaktere ber�cksichtigen
                // Pr�ft das PlayerID-Feld, um den richtigen Spieler zu finden
                if ((isPlayer1 && character.PlayerID == "Player1") || (!isPlayer1 && character.PlayerID == "Player2"))
                {
                    _player = character.transform;
                    break;
                }

        if (_player == null) Debug.LogError($"Kein Spieler gefunden f�r Minimap-Kamera (isPlayer1: {isPlayer1})");
    }

    private void LateUpdate()
    {
        if (_player != null)
        {
            var newPosition = _player.position;
            newPosition.z = transform.position.z; // H�he beibehalten
            transform.position = newPosition;
        }
    }
}