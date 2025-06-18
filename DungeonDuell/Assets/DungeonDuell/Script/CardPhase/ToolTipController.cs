using TMPro;
using UnityEngine;

namespace dungeonduell
{
    public class TooltipController : MonoBehaviour
    {
        [SerializeField] private GameObject tooltipPanel; // Direkt im Canvas
        [SerializeField] private TextMeshProUGUI tooltipText;

        private RectTransform _tooltipRectTransform;

        private void Awake()
        {
            if (tooltipPanel == null || tooltipText == null)
            {
                Debug.LogError("TooltipPanel oder Text nicht zugewiesen!");
                return;
            }

            _tooltipRectTransform = tooltipPanel.GetComponent<RectTransform>();
            HideTooltip();
        }

        public void ShowTooltip(string text, Vector3 worldPosition)
        {
            if (tooltipText == null || _tooltipRectTransform == null) return;

            tooltipText.text = text;
            tooltipPanel.SetActive(true);

            Vector2 localPosition;
            RectTransform canvasRect = tooltipPanel.transform.parent.GetComponent<RectTransform>();

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                Camera.main.WorldToScreenPoint(worldPosition),
                Camera.main,
                out localPosition
            );

            _tooltipRectTransform.anchoredPosition = localPosition;
        }

        public void HideTooltip()
        {
            if (tooltipPanel != null)
                tooltipPanel.SetActive(false);
        }
    }
}
