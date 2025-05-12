using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NodeModel
{
    public int id;
    public string nodeType;
    public bool isVisited;
    public bool isUnlocked;
    public List<int> connectedNodeIds = new List<int>();
    
    // Store position for line drawing
    public Vector2 position;
    
    public NodeModel(int id, string nodeType, Vector2 position)
    {
        this.id = id;
        this.nodeType = nodeType;
        this.position = position;
        isVisited = false;
        isUnlocked = false;
    }
}
