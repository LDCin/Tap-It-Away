using System;
using UnityEngine;

public static class CastHelper
{
    private const float DefaultBoxCastScale = 0.95f;
    public static bool ShootRaycast<T>(
        Camera camera,
        Vector2 screenPosition,
        CastConfig castConfig,
        out T component
    ) where T : Component
    {
        component = null;

        if (camera == null)
        {
            Debug.LogError("Camera is missing.");
            return false;
        }

        Ray ray = camera.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit, castConfig.castDistance, castConfig.castLayer))
        {
            return hit.collider.TryGetComponent(out component);
        }

        return false;
    }

    public static RaycastHit[] ShootBoxCast(
        BoxCollider boxCollider,
        Vector3 direction,
        CastConfig castConfig,
        QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Collide
    )
    {
        if (boxCollider == null)
        {
            Debug.LogError("BoxCollider is missing.");
            return Array.Empty<RaycastHit>();
        }

        if (direction == Vector3.zero)
        {
            return Array.Empty<RaycastHit>();
        }

        RaycastHit[] hits = Physics.BoxCastAll(
            boxCollider.bounds.center,
            boxCollider.bounds.extents * DefaultBoxCastScale,
            direction.normalized,
            boxCollider.transform.rotation,
            castConfig.castDistance,
            castConfig.castLayer,
            queryTriggerInteraction
        );

        return RemoveSelfHits(hits, boxCollider);
    }

    private static RaycastHit[] RemoveSelfHits(RaycastHit[] hits, Collider selfCollider)
    {
        int validHitCount = 0;
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider != selfCollider)
            {
                validHitCount++;
            }
        }

        RaycastHit[] validHits = new RaycastHit[validHitCount];
        int validHitIndex = 0;
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider == selfCollider)
            {
                continue;
            }

            validHits[validHitIndex] = hits[i];
            validHitIndex++;
        }

        return validHits;
    }
}
