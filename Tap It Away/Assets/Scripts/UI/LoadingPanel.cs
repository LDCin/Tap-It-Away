using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPanel : Panel
{
    [SerializeField] private Image progress;
    [SerializeField] private float loadingTime;
    public override void UpdateVisual()
    {
        progress.fillAmount = 0;
        progress.DOFillAmount(1, loadingTime);
    }
}