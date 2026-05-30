using TapAndCollect.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace TapAndCollect.UI
{
    public class GameOverView : MonoBehaviour
    {
        [SerializeField] private Text finalScoreText;

        private void OnEnable()
        {
            GameEvents.StateChanged += HandleStateChanged;
            Refresh();
        }

        private void OnDisable()
        {
            GameEvents.StateChanged -= HandleStateChanged;
        }

        private void HandleStateChanged(GameState state)
        {
            if (state == GameState.GameOver)
            {
                Refresh();
            }
        }

        private void Refresh()
        {
            if (finalScoreText != null && GameManager.Instance != null)
            {
                finalScoreText.text = $"Final Score: {GameManager.Instance.Score}";
            }
        }
    }
}
