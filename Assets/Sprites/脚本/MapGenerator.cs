using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MazeGenerator : MonoBehaviour
{
    public Tilemap tilemap; // Tilemap组件
    public RuleTile groundTile; // 地面Tile
    public RuleTile wallTile; // 墙壁Tile

    public const int MapWidth = 58; // 地图宽度
    public const int MapHeight = 58; // 地图高度

    private int[,] maze; // 迷宫数据，0表示墙壁，1表示路径

    public int roomCount = 3; // 房间数量
    public int roomWidth = 6; // 房间内部宽度
    public int roomHeight = 10; // 房间内部高度

    // 初始化迷宫数据
    void InitializeMaze()
    {
        maze = new int[MapWidth, MapHeight];
        // 初始化为全墙壁
        for (int x = 0; x < MapWidth; x++)
        {
            for (int y = 0; y < MapHeight; y++)
            {
                maze[x, y] = 0; // 0表示墙壁
            }
        }
    }

    // 使用深度优先搜索（DFS）生成迷宫
    void GenerateMaze()
    {
        // 起点（确保是2的倍数，因为路径宽度为2）
        int startX = 2; // 留出围墙空间
        int startY = 2;

        // 递归生成迷宫
        DFS(startX, startY);
    }

    // 深度优先搜索算法
    void DFS(int x, int y)
    {
        // 标记当前2x2区域为路径
        MarkPath(x, y);

        // 定义四个方向
        int[] directions = { 0, 1, 2, 3 };
        ShuffleArray(directions); // 随机打乱方向

        // 遍历四个方向
        foreach (int dir in directions)
        {
            int nx = x + DX(dir) * 4; // 下一个点的x坐标（4格，因为路径宽度为2）
            int ny = y + DY(dir) * 4; // 下一个点的y坐标

            // 检查下一个点是否在地图范围内且是墙壁
            if (nx > 1 && nx < MapWidth - 2 && ny > 1 && ny < MapHeight - 2 && maze[nx, ny] == 0)
            {
                // 打通当前点和下一个点之间的墙壁（2x2区域）
                int wallX = x + DX(dir) * 2;
                int wallY = y + DY(dir) * 2;
                MarkPath(wallX, wallY); // 打通墙壁
                DFS(nx, ny); // 递归
            }
        }
    }

    // 标记2x2区域为路径
    void MarkPath(int x, int y)
    {
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                maze[x + i, y + j] = 1; // 1表示路径
            }
        }
    }

    // 添加两格厚的围墙
    void AddWalls()
    {
        for (int x = 0; x < MapWidth; x++)
        {
            for (int y = 0; y < MapHeight; y++)
            {
                if (x < 2 || x >= MapWidth - 2 || y < 2 || y >= MapHeight - 2)
                {
                    maze[x, y] = 0; // 0表示墙壁
                }
            }
        }
    }

    // 生成房间
    void GenerateRooms()
    {
        for (int i = 0; i < roomCount; i++)
        {
            // 房间的左上角坐标（考虑2格厚的墙壁）
            int roomX = Random.Range(2, MapWidth - (roomWidth + 4)); // +4 是因为墙壁占2格
            int roomY = Random.Range(2, MapHeight - (roomHeight + 4));

            // 生成房间的墙壁（2格厚）
            for (int x = roomX; x < roomX + roomWidth + 4; x++)
            {
                for (int y = roomY; y < roomY + roomHeight + 4; y++)
                {
                    if (x < roomX + 2 || x >= roomX + roomWidth + 2 || y < roomY + 2 || y >= roomY + roomHeight + 2)
                    {
                        maze[x, y] = 0; // 墙壁
                    }
                    else
                    {
                        maze[x, y] = 1; // 房间内部路径
                    }
                }
            }

            // 连接房间与迷宫
            ConnectRoomToMaze(roomX + 2, roomY + 2, roomWidth, roomHeight);
        }
    }

    // 连接房间与迷宫
    void ConnectRoomToMaze(int roomX, int roomY, int roomWidth, int roomHeight)
    {
        // 随机选择房间的一个边缘点作为连接点
        int connectX = roomX + Random.Range(0, roomWidth);
        int connectY = roomY + Random.Range(0, roomHeight);

        // 找到最近的迷宫路径点
        int nearestX = FindNearestPath(connectX, connectY, true);
        int nearestY = FindNearestPath(connectX, connectY, false);

        // 打通连接点与迷宫路径之间的墙壁
        if (nearestX != -1 && nearestY != -1)
        {
            int currentX = connectX;
            int currentY = connectY;

            while (currentX != nearestX || currentY != nearestY)
            {
                if (currentX < nearestX) currentX++;
                else if (currentX > nearestX) currentX--;

                if (currentY < nearestY) currentY++;
                else if (currentY > nearestY) currentY--;

                maze[currentX, currentY] = 1; // 1表示路径
            }
        }
    }

    // 找到最近的迷宫路径点
    int FindNearestPath(int x, int y, bool isX)
    {
        int step = 1;
        while (true)
        {
            if (isX)
            {
                if (x + step < MapWidth && maze[x + step, y] == 1) return x + step;
                if (x - step >= 0 && maze[x - step, y] == 1) return x - step;
            }
            else
            {
                if (y + step < MapHeight && maze[x, y + step] == 1) return y + step;
                if (y - step >= 0 && maze[x, y - step] == 1) return y - step;
            }

            step++;

            if (step > MapWidth && step > MapHeight) return -1; // 未找到路径
        }
    }

    // 渲染迷宫到Tilemap
    void RenderMaze()
    {
        tilemap.ClearAllTiles(); // 清空Tilemap
        for (int x = 0; x < MapWidth; x++)
        {
            for (int y = 0; y < MapHeight; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);
                if (maze[x, y] == 1)
                {
                    tilemap.SetTile(position, groundTile); // 路径
                }
                else
                {
                    tilemap.SetTile(position, wallTile); // 墙壁
                }
            }
        }
    }

    // 工具函数：随机打乱数组
    void ShuffleArray(int[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }

    // 工具函数：获取方向对应的x偏移量
    int DX(int dir)
    {
        switch (dir)
        {
            case 0: return 1; // 右
            case 1: return -1; // 左
            case 2: return 0; // 上
            case 3: return 0; // 下
            default: return 0;
        }
    }

    // 工具函数：获取方向对应的y偏移量
    int DY(int dir)
    {
        switch (dir)
        {
            case 0: return 0; // 右
            case 1: return 0; // 左
            case 2: return 1; // 上
            case 3: return -1; // 下
            default: return 0;
        }
    }

    // 生成地图
    public void Generate()
    {
        InitializeMaze();
        GenerateMaze();
        GenerateRooms();
        AddWalls();
        RenderMaze();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MazeGenerator))]
public class MazeGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MazeGenerator generator = (MazeGenerator)target;
        if (GUILayout.Button("生成地图"))
        {
            generator.Generate();
        }
    }
}
#endif