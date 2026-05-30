using System;
using TapAndCollect.Data;
using UnityEngine;

namespace TapAndCollect.Gameplay
{
    public class FallingObjectSpawner : MonoBehaviour
    {
        [Serializable]
        public class SpawnEntry
        {
            public FallingObject prefab;
            public FallingObjectConfig config;
            [Min(0)] public int weight = 1;
        }

        [SerializeField] private Camera gameplayCamera;
        [SerializeField] private FallingObject fallbackPrefab;
        [SerializeField] private SpawnEntry[] spawnEntries;

        private float nextSpawnTime;
        private float lastSpawnX;
        private bool hasLastSpawnX;

        private void Awake()
        {
            if (gameplayCamera == null)
            {
                gameplayCamera = Camera.main;
            }
        }

        private void Update()
        {
            if (GameManager.Instance == null || !GameManager.Instance.IsPlaying)
            {
                return;
            }

            GameSettings settings = GameManager.Instance.Settings;
            float interval = settings != null ? settings.SpawnInterval : 0.75f;
            if (settings != null)
            {
                float intervalMultiplier = Mathf.Lerp(1f, Mathf.Clamp(settings.MinSpawnIntervalMultiplier, 0.2f, 1f), GameManager.Instance.Difficulty01);
                interval *= intervalMultiplier;
            }

            if (Time.time < nextSpawnTime)
            {
                return;
            }

            Spawn();
            nextSpawnTime = Time.time + interval;
        }

        private void Spawn()
        {
            SpawnEntry entry = PickEntry();
            if (entry == null || entry.config == null)
            {
                return;
            }

            FallingObject prefab = entry.prefab != null ? entry.prefab : fallbackPrefab;
            if (prefab == null)
            {
                return;
            }

            Vector3 spawnPosition = GetSpawnPosition();
            FallingObject spawned = Instantiate(prefab, spawnPosition, Quaternion.identity);
            spawned.Initialize(entry.config);
            lastSpawnX = spawnPosition.x;
            hasLastSpawnX = true;
        }

        private Vector3 GetSpawnPosition()
        {
            GameSettings settings = GameManager.Instance != null ? GameManager.Instance.Settings : null;
            float y = settings != null ? settings.SpawnY : 6f;
            float padding = settings != null ? settings.SpawnXPadding : 0.7f;

            if (gameplayCamera == null)
            {
                return new Vector3(UnityEngine.Random.Range(-2.3f, 2.3f), y, 0f);
            }

            Vector3 left = gameplayCamera.ViewportToWorldPoint(new Vector3(0f, 1f, Mathf.Abs(gameplayCamera.transform.position.z)));
            Vector3 right = gameplayCamera.ViewportToWorldPoint(new Vector3(1f, 1f, Mathf.Abs(gameplayCamera.transform.position.z)));
            float minX = left.x + padding;
            float maxX = right.x - padding;
            float x = PickSpawnX(minX, maxX);
            return new Vector3(x, y, 0f);
        }

        private float PickSpawnX(float minX, float maxX)
        {
            if (minX >= maxX)
            {
                return (minX + maxX) * 0.5f;
            }

            GameSettings settings = GameManager.Instance != null ? GameManager.Instance.Settings : null;
            float minGap = settings != null ? Mathf.Max(0f, settings.SpawnMinHorizontalGap) : 0.9f;
            float x = UnityEngine.Random.Range(minX, maxX);

            if (!hasLastSpawnX || minGap <= 0f || maxX - minX <= minGap)
            {
                return x;
            }

            for (int i = 0; i < 8 && Mathf.Abs(x - lastSpawnX) < minGap; i++)
            {
                x = UnityEngine.Random.Range(minX, maxX);
            }

            if (Mathf.Abs(x - lastSpawnX) >= minGap)
            {
                return x;
            }

            float leftCandidate = Mathf.Clamp(lastSpawnX - minGap, minX, maxX);
            float rightCandidate = Mathf.Clamp(lastSpawnX + minGap, minX, maxX);
            return UnityEngine.Random.value < 0.5f ? leftCandidate : rightCandidate;
        }

        private SpawnEntry PickEntry()
        {
            if (spawnEntries == null || spawnEntries.Length == 0)
            {
                return null;
            }

            int totalWeight = 0;
            foreach (SpawnEntry entry in spawnEntries)
            {
                if (entry != null && entry.config != null)
                {
                    totalWeight += Mathf.Max(0, entry.weight);
                }
            }

            if (totalWeight <= 0)
            {
                return null;
            }

            int roll = UnityEngine.Random.Range(0, totalWeight);
            foreach (SpawnEntry entry in spawnEntries)
            {
                if (entry == null || entry.config == null)
                {
                    continue;
                }

                roll -= Mathf.Max(0, entry.weight);
                if (roll < 0)
                {
                    return entry;
                }
            }

            return spawnEntries[0];
        }
    }
}
