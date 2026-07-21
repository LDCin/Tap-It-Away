using UnityEngine;
using DG.Tweening;
using System;

public class CubeMover : MonoBehaviour
{
    public static event Action<Collision> OnCubeBlock;
    [SerializeField] private float moveDuration = 1;
    [SerializeField] private float moveDistance = 50;
    [SerializeField] private bool isBlocked = false;
    public CubeDirection CubeDirection { get; set; }
    public Vector3 StartPosition { get; set; }

    public void Move(Vector3 position, Action onMoveComplete = null)
    {
        transform.DOMove(position, moveDuration).SetEase(Ease.OutQuad).OnComplete(() =>
        {
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
    private void OnCollisionEnter(Collision collision)
    {
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