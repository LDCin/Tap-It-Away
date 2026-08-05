using UnityEngine;
using DG.Tweening;
using System;
using System.Collections;
// using NUnit.Framework;

public class CubeMover : MonoBehaviour
{
    public static event Action OnCubeBlock;
    public static event Action OnCubeRemoved;
    public static event Action<CubeMover> OnCubeRemovedWithReference;
    public static event Action<CubeMover> OnCubeReturnedWithReference;
    [SerializeField] private float moveSpeed = 1;
    [SerializeField] private float moveDistance = 50;
    // [SerializeField] private float collisionSkin = 0.02f;
    [SerializeField] private bool isBlocked = false;
    [SerializeField] private float scaleRate = 1.5f;
    [SerializeField] private float scaleTime = 0.1f;
    [SerializeField] private float delayActionTime = 1f;
    [SerializeField] private CastConfig castConfig;
    public CubeDirection CubeDirection { get; set; }
    public Vector3 StartPosition { get; set; }
    private bool isMoving = false;
    private bool isShaking = false;
    [SerializeField] private bool isGhost = false;
    private Tween tween;
    private BoxCollider boxCollider;
    private Rigidbody rb;
    private Vector3 originalScale;
    private bool isRemovedFromLevelList = false;
    private bool isRemovedFromLevelState = false;
    private Transform originalParent;
    private Vector3 originalLocalPosition;
    private Quaternion originalLocalRotation;
    private Vector3 originalLocalScale;
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
        originalScale = transform.localScale;
    }
    public void DisableCollider()
    {
        boxCollider.enabled = false;
    }
    public void EnableCollider()
    {
        boxCollider.enabled = true;
    }
    public void SetGhost(bool ghost)
    {
        isGhost = ghost;
    }
    public bool CanMove()
    {
        Vector3 direction = CubeDirectionHelper.GetWorldDirection(CubeDirection, transform);
        RaycastHit[] hits = CastHelper.ShootBoxCast(boxCollider, direction, castConfig);
        if (hits.Length > 0)
        {
            return false;
        }
        return true;
    }
    private void Move(Vector3 position, Action onMoveComplete = null, bool enableColliderOnComplete = true)
    {
        isMoving = true;

        tween?.Kill();
        float duration = Vector3.Distance(transform.position, position) / moveSpeed;
        tween = rb.DOMove(position, duration)
        .SetEase(Ease.OutQuad)
        .SetUpdate(UpdateType.Fixed)
        .OnComplete(() =>
        {
            tween = null;
            StartCoroutine(DelayTouch(delayActionTime));
            if (enableColliderOnComplete)
            {
                EnableCollider();
            }
            SetGhost(false);
            onMoveComplete?.Invoke();
        });
    }
    public void MoveOut()
    {
        if (isMoving)
        {
            return;
        }

        bool canMoveOut = isGhost || CanMove();
        if (canMoveOut)
        {
            RemoveFromLevelState();
            DisableCollider();
        }

        CacheOriginalParent();
        RemoveFromLevelList();
        DetachFromPuzzleRoot();

        StartPosition = transform.position;
        Vector3 directionVector = CubeDirectionHelper.GetWorldDirection(CubeDirection, transform);
        Move(transform.position + directionVector * moveDistance, OnMoveOutCompleted, !canMoveOut);
    }
    private void OnMoveOutCompleted()
    {
        RemoveFromLevelList();
        RemoveFromLevelState();
        Destroy(gameObject);
    }

    private void RemoveFromLevelState()
    {
        if (isRemovedFromLevelState)
        {
            return;
        }

        isRemovedFromLevelState = true;
        OnCubeRemoved?.Invoke();
    }

    private void RemoveFromLevelList()
    {
        if (isRemovedFromLevelList)
        {
            return;
        }

        isRemovedFromLevelList = true;
        OnCubeRemovedWithReference?.Invoke(this);
    }
    private void DetachFromPuzzleRoot()
    {
        if (transform.parent == null)
        {
            return;
        }

        transform.SetParent(null, true);
    }
    private void CacheOriginalParent()
    {
        originalParent = transform.parent;
        originalLocalPosition = transform.localPosition;
        originalLocalRotation = transform.localRotation;
        originalLocalScale = transform.localScale;
    }
    private void ReturnToStartPosition()
    {
        Vector3 returnPosition = originalParent != null
            ? originalParent.TransformPoint(originalLocalPosition)
            : StartPosition;

        Move(returnPosition, OnReturnToStartPositionCompleted);
    }
    private void OnReturnToStartPositionCompleted()
    {
        ReattachToPuzzleRoot();
        isRemovedFromLevelList = false;
        OnCubeReturnedWithReference?.Invoke(this);
    }
    private void ReattachToPuzzleRoot()
    {
        if (originalParent == null)
        {
            return;
        }

        transform.SetParent(originalParent, true);
        transform.localPosition = originalLocalPosition;
        transform.localRotation = originalLocalRotation;
        transform.localScale = originalLocalScale;
    }
    private IEnumerator DelayTouch(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        isMoving = false;
    }
    public void ShakeCube(bool loop = false)
    {
        // boxCollider.enabled = false;

        tween?.Kill();
        transform.localScale = originalScale;

        isShaking = true;
        Sequence shakeSequence = DOTween.Sequence()
            .Append(transform.DOScale(originalScale * scaleRate, scaleTime).SetEase(Ease.OutQuad))
            .Append(transform.DOScale(originalScale, scaleTime).SetEase(Ease.OutBack));

        if (loop)
        {
            shakeSequence.SetLoops(-1, LoopType.Restart);
        }

        tween = shakeSequence
            .OnComplete(() =>
            {
                transform.localScale = originalScale;
                tween = null;
                // boxCollider.enabled = true;
                isShaking = false;
            })
            .OnKill(() =>
            {
                transform.localScale = originalScale;
                // boxCollider.enabled = true;
                isShaking = false;
            });
    }
    private void OnTriggerEnter(Collider other)
    {
        if (isGhost || isShaking || !other.gameObject.CompareTag(GameConfig.CUBE_TAG))
        {
            return;
        }

        if (!isMoving)
        {
            ShakeCube();
            return;
        }

        if (!isBlocked)
        {
            isBlocked = true;
            OnCubeBlock?.Invoke();
        }
        // tween?.Kill();
        ReturnToStartPosition();
    }
}
