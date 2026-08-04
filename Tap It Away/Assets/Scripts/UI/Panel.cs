using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public abstract class Panel : MonoBehaviour
{
    [SerializeField] private bool _destroyOnClose = false;
    [SerializeField] private bool hasEffectOnOpen = false;
    [SerializeField] private bool hasEffectOnClose = false;
    [SerializeField] private UILayer uiLayer = UILayer.Overlay;

    public UILayer UILayer => uiLayer;
    public void Open()
    {
        PlayOpenEffect();
    }

    public abstract void UpdateVisual();

    public void PlayOpenEffect()
    {
        gameObject.SetActive(true);
        if (hasEffectOnOpen)
        {

        }
        UpdateVisual();
    }
    public void PlayCloseEffect()
    {
        if (hasEffectOnClose)
        {

        }
        if (_destroyOnClose)
        {
            UIManager.Instance?.UnregisterPanel(name);
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void Close()
    {
        PlayCloseEffect();
    }
}
