using TapAndCollect.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace TapAndCollect.UI
{
    public class HudView : MonoBehaviour
    {
        [SerializeField] private Text scoreText;
        [SerializeField] private Text livesText;
        [SerializeField] private Text timerText;
        [SerializeField] private float scorePopScale = 1.22f;
        [SerializeField] private float livesPopScale = 1.18f;
        [SerializeField] private float pulseReturnSpeed = 8f;
        [SerializeField] private float livesShakePixels = 8f;
        [SerializeField] private Color lostLifeColor = new Color(1f, 0.18f, 0.12f);

        private RectTransform scoreRect;
        private RectTransform livesRect;
        private Vector3 scoreBaseScale = Vector3.one;
        private Vector3 livesBaseScale = Vector3.one;
        private Vector2 livesBaseAnchoredPosition;
        private Color livesBaseColor = Color.white;
        private float scorePulse;
        private float livesPulse;
        private float livesShake;
        private int previousLives;
        private bool hasPreviousLives;

        private void Awake()
        {
            CacheTextState();
        }

        private void Update()
        {
            scorePulse = Mathf.MoveTowards(scorePulse, 0f, pulseReturnSpeed * Time.unscaledDeltaTime);
            livesPulse = Mathf.MoveTowards(livesPulse, 0f, pulseReturnSpeed * Time.unscaledDeltaTime);
            livesShake = Mathf.MoveTowards(livesShake, 0f, pulseReturnSpeed * Time.unscaledDeltaTime);

            ApplyPulse(scoreRect, scoreBaseScale, scorePulse, scorePopScale);
            ApplyPulse(livesRect, livesBaseScale, livesPulse, livesPopScale);

            if (livesRect != null)
            {
                float shake = Mathf.Sin(Time.unscaledTime * 70f) * livesShake * livesShakePixels;
                livesRect.anchoredPosition = livesBaseAnchoredPosition + new Vector2(shake, 0f);
            }

            if (livesText != null)
            {
                livesText.color = Color.Lerp(livesBaseColor, lostLifeColor, livesShake);
            }
        }

        private void OnEnable()
        {
            GameEvents.ScoreChanged += UpdateScore;
            GameEvents.LivesChanged += UpdateLives;
            GameEvents.TimerChanged += UpdateTimer;

            if (GameManager.Instance != null)
            {
                UpdateScore(GameManager.Instance.Score);
                UpdateLives(GameManager.Instance.Lives);
                UpdateTimer(GameManager.Instance.SecondsLeft);
            }
        }

        private void OnDisable()
        {
            GameEvents.ScoreChanged -= UpdateScore;
            GameEvents.LivesChanged -= UpdateLives;
            GameEvents.TimerChanged -= UpdateTimer;
        }

        private void UpdateScore(int score)
        {
            if (scoreText != null)
            {
                scoreText.text = $"Score: {score}";
                scorePulse = 1f;
            }
        }

        private void UpdateLives(int lives)
        {
            if (livesText != null)
            {
                livesText.text = $"Lives: {lives}";
                livesPulse = 1f;
                if (hasPreviousLives && lives < previousLives)
                {
                    livesShake = 1f;
                }

                previousLives = lives;
                hasPreviousLives = true;
            }
        }

        private void UpdateTimer(float secondsLeft)
        {
            if (timerText != null)
            {
                timerText.text = $"Time: {Mathf.CeilToInt(secondsLeft)}";
            }
        }

        private void CacheTextState()
        {
            if (scoreText != null)
            {
                scoreRect = scoreText.rectTransform;
                scoreBaseScale = scoreRect.localScale;
            }

            if (livesText != null)
            {
                livesRect = livesText.rectTransform;
                livesBaseScale = livesRect.localScale;
                livesBaseAnchoredPosition = livesRect.anchoredPosition;
                livesBaseColor = livesText.color;
            }
        }

        private static void ApplyPulse(RectTransform rect, Vector3 baseScale, float pulse, float maxScale)
        {
            if (rect == null)
            {
                return;
            }

            float easedPulse = 1f - Mathf.Pow(1f - pulse, 2f);
            rect.localScale = baseScale * Mathf.Lerp(1f, maxScale, easedPulse);
        }
    }
}
