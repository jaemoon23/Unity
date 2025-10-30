using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 타일 종류
/// </summary>
public enum TileTypes
{
    Empty = -1, // 빈 타일
    // 0, 14
    Grass = 15, // 풀
    Tree = 16, // 나무
    Hills = 17, // 언덕
    Mountains = 18, // 산
    Towns = 19, // 마을
    Castle = 20, // 성
    Monster = 21 // 몬스터
}

/// <summary>
/// 맵 클래스
/// </summary>
public class Map
{
    public int rows = 0;    // 가로
    public int cols = 0;    // 세로

    public Tile[] tiles;    // 타일 배열

    public Tile castleTile;  // 성 타일
    public Tile startTile;  // 시작 타일

    public List<Tile> path = new List<Tile>(); // 경로  

    public Tile[] CoastTiles
    {

        get
        {
            return tiles.Where(t => t.autoTileId < (int)TileTypes.Grass).ToArray();
        }
    }

    public Tile[] LandTiles
    {
        get
        {
            return tiles.Where(t => t.autoTileId >= (int)TileTypes.Grass).ToArray();
        }
    }

    public void Init(int rows, int cols)   // 0: O 1: X
    {
        this.rows = rows;
        this.cols = cols;

        tiles = new Tile[rows * cols];
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i] = new Tile();
            tiles[i].id = i;
        }

        for (var r = 0; r < rows; ++r)
        {
            for (var c = 0; c < cols; ++c)
            {
                var index = r * cols + c;

                var indexU = (r - 1) * cols + c;
                var indexR = r * cols + c + 1;
                var indexD = (r + 1) * cols + c;
                var indexL = r * cols + c - 1;

                if ((r - 1) >= 0)
                {
                    // 위쪽 인접 타일 설정
                    tiles[index].adjacents[(int)Sides.Top] = tiles[indexU];
                }
                if (c + 1 < cols)
                {
                    // 오른쪽 인접 타일 설정
                    tiles[index].adjacents[(int)Sides.Right] = tiles[indexR];
                }
                if (r + 1 < rows)
                {
                    // 아래쪽 인접 타일 설정
                    tiles[index].adjacents[(int)Sides.Bottom] = tiles[indexD];
                }
                if (c - 1 >= 0)
                {
                    // 왼쪽 인접 타일 설정
                    tiles[index].adjacents[(int)Sides.Left] = tiles[indexL];
                }
            }
        }

        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].UpdateAuotoTileId();   // 타일 아이디 갱신
            tiles[i].UpdateAuotoFowId();    // 안개 아이디 갱신
        }
    }

    public bool CreateIsland(
        float erodePercent, // 침식 비율
        int erodeIterations,// 침식 반복 횟수
        float lakePercent,  // 호수 비율
        float treePercent,  // 나무 비율
        float hillPercent,  // 언덕 비율
        float mountainPercent,  // 산 비율
        float townPercent,  // 마을 비율
        float monsterPercent)   // 몬스터 비율
    {
        DecorateTiles(LandTiles, lakePercent, TileTypes.Empty);

        for (int i = 0; i < erodeIterations; ++i)
            DecorateTiles(CoastTiles, erodePercent, TileTypes.Empty);

        DecorateTiles(LandTiles, treePercent, TileTypes.Tree);
        DecorateTiles(LandTiles, hillPercent, TileTypes.Hills);
        DecorateTiles(LandTiles, mountainPercent, TileTypes.Mountains);
        DecorateTiles(LandTiles, townPercent, TileTypes.Towns);
        DecorateTiles(LandTiles, monsterPercent, TileTypes.Monster);

        var towns = tiles.Where(x => x.autoTileId == (int)TileTypes.Towns).ToArray();
        ShuffleTiles(towns);
        startTile = towns[0];

        var catsleTargets = tiles.Where(x => x.autoTileId <= (int)TileTypes.Grass &&
            x.autoTileId != (int)TileTypes.Empty).ToArray();
        castleTile = catsleTargets[Random.Range(0, catsleTargets.Length)];

        return true;
    }

    public void DecorateTiles(Tile[] tiles, float percent, TileTypes tileType)
    {
        int total = Mathf.FloorToInt(tiles.Length * percent);

        ShuffleTiles(tiles);

        for (int i = 0; i < total; ++i)
        {
            if (tileType == TileTypes.Empty)
                tiles[i].ClearAdjacents();

            tiles[i].autoTileId = (int)tileType;
        }
    }

    /// <summary>
    /// 타일 섞기
    /// </summary>
    public void ShuffleTiles(Tile[] tiles)
    {
        // Fisher-Yates 셔플 알고리즘 구현
        for (int i = tiles.Length - 1; i > 0; i--)
        {
            // 0과 i 사이의 무작위 인덱스 선택
            int randomIndex = Random.Range(0, i + 1);

            // i번째 요소와 무작위로 선택된 요소 교환
            Tile temp = tiles[i];
            tiles[i] = tiles[randomIndex];
            tiles[randomIndex] = temp;
        }
    }
    protected int Heuristic(Tile a, Tile b)
    {
        int ax = a.id % cols;
        int ay = a.id / cols;

        int bx = b.id % cols;
        int by = b.id / cols;

        return Mathf.Abs(ax - bx) + Mathf.Abs(ay - by);
    }

    private void ResetPrevious()
    {
        foreach (var tile in tiles)
        {
            tile.previous = null;
        }
    }
    public bool AStar(Tile start, Tile goal)
    {
        path.Clear();
        ResetPrevious();

        var visited = new HashSet<Tile>(); // 방문한 노드 집합
        var pQueue = new PriorityQueue<Tile, int>();   // 우선순위 큐

        var distances = new int[tiles.Length];
        var scores = new int[tiles.Length];  

        for (int i = 0; i < distances.Length; ++i)
        {
            scores[i] = distances[i] = int.MaxValue;
        }
        distances[start.id] = 0;
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
                if (adjacent == null || !adjacent.CanMove || visited.Contains(adjacent))   // 방문 불가거나 이미 방문한 노드면 무시
                    continue;

                var newDistance = distances[currentNode.id] + adjacent.Weight;  // 현재 노드를 거쳐 인접 노드로 가는 거리 계산

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

        Tile step = goal;
        while (step != null)
        {
            path.Add(step);
            step = step.previous;
        }
        path.Reverse();
        return true;
    }
}
