using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MapGenerator : MonoBehaviour {
    private List<Node> nodes = new List<Node>();
    public Node currentNode;
    
    // To keep track of visited nodes by their index in the hierarchy
    private List<int> visitedNodeIndices = new List<int>();
    private int currentNodeIndex = 0;

    void Start() {
        nodes.Clear();
        nodes.AddRange(GetComponentsInChildren<Node>()); // Get all nodes in the children of this object

        // Lock all nodes first
        foreach (var node in nodes) {
            node.Lock();
        }

        // Draw connection lines (with lower sorting order than nodes)
        foreach (var node in nodes) {
            foreach (var nextNode in node.nextNodes) {
                if (nextNode != null) {
                    DrawLine(node.transform.position, nextNode.transform.position);
                }
            }
        }
        
        // Load saved map state if available
        LoadMapState();
    }

    public void SetCurrentNode(Node node) {
        // Lock all nodes first
        foreach (var n in nodes) {
            n.Lock();
        }

        // Update current node
        currentNode = node;
        currentNodeIndex = nodes.IndexOf(node);
        
        // Unlock current node first (to ensure it's accessible)
        currentNode.Unlock();
        
        // Unlock only the next available nodes that haven't been visited yet
        foreach (var nextNode in node.nextNodes) {
            if (nextNode != null) {
                nextNode.Unlock();
            }
        }
        
        // Save the map state when a new node is selected
        SaveMapState();
    }

    // Save the current map state
    public void SaveMapState() {
        if (currentNode != null) {
            currentNodeIndex = nodes.IndexOf(currentNode);
        } else {
            currentNodeIndex = 0;
        }
        
        // Clear and update visited nodes indices
        visitedNodeIndices.Clear();
        for (int i = 0; i < nodes.Count; i++) {
            if (nodes[i].IsVisited()) {
                visitedNodeIndices.Add(i);
            }
        }
        
        // Save to PlayerPrefs
        PlayerPrefs.SetInt("CurrentNodeIndex", currentNodeIndex);
        
        string visitedIndicesStr = string.Join(",", visitedNodeIndices);
        PlayerPrefs.SetString("VisitedNodeIndices", visitedIndicesStr);
        
        PlayerPrefs.Save();
        Debug.Log($"Map state saved. Current node: {currentNodeIndex}, Visited nodes: {visitedIndicesStr}");
    }

    // Load saved map state
    private void LoadMapState() {
        if (PlayerPrefs.HasKey("CurrentNodeIndex") && PlayerPrefs.HasKey("VisitedNodeIndices")) {
            currentNodeIndex = PlayerPrefs.GetInt("CurrentNodeIndex");
            
            string visitedIndicesStr = PlayerPrefs.GetString("VisitedNodeIndices");
            if (!string.IsNullOrEmpty(visitedIndicesStr)) {
                visitedNodeIndices = visitedIndicesStr.Split(',')
                    .Where(s => !string.IsNullOrEmpty(s))
                    .Select(int.Parse)
                    .ToList();
            }
            
            // Apply the loaded state to nodes
            if (currentNodeIndex >= 0 && currentNodeIndex < nodes.Count) {
                currentNode = nodes[currentNodeIndex];
            } else if (nodes.Count > 0) {
                currentNode = nodes[0];
                currentNodeIndex = 0;
            }
            
            // Mark saved nodes as visited
            foreach (int index in visitedNodeIndices) {
                if (index >= 0 && index < nodes.Count) {
                    Node node = nodes[index];
                    // Force visit the node
                    node.ForceVisit();
                }
            }
            
            // Set current node and unlock next available nodes
            if (currentNode != null) {
                SetCurrentNode(currentNode);
            }
            
            Debug.Log($"Map state loaded. Current node: {currentNodeIndex}, Visited nodes: {visitedIndicesStr}");
        } else {
            // No saved state, use default (first node)
            if (nodes.Count > 0) {
                currentNode = nodes[0];
                // Only unlock the first node at the start
                SetCurrentNode(currentNode);
            }
            Debug.Log("No saved map state found. Starting from the beginning.");
        }
    }

    void DrawLine(Vector3 start, Vector3 end) {
        GameObject lineObj = new GameObject("Connection");
        lineObj.transform.SetParent(transform, false);
        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lr.startWidth = lr.endWidth = 0.05f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = lr.endColor = Color.white;
        lr.sortingOrder = 1; // Set lines to be below nodes but above background
    }
}