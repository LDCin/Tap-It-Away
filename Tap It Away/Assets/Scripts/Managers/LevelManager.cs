using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading.Tasks;

public class LevelManager : Singleton<LevelManager>
{
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
        Observer.Subscribe<CubeMover>(ObserverEvent.OnCubeMove, HandleCubeMoved);
        Observer.Subscribe(ObserverEvent.CubeRemoved, HandleCubeRemoved);
        Observer.Subscribe(ObserverEvent.CubeBlocked, HandleCubeBlocked);
    }
    private void OnDisable()
    {
        Observer.Unsubscribe<CubeMover>(ObserverEvent.OnCubeMove, HandleCubeMoved);
        Observer.Unsubscribe(ObserverEvent.CubeRemoved, HandleCubeRemoved);
        Observer.Unsubscribe(ObserverEvent.CubeBlocked, HandleCubeBlocked);
    }
    [ContextMenu("Test Start Level")]
    private async UniTask StartLevel()
    {
        {
            // InputManager.Instance?.ResetPuzzleRootTransform();
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
            Observer.Publish(ObserverEvent.LevelLoaded);
        }
    }
    public void HandleCubeRemoved()
    {
        CurrentLevelState.RemoveCube();
        Debug.Log("Remaining Cube: " + CurrentLevelState.RemainingCubeCount);
        Observer.Publish(ObserverEvent.CubeCountChanged, CurrentLevelState.RemainingCubeCount);

        if (CurrentLevelState.IsCompleted)
        {
            Debug.Log("Win game");
            SaveCompletedLevel();
            Observer.Publish(ObserverEvent.LevelCompleted);
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

    private void HandleCubeMoved(CubeMover cubeMover)
    {
        if (levelCubeList == null || cubeMover == null)
        {
            return;
        }

        if (cubeMover.transform.parent == null)
        {
            levelCubeList.Remove(cubeMover);
            return;
        }

        if (!levelCubeList.Contains(cubeMover))
        {
            levelCubeList.Add(cubeMover);
        }
    }

    public void HandleCubeBlocked()
    {
        CurrentLevelState.LoseHeart();
        Debug.Log("Remaining Heart: " + CurrentLevelState.RemainingHeartCount);
        Observer.Publish(ObserverEvent.HeartCountChanged, CurrentLevelState.RemainingHeartCount);

        if (CurrentLevelState.IsFailed)
        {
            Debug.Log("Lose game");
            Observer.Publish(ObserverEvent.LevelFailed);
        }
    }

}
