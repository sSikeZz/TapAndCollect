using System.Collections;
using TapAndCollect.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace TapAndCollect.UI
{
    public class ScreenRouter : MonoBehaviour
    {
        [SerializeField] private GameObject homeScreen;
        [SerializeField] private GameObject gameplayScreen;
        [SerializeField] private GameObject pauseOverlay;
        [SerializeField] private GameObject gameOverScreen;
        [SerializeField] private float gameOverRevealDelay = 0.45f;

        private Coroutine gameOverRevealRoutine;
        private GameObject settingsOverlay;
        private Text soundButtonText;
        private bool audioMuted;

        private void Awake()
        {
            audioMuted = PlayerPrefs.GetInt("AudioMuted", 0) == 1;
            ApplyAudioSetting();
            CreateSettingsOverlay();
        }

        private void OnEnable()
        {
            GameEvents.StateChanged += Show;
            if (GameManager.Instance != null)
            {
                Show(GameManager.Instance.State);
            }
        }

        private void OnDisable()
        {
            GameEvents.StateChanged -= Show;
            StopGameOverReveal();
        }

        public void Play() => GameManager.Instance?.Play();
        public void Pause() => GameManager.Instance?.Pause();
        public void Resume() => GameManager.Instance?.Resume();
        public void Restart() => GameManager.Instance?.Restart();
        public void Home() => GameManager.Instance?.GoHome();
        public void Settings() => SetActive(settingsOverlay, true);
        public void CloseSettings() => SetActive(settingsOverlay, false);

        public void ToggleSound()
        {
            audioMuted = !audioMuted;
            PlayerPrefs.SetInt("AudioMuted", audioMuted ? 1 : 0);
            PlayerPrefs.Save();
            ApplyAudioSetting();
        }

        private void Show(GameState state)
        {
            StopGameOverReveal();
            CloseSettings();

            SetActive(homeScreen, state == GameState.Home);
            SetActive(gameplayScreen, state == GameState.Playing || state == GameState.Paused || state == GameState.GameOver);
            SetActive(pauseOverlay, state == GameState.Paused);
            SetActive(gameOverScreen, false);

            if (state == GameState.GameOver)
            {
                gameOverRevealRoutine = StartCoroutine(RevealGameOverAfterDelay());
                return;
            }

            SetActive(gameplayScreen, state == GameState.Playing || state == GameState.Paused);
        }

        private void ApplyAudioSetting()
        {
            AudioListener.volume = audioMuted ? 0f : 1f;
            if (soundButtonText != null)
            {
                soundButtonText.text = audioMuted ? "Sound: Off" : "Sound: On";
            }
        }

        private void CreateSettingsOverlay()
        {
            if (settingsOverlay != null)
            {
                return;
            }

            Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (font == null)
            {
                font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            }

            settingsOverlay = new GameObject("Settings Overlay", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            settingsOverlay.transform.SetParent(transform, false);

            RectTransform overlayRect = settingsOverlay.GetComponent<RectTransform>();
            overlayRect.anchorMin = Vector2.zero;
            overlayRect.anchorMax = Vector2.one;
            overlayRect.offsetMin = Vector2.zero;
            overlayRect.offsetMax = Vector2.zero;

            Image overlayImage = settingsOverlay.GetComponent<Image>();
            overlayImage.color = new Color(0f, 0f, 0f, 0.58f);

            GameObject panel = new GameObject("Panel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            panel.transform.SetParent(settingsOverlay.transform, false);
            RectTransform panelRect = panel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(620f, 520f);
            panel.GetComponent<Image>().color = new Color(0.08f, 0.1f, 0.16f, 0.96f);

            CreateText(panel.transform, "Settings", font, 70, new Vector2(0f, 160f), new Vector2(520f, 100f));
            Button soundButton = CreateButton(panel.transform, "Sound: On", font, new Vector2(0f, 20f), new Vector2(420f, 90f));
            soundButtonText = soundButton.GetComponentInChildren<Text>();
            soundButton.onClick.AddListener(ToggleSound);

            Button closeButton = CreateButton(panel.transform, "Close", font, new Vector2(0f, -115f), new Vector2(320f, 80f));
            closeButton.onClick.AddListener(CloseSettings);

            settingsOverlay.SetActive(false);
            ApplyAudioSetting();
        }

        private static Text CreateText(Transform parent, string text, Font font, int fontSize, Vector2 position, Vector2 size)
        {
            GameObject textObject = new GameObject(text, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            textObject.transform.SetParent(parent, false);

            RectTransform rect = textObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;

            Text label = textObject.GetComponent<Text>();
            label.text = text;
            label.font = font;
            label.fontSize = fontSize;
            label.fontStyle = FontStyle.Bold;
            label.alignment = TextAnchor.MiddleCenter;
            label.color = Color.white;
            return label;
        }

        private static Button CreateButton(Transform parent, string text, Font font, Vector2 position, Vector2 size)
        {
            GameObject buttonObject = new GameObject(text, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(parent, false);

            RectTransform rect = buttonObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;

            Image image = buttonObject.GetComponent<Image>();
            image.color = new Color(0.2f, 0.74f, 0.36f, 1f);

            Button button = buttonObject.GetComponent<Button>();
            button.targetGraphic = image;
            CreateText(buttonObject.transform, text, font, 42, Vector2.zero, size);
            return button;
        }

        private static void SetActive(GameObject target, bool active)
        {
            if (target != null)
            {
                target.SetActive(active);
            }
        }

        private IEnumerator RevealGameOverAfterDelay()
        {
            yield return new WaitForSecondsRealtime(gameOverRevealDelay);
            SetActive(gameplayScreen, false);
            SetActive(gameOverScreen, true);
            gameOverRevealRoutine = null;
        }

        private void StopGameOverReveal()
        {
            if (gameOverRevealRoutine == null)
            {
                return;
            }

            StopCoroutine(gameOverRevealRoutine);
            gameOverRevealRoutine = null;
        }
    }
}
