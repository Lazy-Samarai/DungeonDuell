using TMPro;
using UnityEngine;

namespace dungeonduell
{
    public class TooltipController : MonoBehaviour
    {
        public GameObject tooltipPrefab;
        public GameObject cardCanvas;
        private GameObject _tooltipInstance;
        private RectTransform _tooltipRectTransform;
        private TextMeshProUGUI _tooltipText;

        private void Start()
        {
            if (tooltipPrefab != null)
            {
                _tooltipInstance = Instantiate(tooltipPrefab, cardCanvas.transform);
                _tooltipText = _tooltipInstance.GetComponentInChildren<TextMeshProUGUI>();
                _tooltipRectTransform = _tooltipInstance.GetComponent<RectTransform>();
                HideTooltip();
            }
            else
            {
                Debug.LogError("Tooltip Prefab nicht zugewiesen!");
            }
        }


        public void ShowTooltip(string text, Vector3 position)
        {
            if (_tooltipInstance != null)
            {
                if (_tooltipText == null || _tooltipRectTransform == null)
                {
                    Debug.LogError("Tooltip TextMeshProUGUI oder RectTransform nicht gefunden!");
                    return;
                }

                _tooltipText.text = text;
                _tooltipInstance.SetActive(true);

                // Umwandlung der Weltposition in die Position des UI-Camvas
                Vector2 localPosition;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    (RectTransform)cardCanvas.transform,
                    Camera.main.WorldToScreenPoint(position),
                    Camera.main,
                    out localPosition
                );

                _tooltipRectTransform.anchoredPosition = localPosition;
            }
        }


        public void HideTooltip()
        {
            if (_tooltipInstance != null) _tooltipInstance.SetActive(false);
        }
    }
}