using UnityEngine;

namespace TapAndCollect.Gameplay
{
    public class BasketSpawner : MonoBehaviour
    {
        [SerializeField] private Basket basketPrefab;
        [SerializeField] private Camera gameplayCamera;
        [SerializeField] private float viewportY = 0.1f;
        [SerializeField] private Vector2 fallbackSize = new Vector2(1.6f, 0.45f);
        [SerializeField] private Color fallbackColor = new Color(0.15f, 0.65f, 1f);

        private void Awake()
        {
            if (FindFirstObjectByType<Basket>() != null)
            {
                return;
            }

            if (gameplayCamera == null)
            {
                gameplayCamera = Camera.main;
            }

            Vector3 spawnPosition = GetSpawnPosition();

            if (basketPrefab != null)
            {
                Instantiate(basketPrefab, spawnPosition, Quaternion.identity);
                return;
            }

            CreateFallbackBasket(spawnPosition);
        }

        private Vector3 GetSpawnPosition()
        {
            if (gameplayCamera == null)
            {
                return new Vector3(0f, -4f, 0f);
            }

            Vector3 worldPosition = gameplayCamera.ViewportToWorldPoint(
                new Vector3(0.5f, viewportY, Mathf.Abs(gameplayCamera.transform.position.z))
            );
            worldPosition.z = 0f;
            return worldPosition;
        }

        private void CreateFallbackBasket(Vector3 spawnPosition)
        {
            GameObject basketObject = new GameObject("Basket");
            basketObject.transform.position = spawnPosition;
            basketObject.transform.localScale = new Vector3(fallbackSize.x, fallbackSize.y, 1f);

            SpriteRenderer spriteRenderer = basketObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = CreateWhiteSprite();
            spriteRenderer.color = fallbackColor;
            spriteRenderer.sortingOrder = 5;

            BoxCollider2D collider = basketObject.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;

            basketObject.AddComponent<Basket>();
        }

        private static Sprite CreateWhiteSprite()
        {
            Texture2D texture = new Texture2D(8, 8);
            Color[] pixels = new Color[64];

            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = Color.white;
            }

            texture.SetPixels(pixels);
            texture.Apply();

            return Sprite.Create(texture, new Rect(0f, 0f, 8f, 8f), new Vector2(0.5f, 0.5f), 8f);
        }
    }
}
