using UnityEngine.Tilemaps;
using UnityEngine;

public class GroundTileMapBG : MonoBehaviour
{
    [Header("地图配置")]
    public Tilemap ground_tilemap;//拖动获取地面瓦片地图
    public TileBase groundTile;//拖动获取地面规则瓦片Rule Tile
    public Vector2Int lowerLeftCoo = new Vector2Int(10, 10);//地图起始左下角位置
    public int width = 50;//地图宽度
    public int length = 100;//地图长度
    public float groundStartPro = 0.10f;//生成地面起始点的概率
    public Vector2Int groundLenRan = new Vector2Int(3, 10);//起始地面点生成的长度范围

    void Start()
    {
        CreateMap();

    }

    public void CreateMap()
    {
        GroundStartPoi();
    }

    void GroundStartPoi()//生成地面起始点 用协程可以缓慢一步步生成地图，性能消耗少
    {
        ground_tilemap.ClearAllTiles();// 清空地面瓦片地图
        for (int i = lowerLeftCoo.x; i < (this.length + lowerLeftCoo.x); i++)
        {
            for (int j = lowerLeftCoo.y; j < (this.width + lowerLeftCoo.y); j++)
            {
                bool IsGround = j < (this.width + 3) ? (Random.value <= groundStartPro) : (Random.value <= groundStartPro + 0.05);//三元表达式，地面三行更容易生成地面起始点
                if (IsGround) GroundExtension(i, j);
            }
        }
    }

    void GroundExtension(int i, int j)//从地面起始点开始延长
    {
        int groundLen = Random.Range(groundLenRan.x, groundLenRan.y);
        for (int m = i; m <= i + groundLen; m++)
        {
            //限制超出范围不延长
            int x = Mathf.Clamp(m, lowerLeftCoo.x, this.length + lowerLeftCoo.x - 1);
            ground_tilemap.SetTile(new Vector3Int(x, j, 0), groundTile);
        }
    }
}

