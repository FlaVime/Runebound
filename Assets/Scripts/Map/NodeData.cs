using UnityEngine;

[CreateAssetMenu(fileName = "New Node", menuName = "Node")]
public class NodeData : ScriptableObject
{
    public string nodeType;
    public Sprite nodeIcon;
}