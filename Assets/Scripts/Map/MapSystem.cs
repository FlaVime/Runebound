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

        float yOffset = 0.5f;

        // === Level 0 ===
        CreateNode(0, "Combat", new Vector2(0, -5));

        // === Level 1 ===
        CreateNode(1, "Event", new Vector2(-3, -4 + yOffset));
        CreateNode(2, "Combat", new Vector2(3, -4 + yOffset));

        // === Level 2 ===
        CreateNode(3, "Combat", new Vector2(-4, -2.5f + yOffset));
        CreateNode(4, "Shop", new Vector2(-2, -2.5f + yOffset));
        CreateNode(5, "Event", new Vector2(2, -2.5f + yOffset));
        CreateNode(6, "Combat", new Vector2(4, -2.5f + yOffset));

        // === Level 3 ===
        CreateNode(7, "Event", new Vector2(-3, -1 + yOffset));
        CreateNode(8, "Combat", new Vector2(0, -1 + yOffset));
        CreateNode(9, "Shop", new Vector2(3, -1 + yOffset));

        // === Level 4 ===
        CreateNode(10, "Combat", new Vector2(-2, 0.5f + yOffset));
        CreateNode(11, "Event", new Vector2(2, 0.5f + yOffset));

        // === Level 5 ===
        CreateNode(12, "Combat", new Vector2(-3, 2f + yOffset));
        CreateNode(13, "Event", new Vector2(-1.5f, 2f + yOffset));
        CreateNode(14, "Shop", new Vector2(0, 2f + yOffset));
        CreateNode(15, "Combat", new Vector2(1.5f, 2f + yOffset));
        CreateNode(16, "Event", new Vector2(3, 2f + yOffset));

        // === Final Level ===
        CreateNode(17, "Boss", new Vector2(0, 3.5f + yOffset));

        // === Connections ===
        ConnectNodes(0, 1);
        ConnectNodes(0, 2);

        ConnectNodes(1, 3);
        ConnectNodes(1, 4);
        ConnectNodes(2, 5);
        ConnectNodes(2, 6);

        ConnectNodes(3, 7);
        ConnectNodes(4, 8);
        ConnectNodes(5, 9);
        ConnectNodes(6, 9);

        ConnectNodes(7, 10);
        ConnectNodes(8, 10);
        ConnectNodes(8, 11);
        ConnectNodes(9, 11);

        // Level 4 → Level 5
        ConnectNodes(10, 12);
        ConnectNodes(10, 13);
        ConnectNodes(11, 14);
        ConnectNodes(11, 15);
        ConnectNodes(10, 14);
        ConnectNodes(11, 16);

        // Level 5 → Boss
        ConnectNodes(12, 17);
        ConnectNodes(13, 17);
        ConnectNodes(14, 17);
        ConnectNodes(15, 17);
        ConnectNodes(16, 17);

        MapSaveData generated = new MapSaveData();
        generated.currentNodeId = 0;
        generated.unlockedNodeIds.Add(0);
        nodeModels[0].isUnlocked = true;

        return generated;
    }

    private void CreateNode(int id, string type, Vector2 pos)
    {
        Sprite icon = null;
        foreach (var data in nodeDataTypes)
        {
            if (data.nodeType == type)
            {
                icon = data.nodeIcon;
                break;
            }
        }

        var model = new NodeModel(id, type, pos, icon);
        nodeModels.Add(model);

        var viewObj = Instantiate(nodeViewPrefab, pos, Quaternion.identity, mapContainer);
        var view = viewObj.GetComponent<NodeView>();
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
