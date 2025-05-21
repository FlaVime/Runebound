using UnityEngine;
using UnityEngine.UI;

public class VictoryPanelController : MonoBehaviour
{
    [SerializeField] private Button againButton;
    [SerializeField] private Button exitButton;

    private void OnEnable()
    {
        if (againButton != null)
            againButton.onClick.AddListener(OnAgainButtonClick);

        if (exitButton != null)
            exitButton.onClick.AddListener(OnExitButtonClick);
    }

    private void OnDisable()
    {
        if (againButton != null)
            againButton.onClick.RemoveListener(OnAgainButtonClick);

        if (exitButton != null)
            exitButton.onClick.RemoveListener(OnExitButtonClick);
    }

    private void OnAgainButtonClick()
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
