using UnityEngine;
using UnityEngine.UI;

public class DefeatPanelController : MonoBehaviour
{
    [SerializeField] private Button restartButton;
    [SerializeField] private Button exitButton;

    private void OnEnable()
    {
        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartButtonClick);

        if (exitButton != null)
            exitButton.onClick.AddListener(OnExitButtonClick);
    }

    private void OnDisable()
    {
        if (restartButton != null)
            restartButton.onClick.RemoveListener(OnRestartButtonClick);

        if (exitButton != null)
            exitButton.onClick.RemoveListener(OnExitButtonClick);
    }

    private void OnRestartButtonClick()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetMapOnly();
            GameManager.Instance.ContinueGame();
        }
    }

    private void OnExitButtonClick()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetMapOnly();
            GameManager.Instance.ChangeState(GameState.MainMenu);
        }
    }
}
