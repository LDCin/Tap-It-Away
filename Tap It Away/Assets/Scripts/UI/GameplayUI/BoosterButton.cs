using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoosterButton : Button
{
    private BoosterType boosterType;
    public BoosterType BoosterType => boosterType;

    public void SetBoosterButton(BoosterType boosterType)
    {
        this.boosterType = boosterType;
    }
}