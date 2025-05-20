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

    public Sprite nodeIcon;

    public NodeModel(int id, string nodeType, Vector2 position, Sprite icon = null)
    {
        this.id = id;
        this.nodeType = nodeType;
        this.position = position;
        this.nodeIcon = icon;
        isVisited = false;
        isUnlocked = false;
    }
}
