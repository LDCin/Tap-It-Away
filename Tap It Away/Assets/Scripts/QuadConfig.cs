using UnityEngine;

public struct QuadConfig
{
    public string quadName;
    public Vector3 faceDirection;
    public Vector3 upDirection;

    public QuadConfig(
        string quadName,
        Vector3 faceDirection,
        Vector3 upDirection)
    {
        this.quadName = quadName;
        this.faceDirection = faceDirection;
        this.upDirection = upDirection;
    }
}