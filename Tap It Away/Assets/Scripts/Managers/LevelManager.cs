using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

public class LevelManager : Singleton<LevelManager>
{
    public static event Action<int> OnCubeCountChanged;
    public static  event Action<int> OnHeartCountChanged;
    public static  event Action OnLevelCompleted;
    public static  event Action OnLevelFailed;
    [SerializeField] private LevelLoader levelLoader;
#if UNITY_EDITOR
    [SerializeField] private TextAsset levelDataFile;
#endif
    [SerializeField] private bool useLevelFileToTest = false;
    [SerializeField] private readonly int maxHeart = 3;
    public LevelState CurrentLevelState { get; private set; }
    private List<CubeMover> levelCubeList;
    public List<CubeMover> LevelCubeList => levelCubeList;
    private void OnEnable()
    {
        CubeMover.OnCubeRemoved += HandleCubeRemoved;
        CubeMover.OnCubeBlock += HandleCubeBlocked;
    }
    private void OnDisable()
    {
        CubeMover.OnCubeRemoved -= HandleCubeRemoved;
        CubeMover.OnCubeBlock -= HandleCubeBlocked;
    }
    [ContextMenu("Test Start Level")]
    private async UniTask StartLevel()
    {
        {
#if UNITY_EDITOR
            if (useLevelFileToTest)
            {
                levelLoader.SpawnLevel(levelDataFile);
                return;
            }
#endif
            string levelName = DataManager.Instance.GetCurrentLevelName();
            if (string.IsNullOrEmpty(levelName))
            {
                return;
            }

            await levelLoader.SpawnLevelFromJson(levelName);
            CurrentLevelState = new LevelState(levelLoader.GetCubeCount(), maxHeart);
            levelCubeList = new(levelLoader.CubeList);
            Debug.Log("Remaining Heart: " + CurrentLevelState.RemainingHeartCount);
            Debug.Log("Remaining Cube: " + CurrentLevelState.RemainingCubeCount);
        }
    }
    public void HandleCubeRemoved()
    {
        CurrentLevelState.RemoveCube();
        Debug.Log("Remaining Cube: " + CurrentLevelState.RemainingCubeCount);
        OnCubeCountChanged?.Invoke(CurrentLevelState.RemainingCubeCount);

        if (CurrentLevelState.IsCompleted)
        {
            Debug.Log("Win game");
            OnLevelCompleted?.Invoke();
        }
    }

    public void HandleCubeBlocked()
    {
        CurrentLevelState.LoseHeart();
        Debug.Log("Remaining Heart: " + CurrentLevelState.RemainingHeartCount);
        OnHeartCountChanged?.Invoke(CurrentLevelState.RemainingHeartCount);

        if (CurrentLevelState.IsFailed)
        {
            Debug.Log("Lose game");
            OnLevelFailed?.Invoke();
        }
    }

}
