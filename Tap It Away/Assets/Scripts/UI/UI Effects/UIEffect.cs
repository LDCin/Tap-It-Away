using UnityEngine;

public class UIEffect : MonoBehaviour
{
    [SerializeField] protected bool autoPlay = true;
    [SerializeField] protected bool deltaTimeIndependent = false;
    [SerializeField] protected bool useClosePanelEffect = true;
    [SerializeField] protected float showDelayTime = 0;
    [SerializeField] protected float hidePanelDelayTime = 0;
    public bool UseCloseEffect => useClosePanelEffect;
    public bool DeltaTimeIndependent => deltaTimeIndependent;
    public float HidePanelDelayTime => hidePanelDelayTime;
    public virtual float ClosePanelDuration => 0f;

    private void OnEnable()
    {
        Open();
    }

    private void Open()
    {
        if (autoPlay)
        {
            ShowEffect(showDelayTime);
        }

    }
    public virtual void ShowEffect(float showDelayTime)
    {

    }
    public virtual void HideEffect(float hideDelayTime)
    {
    }
    public virtual void FinishShowEffect()
    {

    }
    public void Close()
    {
        if (useClosePanelEffect)
        {
            HideEffect(0f);
        }
    }
}
