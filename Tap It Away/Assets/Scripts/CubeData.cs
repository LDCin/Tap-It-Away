using UnityEngine;

[CreateAssetMenu(menuName = "Core/Cube", fileName = "Cube Data", order = 0)]
public class CubeData : ScriptableObject
{
    public CubeDirection cubeDirection;
    public Color cubeColor;
    public Color symbolColor;
}