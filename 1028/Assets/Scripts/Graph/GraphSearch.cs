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

    /// <summary>
    /// 깊이 우선 탐색
    /// </summary>
    /// <param name="node">탐색 시작 노드</param>
    public void DFS(GraphNode node)
    {
        path.Clear();

        var visited = new HashSet<GraphNode>(); // 방문한 노드 집합
        var stack = new Stack<GraphNode>(); // 스택 생성

        stack.Push(node); // 시작 노드를 스택에 추가
        while (stack.Count > 0)
        {
            var currentNode = stack.Pop(); // 스택에서 노드 꺼내기
            path.Add(currentNode);
            visited.Add(currentNode); // 방문한 노드로 표시

            // 인접 노드 탐색
            foreach (var adjacent in currentNode.adjacents)
            {
                // 방문하지 않은 노드만 스택에 추가
                if (!adjacent.CanVisit || visited.Contains(adjacent) || stack.Contains(adjacent))
                    continue;

                stack.Push(adjacent);
            }
        }
    }


    public void DFSRecursive(GraphNode node)
    {
        path.Clear();
        DFSRecursive(node, new HashSet<GraphNode>());
    }

    /// <summary>
    /// 재귀 깊이 우선 탐색 (DFS Recursive)
    /// </summary>
    /// <param name="node"></param>
    public void DFSRecursive(GraphNode node, HashSet<GraphNode> visited)
    {
        path.Add(node);
        visited.Add(node);

        // 인접 노드 탐색
        foreach (var adjacent in node.adjacents)
        {
            // 방문하지 않은 노드, 갈수 있는 노드 확인
            if (!visited.Contains(adjacent) && adjacent.CanVisit)
            {
                DFSRecursive(adjacent, visited);
            }
        }
    }

    /// <summary>
    /// 너비 우선 탐색
    /// </summary>
    /// <param name="node">탐색 시작 노드</param>
    public void BFS(GraphNode node)
    {
        path.Clear();
        graph.ResetPreviousNodes();

        var visited = new HashSet<GraphNode>(); // 방문한 노드 집합
        var queue = new Queue<GraphNode>(); // 큐 생성

        queue.Enqueue(node); // 시작 노드를 큐에 추가
        while (queue.Count > 0)
        {
            var currentNode = queue.Dequeue(); // 큐에서 노드 꺼내기
            path.Add(currentNode);
            visited.Add(currentNode); // 방문한 노드로 표시

            // 인접 노드 탐색
            foreach (var adjacent in currentNode.adjacents)
            {
                // 방문하지 않은 노드만 큐에 추가
                if (!adjacent.CanVisit || visited.Contains(adjacent) || queue.Contains(adjacent))
                    continue;

                queue.Enqueue(adjacent);
            }
        }
    }
    
    public bool PathFindingBFS(GraphNode startNode, GraphNode endNode)
    {
        path.Clear();
        
        var visited = new HashSet<GraphNode>(); // 방문한 노드 집합
        var queue = new Queue<GraphNode>(); // 큐 생성

        queue.Enqueue(startNode); // 시작 노드를 큐에 추가
        bool success = false;
        while (queue.Count > 0)
        {
            var currentNode = queue.Dequeue(); // 큐에서 노드 꺼내기
            if (currentNode == endNode)
            {
                success = true;
                break;
            }
            visited.Add(currentNode); // 방문한 노드로 표시

            // 인접 노드 탐색
            foreach (var adjacent in currentNode.adjacents)
            {
                // 방문하지 않은 노드만 큐에 추가
                if (!adjacent.CanVisit || visited.Contains(adjacent) || queue.Contains(adjacent))
                    continue;

                queue.Enqueue(adjacent);
                adjacent.previous = currentNode;

               
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
}
