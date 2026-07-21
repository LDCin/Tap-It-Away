using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class LevelData
{
    public BoardData board;
    public List<CubeData> cubes; 
}

[Serializable]
public class BoardData
{
    public int sizeX;
    public int sizeY;
    public int sizeZ;
}

[Serializable]
public class CubeData
{
    public Vector3 position;
    public CubeDirection moveDirection;
    public string cubeColor;
    public string symbolColor;
}
// [Serializable]
// public class PositionData
// {
//     public int x;
//     public int y;
//     public int z;
// }
