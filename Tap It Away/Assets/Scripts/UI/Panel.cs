using System.Collections;
using UnityEngine;

public abstract class Panel : MonoBehaviour
{
    [SerializeField] private bool _destroyOnClose = false;
    [SerializeField] private UILayer uiLayer = UILayer.Overlay;
    private Coroutine _closeCoroutine;

    public UILayer UILayer => uiLayer;
    public void Open()
    {
        if (_closeCoroutine != null)
        {
            StopCoroutine(_closeCoroutine);
            _closeCoroutine = null;
        }

        gameObject.SetActive(true);
        UpdateVisual();
    }
    public abstract void UpdateVisual();
    // {
        // Debug.Log(gameObject.name + " Update Visual");
    // }
    public void Close()
    {
        if (!gameObject.activeInHierarchy)
        {
            return;
        }

        if (_destroyOnClose)
        {
            UIManager.Instance?.UnregisterPanel(name);
            Destroy(gameObject);
        }
        else
        {
            UIEffect effect = GetComponent<UIEffect>();

            if (effect != null && effect.UseCloseEffect)
            {
                if (_closeCoroutine != null)
                {
                    StopCoroutine(_closeCoroutine);
                }

                _closeCoroutine = StartCoroutine(CloseAfterEffect(effect));
                return;
            }

            gameObject.SetActive(false);
        }
    }

    private IEnumerator CloseAfterEffect(UIEffect effect)
    {
        if (effect.DeltaTimeIndependent)
        {
            yield return new WaitForSecondsRealtime(effect.HidePanelDelayTime);
            effect.Close();
            yield return new WaitForSecondsRealtime(effect.ClosePanelDuration);
        }
        else
        {
            yield return new WaitForSeconds(effect.HidePanelDelayTime);
            effect.Close();
            yield return new WaitForSeconds(effect.ClosePanelDuration);
        }

        _closeCoroutine = null;
        gameObject.SetActive(false);
    }
}
