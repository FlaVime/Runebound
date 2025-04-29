using UnityEngine;

[CreateAssetMenu(fileName = "New Node", menuName = "Node")]
public class NodeData : ScriptableObject
{
    public string nodeType; // node type
    public string nodeDescription; // description of the node
    public Sprite nodeIcon; // icon for the node

}