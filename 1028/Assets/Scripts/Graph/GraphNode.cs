using System.Collections.Generic;

public class GraphNode
{
    public int id;
    public int weight = 1;
    public List<GraphNode> adjacents = new List<GraphNode>();   // 인접 노드 리스트
    public GraphNode previous = null;   // 경로 추적을 위한 이전 노드
    public bool CanVisit => adjacents.Count > 0;    // 방문 가능한 노드인지 여부
}
