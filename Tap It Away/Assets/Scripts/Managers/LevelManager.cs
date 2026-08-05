using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class LevelManager : Singleton<LevelManager>
{
    public static event Action<int> OnCubeCountChanged;
    public static event Action<int> OnHeartCountChanged;
    public static event Action OnLevelCompleted;
    public static event Action OnLevelFailed;
    public static event Action OnLevelLoaded;
    [SerializeField] private LevelLoader levelLoader;
#if UNITY_EDITOR
    [SerializeField] private TextAsset levelDataFile;
    [SerializeField] private bool useLevelFileToTest = false;

#endif
    [SerializeField] private readonly int maxHeart = 3;
    public int MaxHeart => maxHeart;
    public LevelState CurrentLevelState { get; private set; }
    private string levelName;
    public string LevelName => levelName;
    private int levelNumber;
    public int LevelNumber => levelNumber;
    public string LevelDisplayName => $"Level {levelNumber:00}";
    private List<CubeMover> levelCubeList;
    public List<CubeMover> LevelCubeList => levelCubeList;
    private bool isLevelCompletedSaved = false;
    public async void PlayGame()
    {
        await StartLevel();
    }
    private void OnEnable()
    {
        CubeMover.OnCubeRemovedWithReference += HandleCubeRemovedFromList;
        CubeMover.OnCubeReturnedWithReference += HandleCubeReturnedToList;
        CubeMover.OnCubeRemoved += HandleCubeRemoved;
        CubeMover.OnCubeBlock += HandleCubeBlocked;
    }
    private void OnDisable()
    {
        CubeMover.OnCubeRemovedWithReference -= HandleCubeRemovedFromList;
        CubeMover.OnCubeReturnedWithReference -= HandleCubeReturnedToList;
        CubeMover.OnCubeRemoved -= HandleCubeRemoved;
        CubeMover.OnCubeBlock -= HandleCubeBlocked;
    }
    [ContextMenu("Test Start Level")]
    private async UniTask StartLevel()
    {
        {
            InputManager.Instance?.ResetPuzzleRootTransform();
#if UNITY_EDITOR
            if (useLevelFileToTest)
            {
                levelNumber = DataManager.Instance != null ? DataManager.Instance.GetCurrentLevelNumber() : 1;
                levelLoader.SpawnLevel(levelDataFile);
                return;
            }
#endif
            levelNumber = DataManager.Instance.GetCurrentLevelNumber();
            levelName = DataManager.Instance.GetCurrentLevelName();
            if (string.IsNullOrEmpty(levelName))
            {
                return;
            }

            await levelLoader.SpawnLevelFromJsonAsync(levelName);
            CurrentLevelState = new LevelState(levelLoader.GetCubeCount(), maxHeart);
            levelCubeList = new(levelLoader.CubeList);
            isLevelCompletedSaved = false;
            Debug.Log("Remaining Heart: " + CurrentLevelState.RemainingHeartCount);
            Debug.Log("Remaining Cube: " + CurrentLevelState.RemainingCubeCount);
            OnLevelLoaded?.Invoke();
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
            SaveCompletedLevel();
            OnLevelCompleted?.Invoke();
        }
    }

    private void SaveCompletedLevel()
    {
        if (isLevelCompletedSaved)
        {
            return;
        }

        isLevelCompletedSaved = true;
        DataManager.Instance.AdvanceToNextLevel();
    }

    public void DestroyLevel()
    {
        levelLoader.DestroyLevel();
        InputManager.Instance?.ResetPuzzleRootTransform();
        levelCubeList?.Clear();
        CurrentLevelState = null;
    }

    private void HandleCubeRemovedFromList(CubeMover cubeMover)
    {
        levelCubeList?.Remove(cubeMover);
    }

    private void HandleCubeReturnedToList(CubeMover cubeMover)
    {
        if (levelCubeList == null || cubeMover == null || levelCubeList.Contains(cubeMover))
        {
            return;
        }

        levelCubeList.Add(cubeMover);
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
