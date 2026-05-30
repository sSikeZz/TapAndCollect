using TapAndCollect.Data;
using UnityEngine;

namespace TapAndCollect.Gameplay
{
    [RequireComponent(typeof(Collider2D))]
    public class FallingObject : MonoBehaviour
    {
        [SerializeField] private FallingObjectConfig config;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private float rotationSpeed;
        [SerializeField] private bool useTrail = true;
        [SerializeField] private float trailTime = 0.22f;
        [SerializeField] private float trailStartWidth = 0.22f;
        [SerializeField] private float trailEndWidth = 0.02f;
        [Header("Feel")]
        [SerializeField] private Vector2 randomRotationSpeed = new Vector2(-90f, 90f);
        [SerializeField] private float driftAmplitude = 0.16f;
        [SerializeField] private float driftFrequency = 2.4f;
        [SerializeField] private float spawnPopDuration = 0.16f;
        [SerializeField] private float spawnStartScale = 0.72f;

        private bool collected;
        private TrailRenderer trailRenderer;
        private Vector3 baseScale;
        private float age;
        private float spawnX;
        private float driftPhase;

        public FallingObjectConfig Config => config;

        private void Awake()
        {
            Collider2D objectCollider = GetComponent<Collider2D>();
            objectCollider.isTrigger = true;

            Rigidbody2D body = GetComponent<Rigidbody2D>();
            if (body == null)
            {
                body = gameObject.AddComponent<Rigidbody2D>();
            }

            body.bodyType = RigidbodyType2D.Kinematic;
            body.gravityScale = 0f;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }

            SetupTrail();
            baseScale = transform.localScale;
        }

        private void OnEnable()
        {
            collected = false;
            age = 0f;
            spawnX = transform.position.x;
            driftPhase = UnityEngine.Random.value * Mathf.PI * 2f;
            rotationSpeed = UnityEngine.Random.Range(randomRotationSpeed.x, randomRotationSpeed.y);
            transform.localScale = baseScale * spawnStartScale;

            if (trailRenderer != null)
            {
                trailRenderer.Clear();
            }

            ApplyConfigVisuals();
        }

        private void Update()
        {
            if (GameManager.Instance == null || !GameManager.Instance.IsPlaying || config == null)
            {
                return;
            }

            float speed = config.FallSpeed * GameManager.Instance.SpeedMultiplier;
            age += Time.deltaTime;

            Vector3 position = transform.position;
            position.y -= speed * Time.deltaTime;
            position.x = spawnX + Mathf.Sin(age * driftFrequency + driftPhase) * driftAmplitude;
            transform.position = position;

            float popT = spawnPopDuration > 0f ? Mathf.Clamp01(age / spawnPopDuration) : 1f;
            float easedPopT = 1f - Mathf.Pow(1f - popT, 3f);
            transform.localScale = Vector3.LerpUnclamped(baseScale * spawnStartScale, baseScale, easedPopT);

            if (!Mathf.Approximately(rotationSpeed, 0f))
            {
                transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
            }

            float despawnY = GameManager.Instance.Settings != null ? GameManager.Instance.Settings.DespawnY : -6f;
            if (transform.position.y <= despawnY)
            {
                Destroy(gameObject);
            }
        }

        public void Initialize(FallingObjectConfig nextConfig)
        {
            config = nextConfig;
            ApplyConfigVisuals();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (collected || !other.TryGetComponent(out Basket _))
            {
                return;
            }

            collected = true;
            GameManager.Instance?.Collect(config, transform.position);
            Destroy(gameObject);
        }

        private void ApplyConfigVisuals()
        {
            if (spriteRenderer == null || config == null)
            {
                return;
            }

            if (config.Sprite != null)
            {
                spriteRenderer.sprite = config.Sprite;
            }

            if (config.OverridePrefabColor)
            {
                spriteRenderer.color = config.Tint;
            }

            ApplyTrailColor();
        }

        private void SetupTrail()
        {
            if (!useTrail)
            {
                return;
            }

            trailRenderer = GetComponent<TrailRenderer>();
            if (trailRenderer == null)
            {
                trailRenderer = gameObject.AddComponent<TrailRenderer>();
            }

            trailRenderer.time = trailTime;
            trailRenderer.startWidth = trailStartWidth;
            trailRenderer.endWidth = trailEndWidth;
            trailRenderer.minVertexDistance = 0.04f;
            trailRenderer.numCornerVertices = 4;
            trailRenderer.numCapVertices = 4;
            trailRenderer.alignment = LineAlignment.View;
            trailRenderer.textureMode = LineTextureMode.Stretch;
            trailRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            trailRenderer.receiveShadows = false;
            trailRenderer.sortingOrder = -1;
            trailRenderer.material = CreateTrailMaterial();
            ApplyTrailColor();
        }

        private Material CreateTrailMaterial()
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
            if (shader == null)
            {
                shader = Shader.Find("Sprites/Default");
            }

            return shader != null ? new Material(shader) : null;
        }

        private void ApplyTrailColor()
        {
            if (trailRenderer == null)
            {
                return;
            }

            Color coreColor = spriteRenderer != null ? spriteRenderer.color : Color.white;
            if (config != null)
            {
                switch (config.Kind)
                {
                    case Data.FallingObjectKind.Bad:
                        coreColor = new Color(1f, 0.18f, 0.08f, 1f);
                        break;
                    case Data.FallingObjectKind.Bonus:
                        coreColor = new Color(1f, 0.78f, 0.12f, 1f);
                        break;
                    default:
                        coreColor = new Color(0.35f, 1f, 0.1f, 1f);
                        break;
                }
            }

            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new[]
                {
                    new GradientColorKey(Color.white, 0f),
                    new GradientColorKey(coreColor, 0.35f),
                    new GradientColorKey(new Color(1f, 0.38f, 0.05f), 1f)
                },
                new[]
                {
                    new GradientAlphaKey(0.95f, 0f),
                    new GradientAlphaKey(0.55f, 0.35f),
                    new GradientAlphaKey(0f, 1f)
                }
            );

            trailRenderer.colorGradient = gradient;
        }
    }
}
