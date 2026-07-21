using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private LevelLoader levelLoader;
    [SerializeField] private TextAsset userData;
#if UNITY_EDITOR
    [SerializeField] private TextAsset levelDataFile;
#endif
    [SerializeField] private bool useLevelFileToTest = false;
    private async UniTaskVoid Start()
    {
#if UNITY_EDITOR
        if (useLevelFileToTest)
        {
            levelLoader.SpawnLevel(levelDataFile);
            return;
        }
#endif
        await levelLoader.SpawnCurrentLevelFromUserData(userData);
    }
}