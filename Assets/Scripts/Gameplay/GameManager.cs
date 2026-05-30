using TapAndCollect.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TapAndCollect.Gameplay
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private GameSettings settings;
        [SerializeField] private GameState startingState = GameState.Home;

        private int score;
        private int lives;
        private float secondsLeft;
        private GameState state;
        private float speedMultiplier = 1f;
        private float speedMultiplierUntil;

        public GameSettings Settings => settings;
        public int Score => score;
        public int Lives => lives;
        public float SecondsLeft => secondsLeft;
        public GameState State => state;
        public bool IsPlaying => state == GameState.Playing;
        public float Difficulty01
        {
            get
            {
                if (settings == null)
                {
                    return 0f;
                }

                float elapsed = Mathf.Max(0f, settings.RoundDuration - secondsLeft);
                float rampDuration = Mathf.Max(0.01f, settings.DifficultyRampDuration);
                return Mathf.Clamp01(elapsed / rampDuration);
            }
        }

        public float SpeedMultiplier
        {
            get
            {
                float difficultySpeed = settings != null
                    ? Mathf.Lerp(1f, Mathf.Max(1f, settings.MaxDifficultySpeedMultiplier), Difficulty01)
                    : 1f;
                return speedMultiplier * difficultySpeed;
            }
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            ResetRound();
            SetState(startingState);
        }

        private void Update()
        {
            if (state != GameState.Playing)
            {
                return;
            }

            secondsLeft -= Time.deltaTime;
            GameEvents.RaiseTimerChanged(Mathf.Max(0f, secondsLeft));

            if (speedMultiplier != 1f && Time.time >= speedMultiplierUntil)
            {
                speedMultiplier = 1f;
            }

            if (secondsLeft <= 0f)
            {
                EndGame();
            }
        }

        public void Play()
        {
            ResetRound();
            SetState(GameState.Playing);
        }

        public void Pause()
        {
            if (state != GameState.Playing)
            {
                return;
            }

            SetState(GameState.Paused);
        }

        public void Resume()
        {
            if (state == GameState.Paused)
            {
                SetState(GameState.Playing);
            }
        }

        public void Restart()
        {
            ResetRound();
            SetState(GameState.Playing);
        }

        public void GoHome()
        {
            ResetRound();
            SetState(GameState.Home);
        }

        public void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void Collect(FallingObjectConfig config, Vector3 worldPosition)
        {
            if (config == null || state != GameState.Playing)
            {
                return;
            }

            score += config.ScoreValue;
            lives += config.LifeChange;
            GameEvents.RaiseScoreChanged(score);
            GameEvents.RaiseLivesChanged(lives);
            GameEvents.RaiseObjectCollected(config, worldPosition);
            Debug.Log($"Collected {config.Kind} | Score: {score} | Lives: {lives}");

            if (config.Kind == FallingObjectKind.Bonus)
            {
                ApplySlowBonus(config.BonusSlowMultiplier, config.BonusSlowDuration);
            }

            if (lives <= 0)
            {
                EndGame();
            }
        }

        public void Collect(FallingObjectConfig config)
        {
            Collect(config, Vector3.zero);
        }

        private void ApplySlowBonus(float multiplier, float duration)
        {
            speedMultiplier = Mathf.Clamp(multiplier, 0.1f, 1f);
            speedMultiplierUntil = Time.time + Mathf.Max(0f, duration);
        }

        private void ResetRound()
        {
            score = 0;
            lives = settings != null ? settings.StartingLives : 3;
            secondsLeft = settings != null ? settings.RoundDuration : 60f;
            speedMultiplier = 1f;
            speedMultiplierUntil = 0f;

            GameEvents.RaiseScoreChanged(score);
            GameEvents.RaiseLivesChanged(lives);
            GameEvents.RaiseTimerChanged(secondsLeft);
        }

        private void EndGame()
        {
            SetState(GameState.GameOver);
        }

        private void SetState(GameState nextState)
        {
            state = nextState;
            Time.timeScale = state == GameState.Paused ? 0f : 1f;
            GameEvents.RaiseStateChanged(state);
        }
    }
}
