using UnityEngine;

namespace TapAndCollect.Gameplay
{
    public static class GameplayBackground
    {
        private const string ResourceName = "GameplayBackground";
        private const string ObjectName = "Gameplay Background";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void CreateBackground()
        {
            if (GameObject.Find(ObjectName) != null)
            {
                return;
            }

            Sprite sprite = Resources.Load<Sprite>(ResourceName);
            if (sprite == null)
            {
                return;
            }

            Camera camera = Camera.main;
            GameObject background = new GameObject(ObjectName);
            SpriteRenderer renderer = background.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = -100;

            if (camera == null)
            {
                background.transform.position = Vector3.zero;
                return;
            }

            background.transform.position = new Vector3(camera.transform.position.x, camera.transform.position.y, 0f);

            float worldHeight = camera.orthographicSize * 2f;
            float worldWidth = worldHeight * camera.aspect;
            Vector2 spriteSize = sprite.bounds.size;
            float scale = Mathf.Max(worldWidth / spriteSize.x, worldHeight / spriteSize.y);
            background.transform.localScale = Vector3.one * scale;
        }
    }
}
