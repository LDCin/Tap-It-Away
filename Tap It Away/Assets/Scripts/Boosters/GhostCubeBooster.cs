using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GhostCubeBooster : BoosterBase
{
    private int count = 3;
    [ContextMenu("Test Ghost Booster")]
    public void Test()
    {
        Active();
    }
    public override void Active()
    {
        List<CubeMover> cubeMoverList = new(LevelManager.Instance.LevelCubeList);
        foreach (var cubeMover in cubeMoverList)
        {
            Cube cube = cubeMover.gameObject.GetComponent<Cube>();
            // cube.SetOpacity(ghostOpacity);
            cube.SetCubeGhostVisual();
            cubeMover.SetGhost(true);
        }
    }
    public override void Deactive()
    {
        throw new System.NotImplementedException();
    }
}
