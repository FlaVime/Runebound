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
            }
        } 
    }

    private void OnMouseDown() {
        if (!isUnlocked) {
            Debug.Log("This node is locked!");
            return;
        }

        // Check if the node has been visited
        if (!isVisited) {
            isVisited = true; // Mark the node as visited
            Debug.Log($"Node {data.nodeType} clicked!"); // Log the click event
            GetComponent<SpriteRenderer>().color = Color.gray; // Change the color of the node to gray

            mapGenerator.SetCurrentNode(this); // Set the current node in the map generator
        } 
        if (data != null) {
            GameManager.Instance.ChangeState((GameState)System.Enum.Parse(typeof(GameState), data.nodeType)); // Change the game state based on the node type
        }
    }

    public void Unlock() {
        if (!isVisited) {
            isUnlocked = true;
            Highlight();
        }
    }

    public void Lock() {
        isUnlocked = false;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && !isVisited) {
            sr.color = Color.white;
        }
    }

    private void Highlight() {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) {
            sr.color = Color.green;
        }
    }

    // Новый метод для проверки, посещена ли нода
    public bool IsVisited() {
        return isVisited;
    }
}