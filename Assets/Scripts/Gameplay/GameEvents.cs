using System;
using TapAndCollect.Data;
using UnityEngine;

namespace TapAndCollect.Gameplay
{
    public static class GameEvents
    {
        public static event Action<GameState> StateChanged;
        public static event Action<int> ScoreChanged;
        public static event Action<int> LivesChanged;
        public static event Action<float> TimerChanged;
        public static event Action<FallingObjectConfig, Vector3> ObjectCollected;

        public static void RaiseStateChanged(GameState state) => StateChanged?.Invoke(state);
        public static void RaiseScoreChanged(int score) => ScoreChanged?.Invoke(score);
        public static void RaiseLivesChanged(int lives) => LivesChanged?.Invoke(lives);
        public static void RaiseTimerChanged(float secondsLeft) => TimerChanged?.Invoke(secondsLeft);
        public static void RaiseObjectCollected(FallingObjectConfig config, Vector3 worldPosition) => ObjectCollected?.Invoke(config, worldPosition);
    }
}
