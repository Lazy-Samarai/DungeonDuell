using TMPro;
using UnityEngine;

namespace dungeonduell
{
    public class TooltipController : MonoBehaviour
    {
        [SerializeField] private GameObject tooltipPanel;
        [SerializeField] private TextMeshProUGUI tooltipText;

        private RectTransform _tooltipRectTransform;
        private Vector3? worldTargetPosition = null;
        private Camera cam;

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

        private void Update()
        {
            if (tooltipPanel.activeSelf && worldTargetPosition.HasValue && cam != null)
            {
                Vector3 screenPosition = cam.WorldToScreenPoint(worldTargetPosition.Value);
                tooltipPanel.transform.position = screenPosition;
            }
        }

        public void ShowTooltip(string text, Vector3 worldPosition, Camera camera)
        {
            if (tooltipText == null || tooltipPanel == null || camera == null) return;

            tooltipText.text = text;
            cam = camera;
            worldTargetPosition = worldPosition;

            tooltipPanel.SetActive(true);
        }

        public void HideTooltip()
        {
            tooltipPanel.SetActive(false);
            worldTargetPosition = null;
            cam = null;
        }
    }
}
