using UnityEngine;

namespace TapAndCollect.Data
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Tap And Collect/Game Settings")]
    public class GameSettings : ScriptableObject
    {
        [SerializeField] private int startingLives = 3;
        [SerializeField] private float roundDuration = 60f;
        [SerializeField] private float spawnInterval = 0.75f;
        [SerializeField] private float spawnXPadding = 0.7f;
        [SerializeField] private float spawnY = 6f;
        [SerializeField] private float despawnY = -6f;
        [Header("Difficulty Feel")]
        [SerializeField] private float difficultyRampDuration = 45f;
        [SerializeField] private float maxDifficultySpeedMultiplier = 1.45f;
        [SerializeField] private float minSpawnIntervalMultiplier = 0.62f;
        [SerializeField] private float spawnMinHorizontalGap = 0.9f;

        public int StartingLives => startingLives;
        public float RoundDuration => roundDuration;
        public float SpawnInterval => spawnInterval;
        public float SpawnXPadding => spawnXPadding;
        public float SpawnY => spawnY;
        public float DespawnY => despawnY;
        public float DifficultyRampDuration => difficultyRampDuration;
        public float MaxDifficultySpeedMultiplier => maxDifficultySpeedMultiplier;
        public float MinSpawnIntervalMultiplier => minSpawnIntervalMultiplier;
        public float SpawnMinHorizontalGap => spawnMinHorizontalGap;
    }
}
