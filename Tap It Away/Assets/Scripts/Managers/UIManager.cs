using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class UIManager : Singleton<UIManager>
{
    [Header("UI Roots")]
    [SerializeField] private Transform overlayCanvasRoot;
    [SerializeField] private Transform cameraCanvasRoot;

    private Dictionary<string, Panel> _panelDict = new();
    private HashSet<string> _loadingPanels = new();

    public override void Awake()
    {
        base.Awake();

        var existPanelList = GetComponentsInChildren<Panel>(true);

        foreach (var panel in existPanelList)
        {
            if (!_panelDict.ContainsKey(panel.name))
            {
                _panelDict.Add(panel.name, panel);
            }
            else
            {
                Debug.LogWarning($"Duplicate panel name: {panel.name}");
            }
        }
    }

    private Transform GetRoot(UILayer layer)
    {
        return layer switch
        {
            UILayer.Overlay => overlayCanvasRoot,
            UILayer.Camera => cameraCanvasRoot,
            _ => overlayCanvasRoot
        };
    }

    public async UniTask LoadPanel(string panelName)
    {
        if (_panelDict.ContainsKey(panelName))
            return;

        if (_loadingPanels.Contains(panelName))
        {
            await UniTask.WaitUntil(() => !_loadingPanels.Contains(panelName));
            return;
        }

        _loadingPanels.Add(panelName);

        var panelHandle = Addressables.InstantiateAsync(panelName, transform);
        await panelHandle;

        _loadingPanels.Remove(panelName);

        if (panelHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"Failed to load panel: {panelName}");
            return;
        }

        Panel newPanel = panelHandle.Result.GetComponent<Panel>();

        if (newPanel == null)
        {
            Debug.LogError($"Loaded object does not have Panel component: {panelName}");
            Addressables.ReleaseInstance(panelHandle.Result);
            return;
        }

        newPanel.gameObject.name = panelName;

        Transform root = GetRoot(newPanel.UILayer);

        if (root == null)
        {
            Debug.LogError($"Root is missing for UI layer: {newPanel.UILayer}");
            Addressables.ReleaseInstance(panelHandle.Result);
            return;
        }

        newPanel.transform.SetParent(root, false);
        newPanel.transform.SetAsLastSibling();

        _panelDict[panelName] = newPanel;
    }

    public Panel GetPanel(string panelName)
    {
        _panelDict.TryGetValue(panelName, out Panel panel);
        return panel;
    }

    public void UnregisterPanel(string panelName)
    {
        if (string.IsNullOrEmpty(panelName))
            return;

        _panelDict.Remove(panelName);
    }

    public void OpenPanel(string panelName, bool useLoading = false)
    {
        OpenPanelAsync(panelName, useLoading).Forget();
    }

    public async UniTask OpenPanelAsync(string panelName, bool useLoading = false)
    {
        if (useLoading && !panelName.Equals(GameConfig.LOADING_PANEL))
        {
            await OpenPanelWithLoadingAsync(panelName);
            return;
        }

        await OpenPanelInternalAsync(panelName);
    }

    private async UniTask OpenPanelWithLoadingAsync(string panelName)
    {
        await OpenPanelInternalAsync(GameConfig.LOADING_PANEL);

        LoadingPanel loadingPanel = GetPanel(GameConfig.LOADING_PANEL) as LoadingPanel;

        if (loadingPanel != null)
        {
            await UniTask.WaitUntil(() => loadingPanel.IsDone);
        }

        await OpenPanelInternalAsync(panelName);

        if (loadingPanel != null)
        {
            loadingPanel.transform.SetAsLastSibling();
        }

        ClosePanel(GameConfig.LOADING_PANEL);
    }

    private async UniTask OpenPanelInternalAsync(string panelName)
    {
        await LoadPanel(panelName);

        Panel panel = GetPanel(panelName);

        if (panel != null)
        {
            panel.Open();
        }
        else
        {
            Debug.LogError($"Panel not found: {panelName}");
        }
    }

    public void ClosePanel(string panelName)
    {
        Panel panel = GetPanel(panelName);

        if (panel != null)
        {
            panel.Close();
        }
        else
        {
            Debug.LogWarning($"Cannot close unloaded panel: {panelName}");
        }
    }

    public void CloseAllPanelExcept(string panelName)
    {
        var keys = new List<string>(_panelDict.Keys);

        foreach (var key in keys)
        {
            if (!key.Equals(panelName) && _panelDict.TryGetValue(key, out Panel panel))
            {
                panel.Close();
            }
        }
    }
}
