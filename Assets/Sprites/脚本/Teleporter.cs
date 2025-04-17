using UnityEngine;
using System.Collections.Generic;

public class Teleporter : MonoBehaviour
{
    [Header("��������")]
    public float cooldown = 2f;
    public float triggerRadius = 1f;
    public float activationRadius = 2f;
    public bool isMainTeleporter = false;

    [Header("��������")]
    public Animator animator;
    public string activateAnimName = "Activate";
    public string teleportAnimName = "Teleport";
    public string idleAnimName = "Idle";

    private float lastTeleportTime;
    private bool isActive = true;
    private bool playerInActivationRange = false;
    private bool playerInTriggerRange = false;
    private GameObject playerObject;

    // ����ϵͳ����
    private static List<Teleporter> allTeleporters = new List<Teleporter>();
    private static int teleportCount = 0;
    private bool hasBeenUsed = false;
    private Teleporter currentTarget;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        // ע�����д�����
        if (!allTeleporters.Contains(this))
        {
            allTeleporters.Add(this);
            hasBeenUsed = false;
        }
    }

    private void OnDestroy()
    {
        if (allTeleporters.Contains(this))
        {
            allTeleporters.Remove(this);
        }
    }

    private void Update()
    {
        if (!isActive) return;

        // �������Ƿ��ڼ��Χ��
        bool previousActivationState = playerInActivationRange;
        playerInActivationRange = IsPlayerInRadius(activationRadius);
        if (playerInActivationRange != previousActivationState)
        {
            UpdateAnimationState();
        }

        // �������Ƿ��ڴ�����Χ��
        playerInTriggerRange = IsPlayerInRadius(triggerRadius);

        // ��������
        if (playerInTriggerRange && Input.GetKeyDown(KeyCode.E) &&
            Time.time > lastTeleportTime + cooldown)
        {
            TeleportPlayer(playerObject);
        }
    }

    private bool IsPlayerInRadius(float radius)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                playerObject = collider.gameObject;
                return true;
            }
        }
        playerObject = null;
        return false;
    }

    private void UpdateAnimationState()
    {
        if (animator == null) return;

        if (playerInActivationRange)
        {
            animator.Play(activateAnimName);
        }
        else
        {
            animator.Play(idleAnimName);
        }
    }

    private void TeleportPlayer(GameObject player)
    {
        isActive = false;
        animator?.Play(teleportAnimName);

        // ���´��ͼ���
        teleportCount++;
        bool forceToMain = teleportCount >= 5;

        if (forceToMain)
        {
            teleportCount = 0;
            currentTarget = FindMainTeleporter();
            ResetAllTeleporters();
        }
        else
        {
            // ��ȡ����δʹ���Ҳ�������Ĵ����ţ��ų�����������Ϊ����Ŀ�꣩
            List<Teleporter> availableTargets = allTeleporters.FindAll(t =>
                !t.hasBeenUsed && t != this && !t.isMainTeleporter);

            if (availableTargets.Count > 0)
            {
                currentTarget = availableTargets[Random.Range(0, availableTargets.Count)];
                currentTarget.hasBeenUsed = true;
            }
            else
            {
                // ���û�п���Ŀ�꣬������������
                currentTarget = FindMainTeleporter();
                ResetAllTeleporters();
            }
        }

        // ����Ŀ�괫���Ŷ���
        if (currentTarget != null && currentTarget != this)
        {
            currentTarget.isActive = false;
            currentTarget.animator?.Play(teleportAnimName);
        }

        Invoke("ExecuteTeleport", 0.3f);
    }

    private void ExecuteTeleport()
    {
        if (playerObject != null && currentTarget != null)
        {
            playerObject.transform.position = currentTarget.transform.position;
        }

        lastTeleportTime = Time.time;
        currentTarget?.Invoke("ResetTeleporter", cooldown);
        Invoke("ResetTeleporter", cooldown);
    }

    private void ResetTeleporter()
    {
        isActive = true;
        UpdateAnimationState();
    }

    private void ResetAllTeleporters()
    {
        foreach (Teleporter teleporter in allTeleporters)
        {
            teleporter.hasBeenUsed = false;
        }
    }

    private Teleporter FindMainTeleporter()
    {
        foreach (Teleporter teleporter in allTeleporters)
        {
            if (teleporter.isMainTeleporter)
            {
                return teleporter;
            }
        }
        return null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, activationRadius);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, triggerRadius);
    }
}