using System.Collections.Generic;
using TapAndCollect.Data;
using TapAndCollect.Gameplay;
using UnityEngine;

namespace TapAndCollect.Feedback
{
    public class GameFeedbackController : MonoBehaviour
    {
        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip goodCollectClip;
        [SerializeField] private AudioClip badCollectClip;
        [SerializeField] private AudioClip bonusCollectClip;
        [SerializeField] private AudioClip gameOverClip;

        [Header("Camera Feel")]
        [SerializeField] private Camera shakeCamera;
        [SerializeField] private float goodShakeStrength = 0.025f;
        [SerializeField] private float badShakeStrength = 0.14f;
        [SerializeField] private float bonusShakeStrength = 0.06f;
        [SerializeField] private float shakeDecay = 4f;
        [SerializeField] private Color badFlashColor = new Color(1f, 0.05f, 0.02f, 0.32f);
        [SerializeField] private Color bonusFlashColor = new Color(1f, 0.82f, 0.1f, 0.24f);
        [SerializeField] private float flashFadeSpeed = 4.5f;

        [Header("Particles")]
        [SerializeField] private Material particleMaterial;
        [SerializeField] private ParticleSystem goodCollectEffect;
        [SerializeField] private ParticleSystem badCollectEffect;
        [SerializeField] private ParticleSystem bonusCollectEffect;
        [SerializeField] private int poolSizePerEffect = 4;

        [Header("Fallback Particle Colors")]
        [SerializeField] private bool createDefaultEffects = true;
        [SerializeField] private Color goodColor = new Color(0.3f, 1f, 0.45f);
        [SerializeField] private Color badColor = new Color(1f, 0.25f, 0.2f);
        [SerializeField] private Color bonusColor = new Color(1f, 0.85f, 0.2f);

        private readonly Dictionary<ParticleSystem, Queue<ParticleSystem>> pools = new Dictionary<ParticleSystem, Queue<ParticleSystem>>();
        private float shakeIntensity;
        private Vector3 lastShakeOffset;
        private Color activeFlashColor;
        private float flashAlpha;

        private void Awake()
        {
            if (shakeCamera == null)
            {
                shakeCamera = Camera.main;
            }

            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }

            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f;

            if (createDefaultEffects)
            {
                goodCollectEffect = goodCollectEffect != null ? goodCollectEffect : CreateRuntimeEffect("Good Collect Effect", goodColor);
                badCollectEffect = badCollectEffect != null ? badCollectEffect : CreateRuntimeEffect("Bad Collect Effect", badColor);
                bonusCollectEffect = bonusCollectEffect != null ? bonusCollectEffect : CreateRuntimeEffect("Bonus Collect Effect", bonusColor);
            }

            Prewarm(goodCollectEffect);
            Prewarm(badCollectEffect);
            Prewarm(bonusCollectEffect);
        }

        private void Update()
        {
            shakeIntensity = Mathf.MoveTowards(shakeIntensity, 0f, shakeDecay * Time.unscaledDeltaTime);
            flashAlpha = Mathf.MoveTowards(flashAlpha, 0f, flashFadeSpeed * Time.unscaledDeltaTime);
        }

        private void LateUpdate()
        {
            if (shakeCamera == null)
            {
                return;
            }

            shakeCamera.transform.localPosition -= lastShakeOffset;
            lastShakeOffset = Vector3.zero;

            if (shakeIntensity <= 0.001f)
            {
                return;
            }

            lastShakeOffset = new Vector3(
                UnityEngine.Random.Range(-shakeIntensity, shakeIntensity),
                UnityEngine.Random.Range(-shakeIntensity, shakeIntensity),
                0f
            );
            shakeCamera.transform.localPosition += lastShakeOffset;
        }

