using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CountdownVisual : MonoBehaviour
{
    [Header("Element References")]
    public TextMeshProUGUI timerText;                  
    public Image TimeCounter_White;                    // Weißer Diamant
    public Image VerticalSplitter_White;               // Weißer Strich

    [Header("Settings")]
    public bool popOnlyWhenRed = false;                // Nur Pop-Effekt, wenn Schwelle erreicht
    public bool changeBackgroundColor = true;          // Hintergrund- & Linienfarbe ändern

    [Header("Threshold")]
    public int redThreshold = 10;                      // Ab dieser Sekunde wird’s rot

    private int lastSecond = -1;

    private Color textRed = new Color(1f, 0.3f, 0.3f);      // Hellrot für Text
    private Color bgRed = new Color(0.6f, 0f, 0f);          // Dunkelrot für UI
    private Color defaultTextColor = Color.black;
    private Color defaultBGColor = Color.white;

    private void Start()
    {
        timerText.color = defaultTextColor;

        if (TimeCounter_White != null)
            TimeCounter_White.color = defaultBGColor;

        if (VerticalSplitter_White != null)
            VerticalSplitter_White.color = defaultBGColor;
    }

    private void Update()
    {
        if (int.TryParse(timerText.text, out int currentSecond))
        {
            if (currentSecond != lastSecond)
            {
                lastSecond = currentSecond;
                PlayVisualFeedback(currentSecond);
            }
        }
    }

    private void PlayVisualFeedback(int second)
    {
        bool isRedPhase = second <= redThreshold;

        // Textfarbe
        timerText.DOColor(isRedPhase ? textRed : defaultTextColor, 0.2f);

        // Hintergrundfarbe nur ändern wenn aktiviert
        if (changeBackgroundColor)
        {
            if (TimeCounter_White != null)
                TimeCounter_White.DOColor(isRedPhase ? bgRed : defaultBGColor, 0.2f);
            if (VerticalSplitter_White != null)
                VerticalSplitter_White.DOColor(isRedPhase ? bgRed : defaultBGColor, 0.2f);
        }

        // Pop-Effekt
        if (!popOnlyWhenRed || isRedPhase)
        {
            timerText.transform.DOKill();
            timerText.transform.localScale = Vector3.one;
            timerText.transform
                .DOScale(1.25f, 0.15f)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.OutBack);
        }

        // Rotation bei <= 3 Sekunden
        if (second <= 3)
        {
            timerText.transform
                .DORotate(new Vector3(0, 0, 10), 0.1f)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }
    }
}
