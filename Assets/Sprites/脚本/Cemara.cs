using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("����Ŀ��")]
    public Transform target;  // ��ҽ�ɫ

    [Header("��������")]
    [SerializeField] private float smoothTime = 0.25f;
    [SerializeField] private bool enableVerticalFollow = true; // �Ƿ����ô�ֱ�������
    [SerializeField] private float maxSpeed = Mathf.Infinity;

    [Header("ǰհ����")]
    [SerializeField] private Vector2 lookAheadOffset = new Vector2(2f, 1f);
    [SerializeField] private float lookAheadMultiplier = 0.5f;

    [Header("�߽�����")]
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

    // ����ǰհ����
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

    // ����Ŀ��λ��
    private Vector3 CalculateTargetPosition()
    {
        Vector3 targetPosition = target.position;
        targetPosition += (Vector3)(_lookAheadDirection * lookAheadOffset);

        if (!enableVerticalFollow)
        {
            targetPosition.y = transform.position.y;
        }
        targetPosition.z = -10; // ����2D�������

        return targetPosition;
    }

    // Ӧ�ñ߽����ƣ�δӦ�ã�
    private void ApplyCameraBounds(ref Vector3 targetPosition)
    {
        if (!useBounds) return;

        float orthoSize = Camera.main.orthographicSize;
        float aspect = Camera.main.aspect;

        // ���������Ұ�߽�
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

    // ���Ի��Ʊ߽�(δӦ��)
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