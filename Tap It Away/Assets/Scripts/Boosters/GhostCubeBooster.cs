using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class GhostCubeBooster : BoosterBase, IDeactivatableBooster
{
    private int currentCount = 0;
    private List<CubeMover> ghostCubeList = new();
    private List<CubeMover> usedGhostCubeList = new();

    public GhostCubeBooster(BoosterSO boosterSO) : base(boosterSO)
    {
    }

    public override async UniTask Active()
    {
        currentCount = activeCount;
        ghostCubeList.Clear();
        usedGhostCubeList.Clear();
        Observer.Subscribe<CubeMover>(ObserverEvent.TapCube, HandleCubeTapped);

        List<CubeMover> cubeMoverList = new(LevelManager.Instance.LevelCubeList);
        foreach (var cubeMover in cubeMoverList)
        {
            if (cubeMover == null)
            {
                continue;
            }

            Cube cube = cubeMover.GetComponent<Cube>();
            if (cube == null)
            {
                continue;
            }

            cube.SetCubeGhostVisual();
            cubeMover.SetGhost(true);
            ghostCubeList.Add(cubeMover);
        }

        await UniTask.WaitUntil(() => currentCount <= 0);
        Observer.Unsubscribe<CubeMover>(ObserverEvent.TapCube, HandleCubeTapped);
    }

    private void HandleCubeTapped(CubeMover cubeMover)
    {
        if (currentCount > 0)
        {
            currentCount--;
        }

        if (cubeMover != null && !usedGhostCubeList.Contains(cubeMover))
        {
            usedGhostCubeList.Add(cubeMover);
        }
    }
    public void Deactive()
    {
        foreach (var cubeMover in ghostCubeList)
        {
            if (cubeMover == null || usedGhostCubeList.Contains(cubeMover))
            {
                continue;
            }

            Cube cube = cubeMover.GetComponent<Cube>();
            if (cube != null)
            {
                cube.SetCubeNormalVisual();
            }

            cubeMover.SetGhost(false);
        }

        ghostCubeList.Clear();
        usedGhostCubeList.Clear();
    }
    public void Dispose()
    {
        Observer.Unsubscribe<CubeMover>(ObserverEvent.TapCube, HandleCubeTapped);
    }
}
