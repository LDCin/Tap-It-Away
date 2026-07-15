using UnityEngine;
using DG.Tweening;
using System;

public class Cube : MonoBehaviour
{
    [SerializeField] private float moveDuration = 1;
    [SerializeField] private float moveDistance = 10;

    public void Move()
    {
        transform.DOMove(transform.position + Vector3.forward * moveDistance, moveDuration).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }
}
