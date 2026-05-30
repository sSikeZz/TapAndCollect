using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TapAndCollect.Input
{
    public class ScreenPointerInput : MonoBehaviour
    {
        public static event Action<Vector2> WorldPositionChanged;

        [SerializeField] private Camera inputCamera;

        private void Awake()
        {
            if (inputCamera == null)
            {
                inputCamera = Camera.main;
            }
        }

        private void Update()
        {
            if (!TryGetScreenPosition(out Vector2 screenPosition) || inputCamera == null)
            {
                return;
            }

            Vector3 world = inputCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, Mathf.Abs(inputCamera.transform.position.z)));
            WorldPositionChanged?.Invoke(world);
        }

        private static bool TryGetScreenPosition(out Vector2 screenPosition)
        {
            if (Touchscreen.current != null)
            {
                var touch = Touchscreen.current.primaryTouch;
                if (touch.press.isPressed)
                {
                    screenPosition = touch.position.ReadValue();
                    return true;
                }
            }

            if (Mouse.current != null && Mouse.current.leftButton.isPressed)
            {
                screenPosition = Mouse.current.position.ReadValue();
                return true;
            }

            screenPosition = default;
            return false;
        }
    }
}
