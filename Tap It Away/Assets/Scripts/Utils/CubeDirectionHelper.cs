using UnityEngine;

public static class CubeDirectionHelper
{
    public static Vector3 GetDefaultUpDirection(Vector3 faceDirection)
    {
        if (faceDirection == Vector3.up)
        {
            return Vector3.forward;
        }

        if (faceDirection == Vector3.down)
        {
            return Vector3.back;
        }

        return Vector3.up;
    }

    public static Vector3 GetDirectionVector(CubeDirection direction)
    {
        return direction switch
        {
            CubeDirection.Up => Vector3.up,
            CubeDirection.Down => Vector3.down,
            CubeDirection.Left => Vector3.left,
            CubeDirection.Right => Vector3.right,
            CubeDirection.Forward => Vector3.forward,
            CubeDirection.Back => Vector3.back,
            _ => Vector3.zero,
        };
    }

    public static Vector3 GetWorldDirection(CubeDirection direction, Transform transform)
    {
        return transform.TransformDirection(GetDirectionVector(direction)).normalized;
    }
}
