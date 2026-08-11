using UnityEngine;
using DG.Tweening;
using System;
using System.Collections;
// using NUnit.Framework;

public class CubeMover : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1;
    [SerializeField] private float moveDistance = 50;
    // [SerializeField] private float collisionSkin = 0.02f;
    [SerializeField] private bool isBlocked = false;
    [SerializeField] private float scaleRate = 1.5f;
    [SerializeField] private float scaleTime = 0.1f;
    [SerializeField] private float delayActionTime = 1f;
    [SerializeField] private CastConfig castConfig;
    [SerializeField] private CubeVisual cubeVisual;
    public CubeDirection CubeDirection { get; set; }
    public Vector3 StartPosition { get; set; }
    private bool isMoving = false;
    public bool IsMoving => isMoving;
    private bool isShaking = false;
    [SerializeField] private bool isGhost = false;
    private Tween tween;
    private BoxCollider boxCollider;
    private Rigidbody rb;
    private Vector3 originalScale;
    private Coroutine delayTouchCoroutine;
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
        cubeVisual = GetComponent<CubeVisual>();
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
    private void Move(Vector3 position, bool useTrail, Action onMoveComplete = null)
    {
        isMoving = true;

        tween?.Kill();
        // Observer.Publish(ObserverEvent.OnCubeMove);

        if (useTrail)
        {
            cubeVisual.ChangeTrailRendererState(true);
            cubeVisual.SetHasParent(false);
        }
        else
        {
            cubeVisual.ChangeTrailRendererState(false);
        }

        float duration = Vector3.Distance(transform.position, position) / moveSpeed;
        tween = rb.DOMove(position, duration)
        .SetEase(Ease.OutQuad)
        .SetUpdate(UpdateType.Fixed)
        .OnComplete(() =>
        {
            tween = null;
            delayTouchCoroutine = StartCoroutine(DelayTouch(delayActionTime));
            ResetToIdleState();
            onMoveComplete?.Invoke();
        });
    }
    public void MoveOut()
    {
        if (isMoving)
        {
            return;
        }
        StartPosition = transform.position;
        Vector3 directionVector = CubeDirectionHelper.GetWorldDirection(CubeDirection, transform);
        Move(transform.position + directionVector * moveDistance, true, OnMoveOutCompleted);
    }
    private void OnMoveOutCompleted()
    {
        Observer.Publish(ObserverEvent.CubeRemoved);
        Destroy(gameObject);
    }
    private void ReturnToStartPosition()
    {
        Move(StartPosition, false, null);
    }
    private IEnumerator DelayTouch(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        isMoving = false;
        delayTouchCoroutine = null;
    }
    public void ShakeCube(bool loop = false)
    {
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

        // CubeMover otherCube = other.GetComponent<CubeMover>();
        if (!isMoving)
        {
            // if (otherCube.IsMoving)
            // {
            //     ShakeCube();
            // }
            ShakeCube();
            return;
        }

        if (!isBlocked)
        {
            isBlocked = true;
            Observer.Publish(ObserverEvent.CubeBlocked);
        }
        // tween?.Kill();
        ReturnToStartPosition();
    }
    public void ResetToIdleState()
    {
        isMoving = false;
        isShaking = false;
        SetGhost(false);
        EnableCollider();
        transform.localScale = originalScale;

        if (cubeVisual != null)
        {
            cubeVisual.ResetToInitialState();
        }
    }
}
