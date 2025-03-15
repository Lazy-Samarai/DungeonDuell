using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject creditsPanel;
    public GameObject optionsPanel;

    // Wechselt die Szene mit dem angegebenen Namen
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // Kehrt zum Titelbildschirm zurück
    public void ReturnToTitleScreen()
    {
        SceneManager.LoadScene("TitleScreen");
    }

    // Beendet das Spiel
    public void QuitGame()
    {
        Application.Quit();
    }

    // Öffnet oder schließt das Credits-Panel
    public void ToggleCredits()
    {
        if (creditsPanel != null)
        {
            creditsPanel.SetActive(!creditsPanel.activeSelf);
        }
    }

    // Öffnet oder schließt das Options-Panel
    public void ToggleOptions()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(!optionsPanel.activeSelf);
        }
    }
}
