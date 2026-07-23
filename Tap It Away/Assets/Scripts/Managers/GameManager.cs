using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField] private LevelManager levelManager;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private GameStateType startGameStateType = GameStateType.Menu;
    private StateMachine stateMachine;
    private void OnEnable()
    {
        LevelManager.OnLevelCompleted += () => ChangeGameState(GameStateType.Finish);
        LevelManager.OnLevelFailed += () => ChangeGameState(GameStateType.Finish);
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
        stateMachine.RegisterState(GameStateType.Finish, new FailGameState(this, inputManager));
    }
    private void ChangeGameState(GameStateType gameStateType)
    {
        stateMachine.ChangeState(gameStateType);
    }
    private void Update()
    {
        stateMachine.Excute();
    }
}
