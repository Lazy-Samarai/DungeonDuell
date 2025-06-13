using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace dungeonduell
{
    public class UIManager : MonoBehaviour
    {
        public GameObject creditsPanel;
        public GameObject optionsPanel;
        public CanvasGroup fadeCanvas;

        public float fadeDuration = 0.5f;

        private void Start()
        {
            ShowCanvasGroup();
        }

        public void HideCanvasGroup()
        {
            if (fadeCanvas != null)
            {
                fadeCanvas.alpha = 1;
                fadeCanvas.DOFade(0, fadeDuration).OnComplete(() =>
                fadeCanvas.gameObject.SetActive(false));
            }
        }

        public void ShowCanvasGroup()
        {
            if (fadeCanvas != null)
            {
                fadeCanvas.gameObject.SetActive(true);
                fadeCanvas.alpha = 0;
                fadeCanvas.DOFade(1, fadeDuration); // Sanftes Einblenden des UI
            }
        }

        // Wechselt die Szene mit Animation
        public void ChangeScene(string sceneName)
        {
            if (fadeCanvas != null)
                fadeCanvas.DOFade(0, fadeDuration).OnComplete(() => SceneManager.LoadScene(sceneName));
            else
                SceneManager.LoadScene(sceneName);
        }

        // Kehrt zum Titelbildschirm zur�ck
        public void ReturnToTitleScreen()
        {
            ChangeScene("TitleScreen");
        }

        // Beendet das Spiel
        public void QuitGame()
        {
            Application.Quit();
        }

        // �ffnet oder schlie�t das Credits-Panel mit Animation
        public void ToggleCredits()
        {
            if (creditsPanel != null)
            {
                var isActive = creditsPanel.activeSelf;
                creditsPanel.SetActive(true);
                creditsPanel.transform.localScale = Vector3.zero;
                creditsPanel.transform.DOScale(isActive ? 0 : 1, fadeDuration).OnComplete(() =>
                {
                    if (isActive) creditsPanel.SetActive(false);
                });
            }
        }

        // �ffnet oder schlie�t das Options-Panel mit Animation
        public void ToggleOptions()
        {
            if (optionsPanel != null)
            {
                var isActive = optionsPanel.activeSelf;
                optionsPanel.SetActive(true);
                optionsPanel.transform.localScale = Vector3.zero;
                optionsPanel.transform.DOScale(isActive ? 0 : 1, fadeDuration).OnComplete(() =>
                {
                    if (isActive) optionsPanel.SetActive(false);
                });
            }
        }

        private void OnEnable()
        {
            ShowCanvasGroup();
        }
    }
}