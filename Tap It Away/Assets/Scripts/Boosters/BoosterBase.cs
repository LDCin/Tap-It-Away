using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class BoosterBase
{
    protected int activeCount = 1;
    protected bool deactive = false;
    protected BoosterType boosterType;
    protected BoosterSO boosterSO;
    public async UniTask StartBooster()
    {
        await Active();
        if (deactive)
        {
            Deactive();
        }
    }
    public abstract UniTask Active();
    public abstract void Deactive();
    public BoosterBase(BoosterSO boosterSO)
    {
        this.boosterSO = boosterSO;
        activeCount = boosterSO.activeCount;
        deactive = boosterSO.deactive;
    }
}