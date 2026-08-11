using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
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
    private List<CubeMover> levelCubeList;
    public List<CubeMover> LevelCubeList => levelCubeList;
    public async void PlayGame()
    {
        await StartLevel();
    }
    private void OnEnable()
    {
        Observer.Subscribe(ObserverEvent.CubeRemoved, HandleCubeRemoved);
        Observer.Subscribe(ObserverEvent.CubeBlocked, HandleCubeBlocked);
    }
    private void OnDisable()
    {
        Observer.Unsubscribe(ObserverEvent.CubeRemoved, HandleCubeRemoved);
        Observer.Unsubscribe(ObserverEvent.CubeBlocked, HandleCubeBlocked);
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
            levelName = DataManager.Instance.GetCurrentLevelName();
            if (string.IsNullOrEmpty(levelName))
            {
                return;
            }

            await levelLoader.SpawnLevelFromJsonAsync(levelName);
            CurrentLevelState = new LevelState(levelLoader.GetCubeCount(), maxHeart);
            levelCubeList = new(levelLoader.CubeList);
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
            Observer.Publish(ObserverEvent.LevelCompleted);
        }
    }

    public void HandleCubeBlocked()
    {
        CurrentLevelState.LoseHeart();
        Debug.Log("Remaining Heart: " + CurrentLevelState.RemainingHeartCount);

        if (CurrentLevelState.IsFailed)
        {
            Debug.Log("Lose game");
            Observer.Publish(ObserverEvent.LevelFailed);
        }
    }

}
