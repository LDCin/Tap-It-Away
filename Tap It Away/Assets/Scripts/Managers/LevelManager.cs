using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class LevelManager : MonoBehaviour
{
    public static event Action<int> OnCubeCountChanged;
    public static  event Action<int> OnHeartCountChanged;
    public static  event Action OnLevelCompleted;
    public static  event Action OnLevelFailed;
    [SerializeField] private LevelLoader levelLoader;
    [SerializeField] private TextAsset userData;
#if UNITY_EDITOR
    [SerializeField] private TextAsset levelDataFile;
#endif
    [SerializeField] private bool useLevelFileToTest = false;
    [SerializeField] private readonly int maxHeart = 3;
    public LevelState CurrentState { get; private set; }
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
            await levelLoader.SpawnCurrentLevelFromUserData(userData);
            CurrentState = new LevelState(levelLoader.GetCubeCount(), maxHeart);
            Debug.Log("Remaining Heart: " + CurrentState.RemainingHeartCount);
            Debug.Log("Remaining Cube: " + CurrentState.RemainingCubeCount);
        }
    }
    public void HandleCubeRemoved()
    {
        CurrentState.RemoveCube();
        Debug.Log("Remaining Cube: " + CurrentState.RemainingCubeCount);
        OnCubeCountChanged?.Invoke(CurrentState.RemainingCubeCount);

        if (CurrentState.IsCompleted)
        {
            Debug.Log("Win game");
            OnLevelCompleted?.Invoke();
        }
    }

    public void HandleCubeBlocked()
    {
        CurrentState.LoseHeart();
        Debug.Log("Remaining Heart: " + CurrentState.RemainingHeartCount);
        OnHeartCountChanged?.Invoke(CurrentState.RemainingHeartCount);

        if (CurrentState.IsFailed)
        {
            Debug.Log("Lose game");
            OnLevelFailed?.Invoke();
        }
    }

}