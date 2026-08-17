using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ToggleButton
{
    [SerializeField] private Image icon;
    [SerializeField] private Sprite iconOn;
    [SerializeField] private Sprite iconOff;

    public UnityEvent<bool> OnValueChanged;
    
    protected bool _isOn;
    public virtual void SetState(bool state, bool invokeEvent = true)
    {
        if(_isOn == state) return;
        _isOn = state;

        if (_isOn)
        {
            icon.sprite = iconOn;
        }
        else
        {
            icon.sprite = iconOff;
        }
        
        if(invokeEvent)
            OnValueChanged?.Invoke(_isOn);
    }

    public virtual void OnClick()
    {
        SetState(!_isOn);
    }
}