        private void OnGUI()
        {
            if (flashAlpha <= 0.001f)
            {
                return;
            }

            Color previousColor = GUI.color;
            Color color = activeFlashColor;
            color.a *= flashAlpha;
            GUI.color = color;
            GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), Texture2D.whiteTexture);
            GUI.color = previousColor;
        }

        private void OnEnable()
        {
            GameEvents.ObjectCollected += HandleObjectCollected;
            GameEvents.StateChanged += HandleStateChanged;
        }

        private void OnDisable()
        {
            GameEvents.ObjectCollected -= HandleObjectCollected;
            GameEvents.StateChanged -= HandleStateChanged;
        }

        private void HandleObjectCollected(FallingObjectConfig config, Vector3 worldPosition)
        {
            if (config == null)
            {
                return;
            }

            switch (config.Kind)
            {
                case FallingObjectKind.Good:
                    PlayClip(goodCollectClip, UnityEngine.Random.Range(0.96f, 1.06f));
                    PlayEffect(goodCollectEffect, worldPosition);
                    AddShake(goodShakeStrength);
                    break;
                case FallingObjectKind.Bad:
                    PlayClip(badCollectClip, 0.82f);
                    PlayEffect(badCollectEffect, worldPosition);
                    AddShake(badShakeStrength);
                    Flash(badFlashColor);
                    break;
                case FallingObjectKind.Bonus:
                    PlayClip(bonusCollectClip, 1.12f);
                    PlayEffect(bonusCollectEffect, worldPosition);
                    AddShake(bonusShakeStrength);
                    Flash(bonusFlashColor);
                    break;
            }
        }

        private void HandleStateChanged(GameState state)
        {
            if (state == GameState.GameOver)
            {
                PlayClip(gameOverClip, 0.88f);
                AddShake(badShakeStrength);
                Flash(badFlashColor);
            }
        }

        private void PlayClip(AudioClip clip, float pitch = 1f)
        {
            if (clip != null && audioSource != null)
            {
                audioSource.pitch = pitch;
                audioSource.PlayOneShot(clip);
                audioSource.pitch = 1f;
            }
        }

        private void AddShake(float amount)
        {
            shakeIntensity = Mathf.Max(shakeIntensity, amount);
        }

        private void Flash(Color color)
        {
            activeFlashColor = color;
            flashAlpha = 1f;
        }

        private void Prewarm(ParticleSystem prefab)
        {
            if (prefab == null || pools.ContainsKey(prefab))
            {
                return;
            }

            Queue<ParticleSystem> pool = new Queue<ParticleSystem>(poolSizePerEffect);
            pools.Add(prefab, pool);

            for (int i = 0; i < poolSizePerEffect; i++)
            {
                ParticleSystem instance = Instantiate(prefab, transform);
                instance.gameObject.SetActive(false);
                pool.Enqueue(instance);
            }
        }

        private ParticleSystem CreateRuntimeEffect(string effectName, Color color)
        {
            GameObject effectObject = new GameObject(effectName);
            effectObject.transform.SetParent(transform);
            effectObject.SetActive(false);

            ParticleSystem effect = effectObject.AddComponent<ParticleSystem>();
            ParticleSystem.MainModule main = effect.main;
            main.duration = 0.3f;
            main.loop = false;
            main.playOnAwake = false;
            main.startLifetime = 0.35f;
            main.startSpeed = 2.5f;
            main.startSize = 0.12f;
            main.startColor = color;
            main.maxParticles = 24;

            ParticleSystem.EmissionModule emission = effect.emission;
            emission.rateOverTime = 0f;
            emission.SetBursts(new[] { new ParticleSystem.Burst(0f, (short)16) });

            ParticleSystem.ShapeModule shape = effect.shape;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = 0.2f;

            ParticleSystemRenderer renderer = effect.GetComponent<ParticleSystemRenderer>();
            renderer.sharedMaterial = GetParticleMaterial();
            renderer.sortingOrder = 20;

            return effect;
        }

        private Material GetParticleMaterial()
        {
            if (particleMaterial != null)
            {
                return particleMaterial;
            }

            Shader shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
            if (shader == null)
            {
                shader = Shader.Find("Sprites/Default");
            }

            if (shader == null)
            {
                shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");
            }

            if (shader != null)
            {
                particleMaterial = new Material(shader);
            }

            return particleMaterial;
        }

        private void PlayEffect(ParticleSystem prefab, Vector3 worldPosition)
        {
            if (prefab == null)
            {
                return;
            }

            if (!pools.TryGetValue(prefab, out Queue<ParticleSystem> pool))
            {
                Prewarm(prefab);
                pool = pools[prefab];
            }

            ParticleSystem effect = pool.Count > 0 ? pool.Dequeue() : Instantiate(prefab, transform);
            effect.transform.position = worldPosition;
            effect.gameObject.SetActive(true);
            effect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            effect.Play(true);
            StartCoroutine(ReturnWhenFinished(effect, prefab));
        }

        private System.Collections.IEnumerator ReturnWhenFinished(ParticleSystem effect, ParticleSystem prefab)
        {
            while (effect != null && effect.IsAlive(true))
            {
                yield return null;
            }

            if (effect == null)
            {
                yield break;
            }

            effect.gameObject.SetActive(false);
            pools[prefab].Enqueue(effect);
        }
    }
}
