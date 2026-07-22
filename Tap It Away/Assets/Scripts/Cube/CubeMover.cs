using UnityEngine;
using DG.Tweening;
using System;
using NUnit.Framework;

public class CubeMover : MonoBehaviour
{
    public static event Action<Collision> OnCubeBlock;
    [SerializeField] private float moveSpeed = 1;
    [SerializeField] private float moveDistance = 50;
    [SerializeField] private bool isBlocked = false;
    public CubeDirection CubeDirection { get; set; }
    public Vector3 StartPosition { get; set; }
    private bool isMoving = false;
    private Tween tween;
    private BoxCollider boxCollider;
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
    }
    public void Move(Vector3 position, Action onMoveComplete = null)
    {
        float duration = Vector3.Distance(transform.position, position) / moveSpeed;
        tween?.Kill();
        isMoving = true;
        tween = transform.DOMove(position, duration).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            // transform.DOPunchScale(Vector3.one * 0.05f, 0.5f, 10, 1f);
            tween = null;
            isMoving = false;
            onMoveComplete?.Invoke();
        });
    }
    public void MoveOut()
    {
        Vector3 directionVector = CubeDirectionHelper.GetDirectionVector(CubeDirection);
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
    private void ShakeCube()
    {
        Vector3 originalScale = transform.localScale;
        boxCollider.enabled = false;
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
        if (!isMoving)
        {
            ShakeCube();
            return;
        }
        if (collision.gameObject.CompareTag(GameConfig.CUBE_TAG))
        {
            if (!isBlocked)
            {
                isBlocked = true;
                OnCubeBlock?.Invoke(collision);
            }
            transform.DOKill();
            ReturnToStartPosition();
        }
    }
}