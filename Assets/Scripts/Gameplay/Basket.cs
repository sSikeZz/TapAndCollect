using UnityEngine;

namespace TapAndCollect.Gameplay
{
    [RequireComponent(typeof(Collider2D))]
    public class Basket : MonoBehaviour
    {
        [SerializeField] private float minX = -2.35f;
        [SerializeField] private float maxX = 2.35f;
        [SerializeField] private float followSpeed = 18f;
        [SerializeField] private Vector3 collectPopScale = new Vector3(1.16f, 0.84f, 1f);
        [SerializeField] private float popSpeed = 22f;
        [SerializeField] private float returnSpeed = 14f;
        [SerializeField] private float maxMoveTilt = 10f;
        [SerializeField] private float velocityForMaxFeel = 7f;
        [SerializeField] private float moveSquashAmount = 0.08f;
        [SerializeField] private float moveStretchAmount = 0.04f;
        [SerializeField] private float tiltReturnSpeed = 18f;

        private float targetX;
        private Vector3 baseScale;
        private float collectPop;
        private float previousX;

        private void Awake()
        {
            Collider2D basketCollider = GetComponent<Collider2D>();
            basketCollider.isTrigger = true;

            Rigidbody2D body = GetComponent<Rigidbody2D>();
            if (body == null)
            {
                body = gameObject.AddComponent<Rigidbody2D>();
            }

            body.bodyType = RigidbodyType2D.Kinematic;
            body.gravityScale = 0f;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            targetX = transform.position.x;
            baseScale = transform.localScale;
            previousX = transform.position.x;
        }

        private void OnEnable()
        {
            TapAndCollect.Input.ScreenPointerInput.WorldPositionChanged += HandleWorldPositionChanged;
            GameEvents.ObjectCollected += HandleObjectCollected;
        }

        private void OnDisable()
        {
            TapAndCollect.Input.ScreenPointerInput.WorldPositionChanged -= HandleWorldPositionChanged;
            GameEvents.ObjectCollected -= HandleObjectCollected;
        }

        private void Update()
        {
            if (GameManager.Instance != null && !GameManager.Instance.IsPlaying)
            {
                return;
            }

            Vector3 position = transform.position;
            position.x = Mathf.Lerp(position.x, targetX, 1f - Mathf.Exp(-followSpeed * Time.deltaTime));
            transform.position = position;

            float velocity = Time.deltaTime > 0f ? (position.x - previousX) / Time.deltaTime : 0f;
            previousX = position.x;

            collectPop = Mathf.MoveTowards(collectPop, 0f, returnSpeed * Time.deltaTime);

            float velocityT = Mathf.Clamp01(Mathf.Abs(velocity) / Mathf.Max(0.01f, velocityForMaxFeel));
            Vector3 motionScale = new Vector3(
                1f + velocityT * moveStretchAmount,
                1f - velocityT * moveSquashAmount,
                1f
            );
            Vector3 collectScale = Vector3.Lerp(Vector3.one, collectPopScale, collectPop);
            Vector3 desiredScale = Vector3.Scale(baseScale, Vector3.Scale(motionScale, collectScale));
            float scaleSpeed = collectPop > 0f ? popSpeed : returnSpeed;
            transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, 1f - Mathf.Exp(-scaleSpeed * Time.deltaTime));

            float targetTilt = -Mathf.Clamp(velocity / Mathf.Max(0.01f, velocityForMaxFeel), -1f, 1f) * maxMoveTilt;
            Quaternion desiredRotation = Quaternion.Euler(0f, 0f, targetTilt);
            transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, 1f - Mathf.Exp(-tiltReturnSpeed * Time.deltaTime));
        }

        private void HandleWorldPositionChanged(Vector2 worldPosition)
        {
            targetX = Mathf.Clamp(worldPosition.x, minX, maxX);
        }

        private void HandleObjectCollected(TapAndCollect.Data.FallingObjectConfig config, Vector3 worldPosition)
        {
            collectPop = 1f;
        }
    }
}
