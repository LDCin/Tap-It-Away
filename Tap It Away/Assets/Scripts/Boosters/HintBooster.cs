using System.Collections.Generic;
using UnityEngine;

public class HintBooster : BoosterBase
{
    private List<CubeMover> movableCubeList;
    [ContextMenu("Test Hint Booster")]
    public void Test()
    {
        Active();
    }
    public override void Active()
    {
        List<CubeMover> cubeMoverList = new(LevelManager.Instance.LevelCubeList);
        movableCubeList = new();
        foreach (var cube in cubeMoverList)
        {
            if (cube.CanMove())
            {
                movableCubeList.Add(cube);
            }
        }
        PlayHintEffect();
    }
    public override void Deactive()
    {
        throw new System.NotImplementedException();
    }
    public void PlayHintEffect()
    {
        foreach (var cube in movableCubeList)
        {
            Debug.Log("Shake Cube" + cube.CubeDirection);
            // Destroy(cube.gameObject);
            cube.ShakeCube(true);
        }
    }
}
