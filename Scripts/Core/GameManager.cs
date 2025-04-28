using UnityEngine;

public enum GameState { MainMenu, Map, Combat }

public class GameManager : Singleton<GameManager> {
    public GameState CurrentState { get; private set; }

    private void Start() {
        ChangeState(GameState.MainMenu);
    }

    public void ChangeState(GameState newState) {
        CurrentState = newState;
        SceneLoader.Instance.Load(newState.ToString());
    }
}
