using UnityEngine;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour {
    private List<Node> nodes = new List<Node>();
    public Node currentNode;

    void Start() {
        nodes.Clear();
        nodes.AddRange(GetComponentsInChildren<Node>()); // Get all nodes in the children of this object

        // Lock all nodes first
        foreach (var node in nodes) {
            node.Lock();
        }

        if (nodes.Count > 0) {
            currentNode = nodes[0]; // Set the first node as the current node
            currentNode.Unlock(); // Unlock the current node
        }

        foreach (var node in nodes) {
            foreach (var nextNode in node.nextNodes) {
                if (nextNode != null) {
                    DrawLine(node.transform.position, nextNode.transform.position);
                }
            }
        }
    }

    public void SetCurrentNode(Node node) {
        // Lock all nodes first
        foreach (var n in nodes) {
            n.Lock();
        }

        // Update current node
        currentNode = node;
        
        // Unlock only the next available nodes
        foreach (var nextNode in node.nextNodes) {
            if (nextNode != null && !nextNode.IsVisited()) {
                nextNode.Unlock();
            }
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
    }
}