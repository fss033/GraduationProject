using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("跟踪目标")]
    public Transform target;  // 玩家角色

    [Header("基础设置")]
    [SerializeField] private float smoothTime = 0.25f;
    [SerializeField] private bool enableVerticalFollow = true; // 是否启用垂直方向跟随
    [SerializeField] private float maxSpeed = Mathf.Infinity;

    [Header("前瞻功能")]
    [SerializeField] private Vector2 lookAheadOffset = new Vector2(2f, 1f);
    [SerializeField] private float lookAheadMultiplier = 0.5f;

    [Header("边界限制")]
    public bool useBounds = true;
    public Vector2 minBounds;
    public Vector2 maxBounds;

    private Vector3 _currentVelocity;
    private Vector3 _targetLastPosition;
    private Vector2 _lookAheadDirection;

    private void LateUpdate()
    {
        if (target == null) return;

        UpdateLookAheadDirection();
        Vector3 targetPosition = CalculateTargetPosition();
        ApplyCameraBounds(ref targetPosition);

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref _currentVelocity,
            smoothTime,
            maxSpeed
        );
    }

    // 计算前瞻方向
    private void UpdateLookAheadDirection()
    {
        Vector2 targetMovement = (target.position - _targetLastPosition).normalized;
        _lookAheadDirection = Vector2.Lerp(
            _lookAheadDirection,
            targetMovement * lookAheadMultiplier,
            Time.deltaTime * 10f
        );
        _targetLastPosition = target.position;
    }

    // 计算目标位置
    private Vector3 CalculateTargetPosition()
    {
        Vector3 targetPosition = target.position;
        targetPosition += (Vector3)(_lookAheadDirection * lookAheadOffset);

        if (!enableVerticalFollow)
        {
            targetPosition.y = transform.position.y;
        }
        targetPosition.z = -10; // 保持2D相机距离

        return targetPosition;
    }

    // 应用边界限制（未应用）
    private void ApplyCameraBounds(ref Vector3 targetPosition)
    {
        if (!useBounds) return;

        float orthoSize = Camera.main.orthographicSize;
        float aspect = Camera.main.aspect;

        // 计算相机视野边界
        float verticalExtent = orthoSize;
        float horizontalExtent = orthoSize * aspect;

        targetPosition.x = Mathf.Clamp(
            targetPosition.x,
            minBounds.x + horizontalExtent,
            maxBounds.x - horizontalExtent
        );

        targetPosition.y = Mathf.Clamp(
            targetPosition.y,
            minBounds.y + verticalExtent,
            maxBounds.y - verticalExtent
        );
    }

    // 调试绘制边界(未应用)
    private void OnDrawGizmosSelected()
    {
        if (!useBounds) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(
            new Vector3(
                (minBounds.x + maxBounds.x) / 2,
                (minBounds.y + maxBounds.y) / 2,
                0
            ),
            new Vector3(
                maxBounds.x - minBounds.x,
                maxBounds.y - minBounds.y,
                1
            )
        );
    }
}