using UnityEngine;
using System.Collections.Generic;
using System;

public class Node : MonoBehaviour {
    public NodeData data; // Node data
    public List<Node> nextNodes = new List<Node>(); // List of next nodes
    private bool isVisited = false; // Flag to check if the node has been visited
    private bool isUnlocked = false;

    private MapGenerator mapGenerator;

    private void Start() {
        // Initialize the node with data
        mapGenerator = GetComponentInParent<MapGenerator>();
        if (data != null && data.nodeIcon != null) {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null) {
                sr.sprite = data.nodeIcon; // Set the icon of the node
                sr.sortingOrder = 2; // Ensure node icons appear above connections
            }
        } 
    }

    private void OnMouseDown() {
        if (!isUnlocked) {
            Debug.Log("This node is locked!");
            return;
        }

        // Set current node even if already visited
        mapGenerator.SetCurrentNode(this);
        
        // Mark as visited if not already
        if (!isVisited) {
            isVisited = true; // Mark the node as visited
            Debug.Log($"Node {data.nodeType} clicked!"); // Log the click event
            GetComponent<SpriteRenderer>().color = Color.gray; // Change the color of the node to gray
        } 
        
        if (data != null) {
            GameManager.Instance.ChangeState((GameState)System.Enum.Parse(typeof(GameState), data.nodeType)); // Change the game state based on the node type
        }
    }

    public void Unlock() {
        isUnlocked = true;
        if (!isVisited) {
            Highlight();
        } else {
            // If already visited, ensure it remains gray
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null) {
                sr.color = Color.gray;
            }
        }
    }

    public void Lock() {
        isUnlocked = false;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) {
            if (isVisited) {
                sr.color = Color.gray; // Keep visited nodes gray
            } else {
                sr.color = Color.white; // Reset unvisited nodes to white
            }
        }
    }

    private void Highlight() {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && !isVisited) {
            sr.color = Color.green;
        }
    }

    // Method to check if the node is visited
    public bool IsVisited() {
        return isVisited;
    }
    
    // Method to force mark the node as visited (for loading saved game state)
    public void ForceVisit() {
        isVisited = true;
        // Set the color to indicate visited status
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) {
            sr.color = Color.gray;
        }
        Debug.Log($"Node {data?.nodeType} force-visited during map load.");
    }
    
    // Method to check if the node is unlocked
    public bool IsUnlocked() {
        return isUnlocked;
    }
}