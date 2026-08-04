using UnityEngine;

public class MenuPanel : Panel
{
    [SerializeField] private HomeTab homeTab;

    public override void UpdateVisual()
    {
        if (homeTab == null)
        {
            homeTab = GetComponentInChildren<HomeTab>(true);
        }

        homeTab?.Open();
    }
}
