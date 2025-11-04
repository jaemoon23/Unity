using System.Collections;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

public class Stage : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject tilePrefab;
    private GameObject[] tileObjs;
    private GameObject player;
    private Transform playerTransform;

    public int mapWidth = 20;
    public int mapHeight = 20;

    [Range(0f, 0.9f)]
    public float erodePercent = 0.5f;
    public int erodeIteration = 2;
    [Range(0f, 0.9f)]
    public float lakePercent = 0.1f;

    [Range(0f, 0.9f)]
    public float treePercent = 0.1f;
    [Range(0f, 0.9f)]
    public float hillPercent = 0.1f;
    [Range(0f, 0.9f)]
    public float moutainPercent = 0.1f;
    [Range(0f, 0.9f)]
    public float townPercent = 0.1f;
    [Range(0f, 0.9f)]
    public float monsterPercent = 0.1f;

    public Vector2 tileSize = new Vector2(16, 16);

    //public Texture2D islandTexture;
    public Sprite[] islandSprites;
    public Sprite[] fowSprites;

    private Map map;

    public Map Map
    {
        get { return map; }
    }

    private Vector3 firstTilePos;

    /// <summary>
    /// 스크린 좌표를 타일 아이디로 변환
    /// </summary>
    public int ScreenPosToTileId(Vector3 screenPos)
    {
        screenPos.z = Mathf.Abs(transform.position.z - cam.transform.position.z);
        var worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        return WorldPosToTileId(worldPos);
    }

    /// <summary>
    /// 월드 좌표를 타일 아이디로 변환
    /// </summary>
    public int WorldPosToTileId(Vector3 worldPos)
    {
        var pivot = firstTilePos;
        pivot.x -= tileSize.x * 0.5f;
        pivot.y += tileSize.y * 0.5f;

        var diff = worldPos - pivot;
        int x = Mathf.FloorToInt(diff.x / tileSize.x);
        int y = -Mathf.CeilToInt(diff.y / tileSize.y);

        x = Mathf.Clamp(x, 0, mapWidth - 1);
        y = Mathf.Clamp(y, 0, mapHeight - 1);

        return y * mapWidth + x;
    }

    /// <summary>
    /// 타일 좌표를 월드 좌표로 변환
    /// </summary>
    public Vector3 GetTilePos(int y, int x)
    {
        var pos = firstTilePos;
        pos.x += tileSize.x * x;
        pos.y -= tileSize.y * y;
        return pos;
    }

    /// <summary>
    /// 타일 아이디를 월드 좌표로 변환
    /// </summary>
    public Vector3 GetTilePos(int tileId)
    {
        return GetTilePos(tileId / mapWidth, tileId % mapWidth);
    }

    /// <summary>
    /// 스테이지 리셋
    /// </summary>
    private void ResetStage()
    {
        bool succeed = false;
        while (!succeed)
        {
            map = new Map();
            map.Init(mapHeight, mapWidth);
            succeed = map.CreateIsland(erodePercent, erodeIteration, lakePercent,
                treePercent, hillPercent, moutainPercent, townPercent, monsterPercent);
        }
        CreateGrid();
        CreatePlayer();
        CreateCastle();

    }
    private void CreatePlayer()
    {
        if (player != null)
        {
            Destroy(player);
        }
        player = Instantiate(playerPrefab, GetTilePos(map.startTile.id), Quaternion.identity);
    }

    private void CreateCastle()
    {
        bool success = false;
        foreach (var tile in map.tiles) // 모든 타일 순회
        {
            if (tile.autoTileId == (int)TileTypes.Towns)    // 마을 타일이면
            {
                if (map.AStar(map.startTile, tile)) // 시작 타일에서 마을 타일까지 경로 탐색 성공하면
                {
                    tile.autoTileId = (int)TileTypes.Castle;    // 성으로 변경
                    DecorateTile(tile.id);  // 타일 갱신
                    Debug.Log($"경로 탐색 성공 {tile.id}");
                    success = true;
                    break;
                }
            }
        }
        if (!success)
        {
            Debug.Log("경로 탐색 실패");
            ResetStage();   // 스테이지 리셋
        }
    }

    /// <summary>
    /// 그리드 생성
    /// </summary>
    private void CreateGrid()
    {
        if (tileObjs != null)
        {
            foreach (var tile in tileObjs)
            {
                Destroy(tile.gameObject);
            }
        }
        tileObjs = new GameObject[mapHeight * mapWidth];

        firstTilePos = Vector3.zero;
        firstTilePos.x -= mapWidth * tileSize.x * 0.5f;
        firstTilePos.y += mapHeight * tileSize.y * 0.5f;
        var pos = firstTilePos;
        for (int i = 0; i < mapHeight; ++i)
        {
            for (int j = 0; j < mapWidth; ++j)
            {
                var tileId = i * mapWidth + j;
                var tile = map.tiles[tileId];

                var newGo = Instantiate(tilePrefab, transform);
                newGo.transform.localPosition = pos;
                pos.x += tileSize.x;
                newGo.name = $"Tile ({i} , {j})";
                tileObjs[tileId] = newGo;
                DecorateTile(tileId);
            }
            pos.x = firstTilePos.x;
            pos.y -= tileSize.y;
        }
    }

    /// <summary>
    /// 타일 장식
    /// </summary>
    public void DecorateTile(int tileId)
    {
        var tile = map.tiles[tileId];   // 타일 정보 가져오기
        var tileGo = tileObjs[tileId];  // 타일 게임오브젝트 가져오기
        var ren = tileGo.GetComponent<SpriteRenderer>();
        if (tile.autoTileId != (int)TileTypes.Empty)
        {
            ren.sprite = islandSprites[tile.autoTileId];
        }
        else
        {
            ren.sprite = null;
        }

        // if (tile.isVisited)
        // {
        //     if (tile.autoTileId != (int)TileTypes.Empty)
        //     {
        //         ren.sprite = islandSprites[tile.autoTileId];
        //     }
        //     else
        //     {
        //         ren.sprite = null;
        //     }
        // }
        // else
        // {
        //     ren.sprite = fowSprites[tile.autoFowId];
        // }
    }

    public int visiteRadius = 1;

    /// <summary>
    /// 타일 방문 처리
    /// </summary>
    public void OnTileVisited(Tile tile)
    {
        int centerX = tile.id % mapWidth;
        int centerY = tile.id / mapWidth;

        int radius = visiteRadius;
        for (int i = -radius; i <= radius; ++i)
        {
            for (int j = -radius; j <= radius; ++j)
            {
                int x = centerX + j;
                int y = centerY + i;
                if (x < 0 || x >= mapWidth || y < 0 || y >= mapHeight)
                    continue;

                int id = y * mapWidth + x;
                map.tiles[id].isVisited = true;
                DecorateTile(id);
            }
        }
        radius += 1;
        for (int i = -radius; i <= radius; ++i)
        {
            for (int j = -radius; j <= radius; ++j)
            {

                if (i == radius || i == -radius || j == radius || j == -radius)
                {
                    int x = centerX + j;
                    int y = centerY + i;
                    if (x < 0 || x >= mapWidth || y < 0 || y >= mapHeight)
                        continue;

                    int id = y * mapWidth + x;
                    map.tiles[id].UpdateAuotoFowId();
                    DecorateTile(id);
                }
            }
        }
    }

    private Camera cam;
    private bool isMoving = false;
    private CancellationTokenSource cts = new CancellationTokenSource();
    private void Awake()
    {
        cam = Camera.main;
    }
    private void OnDestroy()
    {
        cts.Cancel();
        cts.Dispose();
    }
    // Update is called once per frame
    private async UniTaskVoid Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log(ScreenPosToTileId(Input.mousePosition));

            // 플레이어 마우스 클릭 위치로 이동
            if (player != null && !isMoving)
            {
                CanceleMove();
                isMoving = true;
                var playerTileId = WorldPosToTileId(player.transform.position);
                var tileId = ScreenPosToTileId(Input.mousePosition);
                if (map.AStar(map.tiles[playerTileId], map.tiles[tileId]))
                {
                    Debug.Log("경로 탐색 성공");
                    // player.transform.position = GetTilePos(tileId);
                    await Move();
                }
                else
                {
                    Debug.Log("경로 탐색 실패");
                    isMoving = false;
                }
            }
            else
            {
                if (player == null)
                {
                    Debug.Log("플레이어 널");
                }
                else if (isMoving)
                {
                    Debug.Log("이동 중");
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetStage();
        }
    }

    private void CanceleMove()
    {
        cts.Cancel();
        cts.Dispose();
        cts = new CancellationTokenSource();
        isMoving = false;
    }

    private async UniTask Move()
    {
        try
        {
            // 경로의 첫 번째는 현재 위치이므로 그 다음 타일에서 시작
            for (int i = 1; i < map.path.Count; i++)
            {
                Vector3 targetPos = GetTilePos(map.path[i].id);

                while (Vector3.Distance(player.transform.position, targetPos) > 0.01f)  // 무한루프 방지
                {
                    // 플레이어 위치 이동
                    player.transform.position = Vector3.MoveTowards(player.transform.position, targetPos, 50f * Time.deltaTime);
                    isMoving = true;
                    await UniTask.Yield(cts.Token);
                }
                
                // 이동이 끝나면 타일 위치로 설정
                player.transform.position = targetPos;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"이동 캔슬: {ex.Message}");
        }
        finally
        {
            isMoving = false;
        }
        
    }
}
