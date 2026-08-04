using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // [SerializeField] private LevelManager levelManager;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private GameStateType startGameStateType = GameStateType.Menu;
    private StateMachine stateMachine;
    private void OnEnable()
    {
        MenuPanel.OnPlayGame += HandlePlayGame;
        LevelManager.OnLevelCompleted += () => ChangeGameState(GameStateType.Complete);
        LevelManager.OnLevelFailed += () => ChangeGameState(GameStateType.Fail);
    }
    private void Start()
    {
        InitStateMachine();
        stateMachine.ChangeState(startGameStateType);
    }
    private void InitStateMachine()
    {
        stateMachine = new();
        stateMachine.RegisterState(GameStateType.Menu, new MenuGameState(this, inputManager));
        stateMachine.RegisterState(GameStateType.Play, new PlayGameState(this, inputManager));
        stateMachine.RegisterState(GameStateType.Fail, new FailGameState(this, inputManager));
        stateMachine.RegisterState(GameStateType.Complete, new CompleteGameState(this, inputManager));
        stateMachine.RegisterState(GameStateType.SettingInGame, new SettingInGameState(this, inputManager));
        stateMachine.RegisterState(GameStateType.Loading, new LoadingGameState(this, inputManager));
    }
    private void ChangeGameState(GameStateType gameStateType)
    {
        stateMachine.ChangeState(gameStateType);
    }
    private void Update()
    {
        stateMachine.Excute();
    }
    private void HandlePlayGame()
    {
        ChangeGameState(GameStateType.Play);
    }
}
