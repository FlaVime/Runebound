using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapSystem : MonoBehaviour
{
    [SerializeField] private GameObject nodeViewPrefab;
    [SerializeField] private List<NodeData> nodeDataTypes;
    [SerializeField] private Transform mapContainer;
    
    private List<NodeModel> nodeModels = new List<NodeModel>();
    private List<NodeView> nodeViews = new List<NodeView>();
    private Dictionary<int, LineRenderer> connectionLines = new Dictionary<int, LineRenderer>();
    
    private int currentNodeId = -1;
    private MapSaveData saveData = new MapSaveData();
    
    private void Start()
    {
        Debug.Log("MapSystem Start() called");

        // Reset visited node colors (in case the prefab has wrong color)
        foreach (NodeView view in GetComponentsInChildren<NodeView>())
        {
            SpriteRenderer sr = view.GetComponent<SpriteRenderer>();
            if (sr) sr.color = Color.white;
        }

        // We'll always load map state, which now handles generating the map if needed
        bool mapLoaded = LoadMapState();
        
        // Draw connections and update node visuals
        DrawConnections();
        UpdateNodeStates();
        
        // Debug node states
        for (int i = 0; i < nodeModels.Count; i++)
        {
            Debug.Log($"Node {i}: Visited={nodeModels[i].isVisited}, Unlocked={nodeModels[i].isUnlocked}");
        }
    }
    
    public void GenerateMap()
    {
        // Clear existing nodes and connections
        ClearMap();
        
        // Add an offset to all Y positions
        float yOffset = 2f; // or whatever value centers your map
        
        // TODO: Your map generation algorithm here
        // For this example, we'll create a simple tree-like structure
        
        // Create root node
        CreateNode(0, "Combat", new Vector2(0, -6 + yOffset));
        
        // Create level 1 nodes
        CreateNode(1, "Event", new Vector2(-2, -4 + yOffset));
        CreateNode(2, "Shop", new Vector2(2, -4 + yOffset));
        
        // Create level 2 nodes
        CreateNode(3, "Combat", new Vector2(-3, -2 + yOffset));
        CreateNode(4, "Rest", new Vector2(-1, -2 + yOffset));
        CreateNode(5, "Combat", new Vector2(1, -2 + yOffset));
        CreateNode(6, "Event", new Vector2(3, -2 + yOffset));
        
        // Create level 3 nodes
        CreateNode(7, "Boss", new Vector2(0, 0 + yOffset));
        
        // Connect nodes
        ConnectNodes(0, 1);
        ConnectNodes(0, 2);
        ConnectNodes(1, 3);
        ConnectNodes(1, 4);
        ConnectNodes(2, 5);
        ConnectNodes(2, 6);
        ConnectNodes(3, 7);
        ConnectNodes(4, 7);
        ConnectNodes(5, 7);
        ConnectNodes(6, 7);
        
        // Set initial state
        currentNodeId = 0;
        for (int i = 0; i < nodeModels.Count; i++)
            nodeModels[i].isUnlocked = false;
        nodeModels[0].isUnlocked = true;
        
        // Create initial save data
        saveData = new MapSaveData();
        saveData.currentNodeId = currentNodeId;
        saveData.unlockedNodeIds.Add(currentNodeId);
        
        SaveMapState();
    }
    
    private void CreateNode(int id, string nodeType, Vector2 position)
    {
        NodeModel model = new NodeModel(id, nodeType, position);
        nodeModels.Add(model);
        
        GameObject nodeObject = Instantiate(nodeViewPrefab, position, Quaternion.identity, mapContainer);
        nodeObject.name = $"Node_{id}_{nodeType}";
        
        NodeView view = nodeObject.GetComponent<NodeView>();
        
        // Find the right NodeData
        NodeData data = nodeDataTypes.Find(d => d.nodeType == nodeType);
        if (data != null)
        {
            view.SetNodeType(data);
        }
        
        view.Initialize(model, this);
        nodeViews.Add(view);
    }
    
    private void ConnectNodes(int fromId, int toId)
    {
        // Add connection to the model
        if (fromId < nodeModels.Count && toId < nodeModels.Count)
        {
            nodeModels[fromId].connectedNodeIds.Add(toId);
        }
    }
    
    private void DrawConnections()
    {
        // Clear existing connections
        foreach (var line in connectionLines.Values)
        {
            if (line != null)
            {
                Destroy(line.gameObject);
            }
        }
        connectionLines.Clear();
        
        // Draw connections based on model data
        for (int i = 0; i < nodeModels.Count; i++)
        {
            NodeModel model = nodeModels[i];
            
            foreach (int connectedId in model.connectedNodeIds)
            {
                if (connectedId < nodeModels.Count)
                {
                    DrawLine(model.position, nodeModels[connectedId].position, i, connectedId);
                }
            }
        }
    }
    
    private void DrawLine(Vector2 start, Vector2 end, int fromId, int toId)
    {
        string lineId = $"{fromId}_{toId}";
        GameObject lineObj = new GameObject($"Connection_{lineId}");
        lineObj.transform.SetParent(mapContainer);
        
        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lr.startWidth = lr.endWidth = 0.05f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = lr.endColor = Color.white;
        lr.sortingOrder = 1;
        
        connectionLines.Add(connectionLines.Count, lr);
    }
    
    public void SelectNode(int nodeId)
    {
        if (nodeId >= nodeModels.Count) return;
        
        NodeModel selectedNode = nodeModels[nodeId];
        
        // If node is not unlocked, do nothing
        if (!selectedNode.isUnlocked) 
        {
            Debug.Log($"Cannot select node {nodeId} as it's locked");
            return;
        }
        
        Debug.Log($"Selecting node {nodeId} of type {selectedNode.nodeType}");
        
        // Mark as visited
        selectedNode.isVisited = true;
        
        // Store previous node in case it's useful
        int previousNodeId = currentNodeId;
        
        // Update current node
        currentNodeId = nodeId;
        
        // Update save data
        saveData.currentNodeId = currentNodeId;
        
        if (!saveData.visitedNodeIds.Contains(nodeId))
        {
            saveData.visitedNodeIds.Add(nodeId);
            Debug.Log($"Added node {nodeId} to visited list");
        }
        
        // Update node states - unlock connected nodes
        UpdateNodeStates();
        
        // Save before entering node content
        SaveMapState();
        
        // Trigger scene change
        GameManager.Instance.ChangeState((GameState)System.Enum.Parse(typeof(GameState), selectedNode.nodeType));
    }
    
    private void UpdateNodeStates()
    {
        // Lock all nodes
        foreach (var model in nodeModels)
        {
            model.isUnlocked = false;
        }

        // Filter out any invalid indices from the saved data first
        List<int> validVisitedIds = new List<int>();
        foreach (int id in saveData.visitedNodeIds)
        {
            if (id >= 0 && id < nodeModels.Count)
            {
                validVisitedIds.Add(id);
            }
            else
            {
                Debug.LogWarning($"Ignoring invalid visited node index: {id}");
            }
        }
        saveData.visitedNodeIds = validVisitedIds;

        // Similarly, filter out invalid unlocked indices
        List<int> validUnlockedIds = new List<int>();
        foreach (int id in saveData.unlockedNodeIds)
        {
            if (id >= 0 && id < nodeModels.Count)
            {
                validUnlockedIds.Add(id);
            }
            else
            {
                Debug.LogWarning($"Ignoring invalid unlocked node index: {id}");
            }
        }
        saveData.unlockedNodeIds = validUnlockedIds;

        // Make sure current node ID is valid
        if (currentNodeId < 0 || currentNodeId >= nodeModels.Count)
        {
            Debug.LogWarning($"Current node ID {currentNodeId} is invalid, resetting to 0");
            currentNodeId = 0;
            saveData.currentNodeId = 0;
        }

        // First, apply visited states from saved data
        foreach (int visitedId in saveData.visitedNodeIds)
        {
            // We already filtered invalid indices, but double check anyway
            if (visitedId >= 0 && visitedId < nodeModels.Count)
            {
                nodeModels[visitedId].isVisited = true;
                Debug.Log($"Applied visited state to node {visitedId}");
            }
        }

        // Unlock the current node if it's not visited yet
        if (currentNodeId >= 0 && currentNodeId < nodeModels.Count)
        {
            NodeModel currentNode = nodeModels[currentNodeId];
            currentNode.isUnlocked = true;
            Debug.Log($"Current node {currentNodeId} unlocked");
            
            // If the current node is visited, unlock its connected non-visited nodes
            if (currentNode.isVisited)
            {
                foreach (int connectedId in currentNode.connectedNodeIds)
                {
                    if (connectedId >= 0 && connectedId < nodeModels.Count)
                    {
                        // Unlock connected nodes if they haven't been visited yet
                        if (!nodeModels[connectedId].isVisited)
                        {
                            nodeModels[connectedId].isUnlocked = true;
                            Debug.Log($"Connected node {connectedId} unlocked (from visited node {currentNodeId})");
                        }
                    }
                }
            }
        }

        // Also unlock next nodes from ALL visited nodes (important for branching paths)
        foreach (int visitedId in saveData.visitedNodeIds)
        {
            if (visitedId >= 0 && visitedId < nodeModels.Count)
            {
                NodeModel visitedNode = nodeModels[visitedId];
                
                // Unlock all connected nodes that haven't been visited yet
                foreach (int connectedId in visitedNode.connectedNodeIds)
                {
                    if (connectedId >= 0 && connectedId < nodeModels.Count && !nodeModels[connectedId].isVisited)
                    {
                        nodeModels[connectedId].isUnlocked = true;
                        Debug.Log($"Connected node {connectedId} unlocked (from any visited node {visitedId})");
                    }
                }
            }
        }

        // Apply node state to views
        for (int i = 0; i < nodeViews.Count; i++)
        {
            nodeViews[i].UpdateVisuals();
        }
        
        Debug.Log($"After update: Current node {currentNodeId}, Visited nodes: {string.Join(",", saveData.visitedNodeIds)}");
    }
    
    private bool LoadMapState()
    {
        try
        {
            // Check if we need to generate a fresh map
            bool needsFreshMap = !PlayerPrefs.HasKey("MapSaveData");
            
            if (needsFreshMap)
            {
                Debug.Log("No map save data found, generating a fresh map");
                ClearMap();
                GenerateMap();
                
                // Initialize with node 0 selected
                currentNodeId = 0;
                saveData = new MapSaveData();
                saveData.currentNodeId = 0;
                saveData.unlockedNodeIds.Add(0);
                
                // Make sure node 0 is unlocked
                if (nodeModels.Count > 0)
                {
                    nodeModels[0].isUnlocked = true;
                }
                
                // Save this initial state
                SaveMapState();
                return true;
            }
            
            // If we have save data, load the map structure first
            GenerateMap();
            
            // Load the saved map state
            string json = PlayerPrefs.GetString("MapSaveData");
            MapSaveData loadedSaveData = JsonUtility.FromJson<MapSaveData>(json);
            
            if (loadedSaveData == null)
            {
                Debug.LogError("Failed to deserialize map save data");
                return true; // Return true since we've already generated a map
            }
            
            // Set the current node from saved data
            currentNodeId = loadedSaveData.currentNodeId;
            
            // If the loaded map is invalid, keep using the fresh map
            if (currentNodeId == -1 || currentNodeId >= nodeModels.Count)
            {
                Debug.LogWarning("Loaded map has invalid currentNodeId, using fresh map and resetting to node 0.");
                currentNodeId = 0; // Reset to first node
                loadedSaveData.currentNodeId = 0;
            }
            
            // Filter invalid indices from the loaded save data
            List<int> validVisitedIds = new List<int>();
            foreach (int id in loadedSaveData.visitedNodeIds)
            {
                if (id >= 0 && id < nodeModels.Count)
                {
                    validVisitedIds.Add(id);
                }
                else
                {
                    Debug.LogWarning($"Loaded data contained invalid visited node ID: {id}");
                }
            }
            loadedSaveData.visitedNodeIds = validVisitedIds;
            
            // Update our save data with the loaded data
            saveData = loadedSaveData;
            
            // Apply visited states to the node models
            foreach (int visitedId in saveData.visitedNodeIds)
            {
                if (visitedId >= 0 && visitedId < nodeModels.Count)
                {
                    nodeModels[visitedId].isVisited = true;
                    Debug.Log($"Marked node {visitedId} as visited from saved data");
                }
            }
            
            // Apply unlocked states to the node models
            foreach (int unlockedId in saveData.unlockedNodeIds)
            {
                if (unlockedId >= 0 && unlockedId < nodeModels.Count)
                {
                    nodeModels[unlockedId].isUnlocked = true;
                    Debug.Log($"Marked node {unlockedId} as unlocked from saved data");
                }
            }
            
            Debug.Log($"Map loaded: Current node {currentNodeId}, Visited: {string.Join(",", saveData.visitedNodeIds)}");
            return true;
        }
        catch (System.Exception e)
        {
            // If there's an error loading map state, log it, use the fresh map,
            // and delete the problematic saved data so it doesn't cause errors next time
            Debug.LogError($"Error loading map state: {e.Message}");
            PlayerPrefs.DeleteKey("MapSaveData");
            currentNodeId = 0;
            saveData = new MapSaveData();
            saveData.currentNodeId = 0;
            return true; // Return true since we've already generated a map
        }
    }
    
    public void SaveMapState()
    {
        // Before saving, ensure we get the latest state from PlayerPrefs
        // This helps avoid losing data between scene transitions
        string existingVisitedStr = PlayerPrefs.GetString("VisitedNodeIndices", "");
        List<int> existingVisitedNodes = new List<int>();
        
        // Parse existing visited nodes, if any
        if (!string.IsNullOrEmpty(existingVisitedStr))
        {
            try
            {
                string[] parts = existingVisitedStr.Split(',');
                foreach (string part in parts)
                {
                    if (int.TryParse(part, out int nodeId))
                    {
                        existingVisitedNodes.Add(nodeId);
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error parsing existing visited nodes: {e.Message}");
            }
        }
        
        // Update save data
        saveData.currentNodeId = currentNodeId;
        
        // Update visited nodes list - include both current model state and existing saved state
        saveData.visitedNodeIds.Clear();
        
        // First add nodes that are marked as visited in the model
        for (int i = 0; i < nodeModels.Count; i++)
        {
            if (nodeModels[i].isVisited)
            {
                saveData.visitedNodeIds.Add(i);
            }
        }
        
        // Then add any previously saved nodes not captured in our models
        foreach (int visitedId in existingVisitedNodes)
        {
            if (!saveData.visitedNodeIds.Contains(visitedId))
            {
                saveData.visitedNodeIds.Add(visitedId);
            }
        }
        
        // Make sure current node is in the visited list if it's being visited now
        if (currentNodeId >= 0 && !saveData.visitedNodeIds.Contains(currentNodeId) && 
            currentNodeId < nodeModels.Count && nodeModels[currentNodeId].isVisited)
        {
            saveData.visitedNodeIds.Add(currentNodeId);
        }
        
        // Store unlocked nodes
        saveData.unlockedNodeIds.Clear();
        for (int i = 0; i < nodeModels.Count; i++)
        {
            if (nodeModels[i].isUnlocked)
            {
                saveData.unlockedNodeIds.Add(i);
            }
        }
        
        // Save to PlayerPrefs
        string json = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString("MapSaveData", json);
        
        // Also save the raw visited node indices for easier access by other scripts
        string visitedIndicesStr = string.Join(",", saveData.visitedNodeIds);
        PlayerPrefs.SetString("VisitedNodeIndices", visitedIndicesStr);
        
        PlayerPrefs.Save();
        
        Debug.Log($"Map saved: Current node {currentNodeId}, Visited: {string.Join(",", saveData.visitedNodeIds)}");
    }
    
    private void ClearMap()
    {
        // Destroy all node views
        foreach (var view in nodeViews)
        {
            if (view != null)
            {
                Destroy(view.gameObject);
            }
        }
        
        // Clear all collections
        nodeModels.Clear();
        nodeViews.Clear();
        
        // Clear connection lines
        foreach (var line in connectionLines.Values)
        {
            if (line != null)
            {
                Destroy(line.gameObject);
            }
        }
        connectionLines.Clear();
    }
    
    // Call this from GameManager when starting a new game
    public void ResetMap()
    {
        Debug.Log("ResetMap called - creating new map");
        ClearMap();
        
        // Important: Make sure we delete the save data key to force a new map on reload
        PlayerPrefs.DeleteKey("MapSaveData");
        
        // Reset all data
        currentNodeId = 0; // Start with the first node, not -1
        saveData = new MapSaveData();
        saveData.currentNodeId = 0;
        
        // Generate the new map
        GenerateMap();
        
        // Draw the map visually
        DrawConnections();
        UpdateNodeStates();
        
        Debug.Log("Map has been reset. Starting with node 0");
    }
}
