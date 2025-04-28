using UnityEngine;

public class MapGenerator : MonoBehaviour {
    public GameObject nodePrefab;
    public int nodeCount = 8;
    public float radius = 4f;

    private void Start() {
        for (int i = 0; i < nodeCount; i++) {
            float angle = i * Mathf.PI * 2f / nodeCount;
            Vector3 pos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;

            var obj = Instantiate(nodePrefab, pos, Quaternion.identity, transform);
            var node = obj.GetComponent<Node>();
            node.Type = (NodeType)Random.Range(0, System.Enum.GetValues(typeof(NodeType)).Length);
        }
    }
}
