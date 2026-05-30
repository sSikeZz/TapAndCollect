using UnityEngine;

namespace TapAndCollect.Data
{
    [CreateAssetMenu(fileName = "FallingObjectConfig", menuName = "Tap And Collect/Falling Object Config")]
    public class FallingObjectConfig : ScriptableObject
    {
        [SerializeField] private FallingObjectKind kind = FallingObjectKind.Good;
        [SerializeField] private Sprite sprite;
        [SerializeField] private bool overridePrefabColor;
        [SerializeField] private Color tint = Color.white;
        [SerializeField] private int scoreValue = 10;
        [SerializeField] private int lifeChange;
        [SerializeField] private float fallSpeed = 4f;
        [SerializeField] private float bonusSlowDuration = 3f;
        [SerializeField] private float bonusSlowMultiplier = 0.6f;

        public FallingObjectKind Kind => kind;
        public Sprite Sprite => sprite;
        public bool OverridePrefabColor => overridePrefabColor;
        public Color Tint => tint;
        public int ScoreValue => scoreValue;
        public int LifeChange => lifeChange;
        public float FallSpeed => fallSpeed;
        public float BonusSlowDuration => bonusSlowDuration;
        public float BonusSlowMultiplier => bonusSlowMultiplier;
    }
}
