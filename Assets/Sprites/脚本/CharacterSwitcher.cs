using UnityEngine;

public class CharacterSwitcher : MonoBehaviour
{
    public GameObject[] characters; // 所有可切换的角色
    public CameraController cameraController; // 镜头控制器
    public float switchRange = 2f; // 切换范围
    private int currentCharacterIndex = 0;

    void Start()
    {
        // 初始化，激活第一个角色，其他角色设置为静态
        SwitchCharacter(currentCharacterIndex);
    }

    void Update()
    {
        // 检测按 E 键切换角色
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
        // 禁用当前角色的控制
        characters[currentCharacterIndex].GetComponent<PlayerController>().enabled = false;
        characters[currentCharacterIndex].GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;

        // 激活新角色的控制
        characters[newIndex].GetComponent<PlayerController>().enabled = true;
        characters[newIndex].GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;

        // 更新镜头目标
        cameraController.target = characters[newIndex].transform;

        // 更新当前角色索引
        currentCharacterIndex = newIndex;
    }
}