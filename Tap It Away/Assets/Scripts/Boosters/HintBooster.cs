using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class HintBooster : BoosterBase
{
    private List<CubeMover> movableCubeList;
    public HintBooster(BoosterSO boosterSO) : base(boosterSO)
    {
    }
    public override UniTask Active()
    {
        List<CubeMover> cubeMoverList = new(LevelManager.Instance.LevelCubeList);
        movableCubeList = new();

        foreach (var cube in cubeMoverList)
        {
            if (cube == null)
            {
                continue;
            }

            if (cube.CanMove())
            {
                movableCubeList.Add(cube);
            }
        }

        PlayHintEffect();

        return UniTask.CompletedTask;
    }
    public void PlayHintEffect()
    {
        foreach (var cube in movableCubeList)
        {
            if (cube == null)
            {
                continue;
            }

            Debug.Log("Shake Cube" + cube.CubeDirection);
            // Destroy(cube.gameObject);
            cube.ShakeCube(true);
        }
    }
}
