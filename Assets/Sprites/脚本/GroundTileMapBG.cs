using UnityEngine.Tilemaps;
using UnityEngine;

public class GroundTileMapBG : MonoBehaviour
{
    [Header("��ͼ����")]
    public Tilemap ground_tilemap;//�϶���ȡ������Ƭ��ͼ
    public TileBase groundTile;//�϶���ȡ���������ƬRule Tile
    public Vector2Int lowerLeftCoo = new Vector2Int(10, 10);//��ͼ��ʼ���½�λ��
    public int width = 50;//��ͼ���
    public int length = 100;//��ͼ����
    public float groundStartPro = 0.10f;//���ɵ�����ʼ��ĸ���
    public Vector2Int groundLenRan = new Vector2Int(3, 10);//��ʼ��������ɵĳ��ȷ�Χ

    void Start()
    {
        CreateMap();

    }

    public void CreateMap()
    {
        GroundStartPoi();
    }

    void GroundStartPoi()//���ɵ�����ʼ�� ��Э�̿��Ի���һ�������ɵ�ͼ������������
    {
        ground_tilemap.ClearAllTiles();// ��յ�����Ƭ��ͼ
        for (int i = lowerLeftCoo.x; i < (this.length + lowerLeftCoo.x); i++)
        {
            for (int j = lowerLeftCoo.y; j < (this.width + lowerLeftCoo.y); j++)
            {
                bool IsGround = j < (this.width + 3) ? (Random.value <= groundStartPro) : (Random.value <= groundStartPro + 0.05);//��Ԫ���ʽ���������и��������ɵ�����ʼ��
                if (IsGround) GroundExtension(i, j);
            }
        }
    }

    void GroundExtension(int i, int j)//�ӵ�����ʼ�㿪ʼ�ӳ�
    {
        int groundLen = Random.Range(groundLenRan.x, groundLenRan.y);
        for (int m = i; m <= i + groundLen; m++)
        {
            //���Ƴ�����Χ���ӳ�
            int x = Mathf.Clamp(m, lowerLeftCoo.x, this.length + lowerLeftCoo.x - 1);
            ground_tilemap.SetTile(new Vector3Int(x, j, 0), groundTile);
        }
    }
}

