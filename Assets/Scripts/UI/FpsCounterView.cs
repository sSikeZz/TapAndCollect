using UnityEngine;
using UnityEngine.UI;

namespace TapAndCollect.UI
{
    public class FpsCounterView : MonoBehaviour
    {
        [SerializeField] private Text fpsText;
        [SerializeField] private float refreshInterval = 0.25f;

        private int frames;
        private float elapsed;

        private void Awake()
        {
            if (fpsText == null)
            {
                fpsText = GetComponent<Text>();
            }
        }

        private void Update()
        {
            frames++;
            elapsed += Time.unscaledDeltaTime;

            if (elapsed < refreshInterval)
            {
                return;
            }

            if (fpsText != null)
            {
                fpsText.text = $"{Mathf.RoundToInt(frames / elapsed)} FPS";
            }

            frames = 0;
            elapsed = 0f;
        }
    }
}
