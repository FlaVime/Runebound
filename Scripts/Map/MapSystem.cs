using System.Collections.Generic;
using UnityEngine;

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
        ClearMap();
        
        if (!LoadMapState())
        {
            GenerateMap();
            SaveMapState();
        }
        
        // Draw connections and update node visuals
        DrawConnections();
        UpdateNodeStates();
    }
    
    public MapSaveData GenerateMap()
    {
        nodeModels.Clear();
        nodeViews.Clear();
        
        float yOffset = 2f;
        
        // Create root node
        CreateNode(0, "Combat", new Vector2(0, -6 + yOffset));
        
        // Create level 1 nodes
        CreateNode(1, "Event", new Vector2(-2, -4 + yOffset));
        CreateNode(2, "Shop", new Vector2(2, -4 + yOffset));
        
        // Create level 2 nodes
        CreateNode(3, "Combat", new Vector2(-3, -2 + yOffset));
        CreateNode(4, "Event", new Vector2(-1, -2 + yOffset));
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
        
        MapSaveData generated = new MapSaveData();
        generated.currentNodeId = 0;
        generated.unlockedNodeIds.Add(0);
        nodeModels[0].isUnlocked = true;

        return generated;
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
            view.SetNodeType(data);
        
        view.Initialize(model, this);
        nodeViews.Add(view);
    }
    
    private void ConnectNodes(int fromId, int toId)
    {
        if (fromId < nodeModels.Count && toId < nodeModels.Count)
            nodeModels[fromId].connectedNodeIds.Add(toId);
    }
    
    private void DrawConnections()
    {
        // Clear existing connections
        foreach (var line in connectionLines.Values)
        {
            if (line != null)
                Destroy(line.gameObject);
        }
        connectionLines.Clear();
        
        // Draw connections based on model data
        for (int i = 0; i < nodeModels.Count; i++)
        {
            NodeModel model = nodeModels[i];
            
            foreach (int connectedId in model.connectedNodeIds)
            {
                if (connectedId < nodeModels.Count)
                    DrawLine(model.position, nodeModels[connectedId].position, i, connectedId);
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
        
        selectedNode.isVisited = true;
        
        // Update current node
        currentNodeId = nodeId;
        
        // Update save data
        saveData.currentNodeId = currentNodeId;
        
        if (!saveData.visitedNodeIds.Contains(nodeId))
            saveData.visitedNodeIds.Add(nodeId);
        
        UpdateNodeStates();
        SaveMapState();
        
        GameManager.Instance.ChangeState((GameState)System.Enum.Parse(typeof(GameState), selectedNode.nodeType));
    }
    
    private void UpdateNodeStates()
    {
        foreach (var model in nodeModels)
            model.isUnlocked = false;

        // Filter out any invalid indices from the saved data first
        List<int> validVisitedIds = new List<int>();
        foreach (int id in saveData.visitedNodeIds)
        {
            if (id >= 0 && id < nodeModels.Count)
                validVisitedIds.Add(id);
        }
        saveData.visitedNodeIds = validVisitedIds;

        // Similarly, filter out invalid unlocked indices
        List<int> validUnlockedIds = new List<int>();
        foreach (int id in saveData.unlockedNodeIds)
        {
            if (id >= 0 && id < nodeModels.Count)
                validUnlockedIds.Add(id);
        }
        saveData.unlockedNodeIds = validUnlockedIds;

        // Make sure current node ID is valid
        if (currentNodeId < 0 || currentNodeId >= nodeModels.Count)
        {
            currentNodeId = 0;
            saveData.currentNodeId = 0;
        }

        // First, apply visited states from saved data
        foreach (int visitedId in saveData.visitedNodeIds)
        {
            if (visitedId >= 0 && visitedId < nodeModels.Count)
                nodeModels[visitedId].isVisited = true;
        }

        // Unlock the current node if it's not visited yet
        if (currentNodeId >= 0 && currentNodeId < nodeModels.Count)
        {
            NodeModel currentNode = nodeModels[currentNodeId];
            currentNode.isUnlocked = true;
            
            // If the current node is visited, unlock its connected non-visited nodes
            if (currentNode.isVisited)
            {
                foreach (int connectedId in currentNode.connectedNodeIds)
                {
                    if (connectedId >= 0 && connectedId < nodeModels.Count)
                    {
                        // Unlock connected nodes if they haven't been visited yet
                        if (!nodeModels[connectedId].isVisited)
                            nodeModels[connectedId].isUnlocked = true;
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
                        nodeModels[connectedId].isUnlocked = true;
                }
            }
        }

        // Apply node state to views
        for (int i = 0; i < nodeViews.Count; i++)
            nodeViews[i].UpdateVisuals();
    }
    
    private bool LoadMapState()
    {
        try
        {
            bool needsFreshMap = !PlayerPrefs.HasKey("MapSaveData");
            
            if (needsFreshMap)
            {
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
                return true; // Return true since we've already generated a map
            
            // Set the current node from saved data
            currentNodeId = loadedSaveData.currentNodeId;
            
            // If the loaded map is invalid, keep using the fresh map
            if (currentNodeId == -1 || currentNodeId >= nodeModels.Count)
            {
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
            }
            loadedSaveData.visitedNodeIds = validVisitedIds;
            
            saveData = loadedSaveData;
            
            // Apply visited states to the node models
            foreach (int visitedId in saveData.visitedNodeIds)
            {
                if (visitedId >= 0 && visitedId < nodeModels.Count)
                    nodeModels[visitedId].isVisited = true;
            }
            
            // Apply unlocked states to the node models
            foreach (int unlockedId in saveData.unlockedNodeIds)
            {
                if (unlockedId >= 0 && unlockedId < nodeModels.Count)
                    nodeModels[unlockedId].isUnlocked = true;
            }
            
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error loading map state: {e.Message}");
            PlayerPrefs.DeleteKey("MapSaveData");
            currentNodeId = 0;
            saveData = new MapSaveData();
            saveData.currentNodeId = 0;
            return true;
        }
    }
    
    public void SaveMapState()
    {
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
                        existingVisitedNodes.Add(nodeId);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error parsing existing visited nodes: {e.Message}");
            }
        }
        
        saveData.currentNodeId = currentNodeId;
        saveData.visitedNodeIds.Clear();
        
        // First add nodes that are marked as visited in the model
        for (int i = 0; i < nodeModels.Count; i++)
        {
            if (nodeModels[i].isVisited)
                saveData.visitedNodeIds.Add(i);
        }
        
        // Then add any previously saved nodes not captured in our models
        foreach (int visitedId in existingVisitedNodes)
        {
            if (!saveData.visitedNodeIds.Contains(visitedId))
                saveData.visitedNodeIds.Add(visitedId);
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
                saveData.unlockedNodeIds.Add(i);
        }
        
        // Save to PlayerPrefs
        string json = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString("MapSaveData", json);
        
        // Also save the raw visited node indices for easier access by other scripts
        string visitedIndicesStr = string.Join(",", saveData.visitedNodeIds);
        PlayerPrefs.SetString("VisitedNodeIndices", visitedIndicesStr);
        
        PlayerPrefs.Save();
    }
    
    private void ClearMap()
    {
        foreach (var view in nodeViews)
        {
            if (view != null)
                Destroy(view.gameObject);
        }
        
        nodeModels.Clear();
        nodeViews.Clear();
        
        foreach (var line in connectionLines.Values)
        {
            if (line != null)
                Destroy(line.gameObject);
        }
        connectionLines.Clear();
    }
}
