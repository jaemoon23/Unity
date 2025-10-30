using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


public class GraphSearch
{
    private Graph graph;
    public List<GraphNode> path = new List<GraphNode>();

    public void Init(Graph graph)
    {
        this.graph = graph;
    }
    

    public void DFS(GraphNode node)
    {
        path.Clear();

        var visited = new HashSet<GraphNode>();
        var stack = new Stack<GraphNode>();

        stack.Push(node);
        while (stack.Count > 0)
        {
            var currentNode = stack.Pop();
            path.Add(currentNode);
            visited.Add(currentNode);
            foreach (var adjacent in currentNode.adjacents)
            {
                if (!adjacent.CanVisit || visited.Contains(adjacent) || stack.Contains(adjacent))
                    continue;

                stack.Push(adjacent);
            }
        }
    }

    public void BFS(GraphNode node)
    {
        path.Clear();

        var visited = new HashSet<GraphNode>();
        var queue = new Queue<GraphNode>();

        queue.Enqueue(node);
        while (queue.Count > 0)
        {
            var currentNode = queue.Dequeue();
            path.Add(currentNode);
            visited.Add(currentNode);
            foreach (var adjacent in currentNode.adjacents)
            {
                if (!adjacent.CanVisit || visited.Contains(adjacent) || queue.Contains(adjacent))
                    continue;

                queue.Enqueue(adjacent);
            }
        }
    }

    public void DFSRecursive(GraphNode node)
    {
        path.Clear();
        DFSRecursive(node, new HashSet<GraphNode>());
    }

    protected void DFSRecursive(GraphNode node, HashSet<GraphNode> visited)
    {
        path.Add(node);
        visited.Add(node);
        foreach (var adjacent in node.adjacents)
        {
            if (!adjacent.CanVisit || visited.Contains(adjacent))
                continue;
            DFSRecursive(adjacent, visited);
        }
    }

    public bool PathFindingBFS(GraphNode startNode, GraphNode endNode)
    {
        path.Clear();
        graph.ResetNodePrevious();

        var visited = new HashSet<GraphNode>();
        var queue = new Queue<GraphNode>();

        queue.Enqueue(startNode);
        bool success = false;

        while (queue.Count > 0)
        {
            var currentNode = queue.Dequeue();
            if (currentNode == endNode)
            {
                success = true;
                break;
            }

            visited.Add(currentNode);
            foreach (var adjacent in currentNode.adjacents)
            {
                if (!adjacent.CanVisit || visited.Contains(adjacent) || queue.Contains(adjacent))
                    continue;

                adjacent.previous = currentNode;
                queue.Enqueue(adjacent);
            }
        }

        if (!success)
        {
            return false;
        }

        GraphNode step = endNode;
        while (step != null)
        {
            path.Add(step);
            step = step.previous;
        }

        path.Reverse();
        return true;
    }

    public bool Dikjstra(GraphNode start, GraphNode goal)
    {
        path.Clear();
        graph.ResetNodePrevious();

        var visited = new HashSet<GraphNode>();
        var pQueue = new PriorityQueue<GraphNode, int>();
        var distances = new int[graph.nodes.Length];
        for (int i = 0; i < distances.Length; ++i)
        {
            distances[i] = int.MaxValue;
        }

        distances[start.id] = start.weight;
        pQueue.Enqueue(start, distances[start.id]);

        bool success = false;
        while (pQueue.Count > 0)
        {
            var currentNode = pQueue.Dequeue();
            if (visited.Contains(currentNode))
                continue;

            if (currentNode == goal)
            {
                success = true;
                break;
            }

            visited.Add(currentNode);

            foreach (var adjacent in currentNode.adjacents)
            {
                if (!adjacent.CanVisit || visited.Contains(adjacent))
                    continue;

                var newDistance = distances[currentNode.id] + adjacent.weight;
                if (distances[adjacent.id] > newDistance)
                {
                    distances[adjacent.id] = newDistance;
                    adjacent.previous = currentNode;
                    pQueue.Enqueue(adjacent, newDistance);
                }
            }
        }

        if (!success)
        {
            return false;
        }

        GraphNode step = goal;
        while (step != null)
        {
            path.Add(step);
            step = step.previous;
        }
        path.Reverse();
        return true;
    }

    protected int Heuristic(GraphNode a, GraphNode b)
    {
        int ax = a.id % graph.cols;
        int ay = a.id / graph.cols;

        int bx = b.id % graph.cols;
        int by = b.id / graph.cols;

        return Mathf.Abs(ax - bx) + Mathf.Abs(ay - by);
    }

    public bool AStar(GraphNode start, GraphNode goal)
    {
        path.Clear();
        graph.ResetNodePrevious();

        var visited = new HashSet<GraphNode>(); // 방문한 노드 집합
        var pQueue = new PriorityQueue<GraphNode, int>();   // 우선순위 큐

        var distances = new int[graph.nodes.Length];
        var scores = new int[graph.nodes.Length];  

        for (int i = 0; i < distances.Length; ++i)
        {
            scores[i] = distances[i] = int.MaxValue;
        }
        distances[start.id] = start.weight;
        scores[start.id] = distances[start.id] + Heuristic(start, goal);
        pQueue.Enqueue(start, scores[start.id]);

        bool success = false;
        while (pQueue.Count > 0)
        {
            var currentNode = pQueue.Dequeue();

            if (visited.Contains(currentNode))  // 이미 방문한 노드면 무시
            {
                continue;
            }

            if (currentNode == goal)    // 목표 노드에 도달했으면 종료
            {
                success = true;
                break;
            }

            visited.Add(currentNode);   // 현재 노드를 방문 처리

            foreach (var adjacent in currentNode.adjacents) // 인접 노드들 검사
            {
                if (!adjacent.CanVisit || visited.Contains(adjacent))   // 방문 불가거나 이미 방문한 노드면 무시
                    continue;

                var newDistance = distances[currentNode.id] + adjacent.weight;  // 현재 노드를 거쳐 인접 노드로 가는 거리 계산

                // 갱신된 거리가 기존 거리보다 짧으면 업데이트
                if (distances[adjacent.id] > newDistance)
                {
                    distances[adjacent.id] = newDistance;   // 거리 갱신
                    scores[adjacent.id] = distances[adjacent.id] + Heuristic(adjacent, goal);  // 점수 갱신
                    adjacent.previous = currentNode;          // 이전 노드 설정

                    pQueue.Enqueue(adjacent, scores[adjacent.id]);  // 우선순위 큐에 추가
                }
            }

        }

        if (!success)
        {
            return false;
        }

        GraphNode step = goal;
        while (step != null)
        {
            path.Add(step);
            step = step.previous;
        }
        path.Reverse();
        return true;
    }
}
