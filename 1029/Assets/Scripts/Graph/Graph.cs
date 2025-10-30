using UnityEngine;

public class Graph
{
    public int rows = 0;
    public int cols = 0;
    public GraphNode[] nodes;

    public void Init(int[,] grid)
    {
        rows = grid.GetLength(0);
        cols = grid.GetLength(1);

        nodes = new GraphNode[grid.Length];
        for (int i = 0; i < nodes.Length; i++)
        {
            nodes[i] = new GraphNode();
            nodes[i].id = i;
        }

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                int index = r * cols + c; // 행 인덱스 * 열 + 열 인덱스 = 1차원 배열 인덱스
                nodes[index].weight = grid[r, c];

                if (grid[r, c] == -1)
                    continue;

                // 위
                if (r - 1 >= 0 && grid[r - 1, c] >= 0)  // 위 노드가 유효한지 체크
                {
                    nodes[index].adjacents.Add(nodes[index - cols]);    // 위 노드 추가
                }

                // 오른쪽
                if (c + 1 < cols && grid[r, c + 1] >= 0)    // 오른쪽 노드가 유효한지 체크
                {
                    nodes[index].adjacents.Add(nodes[index + 1]);   // 오른쪽 노드 추가
                }

                // 아래
                if (r + 1 < rows && grid[r + 1, c] >= 0)    // 아래 노드가 유효한지 체크
                {
                    nodes[index].adjacents.Add(nodes[index + cols]);    // 아래 노드 추가
                }

                // 왼쪽
                if (c - 1 >= 0 && grid[r, c - 1] >= 0)  // 왼쪽 노드가 유효한지 체크
                {
                    nodes[index].adjacents.Add(nodes[index - 1]);   // 왼쪽 노드 추가
                }
            }
        }
    }

    public void ResetNodePrevious()
    {
        foreach (var node in nodes)
        {
            node.previous = null;
        }
    }
}
