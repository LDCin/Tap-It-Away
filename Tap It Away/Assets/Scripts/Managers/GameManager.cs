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
        Observer.Subscribe(ObserverEvent.PlayGame, HandlePlayGame);
        Observer.Subscribe(ObserverEvent.LevelCompleted, HandleLevelCompleted);
        Observer.Subscribe(ObserverEvent.LevelFailed, HandleLevelFailed);
        Observer.Subscribe(ObserverEvent.OnOpenSettingInGame, HandleOpenSettingInGame);
        Observer.Subscribe(ObserverEvent.OnCloseSettingInGame, HandleCloseSettingInGame);
        Observer.Subscribe(ObserverEvent.OnBackToMenu, HandleBackToMenu);
    }

    private void OnDisable()
    {
        Observer.Unsubscribe(ObserverEvent.PlayGame, HandlePlayGame);
        Observer.Unsubscribe(ObserverEvent.LevelCompleted, HandleLevelCompleted);
        Observer.Unsubscribe(ObserverEvent.LevelFailed, HandleLevelFailed);
        Observer.Unsubscribe(ObserverEvent.OnOpenSettingInGame, HandleOpenSettingInGame);
        Observer.Unsubscribe(ObserverEvent.OnCloseSettingInGame, HandleCloseSettingInGame);
        Observer.Unsubscribe(ObserverEvent.OnBackToMenu, HandleBackToMenu);
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
    private async void HandlePlayGame()
    {
        await LevelManager.Instance.StartLevel();
        ChangeGameState(GameStateType.Play);
    }

    private void HandleLevelCompleted()
    {
        ChangeGameState(GameStateType.Complete);
    }

    private void HandleLevelFailed()
    {
        ChangeGameState(GameStateType.Fail);
    }
    private void HandleOpenSettingInGame()
    {
        ChangeGameState(GameStateType.SettingInGame);
    }

    private void HandleCloseSettingInGame()
    {
        ChangeGameState(GameStateType.Play);
    }

    private void HandleBackToMenu()
    {
        ChangeGameState(GameStateType.Menu);
    }
}
