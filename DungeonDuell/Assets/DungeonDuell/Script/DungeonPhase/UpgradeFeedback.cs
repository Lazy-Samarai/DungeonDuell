using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Image))]
public class UpgradeFeedback : MonoBehaviour
{
    [Header("Flash Settings")]
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashDuration = 0.5f;
    [SerializeField] private int flashCount = 2;

    [SerializeField] private Image img;
    private Color originalColor;

    void Awake()
    {
        img = GetComponent<Image>();
        originalColor = img.color;
    }

    public void PlayFeedback()
    {
        StopAllCoroutines();
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        for (int i = 0; i < flashCount; i++)
        {
            img.color = flashColor;
            yield return new WaitForSeconds(flashDuration);

            img.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
        }
    }
}