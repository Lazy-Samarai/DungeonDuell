using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace dungeonduell
{
    public class TooltipController : MonoBehaviour
    {
        public GameObject tooltipPrefab;
        public GameObject cardCanvas;
        private GameObject tooltipInstance;
        private TextMeshProUGUI tooltipText;
        private RectTransform tooltipRectTransform;

        void Start()
        {
            if (tooltipPrefab != null)
            {
                tooltipInstance = Instantiate(tooltipPrefab, cardCanvas.transform);
                tooltipText = tooltipInstance.GetComponentInChildren<TextMeshProUGUI>();
                tooltipRectTransform = tooltipInstance.GetComponent<RectTransform>();
                HideTooltip();
            }
            else
            {
                Debug.LogError("Tooltip Prefab nicht zugewiesen!");
            }
        }


        public void ShowTooltip(string text, Vector3 position)
        {
            if (tooltipInstance != null)
            {
                if (tooltipText == null || tooltipRectTransform == null)
                {
                    Debug.LogError("Tooltip TextMeshProUGUI oder RectTransform nicht gefunden!");
                    return;
                }

                tooltipText.text = text;
                tooltipInstance.SetActive(true);

                // Umwandlung der Weltposition in die Position des UI-Camvas
                Vector2 localPosition;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    (RectTransform)cardCanvas.transform,
                    Camera.main.WorldToScreenPoint(position),
                    Camera.main,
                    out localPosition
                );

                tooltipRectTransform.anchoredPosition = localPosition;
            }
        }


        public void HideTooltip()
        {
            if (tooltipInstance != null)
            {
                tooltipInstance.SetActive(false);
            }
        }
    }
}
