using UnityEngine;
using UnityEngine.UI;

namespace TapAndCollect.UI
{
    public class BonusDebugOverlay : MonoBehaviour
    {
        private const string OverlayName = "Bonus Debug Overlay";

        [SerializeField] private Font font;
        [SerializeField] private Vector2 anchoredPosition = new Vector2(24f, 24f);
        [SerializeField] private Vector2 size = new Vector2(430f, 150f);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void CreateOverlay()
        {
            if (GameObject.Find(OverlayName) != null)
            {
                return;
            }

            Canvas canvas = Object.FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                return;
            }

            GameObject overlay = new GameObject(OverlayName);
            overlay.transform.SetParent(canvas.transform, false);
            overlay.AddComponent<RectTransform>();
            overlay.AddComponent<BonusDebugOverlay>();
        }

        private void Awake()
        {
            if (transform.childCount > 0)
            {
                return;
            }

            if (font == null)
            {
                font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                if (font == null)
                {
                    font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                }
            }

            RectTransform rect = GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.zero;
            rect.pivot = Vector2.zero;
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = size;

            Image background = gameObject.AddComponent<Image>();
            background.color = new Color(0f, 0f, 0f, 0.45f);
            background.raycastTarget = false;

            Text fpsText = CreateText("FPS", new Vector2(12f, -8f), new Vector2(size.x - 24f, 36f), 28, FontStyle.Bold);
            fpsText.text = "FPS";
            fpsText.gameObject.AddComponent<FpsCounterView>();

            Text logText = CreateText("Debug Logs", new Vector2(12f, -46f), new Vector2(size.x - 24f, size.y - 56f), 20, FontStyle.Normal);
            logText.text = "Logs";
            logText.gameObject.AddComponent<DebugLogView>();
        }

        private Text CreateText(string objectName, Vector2 position, Vector2 textSize, int fontSize, FontStyle style)
        {
            GameObject textObject = new GameObject(objectName);
            textObject.transform.SetParent(transform, false);

            RectTransform rect = textObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = position;
            rect.sizeDelta = textSize;

            Text text = textObject.AddComponent<Text>();
            text.font = font;
            text.fontSize = fontSize;
            text.fontStyle = style;
            text.alignment = TextAnchor.UpperLeft;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.color = Color.white;
            text.raycastTarget = false;

            return text;
        }
    }
}
