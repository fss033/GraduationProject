using UnityEngine;
using System.Collections.Generic;

public class Teleporter : MonoBehaviour
{
    [Header("传送设置")]
    public float cooldown = 2f;
    public float triggerRadius = 1f;
    public float activationRadius = 2f;
    public bool isMainTeleporter = false;

    [Header("动画设置")]
    public Animator animator;
    public string activateAnimName = "Activate";
    public string teleportAnimName = "Teleport";
    public string idleAnimName = "Idle";

    private float lastTeleportTime;
    private bool isActive = true;
    private bool playerInActivationRange = false;
    private bool playerInTriggerRange = false;
    private GameObject playerObject;

    // 传送系统变量
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

        // 注册所有传送门
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

        // 检测玩家是否在激活范围内
        bool previousActivationState = playerInActivationRange;
        playerInActivationRange = IsPlayerInRadius(activationRadius);
        if (playerInActivationRange != previousActivationState)
        {
            UpdateAnimationState();
        }

        // 检测玩家是否在触发范围内
        playerInTriggerRange = IsPlayerInRadius(triggerRadius);

        // 触发传送
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

        // 更新传送计数
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
            // 获取所有未使用且不是自身的传送门（排除主传送门作为常规目标）
            List<Teleporter> availableTargets = allTeleporters.FindAll(t =>
                !t.hasBeenUsed && t != this && !t.isMainTeleporter);

            if (availableTargets.Count > 0)
            {
                currentTarget = availableTargets[Random.Range(0, availableTargets.Count)];
                currentTarget.hasBeenUsed = true;
            }
            else
            {
                // 如果没有可用目标，传回主传送门
                currentTarget = FindMainTeleporter();
                ResetAllTeleporters();
            }
        }

        // 激活目标传送门动画
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