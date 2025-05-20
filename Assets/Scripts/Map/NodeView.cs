using UnityEngine;

public class NodeView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SpriteRenderer iconRenderer;

    public NodeModel model { get; private set; }

    private Color visitedColor = new Color(0.5f, 0.5f, 0.5f);
    private Color lockedColor = new Color(0.4f, 0.3f, 0.2f);
    private Color unlockedColor = new Color(0.2f, 0.8f, 0.3f);
    private MapSystem mapSystem;

    public void Initialize(NodeModel model, MapSystem mapSystem)
    {
        this.model = model;
        this.mapSystem = mapSystem;

        UpdateVisuals();
        SetIcon();
    }

    private void OnMouseDown()
    {
        if (Time.timeScale == 0f) return;
        if (!model.isUnlocked || model.isVisited) return;

        mapSystem.SelectNode(model.id);
    }

    public void UpdateVisuals()
    {
        if (model.isVisited)
            spriteRenderer.color = visitedColor;
        else if (model.isUnlocked)
            spriteRenderer.color = unlockedColor;
        else
            spriteRenderer.color = lockedColor;
    }

    private void SetIcon()
    {
        Debug.Log("SetIcon called — icon = " + model.nodeIcon);
        if (iconRenderer != null && model.nodeIcon != null)
        {
            iconRenderer.sprite = model.nodeIcon;
        }
    }
}
