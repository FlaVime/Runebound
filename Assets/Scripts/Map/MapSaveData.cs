using System;
using System.Collections.Generic;

[Serializable]
public class MapSaveData
{
    public int currentNodeId;
    public List<int> visitedNodeIds = new List<int>();
    public List<int> unlockedNodeIds = new List<int>();
    
    public MapSaveData()
    {
        currentNodeId = -1;
    }
}
