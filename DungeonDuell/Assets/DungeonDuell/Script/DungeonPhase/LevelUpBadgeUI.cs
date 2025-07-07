using TMPro;
using UnityEngine;

public class LevelUpBadgeUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject badgeObject;         // Der rote Kreis (Benachrichtigung)
    public TMP_Text badgeCountText;        // Text im roten Kreis, z. B. "2"
    public GameObject fullGroup;           // Z.B. LevelUpAvailibleText (ganzes Element)

    private int levelUps = 0;

    /// <summary>
    /// Setzt den aktuellen Level-Up-Zähler und aktualisiert die UI.
    /// </summary>
    /// <param name="count">Anzahl verfügbarer Level-Ups</param>
    public void SetLevelUpCount(int count)
    {
        levelUps = count;

        // Gesamte Anzeige ein- oder ausblenden
        fullGroup.SetActive(levelUps > 0);

        // Roter Kreis (Badge) nur anzeigen, wenn mehr als 1 Level-Up vorhanden
        badgeObject.SetActive(levelUps > 1);

        // Zahl im Kreis aktualisieren
        badgeCountText.text = levelUps.ToString();
    }

    /// <summary>
    /// Erhöht den Zähler um 1.
    /// </summary>
    public void AddLevelUp()
    {
        SetLevelUpCount(levelUps + 1);
    }

    /// <summary>
    /// Verringert den Zähler um 1 (niemals unter 0).
    /// </summary>
    public void ConsumeLevelUp()
    {
        SetLevelUpCount(Mathf.Max(0, levelUps - 1));
    }
}