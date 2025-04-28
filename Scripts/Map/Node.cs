using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Node : MonoBehaviour {
    public NodeType Type;
    private bool visited = false;

    private void OnMouseDown() {
        if (visited) return;
        visited = true;

        // Меняем цвет узла чтобы показать, что он уже пройден
        GetComponent<SpriteRenderer>().color = Color.gray;

        // Переходим в сцену в зависимости от типа узла
        GameManager.Instance.ChangeState((GameState)System.Enum.Parse(typeof(GameState), Type.ToString()));
    }
}
