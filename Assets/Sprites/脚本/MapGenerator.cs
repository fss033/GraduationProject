using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MazeGenerator : MonoBehaviour
{
    public Tilemap tilemap; // Tilemap���
    public RuleTile groundTile; // ����Tile
    public RuleTile wallTile; // ǽ��Tile

    public const int MapWidth = 58; // ��ͼ���
    public const int MapHeight = 58; // ��ͼ�߶�

    private int[,] maze; // �Թ����ݣ�0��ʾǽ�ڣ�1��ʾ·��

    public int roomCount = 3; // ��������
    public int roomWidth = 6; // �����ڲ����
    public int roomHeight = 10; // �����ڲ��߶�

    // ��ʼ���Թ�����
    void InitializeMaze()
    {
        maze = new int[MapWidth, MapHeight];
        // ��ʼ��Ϊȫǽ��
        for (int x = 0; x < MapWidth; x++)
        {
            for (int y = 0; y < MapHeight; y++)
            {
                maze[x, y] = 0; // 0��ʾǽ��
            }
        }
    }

    // ʹ���������������DFS�������Թ�
    void GenerateMaze()
    {
        // ��㣨ȷ����2�ı�������Ϊ·�����Ϊ2��
        int startX = 2; // ����Χǽ�ռ�
        int startY = 2;

        // �ݹ������Թ�
        DFS(startX, startY);
    }

    // ������������㷨
    void DFS(int x, int y)
    {
        // ��ǵ�ǰ2x2����Ϊ·��
        MarkPath(x, y);

        // �����ĸ�����
        int[] directions = { 0, 1, 2, 3 };
        ShuffleArray(directions); // ������ҷ���

        // �����ĸ�����
        foreach (int dir in directions)
        {
            int nx = x + DX(dir) * 4; // ��һ�����x���꣨4����Ϊ·�����Ϊ2��
            int ny = y + DY(dir) * 4; // ��һ�����y����

            // �����һ�����Ƿ��ڵ�ͼ��Χ������ǽ��
            if (nx > 1 && nx < MapWidth - 2 && ny > 1 && ny < MapHeight - 2 && maze[nx, ny] == 0)
            {
                // ��ͨ��ǰ�����һ����֮���ǽ�ڣ�2x2����
                int wallX = x + DX(dir) * 2;
                int wallY = y + DY(dir) * 2;
                MarkPath(wallX, wallY); // ��ͨǽ��
                DFS(nx, ny); // �ݹ�
            }
        }
    }

    // ���2x2����Ϊ·��
    void MarkPath(int x, int y)
    {
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                maze[x + i, y + j] = 1; // 1��ʾ·��
            }
        }
    }

    // ���������Χǽ
    void AddWalls()
    {
        for (int x = 0; x < MapWidth; x++)
        {
            for (int y = 0; y < MapHeight; y++)
            {
                if (x < 2 || x >= MapWidth - 2 || y < 2 || y >= MapHeight - 2)
                {
                    maze[x, y] = 0; // 0��ʾǽ��
                }
            }
        }
    }

    // ���ɷ���
    void GenerateRooms()
    {
        for (int i = 0; i < roomCount; i++)
        {
            // ��������Ͻ����꣨����2����ǽ�ڣ�
            int roomX = Random.Range(2, MapWidth - (roomWidth + 4)); // +4 ����Ϊǽ��ռ2��
            int roomY = Random.Range(2, MapHeight - (roomHeight + 4));

            // ���ɷ����ǽ�ڣ�2���
            for (int x = roomX; x < roomX + roomWidth + 4; x++)
            {
                for (int y = roomY; y < roomY + roomHeight + 4; y++)
                {
                    if (x < roomX + 2 || x >= roomX + roomWidth + 2 || y < roomY + 2 || y >= roomY + roomHeight + 2)
                    {
                        maze[x, y] = 0; // ǽ��
                    }
                    else
                    {
                        maze[x, y] = 1; // �����ڲ�·��
                    }
                }
            }

            // ���ӷ������Թ�
            ConnectRoomToMaze(roomX + 2, roomY + 2, roomWidth, roomHeight);
        }
    }

    // ���ӷ������Թ�
    void ConnectRoomToMaze(int roomX, int roomY, int roomWidth, int roomHeight)
    {
        // ���ѡ�񷿼��һ����Ե����Ϊ���ӵ�
        int connectX = roomX + Random.Range(0, roomWidth);
        int connectY = roomY + Random.Range(0, roomHeight);

        // �ҵ�������Թ�·����
        int nearestX = FindNearestPath(connectX, connectY, true);
        int nearestY = FindNearestPath(connectX, connectY, false);

        // ��ͨ���ӵ����Թ�·��֮���ǽ��
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

                maze[currentX, currentY] = 1; // 1��ʾ·��
            }
        }
    }

    // �ҵ�������Թ�·����
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

            if (step > MapWidth && step > MapHeight) return -1; // δ�ҵ�·��
        }
    }

    // ��Ⱦ�Թ���Tilemap
    void RenderMaze()
    {
        tilemap.ClearAllTiles(); // ���Tilemap
        for (int x = 0; x < MapWidth; x++)
        {
            for (int y = 0; y < MapHeight; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);
                if (maze[x, y] == 1)
                {
                    tilemap.SetTile(position, groundTile); // ·��
                }
                else
                {
                    tilemap.SetTile(position, wallTile); // ǽ��
                }
            }
        }
    }

    // ���ߺ����������������
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

    // ���ߺ�������ȡ�����Ӧ��xƫ����
    int DX(int dir)
    {
        switch (dir)
        {
            case 0: return 1; // ��
            case 1: return -1; // ��
            case 2: return 0; // ��
            case 3: return 0; // ��
            default: return 0;
        }
    }

    // ���ߺ�������ȡ�����Ӧ��yƫ����
    int DY(int dir)
    {
        switch (dir)
        {
            case 0: return 0; // ��
            case 1: return 0; // ��
            case 2: return 1; // ��
            case 3: return -1; // ��
            default: return 0;
        }
    }

    // ���ɵ�ͼ
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
        if (GUILayout.Button("���ɵ�ͼ"))
        {
            generator.Generate();
        }
    }
}
#endif