using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuPanel : Panel
{
    public static event Action OnPlayGame;
    [SerializeField] private Button play;
    public override void UpdateVisual()
    {
        
    }
    public void Play()
    {
        OnPlayGame?.Invoke();
    }
}