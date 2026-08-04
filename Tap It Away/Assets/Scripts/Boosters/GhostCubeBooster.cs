using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GhostCubeBooster : BoosterBase
{
    private int currentCount = 0;
    public GhostCubeBooster(BoosterSO boosterSO) : base(boosterSO)
    {
        CubeMover.OnCubeRemoved += Handle;
    }
    public override async UniTask Active()
    {
        currentCount = activeCount;
        List<CubeMover> cubeMoverList = new(LevelManager.Instance.LevelCubeList);
        foreach (var cubeMover in cubeMoverList)
        {
            Cube cube = cubeMover.gameObject.GetComponent<Cube>();
            // cube.SetOpacity(ghostOpacity);
            cube.SetCubeGhostVisual();
            cubeMover.SetGhost(true);
        }
        await UniTask.WaitUntil(() => currentCount <= 0);
    }
    public void Handle()
    {
        if (currentCount > 0)
        {
            currentCount--;
        }
    }
    public override void Deactive()
    {

    }
    public void Dispose()
    {
        CubeMover.OnCubeRemoved -= Handle;
    }
}
