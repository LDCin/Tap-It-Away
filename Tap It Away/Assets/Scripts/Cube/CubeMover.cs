using UnityEngine;
using DG.Tweening;
using System;
using System.Collections;
using Unity.VisualScripting;

public class CubeMover : MonoBehaviour
{
    public static event Action<Collision> OnCubeBlock;
    [SerializeField] private float moveSpeed = 1;
    [SerializeField] private float moveDistance = 50;
    [SerializeField] private float collisionSkin = 0.02f;
    [SerializeField] private bool isBlocked = false;
    [SerializeField] private float delayActionTime = 1f;
    public CubeDirection CubeDirection { get; set; }
    public Vector3 StartPosition { get; set; }
    public bool IsMoving => isMoving;
    private bool isMoving = false;
    private Tween tween;
    private BoxCollider boxCollider;
    private Rigidbody rb;
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
    }
    private void Move(Vector3 position, Action onMoveComplete = null)
    {
        isBlocked = false;
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
        Move(transform.position + directionVector * moveDistance, OnMoveOutCompleted);
    }
    private void OnMoveOutCompleted()
    {
        Destroy(gameObject);
    }
    private void ReturnToStartPosition()
    {
        Move(StartPosition);
    }
    private IEnumerator DelayTouch(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        isMoving = false;
    }
    private void ShakeCube()
    {
        Vector3 originalScale = transform.localScale;
        boxCollider.enabled = false;

        tween?.Kill();
        tween = DOTween.Sequence()
            .Append(transform.DOScale(originalScale * 1.12f, 0.08f).SetEase(Ease.OutQuad))
            .Append(transform.DOScale(originalScale, 0.12f).SetEase(Ease.OutBack))
            .OnComplete(() =>
            {
                // transform.localScale = originalScale;
                tween = null;
                boxCollider.enabled = true;
            });
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag(GameConfig.CUBE_TAG))
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
            OnCubeBlock?.Invoke(collision);
        }
        tween?.Kill();
        ReturnToStartPosition();
    }
}
