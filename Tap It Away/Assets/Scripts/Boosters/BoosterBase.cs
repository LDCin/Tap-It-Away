using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BoosterBase
{
    private int activeCount = 1;
    public static event Action OnBoosterUsed;
    private BoosterType boosterType;
    public abstract void Active();
    public abstract void Deactive();
}