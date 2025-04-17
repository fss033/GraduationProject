using UnityEngine;

public class CharacterSwitcher : MonoBehaviour
{
    public GameObject[] characters; // ���п��л��Ľ�ɫ
    public CameraController cameraController; // ��ͷ������
    public float switchRange = 2f; // �л���Χ
    private int currentCharacterIndex = 0;

    void Start()
    {
        // ��ʼ���������һ����ɫ��������ɫ����Ϊ��̬
        SwitchCharacter(currentCharacterIndex);
    }

    void Update()
    {
        // ��ⰴ E ���л���ɫ
        if (Input.GetKeyDown(KeyCode.E))
        {
            GameObject nearestCharacter = FindNearestCharacter();
            if (nearestCharacter != null)
            {
                int newIndex = System.Array.IndexOf(characters, nearestCharacter);
                if (newIndex != currentCharacterIndex)
                {
                    SwitchCharacter(newIndex);
                }
            }
        }
    }

    GameObject FindNearestCharacter()
    {
        GameObject nearestCharacter = null;
        float nearestDistance = switchRange;

        foreach (var character in characters)
        {
            if (character != characters[currentCharacterIndex])
            {
                float distance = Vector2.Distance(characters[currentCharacterIndex].transform.position, character.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestCharacter = character;
                }
            }
        }

        return nearestCharacter;
    }

    void SwitchCharacter(int newIndex)
    {
        // ���õ�ǰ��ɫ�Ŀ���
        characters[currentCharacterIndex].GetComponent<PlayerController>().enabled = false;
        characters[currentCharacterIndex].GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;

        // �����½�ɫ�Ŀ���
        characters[newIndex].GetComponent<PlayerController>().enabled = true;
        characters[newIndex].GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;

        // ���¾�ͷĿ��
        cameraController.target = characters[newIndex].transform;

        // ���µ�ǰ��ɫ����
        currentCharacterIndex = newIndex;
    }
}